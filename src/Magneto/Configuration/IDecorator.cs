using System;
using System.Threading.Tasks;

namespace Magneto.Configuration
{
	public interface IDecorator : ISyncDecorator, IAsyncDecorator { }

	public interface ISyncDecorator
	{
		T Decorate<T>(object operation, Func<T> invoke);
		void Decorate(object operation, Action invoke);
	}

	public interface IAsyncDecorator
	{
		Task<T> Decorate<T>(object operation, Func<Task<T>> invoke);
		Task Decorate(object operation, Func<Task> invoke);
	}
}
