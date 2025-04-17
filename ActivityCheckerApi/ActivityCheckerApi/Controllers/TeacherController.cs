using ActivityCheckerApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityCheckerApi.Controllers
{
	[ApiController]
	[Authorize]
	[Route("[controller]")]
	public class TeacherController : ControllerBase
	{
		private TeacherViewModel vm;

		public TeacherController(TeacherViewModel vm)
		{
			this.vm = vm;
		}

		[HttpGet("GetAllStudent")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> GetAllStudents()
		{
			return Ok(await vm.GetStudentListAsync());
		}

		[HttpGet("GetHistoryForStudent")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> GetHistoryForStudent(int studentId)
		{
			return Ok(await vm.GetActivityForStudent(studentId));
		}
	}
}
