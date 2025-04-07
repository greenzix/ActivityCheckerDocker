using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ActivityCheckerApi.Models.ViewModels
{
	public class ActivityViewModel
	{
		private readonly AppDbContext _context;

		public ActivityViewModel(AppDbContext context)
		{
			_context = context;
		}

		public async Task<bool> JoinRoom(int roomId,int userId, string roomCode)
		{
			Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Id == roomId);
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (room == null || user == null) return false;

			if(roomCode != room.Code) return false;
			List<RoomPresence>? existingPresence = await _context.RoomPresences.Where(u => u.UserId == user.Id).ToListAsync();
			if(existingPresence.Count >= 1) return false;

			DateTime now = DateTime.Now;
			DateTime periodEnd = TimeHelper.GetNextPeriodEnd(now);

			Activity activity = new Activity 
			{
				UserId = userId,
				RoomId = roomId,
				Action = Action.Join,
				Date = DateTime.Now
			};

			RoomPresence presence = new RoomPresence 
			{
				JoinedAt = DateTime.Now,
				RoomId = room.Id,
				UserId = user.Id,
				//expire time
				ExpiresAt = periodEnd
			};
			_context.Activities.Add(activity);
			_context.RoomPresences.Add(presence);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> LeaveRoom(int roomId, int userId)
		{
			RoomPresence? presence = await _context.RoomPresences.FirstOrDefaultAsync(u => u.UserId == userId && u.RoomId == roomId);
			if (presence == null) return false;

			Activity activity = new Activity
			{
				UserId = userId,
				RoomId = roomId,
				Action = Action.Leave,
				Date = DateTime.Now
			};

			_context.RoomPresences.Remove(presence);
			_context.Activities.Add(activity);

			await _context.SaveChangesAsync();
			return true;
		}


		public async Task<List<RoomPresence>> GetUsersInRoom(int roomId)
		{
			List<RoomPresence>? presence = await _context.RoomPresences.Where(u => u.RoomId == roomId).ToListAsync();
			return presence;
		}

		public async Task<List<UserActivity>> GetUserActivity(int userId)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null) return null;
			List<Activity>? activitys = new List<Activity>();
			List<UserActivity> userActivitys = new List<UserActivity>();
			activitys = await _context.Activities.Where(u => u.UserId == userId).ToListAsync();
			foreach (var ac in activitys)
			{
				Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Id == ac.RoomId);
				userActivitys.Add(new UserActivity { Id = ac.Id, Action = ac.Action, Date = ac.Date, RoomId = ac.RoomId, UserId = ac.UserId, RoomName = room.Name ?? "" });
			}
			return userActivitys;
		}

		public async Task<List<TeacherActivity>> GetTeacherActivity()
		{
			List<Activity>? activitys = new List<Activity>();
			List<TeacherActivity> teacherActivites = new List<TeacherActivity>();
			activitys = await _context.Activities.ToListAsync();
			foreach (var ac in activitys)
			{
				Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Id == ac.RoomId);
				User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == ac.UserId);

				string roomName = "Room was removed";
				string username = "User was removed";
				if (room != null) roomName = room.Name;
				if (user != null) username = user.Name;
				teacherActivites.Add(new TeacherActivity { Id = ac.Id, Action = ac.Action, Date = ac.Date, RoomId = ac.RoomId, UserId = ac.UserId, RoomName = roomName, Username = username});
			}
			return teacherActivites;
		}

		public async Task<List<RoomActivity>> GetRoomactivity(int adminId,int roomId)
		{
			RoomOrder? order = await _context.RoomOrders.FirstOrDefaultAsync(u => u.RoomId == roomId && u.UserId == adminId);
			//Ce ni amdin sobe returtna null
			if (order == null || order.Permission != Permission.Admin) return null;

			List<Activity> activities = new List<Activity>();
			List<RoomActivity> roomActivitys = new List<RoomActivity>();
			activities = await _context.Activities.Where(u => u.RoomId == roomId).ToListAsync();
			foreach (var ac in activities)
			{
				User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == ac.UserId);
				string username = "";
				if (user != null) username = user.Name;
				else username = "user was removed";
				roomActivitys.Add(new RoomActivity { Id = ac.Id, Action = ac.Action, Date = ac.Date, RoomId = ac.RoomId, UserId = ac.UserId, Username = username });
			}
			return roomActivitys;
		}
	}
}
