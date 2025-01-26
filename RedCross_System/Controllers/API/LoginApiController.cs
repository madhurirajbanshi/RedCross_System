using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Login;
using System.Security.Claims;
using System.Threading.Tasks;
using RedCross_System.Data;
using RedCross_System.Helpers;

namespace RedCross_System.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginApiController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public LoginApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] IndexViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { message = "Invalid input." });
			}

			var user = await _context.Users
																.Include(u => u.Role)
																.FirstOrDefaultAsync(u => u.Name == model.UserName);

			if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
			{
				return Unauthorized(new { message = "Invalid username or password." });
			}

			if (user.Role == null)
			{
				return BadRequest(new { message = "User role is not defined." });
			}

			var claims = new List<Claim>
						{
								new Claim(ClaimTypes.Name, user.Name),
								new Claim(ClaimTypes.Role, user.Role.Name)
						};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			return Ok(new
			{
				message = "Login successful",
				role = user.Role.Name
			});
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok(new { message = "Logout successful." });
		}

		[HttpGet("status")]
		public IActionResult GetLoginStatus()
		{
			var user = HttpContext.User;
			if (user.Identity.IsAuthenticated)
			{
				var userName = user.Identity.Name;
				var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

				return Ok(new
				{
					message = "User is logged in",
					userName = userName,
					role = userRole
				});
			}

			return Unauthorized(new { message = "User is not logged in" });
		}
	}
}
