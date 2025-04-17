using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.Services;
using ActivityCheckerApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ActivityCheckerApi.Controllers
{
	[ApiController]
	[Authorize]
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private UserViewModel vm;

		public UserController(UserViewModel vm)
		{
			this.vm = vm;

		}

		[HttpGet("GetAllUsers")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> GetAllUsers()
		{
			return Ok(await vm.GetAllUsers());
		}

		[HttpGet("GetUserById")]
		public async Task<IActionResult> GetUserById(int userId)
		{
			User? user = await vm.GetUserbyId(userId);
			if (user == null) return BadRequest();
			return Ok(user);
		}

		[HttpGet("GetRoomPresenceForUser")]
		public async Task<IActionResult> GetRoomPresenceForUser()
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			RoomPresence? presence = await vm.GetRoomPresenceForUser(userId);
			if (presence == null) return BadRequest();
			return Ok(presence);
		}

		[HttpPut("AddRole")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> AddRoleForUser(string username, string role)
		{
			bool result = await vm.AddRole(username, role);
			if (!result) return BadRequest();
			return Ok("Role Added");
		}


		[HttpPut("RemoveRole")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> RemoveRollForUser(string username, string role)
		{
			bool result = await vm.RemoveRoll(username, role);
			if (!result) return BadRequest();
			return Ok("Role Removed");
		}

		[HttpPost("SetTablet")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> SetTabletUser(string username)
		{
			bool result = await vm.SetTablet(username);
			if (!result) BadRequest();
			return Ok("Tablet set");
		}

		[HttpPost("SetUser")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> SetUser(string username)
		{
			bool result = await vm.SetUser(username);
			if (!result) BadRequest();
			return Ok("User set");
		}

		[HttpDelete("RemoveUser")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> RemoveUser(int userId)
		{
			await vm.RemoveUser(userId);
			return Ok();
		}

		
	}
}
