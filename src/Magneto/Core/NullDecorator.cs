using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	class NullDecorator : IDecorator
	{
		internal static IDecorator Instance { get; } = new NullDecorator();

		NullDecorator() { }

		public TResult Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, TResult> invoke) => invoke(context);

		public Task<TResult> Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, Task<TResult>> invoke) => invoke(context);

		public void Decorate<TContext>(object operation, TContext context, Action<TContext> invoke)
		{
			invoke(context);
		}

		public Task Decorate<TContext>(object operation, TContext context, Func<TContext, Task> invoke) => invoke(context);
	}
}
