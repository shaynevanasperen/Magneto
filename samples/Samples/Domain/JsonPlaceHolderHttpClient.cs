using System;
using System.Net.Http;
using Samples.Infrastructure;

namespace Samples.Domain
{
	public class JsonPlaceHolderHttpClient : HttpClient
	{
		public JsonPlaceHolderHttpClient(HttpClient httpClient) : base(httpClient.GetHandler(), false) =>
			BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
	}
}
