using System;
using System.Net.Http;
using Samples.Infrastructure;

namespace Samples.Domain
{
	public class JsonPlaceHolderHttpClient : HttpClient
	{
		public JsonPlaceHolderHttpClient() : base(new EnsureSuccessHandler()) => BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
	}
}
