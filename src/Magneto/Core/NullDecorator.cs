using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	public class NullDecorator : IDecorator
	{
		public T Decorate<T>(object operation, Func<T> invoke) => invoke();

		public Task<T> Decorate<T>(object operation, Func<Task<T>> invoke) => invoke();

		public void Decorate(object operation, Action invoke)
		{
			invoke();
		}

		public Task Decorate(object operation, Func<Task> invoke) => invoke();
	}
}
