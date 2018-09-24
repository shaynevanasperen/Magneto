using System;
using System.Net.Http;
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
			return await response.Content.ReadAsAsync<T>(cancellationToken);
		}
	}
}
