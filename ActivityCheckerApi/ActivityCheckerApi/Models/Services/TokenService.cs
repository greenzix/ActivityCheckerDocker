using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ActivityCheckerApi.Models.Services
{
	public class TokenService
	{
		private readonly IConfiguration Configuration;

		public TokenService(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		public JwtSecurityToken GenerateAccessToken(User user)
		{
			// Create user claims
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim("userId",user.Id.ToString())
			};

			claims.AddRange(user.Roles.Select(u => new Claim(ClaimTypes.Role, u)));

			// Create a JWT
			var token = new JwtSecurityToken(
				issuer: Configuration["JwtSettings:Issuer"],
				audience: Configuration["JwtSettings:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(7), // Token expiration time
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"])),
					SecurityAlgorithms.HmacSha256)
			);

			return token;
		}

		public string GenerateRefreshToken()
		{

			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
			}

			return Convert.ToBase64String(randomNumber);
		}
	}
}
