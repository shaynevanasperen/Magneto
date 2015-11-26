using System;
using System.Collections.Generic;
using System.Diagnostics;
using Machine.Specifications;
using Quarks.IDictionaryExtensions;

namespace Data.Operations.Tests
{
	[Subject(typeof(DataFactory))]
	class When_creating_data
	{
		[Ignore("This is a benchmark test for manual execution")]
		It should_be_fast = () =>
		{
			Console.WriteLine("Baseline: {0:N}ns", baselineTime);
			Console.WriteLine("Activator.CreateInstance: {0:N}ns", activatorCreateInstanceTime);
			Console.WriteLine("DataFactory Initial: {0:N}ns", dataFactoryInitialTime);
			Console.WriteLine("DataFactory: {0:N}ns ({1:N3} times slower than Baseline, {2:N3} times faster than Activator.CreateInstance)",
				dataFactoryTime,
				dataFactoryTime / baselineTime,
				activatorCreateInstanceTime / dataFactoryTime);
			Console.WriteLine("Configured DataFactory: {0:N}ns ({1:N3} times slower than Baseline, {2:N3} times faster than Activator.CreateInstance)",
				configuredDataFactoryTime,
				configuredDataFactoryTime / baselineTime,
				activatorCreateInstanceTime / configuredDataFactoryTime);

			dataFactoryTime.ShouldBeLessThan(baselineTime * 12);
			dataFactoryTime.ShouldBeLessThan(activatorCreateInstanceTime / 4);
			configuredDataFactoryTime.ShouldBeLessThan(baselineTime * 9);
			configuredDataFactoryTime.ShouldBeLessThan(activatorCreateInstanceTime / 5);
		};

		It should_support_a_value_type_context = () =>
		{
			Catch.Exception(() => 1.Data()).ShouldBeNull();
			Catch.Exception(() => DayOfWeek.Monday.Data()).ShouldBeNull();
		};

		Because of = () =>
		{
			const int iterations = 50000;
			const string context = "context";
			baselineTime = time(() => new Data<string>(new Data(), context), iterations);
			activatorCreateInstanceTime = time(() => ActivatorCreateInstanceDataFactory.Data(context), iterations);
			dataFactoryInitialTime = time(() => context.Data(), 1);
			dataFactoryTime = time(() => context.Data(), iterations);
			var data = new Data(new DataQueryCache(new MemoryCacheDefaultCacheStore()));
			DataFactory.SetFactory<string>(x => new Data<string>(data, x));
			configuredDataFactoryTime = time(() => context.Data(), iterations);
		};

		static double time(Action action, int iterations)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (var i = 0; i < iterations; i++)
				action();
			return stopwatch.ElapsedTicks / ((double)Stopwatch.Frequency / (1000L * 1000L * 1000L)) / iterations;
		}

		static double baselineTime, activatorCreateInstanceTime, dataFactoryInitialTime, dataFactoryTime, configuredDataFactoryTime;
	}

	static class ActivatorCreateInstanceDataFactory
	{
		static readonly IDictionary<Type, IDataQueryCache> _dataQueryCaches = new Dictionary<Type, IDataQueryCache>();

		static IData _createData<TContext>()
		{
			return new Data(_dataQueryCaches.GetValueOrDefault(typeof(TContext), (IDataQueryCache)null));
		}

		static readonly IDictionary<Type, Func<object, object>> _dataFactories = new Dictionary<Type, Func<object, object>>();
		public static IData<TContext> Data<TContext>(TContext context)
		{
			return _dataFactories.GetValueOrDefault(typeof(TContext), _createData<TContext>).Invoke(context) as IData<TContext>;
		}

		static object _createData<TContext>(object context)
		{
			var dataType = typeof(Data<>).MakeGenericType(context.GetType());
			return Activator.CreateInstance(dataType, _createData<TContext>(), context);
		}
	}
}
