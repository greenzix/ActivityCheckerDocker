using System.Security.Cryptography;
using System.Text;

namespace ActivityCheckerApi.Models.Services
{
	public class PasswordResetTokenGenerator
	{
		private const string SecretKey = "123321";
		private const int TimeWindowMinutes = 10; 

		public string GenerateToken(string username)
		{
			var timestamp = GetCurrentTimestamp();
			var tokenValue = GenerateTokenValue(username, timestamp);
			return ConvertToBase62(tokenValue).PadLeft(6, '0')[..6];
		}

		public bool ValidateToken(string username, string token)
		{
			var current = GetCurrentTimestamp();
			return CheckToken(username, token, current) ||
				   CheckToken(username, token, current - 1);
		}

		private static long GetCurrentTimestamp()
		{
			var totalMinutes = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60;
			return totalMinutes / TimeWindowMinutes;
		}

		private static uint GenerateTokenValue(string username, long timestamp)
		{
			using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
			var message = $"{username}|{timestamp}";
			var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
			return BitConverter.ToUInt32(hash, 0);
		}


		private static bool CheckToken(string username, string token, long timestamp)
		{
			var expectedValue = GenerateTokenValue(username, timestamp);
			return ConvertToBase62(expectedValue).PadLeft(6, '0')[..6] == token;
		}

		private static string ConvertToBase62(uint number)
		{
			const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			char[] buffer = new char[6];

			for (int i = 5; i >= 0; i--)
			{
				buffer[i] = chars[(int)(number % 62)];
				number /= 62;
			}
			return new string(buffer);
		}
	}
}
