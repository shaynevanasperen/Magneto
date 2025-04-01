using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core;

class NullDecorator : IDecorator
{
	internal static IDecorator Instance { get; } = new NullDecorator();

	NullDecorator() { }

	public TResult Decorate<TResult>(string operationName, Func<TResult> invoke) => invoke();

	public Task<TResult> Decorate<TResult>(string operationName, Func<Task<TResult>> invoke) => invoke();

	public void Decorate(string operationName, Action invoke) => invoke();

	public Task Decorate(string operationName, Func<Task> invoke) => invoke();
}
