using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ActivityCheckerApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class ActivityController : ControllerBase
	{
		private readonly ActivityViewModel vm;

		public ActivityController(ActivityViewModel vm)
		{
			this.vm = vm;
		}

		[HttpPost("JoinRoom")]
		public async Task<IActionResult> JoinRoom(int roomId, string roomCode)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			bool result = await vm.JoinRoom(roomId, userId, roomCode);
			if (!result) return BadRequest();
			return Ok("Room joined");
		}

		[HttpPost("LeaveRoom")]
		public async Task<IActionResult> LeaveRoom(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			bool result = await vm.LeaveRoom(roomId,userId);
			if (!result) return BadRequest();
			return Ok("Room left");
		}

		[HttpGet("GetUsersInRoom")]
		public async Task<IActionResult> GetUsersInRoom(int roomId)
		{
			return Ok(await vm.GetUsersInRoom(roomId));
		}

		[HttpGet("GetUserHistory")]
		public async Task<IActionResult> GetUserHistory()
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			List<UserActivity>? activitys = await vm.GetUserActivity(userId);

			if (activitys == null) return BadRequest();
			return Ok(activitys);
		}

		[HttpGet("GetTeacherHistory")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> GetTeacherHistory()
		{
			return Ok(await vm.GetTeacherActivity());
		}

		[HttpGet("GetStudentHistory")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> GeStudentHistoryy()
		{
			return Ok(await vm.GetTeacherActivity());
		}

		[HttpGet("GetRoomHistory")]
		public async Task<IActionResult> GetRoomHistory(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			List<RoomActivity>? activitys = await vm.GetRoomactivity(userId, roomId);
			if (activitys == null) return BadRequest();

			return Ok(activitys);
		}
	}
}
