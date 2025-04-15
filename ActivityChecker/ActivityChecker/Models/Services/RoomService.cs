using ActivityChecker.Components.Pages;
using ActivityChecker.Models.DTO;
using ActivityChecker.Models.Entities;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace ActivityChecker.Models.Services
{
	public class RoomService
	{
		private HttpClient _httpClient;

		public RoomService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<Room> GetRoomCode(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/GetRoomCode?roomId={roomId}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<Room>();
					return result;
				}
				else return null;
			} 
			catch(Exception ex) 
			{
				return null;
			}
		}

		public async Task<string> GetRoomName(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/GetRoomName?roomId={roomId}");


				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsStringAsync();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task <DateTime> GetExpireDate()
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/GetExpireTime");


				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<DateTime>();
					return result;
				}
				else return new DateTime();
			}
			catch (Exception ex)
			{
				return new DateTime();
			}
		}

		public async Task<bool> ChangeRoomPermissions(int userId, int roomId, Permission permission)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Room/ChangePermission?userId={userId}&roomId={roomId}&permission={(int)permission}");

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

		public async Task<bool> BecomeRoomAdmin(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Room/GetAdmin?roomId={roomId}");

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

		public async Task<RoomOrder> GetRoomOrder(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/CheckRoomOrder?roomId={roomId}");


				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<RoomOrder>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<RoomMember>> GetRoomMembers(int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/GetRoomMembers?roomId={roomId}");


				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<RoomMember>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<bool> AssignRoom(string roomCode)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Room/AssignRoom?roomCode={roomCode}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<RoomOrder>();
					if (result != null) return true;
					return false;
				}
				else return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}




		public async Task<bool> KickUser(int userId, int roomId)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Delete, $"Room/KickUser?userId={userId}&roomId={roomId}");

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

		public async Task<List<Room>> ConvertRooms(List<RoomOrder> list)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Room/ConvertOrderToRoom");
				var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(list),Encoding.UTF8,"application/json");
				request.Content = content;

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<Room>>();
					return result;
				}
				else return new List<Room>();
			}
			catch (Exception ex)
			{
				return new List<Room>();
			}
		}

		public async Task<List<Room>> GetAllRooms(string token)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/GetAllRooms");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<Room>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<RoomOrder>> GetMyRooms()
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, $"Room/GetRoomsForUser");


				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<RoomOrder>>();
					return result;
				}
				else return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<bool> DeleteRoom(string roomName)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Delete, $"Room/RemoveRoom?roomName={roomName}");

				var response = await _httpClient.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					return true;
				}
				return false;
			}
			catch { return false; }
		}

		public async Task<bool> CreateRoom(CreateRoomDTO dto)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Post, $"Room/CreateRoom");
				var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
				request.Content = content;

				var response = await _httpClient.SendAsync(request);

				if(response.IsSuccessStatusCode)
				{
					return true;
				}
				return false;
			}
			catch 
			{
				return false;
			}
		}
	}
}
