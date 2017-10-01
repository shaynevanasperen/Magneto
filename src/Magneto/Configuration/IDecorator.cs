using System;
using System.Threading.Tasks;

namespace Magneto.Configuration
{
	public interface IDecorator : ISyncDecorator, IAsyncDecorator { }

	public interface ISyncDecorator
	{
		TResult Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, TResult> invoke);
		void Decorate<TContext>(object operation, TContext context, Action<TContext> invoke);
	}

	public interface IAsyncDecorator
	{
		Task<TResult> Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, Task<TResult>> invoke);
		Task Decorate<TContext>(object operation, TContext context, Func<TContext, Task> invoke);
	}
}
