using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.Services;
using ActivityCheckerApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ActivityCheckerApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthController : ControllerBase
	{
		private UserViewModel vm;
		private readonly TokenService tokenService;
		private readonly PasswordResetTokenGenerator passwordResetTokenGenerator;

		public AuthController(UserViewModel vm, TokenService tk, PasswordResetTokenGenerator prtg)
		{
			this.vm = vm;
			tokenService = tk;
			passwordResetTokenGenerator = prtg;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(UserDTO DTO)
		{
			User? result = await vm.Login(DTO);
			if (result == null) return BadRequest("Login failed");
			var token = tokenService.GenerateAccessToken(result);
			return Ok(new { AccessToken = new JwtSecurityTokenHandler().WriteToken(token) });
		}

		[HttpPost("CreateUser")]
		public async Task<IActionResult> CreateUser(UserDTO DTO)
		{
			User user = await vm.CreateUser(DTO);
			if (user == null) return BadRequest();
			return Ok(user);
		}

		[HttpGet("CreateResetToken")]
		[Authorize(Policy = "AdminPolicy")]
		public IActionResult CreateResetToken(string username)
		{
		
			return Ok(passwordResetTokenGenerator.GenerateToken(username));
		}

		[HttpGet("ValidateResetToken")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> ValidateResetToken(string token)
		{
			int userId = Int32.Parse(User.FindFirstValue("userId"));
			bool result = await vm.ValidateToken(userId, token);
			if (!result) return BadRequest("Token not valid");
			return Ok("Token is valid");
		}

		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword(string username, string newPassword, string token)
		{
			bool result = await vm.ChangePassword(username, newPassword, token);
			if (!result) return BadRequest();
			return Ok();
		}

	}
}
