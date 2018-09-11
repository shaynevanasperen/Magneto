using System.Net.Http;
using System.Reflection;

namespace Samples.Infrastructure
{
	public static class HttpClientExtensions
	{
		static readonly FieldInfo HandlerFieldInfo = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance);

		public static HttpMessageHandler GetHandler(this HttpClient httpClient) => (HttpMessageHandler)HandlerFieldInfo.GetValue(httpClient);
	}
}