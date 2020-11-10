using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Domain
{
	public class JsonPlaceHolderHttpClient
	{
		readonly HttpClient _httpClient;

		public JsonPlaceHolderHttpClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
		}

		public async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
		{
			var response = await _httpClient.GetAsync(requestUri, cancellationToken);
			return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), new(JsonSerializerDefaults.Web), cancellationToken);
		}

#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
		public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T data, CancellationToken cancellationToken = default) =>
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Review unused parameters
			Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
	}
}
