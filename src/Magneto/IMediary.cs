using System.Threading;
using System.Threading.Tasks;

namespace Magneto
{
	/// <summary>
	/// A mediary for invoking queries and commands, and managing cached results of queries.
	/// </summary>
	public interface IMediary : ISyncMediary, IAsyncMediary, IQueryMediary, ICommandMediary, ICacheManager { }

	/// <summary>
	/// A mediary for invoking queries.
	/// </summary>
	public interface IQueryMediary : ISyncQueryMediary, IAsyncQueryMediary { }

	/// <summary>
	/// A mediary for invoking commands.
	/// </summary>
	public interface ICommandMediary : ISyncCommandMediary, IAsyncCommandMediary { }

	/// <summary>
	/// A mediary for invoking queries and commands, and managing cached results of queries, synchronously.
	/// </summary>
	public interface ISyncMediary : ISyncQueryMediary, ISyncCommandMediary, ISyncCacheManager { }

	/// <summary>
	/// A mediary for invoking queries and commands, and managing cached results of queries, asynchronously.
	/// </summary>
	public interface IAsyncMediary : IAsyncQueryMediary, IAsyncCommandMediary, IAsyncCacheManager { }

	/// <summary>
	/// A mediary for invoking queries synchronously.
	/// </summary>
	public interface ISyncQueryMediary
	{
		/// <summary>
		/// Executes the given <paramref name="query"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
		/// <param name="query">The query object which will be executed.</param>
		/// <param name="context">The context with which to execute the query.</param>
		/// <returns>The result of the query execution.</returns>
		TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query, TContext context);

		/// <summary>
		/// Executes the given <paramref name="query"/> using the supplied <paramref name="context"/> and <paramref name="cacheOption"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
		/// <param name="query">The query object which will be executed.</param>
		/// <param name="context">The context with which to execute the query.</param>
		/// <param name="cacheOption">Optional. An option designating whether or not the cache should be read when executing the query.</param>
		/// <returns>The result of the query execution (which could be a value returned from the cache).</returns>
		TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
	}

	/// <summary>
	/// A mediary for invoking queries asynchronously.
	/// </summary>
	public interface IAsyncQueryMediary
	{
		/// <summary>
		/// Executes the given <paramref name="query"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
		/// <param name="query">The query object which will be executed.</param>
		/// <param name="context">The context with which to execute the query.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>The result of the query execution.</returns>
		Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, TContext context, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes the given <paramref name="query"/> using the supplied <paramref name="context"/> and <paramref name="cacheOption"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
		/// <param name="query">The query object which will be executed.</param>
		/// <param name="context">The context with which to execute the query.</param>
		/// <param name="cacheOption">Optional. An option designating whether or not the cache should be read when executing the query.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>The result of the query execution (which could be a value returned from the cache).</returns>
		Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default, CancellationToken cancellationToken = default);
	}

	/// <summary>
	/// A mediary for invoking commands synchronously.
	/// </summary>
	public interface ISyncCommandMediary
	{
		/// <summary>
		/// Executes the given <paramref name="command"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="command"/>.</typeparam>
		/// <param name="command">The command object which will be executed.</param>
		/// <param name="context">The context with which to execute the command.</param>
		void Command<TContext>(ISyncCommand<TContext> command, TContext context);

		/// <summary>
		/// Executes the given <paramref name="command"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="command"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="command"/> result.</typeparam>
		/// <param name="command">The command object which will be executed.</param>
		/// <param name="context">The context with which to execute the command.</param>
		/// <returns>The result of the command execution.</returns>
		TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command, TContext context);
	}

	/// <summary>
	/// A mediary for invoking commands asynchronously.
	/// </summary>
	public interface IAsyncCommandMediary
	{
		/// <summary>
		/// Executes the given <paramref name="command"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="command"/>.</typeparam>
		/// <param name="command">The command object which will be executed.</param>
		/// <param name="context">The context with which to execute the command.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>A task representing the execution of the command.</returns>
		Task CommandAsync<TContext>(IAsyncCommand<TContext> command, TContext context, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes the given <paramref name="command"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="command"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="command"/> result.</typeparam>
		/// <param name="command">The command object which will be executed.</param>
		/// <param name="context">The context with which to execute the command.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>The result of the command execution.</returns>
		Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, TContext context, CancellationToken cancellationToken = default);
	}
}
