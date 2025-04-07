using static System.Net.WebRequestMethods;

namespace ActivityChecker.Models.Services
{
	public class PasswordResetService
	{
		private readonly HttpClient _httpClient;

		public PasswordResetService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GenerateToken(string username)
		{
			var response = await _httpClient.GetAsync($"Auth/CreateResetToken?username={username}");
			if(response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			return null;
		}

		public async Task<bool> ValidateToken(string token)
		{
			var response = await _httpClient.GetAsync($"Auth/ValidateResetToken?token={token}");
			if (response.IsSuccessStatusCode) return true;
			return false;
		}
	}
}
