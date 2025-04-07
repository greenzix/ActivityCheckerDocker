using Microsoft.JSInterop;

namespace ActivityChecker.Models.Services
{
	public class AuthenticationHeaderHandler : DelegatingHandler
	{
		private readonly string _token;

		public AuthenticationHeaderHandler(string token)
		{
			_token = token;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (!string.IsNullOrWhiteSpace(_token))
			{
				request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
