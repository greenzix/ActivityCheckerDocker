namespace ActivityChecker.Models.Services
{
	public class RoleService
	{
		private HttpClient _httpClient;

		public RoleService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<bool> ConvertTablet(string username)
		{
			var request = await _httpClient.PostAsync($"User/SetTablet?username={username}", null);
			if(request.IsSuccessStatusCode) return true;
			return false;
		}

		public async Task<bool> ConvertUser(string username)
		{
			var request = await _httpClient.PostAsync($"User/SetUser?username={username}", null);
			if (request.IsSuccessStatusCode) return true;
			return false;
		}

		public async Task<bool> AddTeacher(string username)
		{
			var request = await _httpClient.PutAsync($"User/AddRole?username={username}&role=Teacher", null);
			if (request.IsSuccessStatusCode) return true;
			return false;
		}

		public async Task<bool> RemoveTeacher(string username)
		{
			var request = await _httpClient.PutAsync($"User/RemoveRole?username={username}&role=Teacher", null);
			if (request.IsSuccessStatusCode) return true;
			return false;
		}

	}
}
