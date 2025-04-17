using ActivityChecker.Models.DTO;
using ActivityChecker.Models.Entities;

namespace ActivityChecker.Models.Services
{
	public class TeacherService
	{
		private HttpClient _httpClient;

		public TeacherService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<StudentDTO>> GetAllStudentsAsync()
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Teacher/GetAllStudent");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<StudentDTO>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<UserActivity>> GetHistoryForStudentAsync(int studentId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Teacher/GetHistoryForStudent?studentId={studentId}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<UserActivity>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
