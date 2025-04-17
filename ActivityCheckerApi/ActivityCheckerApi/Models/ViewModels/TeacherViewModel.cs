using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ActivityCheckerApi.Models.ViewModels
{
	public class TeacherViewModel
	{
		private readonly AppDbContext _context;

		public TeacherViewModel(AppDbContext context)
		{
			_context = context;
		}
		

		public async Task<List<Activity>> GetAllActivity() => await _context.Activities.ToListAsync();

		public async Task<List<StudentDTO>> GetStudentListAsync()
		{
			List<StudentDTO> studentList = new();			
			List<User>? userList = await _context.Users.Where(u => !u.Roles.Contains("Teacher") && !u.Roles.Contains("Tablet")).ToListAsync();
			foreach(var student in userList)
			{
				studentList.Add(new StudentDTO { Id = student.Id, Name = student.Name});
			}
			return studentList;
		}
		
		public async Task<List<UserActivity>> GetActivityForStudent(int studentId)
		{
			List<Activity>? activities = await _context.Activities.Where(u => u.UserId == studentId).ToListAsync();
			List<UserActivity> userActivity = new();
			foreach(var activity in activities)
			{
				Room? room = await _context.Rooms.FirstOrDefaultAsync(u => u.Id == activity.RoomId);
				userActivity.Add(new UserActivity { Id = activity.Id, UserId = activity.UserId, Date = activity.Date, Action = activity.Action, RoomId = activity.RoomId, RoomName = room.Name ??= "Room deleted" });
			}
			return userActivity;
		}
		 
	}
}
