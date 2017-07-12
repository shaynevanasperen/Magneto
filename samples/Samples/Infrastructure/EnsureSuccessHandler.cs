using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Infrastructure
{
	public class EnsureSuccessHandler : DelegatingHandler
	{
		public EnsureSuccessHandler() : base(new HttpClientHandler()) { }

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);
			response.EnsureSuccessStatusCode();
			return response;
		}
	}
}
