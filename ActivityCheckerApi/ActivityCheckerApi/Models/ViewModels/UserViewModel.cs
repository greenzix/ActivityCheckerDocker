using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.DTO;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace ActivityCheckerApi.Models.ViewModels
{
	public class UserViewModel
	{
		private AppDbContext _context;
		private PasswordResetTokenGenerator passwordResetTokenGenerator;

		public UserViewModel(AppDbContext ctx, PasswordResetTokenGenerator pr)
		{
			_context = ctx;
			passwordResetTokenGenerator = pr;
		}

		public async Task<User> CreateUser(UserDTO DTO)
		{
			User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == DTO.Name);
			if (existingUser != null) return null;
			User newUser = new User { Name = DTO.Name, JoinedDate = DateTime.Now, Password = Hasher.EncodePasswordToBase64(DTO.Password), Roles = { "User" } };
			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();
			return newUser;
		}

		public async Task<List<User>> GetAllUsers()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<User> GetUserbyId(int userId)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? null;
		}

		public async Task<RoomPresence> GetRoomPresenceForUser(int userId)
		{
			return await _context.RoomPresences.FirstOrDefaultAsync(u => u.UserId == userId) ?? null;
		}

		public async Task<User?> Login(UserDTO DTO)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == DTO.Name);
			if (user == null) return null;

			if (string.IsNullOrEmpty(user.Password) || DTO.Password != Hasher.DecodeFrom64(user.Password))
			{
				return null;
			}

			return user;
		}

		public async Task RemoveUser(int userId)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
			List<RoomOrder>? orders = await _context.RoomOrders.Where(u => u.UserId == userId).ToListAsync();
			List<RoomPresence> presence = await _context.RoomPresences.Where(u => u.UserId == userId).ToListAsync();
			if (user != null) _context.Users.Remove(user);
			if (orders != null) _context.RoomOrders.RemoveRange(orders);
			if (presence != null) _context.RoomPresences.RemoveRange(presence);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> ChangePassword(string username, string newPassword, string token)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
			if (user == null) return false;
			bool validate = passwordResetTokenGenerator.ValidateToken(user.Name, token);
			if (!validate) return false;
			user.Password = Hasher.EncodePasswordToBase64(newPassword);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> AddRole(string username, string role)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
			if (user == null) return false;
			user.Roles.Add(role);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RemoveRoll(string username, string role)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
			if (user == null) return false;
			user.Roles.Remove(role);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ValidateToken(int userId, string token)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null) return false;
			return passwordResetTokenGenerator.ValidateToken(user.Name,token);
		}

		public async Task<bool> SetTablet(string username)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
			if (user == null) return false;
			user.Roles.Clear();
			user.Roles.Add("Tablet");
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> SetUser(string username)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
			if (user == null) return false;
			user.Roles.Clear();
			user.Roles.Add("User");
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

	}
}
