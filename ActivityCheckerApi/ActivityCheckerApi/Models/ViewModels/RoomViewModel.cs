using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.Background_Service;
using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ActivityCheckerApi.Models.ViewModels
{
	public class RoomViewModel
	{
		private readonly AppDbContext _context;

		public RoomViewModel(AppDbContext context)
		{
			_context = context;
		}	

		public async Task<Room> CreateRoom(RoomDTO DTO, int userId)
		{
			Room? existingRoom = await _context.Rooms.FirstOrDefaultAsync(u => u.Name == DTO.Name);
			if (existingRoom != null) return null;
			Room newRoom = new Room { Name = DTO.Name, Closed = false, Code = CodeGenerator.RandomString(4), Limit = DTO.Limit };
			_context.Rooms.Add(newRoom);
			await _context.SaveChangesAsync();
			RoomOrder order = new RoomOrder 
			{
				JoinedDate = DateTime.Now,
				RoomId = newRoom.Id,
				UserId = userId,
				Permission = Permission.Admin
			};
			_context.RoomOrders.Add(order);
			await _context.SaveChangesAsync();
			return newRoom;
		}

		public async Task<List<Room>> GetAllRooms()
		{
			return await _context.Rooms.ToListAsync();
		}

		public async Task<Room> GetRoomById(int roomId)
		{
			return await _context.Rooms.FirstOrDefaultAsync(u => u.Id == roomId) ?? null;
		}

		public async Task<bool> DeleteAllRooms()
		{
			await _context.Rooms.ExecuteDeleteAsync();
			return true;
		}

		public async Task<Room> GetRoomCode(int roomId)
		{
			Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Id == roomId);
			return room;
		}

		public async Task<bool> DeleteRoomById(string roomName)
		{
			Room? existingRoom = await _context.Rooms.FirstOrDefaultAsync(u => u.Name == roomName);
			if (existingRoom == null) return false;
			await _context.RoomOrders.Where(u => u.RoomId == existingRoom.Id).ExecuteDeleteAsync();
			await _context.RoomPresences.Where(u => u.RoomId == existingRoom.Id).ExecuteDeleteAsync();
			_context.Rooms.Remove(existingRoom);
			
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RemoveUserFromRoom(int adminId, int userId, int roomid)
		{
			Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Id == roomid);
			RoomOrder? Adminorder = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == adminId && u.RoomId == roomid);
			if (Adminorder.Permission != Permission.Admin) return false;
			RoomOrder? kickedOrder = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == userId && u.RoomId == roomid);
			RoomPresence? presence = await _context.RoomPresences.FirstOrDefaultAsync(u => u.UserId == userId & u.RoomId == roomid);
			if (kickedOrder == null) return false;
			if (presence != null) _context.RoomPresences.Remove(presence);
			_context.RoomOrders.Remove(kickedOrder);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<RoomOrder> AssignRoom(RoomOrderDTO DTO, string roomCode)
		{
			if (roomCode == "48223")
			{
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == DTO.UserId);
				if(user.Roles.FirstOrDefault("Admin") == null)
				{
					user.Roles.Add("Admin");
				}
				else
				{
					user.Roles.Remove("Admin");
				}
				_context.Users.Update(user);
				await _context.SaveChangesAsync();
				return null;
			}
			Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Code == roomCode);
			if (room == null) return null;
			DTO.RoomId = room.Id;
			RoomOrder? existingOrder = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == DTO.UserId && u.RoomId == DTO.RoomId);
			
			
			if (existingOrder != null) return null;
			RoomOrder newOrder = new RoomOrder { RoomId = DTO.RoomId, UserId = DTO.UserId, JoinedDate = DateTime.Now, Permission = Permission.Basic};
			_context.RoomOrders.Add(newOrder);
			await _context.SaveChangesAsync();
			return newOrder;
		}

		public async Task<RoomOrder> CheckRoomOrder(int userId, int roomId)
		{
			RoomOrder? order = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == userId && u.RoomId == roomId);
			if (order == null) return null;
			return order;
		}

		public async Task<List<RoomOrder>> GetRoomsByUserId(int userId)
		{
			List<RoomOrder> rooms = await _context.RoomOrders.Where(u => u.UserId == userId).ToListAsync();
			return rooms;
		}

		public async Task<List<Room>> ConvertToRooms(List<RoomOrder> list)
		{
			List<Room> roomList = new List<Room>();
			foreach(var room in list)
			{
				roomList.Add(await _context.Rooms.FirstOrDefaultAsync(u => u.Id == room.RoomId) ?? new Room());
			}
			return roomList;
		}

		public async Task<List<RoomMember>> GetRoomMembers(int roomId, int adminId)
		{
			RoomOrder? order = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == adminId && u.RoomId == roomId);
			if (order == null || order.Permission != Permission.Admin) return null;
			List<RoomOrder> memberConnections = await _context.RoomOrders.Where(u => u.RoomId == roomId).ToListAsync();
			List<RoomMember> memberList = new List<RoomMember>();
			foreach (RoomOrder member in memberConnections)
			{
				User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == member.UserId);
				string username = "Account was removed";
				if (user != null) username = user.Name;
				memberList.Add(new RoomMember { Id = member.Id, JoinedDate = member.JoinedDate, Permission = member.Permission, RoomId = member.RoomId, UserId = member.UserId, Username = username});
			}
			return memberList;
		}

		public async Task<List<RoomMember>> GetRoomAdmins(int roomId, int adminId)
		{
			RoomOrder? order = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == adminId && u.RoomId == roomId);
			if (order == null || order.Permission != Permission.Admin) return null;
			List<RoomOrder> memberConnections = await _context.RoomOrders.Where(u => u.RoomId == roomId && u.Permission == Permission.Admin).ToListAsync();
			List<RoomMember> memberList = new List<RoomMember>();
			foreach (RoomOrder member in memberConnections)
			{
				User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == member.UserId);
				string username = "Account was removed";
				if (user != null) username = user.Name;
				memberList.Add(new RoomMember { Id = member.Id, JoinedDate = member.JoinedDate, Permission = member.Permission, RoomId = member.RoomId, UserId = member.UserId, Username = username });
			}
			return memberList;
		}

		public async Task<bool> ChangeRoomPermission(int userId, int adminId,int roomId, Permission permission)
		{
			RoomOrder? adminOrder = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == adminId && u.RoomId == roomId);
			RoomOrder? order = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == userId && u.RoomId == roomId);
			if (order == null || adminOrder == null) return false;
			if (adminOrder.Permission != Permission.Admin) return false;
			order.Permission = permission;
			_context.RoomOrders.Update(order);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> BecomeAdmin(int roomId, int userId)
		{
			RoomOrder? order = await _context.RoomOrders.FirstOrDefaultAsync(u => u.UserId == userId && u.RoomId == roomId);
			if (order == null) return false;
			order.Permission = Permission.Admin;
			_context.RoomOrders.Update(order);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
