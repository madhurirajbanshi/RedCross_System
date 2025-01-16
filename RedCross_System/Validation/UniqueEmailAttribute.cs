using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Controllers;
using RedCross_System.Data;

namespace RedCross_System.Validation
{
	public class UniqueEmailAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var context = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

			if (context == null)
			{
				return new ValidationResult("Database context could not be resolved.");
			}

			var email = value as string;

			if (string.IsNullOrEmpty(email))
			{
				return ValidationResult.Success;
			}

			var userExists = context.Users.Any(u => u.Email == email);

			if (userExists)
			{
				return new ValidationResult(ErrorMessage ?? "Email is already in use.");
			}

			return ValidationResult.Success;
		}
	}
}