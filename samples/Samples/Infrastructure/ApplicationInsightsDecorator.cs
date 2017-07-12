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

		public T Decorate<T>(object operation, Func<T> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var result = invoke();
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

		public async Task<T> Decorate<T>(object operation, Func<Task<T>> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var result = await invoke();
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

		public void Decorate(object operation, Action invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				invoke();
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operation.GetType().FullName, elapsed);
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public async Task Decorate(object operation, Func<Task> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				await invoke();
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
