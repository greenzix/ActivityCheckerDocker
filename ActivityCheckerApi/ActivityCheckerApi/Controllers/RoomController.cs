using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models;
using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace ActivityCheckerApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class RoomController : ControllerBase
	{
		private readonly RoomViewModel vm;

		public RoomController(RoomViewModel _vm) { vm = _vm; }

		[HttpPost("CreateRoom")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> CreateRoom(RoomDTO DTO)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			Room result = await vm.CreateRoom(DTO, userId);
			if (result == null) return BadRequest();
			return Ok(result);
		}

		[HttpDelete("RemoveRoom")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> RemoveRoom(string roomName)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			bool result = await vm.DeleteRoomById(roomName);
			if (!result) return BadRequest();
			return Ok("Room removed");
		}

		[HttpGet("GetAllRooms")]
		[Authorize(Policy = "TeacherPolicy")]
		public async Task<IActionResult> GetAllRooms()
		{
			List<Room> result = await vm.GetAllRooms();
			return Ok(result);
		}

		[HttpPost("ConvertOrderToRoom")]
		public async Task<IActionResult> ConvertToRooms(List<RoomOrder> list)
		{
			return Ok(await vm.ConvertToRooms(list));
		}

		[HttpGet("CheckRoomOrder")]
		public async Task<IActionResult> CheckRoomOrder(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			RoomOrder? result = await vm.CheckRoomOrder(userId, roomId);
			if (result == null) return BadRequest("No room order");
			return Ok(result);
		}

		[HttpGet("GetRoomById")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> GetRoomById(int roomId)
		{
			Room? result = await vm.GetRoomById(roomId);
			if (result == null) return BadRequest();
			return Ok(result);
		}

		[HttpPost("AssignRoom")]
		public async Task<IActionResult> AssignRoom(string roomCode)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			RoomOrderDTO DTO = new RoomOrderDTO { RoomId = 0, UserId = userId };
			RoomOrder? result = await vm.AssignRoom(DTO, roomCode);
			if (result == null) return BadRequest();
			return Ok(result);
		}

		[HttpGet("GetRoomsForUser")]
		public async Task<IActionResult> GetRoomsForUser()
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			var result = await vm.GetRoomsByUserId(userId);
			return Ok(result);
		}

		[HttpPost("ChangePermission")]
		public async Task<IActionResult> ChangePermission(int userId, int roomId, Permission permission)
		{
			int adminId = Int32.Parse(User.FindFirstValue("userId"));

			bool result = await vm.ChangeRoomPermission(userId, adminId, roomId, permission);
			if(!result) return BadRequest();
			return Ok(result);
		}

		[HttpPost("GetAdmin")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> GetRoomAdmin(int roomId)
		{
			int adminId = Int32.Parse(User.FindFirstValue("userId"));
			bool result = await vm.BecomeAdmin(roomId, adminId);
			if(!result) return BadRequest();
			return Ok();
		}

		[HttpGet("GetRoomCode")]
		[Authorize(Policy = "TabletPolicy")]
		public async Task<IActionResult> GetRoomCode(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			Room? room = await vm.GetRoomCode(roomId);
			if (room == null) return BadRequest();

			return Ok(room);
		}

		[HttpGet("GetExpireTime")]
		[Authorize(Policy = "TabletPolicy")]
		public async Task<IActionResult> GetExpireTime()
		{
			return Ok(await vm.GetExpireTime());
		}


		[HttpGet("GetRoomName")]
		public async Task<IActionResult> GetRoomName(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			Room? room = await vm.GetRoomCode(roomId);
			if (room == null) return BadRequest();

			return Ok(room.Name);
		}

		[HttpGet("GetRoomMembers")]
		public async Task<IActionResult> GetRoomMembers(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			List<RoomMember>? members = await vm.GetRoomMembers(roomId, userId);
			if (members == null) return BadRequest();
			return Ok(members);
		}

		[HttpGet("GetRoomAdmins")]
		public async Task<IActionResult> GetRoomAdmins(int roomId)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));

			List<RoomMember>? members = await vm.GetRoomAdmins(roomId, userId);
			if (members == null) return BadRequest();
			return Ok(members);
		}

		[HttpDelete("KickUser")]
		public async Task<IActionResult> KickUser(int userId, int roomId)
		{
			int admin = Int32.Parse(User.FindFirstValue("userId"));

			bool result = await vm.RemoveUserFromRoom(admin, userId, roomId);

			if (!result) return BadRequest();
			return Ok();
		}
	}
}
