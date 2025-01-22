using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Login;
using System.Security.Claims;
using RedCross_System.Helpers;
using RedCross_System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;

namespace RedCross_System.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginApiController : ControllerBase
	{
		private readonly IEmailSender _emailSender;
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public LoginApiController(ApplicationDbContext context, SessionHelper sessionHelper, IEmailSender emailSender)
		{
			_context = context;
			_sessionHelper = sessionHelper;
			_emailSender = emailSender;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] IndexViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _context.Users
								.Include(u => u.Role)
								.FirstOrDefaultAsync(u => u.Name == model.UserName);

				if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
				{
					if (user.Role == null)
					{
						return BadRequest(new { message = "User role is not defined." });
					}

					// You can return a token here (JWT or session token)
					var claims = new List<Claim>
										{
												new Claim(ClaimTypes.Name, user.Name),
												new Claim(ClaimTypes.Role, user.Role.Name)
										};
					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

					// Return a success response along with role info (optional)
					return Ok(new
					{
						message = "Login successful",
						role = user.Role.Name
					});
				}
				else
				{
					return Unauthorized(new { message = "Invalid username or password." });
				}
			}

			return BadRequest(new { message = "Invalid input." });
		}

		// API endpoint for Logout (POST)
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok(new { message = "Logout successful." });
		}

	
	}
}
