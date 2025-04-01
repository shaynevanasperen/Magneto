using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Domain;

public class JsonPlaceHolderHttpClient
{
	static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

	readonly HttpClient _httpClient;

	public JsonPlaceHolderHttpClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_httpClient.BaseAddress = new("https://jsonplaceholder.typicode.com");
	}

	public async ValueTask<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
	{
		var response = await _httpClient.GetAsync(requestUri, cancellationToken);
		var item = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), JsonSerializerOptions, cancellationToken);
		return item!;
	}

	public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T data, CancellationToken cancellationToken = default) =>
		Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
}
