using Microsoft.AspNetCore.Identity;
using RedCross_System.Data;
using RedCross_System.Models.Domain;

namespace RedCross_System.Service
{
	
		public interface IUserService
		{
			Task<bool> ResetPasswordAsync(string userName, string oldPassword, string newPassword);
		}

		public class UserService : IUserService
		{
			private readonly ApplicationDbContext _context;
			private readonly UserManager<User> _userManager;

			public UserService(ApplicationDbContext context, UserManager<User> userManager)
			{
				_context = context;
				_userManager = userManager;
			}

			public async Task<bool> ResetPasswordAsync(string userName, string oldPassword, string newPassword)
			{
				var user = await _userManager.FindByNameAsync(userName);
				if (user != null)
				{
					var result = await _userManager.CheckPasswordAsync(user, oldPassword);
					if (result)
					{
						var passwordHasher = new PasswordHasher<User>();
						user.Password = passwordHasher.HashPassword(user, newPassword);
						_context.Users.Update(user);
						await _context.SaveChangesAsync();
						return true;
					}
				}
				return false;
			}
		}
}
