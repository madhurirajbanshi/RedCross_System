using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using RedCross_System.Models.Domain;
using System.Security.Claims;
using RedCross_System.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RedCross_System.Data;

namespace RedCross_System.Helpers;

public class SessionHelper
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IHttpContextAccessor _httpContextAccessor;
	public SessionHelper(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
	{
		_dbContext = dbContext;
		_httpContextAccessor = httpContextAccessor;
	}
	public async Task SetSessionAsync(User user)
	{
		var httpContext = _httpContextAccessor.HttpContext;

		httpContext.Session.SetString("Username", user.Name);
		httpContext.Session.SetString("UserId", user.Id.ToString());
		httpContext.Session.SetString("Role", user.Role.Name);

		var claims = new List<Claim>
		{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Role, user.Role.Name)
		};
		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var authProperties = new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
		};

		await httpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity),
				authProperties);
	}

	public async Task<User> CurrentUser()
	{
		var httpContext = _httpContextAccessor.HttpContext;
		var userId = httpContext.Session.GetString("UserId");
		if (userId == null)
			throw new Exception("Invalid Session");

		var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == int.Parse(userId)) ?? throw new Exception("User Not Found");
		return user;
	}



}
