using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Magneto.Configuration;
using Microsoft.ApplicationInsights;

namespace Samples.Infrastructure
{
	public class ApplicationInsightsDecorator : IDecorator
	{
		readonly TelemetryClient _telemetryClient;

		public ApplicationInsightsDecorator(TelemetryClient telemetryClient) => _telemetryClient = telemetryClient;

		public TResult Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, TResult> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var result = invoke(context);
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operation.GetType().FullName, elapsed);
				return result;
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public async Task<TResult> Decorate<TContext, TResult>(object operation, TContext context, Func<TContext, Task<TResult>> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var result = await invoke(context);
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operation.GetType().FullName, elapsed);
				return result;
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public void Decorate<TContext>(object operation, TContext context, Action<TContext> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				invoke(context);
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operation.GetType().FullName, elapsed);
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public async Task Decorate<TContext>(object operation, TContext context, Func<TContext, Task> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				await invoke(context);
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operation.GetType().FullName, elapsed);
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}
	}
}
