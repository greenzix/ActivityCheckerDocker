using ActivityCheckerApi.DbContext;
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
		public async Task<List<Activity>> GetActivityForStudent(int studentId) => await _context.Activities.Where(u => u.UserId == studentId).ToListAsync();
	}
}
