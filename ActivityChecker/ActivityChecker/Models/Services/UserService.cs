using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Text;
using ActivityChecker.Models.DTO;

namespace ActivityChecker.Models.Services
{
	public class UserService
	{

		private HttpClient _httpClient;

		public UserService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GetToken(LoginDTO dto)
		{

			var jsonContent = JsonSerializer.Serialize(dto);
			var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			try
			{
				var response = await _httpClient.PostAsync("Auth/Login", content);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
					var token = result?.accessToken;

					if (!string.IsNullOrEmpty(token))
					{
						return token;	
					}

					return null;
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<bool> ResetPassword(string username ,string newPassword, string token)
		{
			var response = await _httpClient.PostAsync($"Auth/ResetPassword?username={username}&newPassword={newPassword}&token={token}", null);
			if (response.IsSuccessStatusCode) return true;
			return false;
		}
	}

	class LoginResponse
	{
		public string accessToken { get; set; } = string.Empty;
	}
}

