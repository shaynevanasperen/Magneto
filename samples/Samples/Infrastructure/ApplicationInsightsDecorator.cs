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

		public TResult Decorate<TResult>(string operationName, Func<TResult> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var result = invoke();
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operationName, elapsed);
				return result;
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public async Task<TResult> Decorate<TResult>(string operationName, Func<Task<TResult>> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var result = await invoke();
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operationName, elapsed);
				return result;
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public void Decorate(string operationName, Action invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				invoke();
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operationName, elapsed);
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}

		public async Task Decorate(string operationName, Func<Task> invoke)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				await invoke();
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				_telemetryClient.TrackMetric(operationName, elapsed);
			}
			catch (Exception e)
			{
				_telemetryClient.TrackException(e);
				throw;
			}
		}
	}
}
