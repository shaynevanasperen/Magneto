using System.Threading.Tasks;

namespace Magneto
{
	public interface IMediary : ISyncMediary, IAsyncMediary, IQueryMediary, ICommandMediary, ICacheManager { }

	public interface IQueryMediary : ISyncQueryMediary, IAsyncQueryMediary { }

	public interface ICommandMediary : ISyncCommandMediary, IAsyncCommandMediary { }

	public interface ISyncMediary : ISyncQueryMediary, ISyncCommandMediary, ISyncCacheManager { }

	public interface IAsyncMediary : IAsyncQueryMediary, IAsyncCommandMediary, IAsyncCacheManager { }

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
		/// <param name="cacheOption">An option designating whether or not the cache should be read when executing the query.</param>
		/// <returns>The result of the query execution (which could be a value returned from the cache).</returns>
		TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
	}

	public interface IAsyncQueryMediary
	{
		/// <summary>
		/// Executes the given <paramref name="query"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
		/// <param name="query">The query object which will be executed.</param>
		/// <param name="context">The context with which to execute the query.</param>
		/// <returns>The result of the query execution.</returns>
		Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, TContext context);

		/// <summary>
		/// Executes the given <paramref name="query"/> using the supplied <paramref name="context"/> and <paramref name="cacheOption"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the <paramref name="query"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="query"/> result.</typeparam>
		/// <param name="query">The query object which will be executed.</param>
		/// <param name="context">The context with which to execute the query.</param>
		/// <param name="cacheOption">An option designating whether or not the cache should be read when executing the query.</param>
		/// <returns>The result of the query execution (which could be a value returned from the cache).</returns>
		Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default);
	}

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

	public interface IAsyncCommandMediary
	{
		/// <summary>
		/// Executes the given <paramref name="command"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="command"/>.</typeparam>
		/// <param name="command">The command object which will be executed.</param>
		/// <param name="context">The context with which to execute the command.</param>
		/// <returns>A task representing the execution of the command.</returns>
		Task CommandAsync<TContext>(IAsyncCommand<TContext> command, TContext context);

		/// <summary>
		/// Executes the given <paramref name="command"/> using the supplied <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TContext">The type of <paramref name="context"/> with which to execute the <paramref name="command"/>.</typeparam>
		/// <typeparam name="TResult">The type of the <paramref name="command"/> result.</typeparam>
		/// <param name="command">The command object which will be executed.</param>
		/// <param name="context">The context with which to execute the command.</param>
		/// <returns>The result of the command execution.</returns>
		Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, TContext context);
	}
}
