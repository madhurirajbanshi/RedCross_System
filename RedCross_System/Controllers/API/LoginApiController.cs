using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using RedCross_System.Models.Domain;
using RedCross_System.Services;
using RedCross_System.Data;
using RedCross_System.ViewModel.Login;

namespace RedCross_System.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginApiController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly JwtService _jwtService;

		public LoginApiController(ApplicationDbContext context, JwtService jwtService)
		{
			_context = context;
			_jwtService = jwtService;
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

			// Generate JWT Token
			var token = _jwtService.GenerateJwtToken(user);

			return Ok(new
			{
				message = "Login successful",
				token = token,
				UserId=user.Id // Return token to the client
			});
		}
	}
}
