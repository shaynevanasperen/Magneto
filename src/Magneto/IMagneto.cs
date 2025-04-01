using System.Threading;
using System.Threading.Tasks;

namespace Magneto;

/// <summary>
/// A mediary for invoking queries and commands, and managing cached results of queries.
/// </summary>
public interface IMagneto : ISyncMagneto, IAsyncMagneto, IQueryMagneto, ICommandMagneto, ICacheManager { }

/// <summary>
/// A mediary for invoking queries.
/// </summary>
public interface IQueryMagneto : ISyncQueryMagneto, IAsyncQueryMagneto { }

/// <summary>
/// A mediary for invoking commands.
/// </summary>
public interface ICommandMagneto : ISyncCommandMagneto, IAsyncCommandMagneto { }

/// <summary>
/// A mediary for invoking queries and commands, and managing cached results of queries, synchronously.
/// </summary>
public interface ISyncMagneto : ISyncQueryMagneto, ISyncCommandMagneto, ISyncCacheManager { }

/// <summary>
/// A mediary for invoking queries and commands, and managing cached results of queries, asynchronously.
/// </summary>
public interface IAsyncMagneto : IAsyncQueryMagneto, IAsyncCommandMagneto, IAsyncCacheManager { }

/// <summary>
/// A mediary for invoking queries synchronously.
/// </summary>
public interface ISyncQueryMagneto
{
	/// <summary>
	/// Executes the given <paramref name="query"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="query"/>.</typeparam>
	/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
	/// <param name="query">The query object which will be executed.</param>
	/// <returns>The result of the query execution.</returns>
	TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query);

	/// <summary>
	/// Executes the given <paramref name="query"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="query"/>.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
	/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
	/// <param name="query">The query object which will be executed.</param>
	/// <param name="cacheOption">An option designating whether the cache should be checked when executing the query.
	/// Use <see cref="CacheOption.Refresh"/> to skip reading from the cache and ensure a fresh result.</param>
	/// <returns>The result of the query execution (which could be a value returned from the cache).</returns>
	TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption);
}

/// <summary>
/// A mediary for invoking queries asynchronously.
/// </summary>
public interface IAsyncQueryMagneto
{
	/// <summary>
	/// Executes the given <paramref name="query"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="query"/>.</typeparam>
	/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
	/// <param name="query">The query object which will be executed.</param>
	/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>The result of the query execution.</returns>
	Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes the given <paramref name="query"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="query"/>.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
	/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
	/// <param name="query">The query object which will be executed.</param>
	/// <param name="cacheOption">An option designating whether the cache should be checked when executing the query.
	/// Use <see cref="CacheOption.Refresh"/> to skip reading from the cache and ensure a fresh result.</param>
	/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>The result of the query execution (which could be a value returned from the cache).</returns>
	Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, CacheOption cacheOption, CancellationToken cancellationToken = default);
}

/// <summary>
/// A mediary for invoking commands synchronously.
/// </summary>
public interface ISyncCommandMagneto
{
	/// <summary>
	/// Executes the given <paramref name="command"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="command"/>.</typeparam>
	/// <param name="command">The command object which will be executed.</param>
	void Command<TContext>(ISyncCommand<TContext> command);

	/// <summary>
	/// Executes the given <paramref name="command"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="command"/>.</typeparam>
	/// <typeparam name="TResult">The type of the <paramref name="command"/> result.</typeparam>
	/// <param name="command">The command object which will be executed.</param>
	/// <returns>The result of the command execution.</returns>
	TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command);
}

/// <summary>
/// A mediary for invoking commands asynchronously.
/// </summary>
public interface IAsyncCommandMagneto
{
	/// <summary>
	/// Executes the given <paramref name="command"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="command"/>.</typeparam>
	/// <param name="command">The command object which will be executed.</param>
	/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>A task representing the execution of the command.</returns>
	Task CommandAsync<TContext>(IAsyncCommand<TContext> command, CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes the given <paramref name="command"/> using an instance of <typeparamref name="TContext"/> obtained from the current scope.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which to execute the <paramref name="command"/>.</typeparam>
	/// <typeparam name="TResult">The type of the <paramref name="command"/> result.</typeparam>
	/// <param name="command">The command object which will be executed.</param>
	/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
	/// <returns>The result of the command execution.</returns>
	Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, CancellationToken cancellationToken = default);
}
