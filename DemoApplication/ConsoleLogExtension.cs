using System;
using System.Linq;
using System.Linq.Expressions;

namespace DemoApplication
{
	static class ConsoleLogExtension
	{
		internal static void Log<T, TReturn>(this T instance, Expression<Func<T, TReturn>> func)
		{
			Console.WriteLine("Calling {0}.{1}({2})", 
				instance.GetType().Name, 
				((MethodCallExpression)func.Body).Method.Name,
				string.Join(", ", ((MethodCallExpression)func.Body).Arguments.Select(
				x =>
				{
					var l = Expression.Lambda(Expression.Convert(x, x.Type));
					return l.Compile().DynamicInvoke();
				})));
		}
	}
}
