using System.ComponentModel.DataAnnotations;

namespace RedCross_System.Views.Login
{
	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email address")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Token { get; set; }
	}
}
