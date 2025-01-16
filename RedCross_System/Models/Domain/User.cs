using System;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.Models.Domain
{
	public class User
	{

		[Key]
		public int Id { get; set; }

		public string Name { get; set; }
		
		public string Email { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }

		public Role Role { get; set; }
		public int RoleId { get; set; }
		public string ?PasswordResetToken { get; set; } 
		public DateTime? PasswordResetTokenExpiration { get; set; } 

	}
}
