using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Magneto.Configuration;
using Microsoft.ApplicationInsights;

namespace Samples.Infrastructure;

class ApplicationInsightsDecorator(TelemetryClient telemetryClient) : IDecorator
{
	public TResult Decorate<TResult>(string operationName, Func<TResult> invoke)
	{
		try
		{
			var stopwatch = Stopwatch.StartNew();
			var result = invoke();
			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			telemetryClient.TrackMetric(operationName, elapsed);
			return result;
		}
		catch (Exception e)
		{
			telemetryClient.TrackException(e);
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
			telemetryClient.TrackMetric(operationName, elapsed);
			return result;
		}
		catch (Exception e)
		{
			telemetryClient.TrackException(e);
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
			telemetryClient.TrackMetric(operationName, elapsed);
		}
		catch (Exception e)
		{
			telemetryClient.TrackException(e);
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
			telemetryClient.TrackMetric(operationName, elapsed);
		}
		catch (Exception e)
		{
			telemetryClient.TrackException(e);
			throw;
		}
	}
}
