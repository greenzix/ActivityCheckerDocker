using ActivityChecker.Components.Pages;
using ActivityChecker.Models.DTO;
using ActivityChecker.Models.Entities;
using ActivityCheckerApi.Models.Entities;
using System.Collections.Generic;
using System.Text;

namespace ActivityChecker.Models.Services
{
	public class ActivityService
	{
		private HttpClient _httpClient;

		public ActivityService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}


		public async Task<bool> JoinRoom(int roomId ,string roomCode)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Activity/JoinRoom?roomId={roomId}&roomCode={roomCode}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					return true;
				}
				else return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> LeaveRoom(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Activity/LeaveRoom?roomId={roomId}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					return true;
				}
				else return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<User> GetUserById(int userId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"User/GetUserById?userId={userId}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<User>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<User> CreateAccount(LoginDTO dto)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Auth/CreateUser");
				var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
				request.Content = content;

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<User>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<RoomPresence> GetPresence()
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"User/GetRoomPresenceForUser");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<RoomPresence>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<RoomPresence>> GetPresenceInRoom(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Activity/GetUsersInRoom?roomId={roomId}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<RoomPresence>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<UserActivity>> GetUserHistory()
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Activity/GetUserHistory");

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

		public async Task<List<TeacherActivity>> GetTeacherHistory()
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Activity/GetTeacherHistory");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<TeacherActivity>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<RoomActivity>> GetRoomHistory(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Activity/GetRoomHistory?roomId={roomId}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<RoomActivity>>();
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
