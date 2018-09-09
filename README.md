<img src="Magneto.png" align="right" />

[![Build status](https://ci.appveyor.com/api/projects/status/3auwev7g464o6ax3?svg=true)](https://ci.appveyor.com/project/shaynevanasperen/magneto)
[![Join the chat at https://gitter.im/magneto-project](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/magneto-project)
![License](https://img.shields.io/github/license/shaynevanasperen/magneto.svg)

[![NuGet](https://img.shields.io/nuget/v/Magneto.svg)](https://www.nuget.org/packages/Magneto)
[![NuGet](https://img.shields.io/nuget/dt/Magneto.svg)](https://www.nuget.org/packages/Magneto)

## Magneto

A library for implementing the [Command Pattern](https://en.wikipedia.org/wiki/Command_pattern), providing of a set
of base classes for operations and an _invoker_/_dispatcher_ class for executing them. Useful for abstracting data access
and API calls as either _queries_ (for read operations) or _commands_ (for write operations). `Query` and `Command` classes
declare the type of context they require for execution, and the type of the result, and optionally the type of cache options
they require for caching the result. Parameters are modelled as properties on the `Query` and `Command` classes.

Define a query object:

```cs
public class PostById : AsyncQuery<HttpClient, Post>
{
    // The context here is an HttpClient, but it could be an IDbConnection or anything you want
    public override async Task<Post> ExecuteAsync(HttpClient context)
    {
        var response = await context.GetAsync($"https://jsonplaceholder.typicode.com/posts/{Id}");
        return await response.Content.ReadAsAsync<Post>();
    }
    
    // We could also add a constructor and make this a readonly property
    public int Id { get; set; }
}
```

Invoke it:

```cs
// Allow IMagneto to fetch the appropriate context from the ServiceProvider
var post = await _magneto.QueryAsync(new PostById { Id = 1 });
// Or pass the context yourself by using IMediary
var post = await _mediary.QueryAsync(new PostById { Id = 1 }, _httpClient);
```

Mock the result in a unit test:

```cs
// Moq
magnetoMock.Setup(x => x.QueryAsync(new PostById { Id = 1 })).ReturnsAsync(new Post());
// NSubstitute
magneto.QueryAsync(new PostById { Id = 1 }).Returns(new Post());
```

Leverage built-in caching by deriving from a base class and specifying the type of cache options:

```cs
public class CommentsByPostId : AsyncCachedQuery<HttpClient, MemoryCacheEntryOptions, Comment[]>
{
    // Here we get to specify which parameters comprise the cache key
    protected override void ConfigureCache(ICacheConfig cacheConfig) => cacheConfig.VaryBy = PostId;
    
    // Here we get to specify the caching policy (absolute/sliding)
    protected override MemoryCacheEntryOptions GetCacheEntryOptions(HttpClient context) =>
        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

    protected override async Task<Comment[]> QueryAsync(HttpClient context)
    {
        var response = await context.GetAsync($"https://jsonplaceholder.typicode.com/posts/{PostId}/comments");
        return await response.Content.ReadAsAsync<Comment[]>();
    }
    
    public int PostId { get; set; }
}
```

Cache intermediate results and transform to a final result:

```cs
public class UserById : AsyncTransformedCachedQuery<HttpClient, DistributedCacheEntryOptions, User[], User>
{
    protected override DistributedCacheEntryOptions GetCacheEntryOptions(HttpClient context) =>
        new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
    
    protected override async Task<User[]> QueryAsync(HttpClient context)
    {
        var response = await context.GetAsync("https://jsonplaceholder.typicode.com/users");
        return await response.Content.ReadAsAsync<User[]>();
    }
    
    protected override Task<User> TransformCachedResultAsync(User[] cachedResult) =>
        Task.FromResult(cachedResult.Single(x => x.Id == Id));
    
    public int Id { get; set; }
}
```

Register a decorator to apply cross-cutting concerns:

```cs
// In application startup
...
services.AddSingleton<IDecorator, ApplicationInsightsDecorator>();
...

// Decorator implementation
public class ApplicationInsightsDecorator : IDecorator
{
    readonly TelemetryClient _telemetryClient;

    public ApplicationInsightsDecorator(TelemetryClient telemetryClient) => _telemetryClient = telemetryClient;

    public TResult Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, TResult> invoke)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = invoke(context);
            var elapsed = stopwatch.Elapsed.TotalMilliseconds;
            _telemetryClient.TrackMetric(operation.GetType().FullName, elapsed);
            return result;
        }
        catch (Exception e)
        {
            _telemetryClient.TrackException(e);
            throw;
        }
    }
    ...
}
```

See the [bundled sample application](https://github.com/shaynevanasperen/Magneto/tree/master/samples) for further examples of usage.
