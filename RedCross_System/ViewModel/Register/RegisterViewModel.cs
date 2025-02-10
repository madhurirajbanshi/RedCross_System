using Microsoft.AspNetCore.Mvc.Rendering;
using RedCross_System;
using RedCross_System.ViewModel;
using RedCross_System.ViewModel.Register;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.ViewModel.Register
{
	public class RegisterViewModel
	{


		[Required]
		[StringLength(100, MinimumLength = 3)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		[StringLength(10, ErrorMessage = "Phone number cannot exceed 10 characters.")]
		public string Phone { get; set; }
		public List<SelectListItem>? Roles { get; set; }
		public int RoleId { get; set; }

		public List<SelectListItem>? BloodTypes { get; set; }
		public int BloodTypeId { get; set; }

	}
}
