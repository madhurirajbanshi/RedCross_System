using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.ViewModel.User
{
	public class UserAddViewModel
	{

		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		[Remote("IsEmailUnique", "User", ErrorMessage = "Email is already in use")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		public string Phone { get; set; }
		public List<SelectListItem>? Roles { get; set; }
		public string Role { get; set; }

		public List<SelectListItem>? BloodTypes { get; set; }
		public string BloodType { get; set; }

	}
}
