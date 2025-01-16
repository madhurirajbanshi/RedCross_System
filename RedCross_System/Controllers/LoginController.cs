using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Login;
using System.Security.Claims;
using RedCross_System.Helpers;
using RedCross_System.Data;
using RedCross_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace RedCross_System.Controllers
{
	public class LoginController : Controller
	{
		private readonly IEmailSender _emailSender;
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public LoginController(ApplicationDbContext context, SessionHelper sessionHelper, IEmailSender emailSender)
		{
			_context = context;
			_sessionHelper = sessionHelper;
			_emailSender = emailSender;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(IndexViewModel model)
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
						ModelState.AddModelError("", "User role is not defined.");
						return View(model);
					}

					await _sessionHelper.SetSessionAsync(user);

					switch (user.Role.Name)
					{
						case "ProvinceUser":
						case "BranchUser":
						case "DistrictUser":
						case "SuperAdmin":
							return RedirectToAction("Index", "Home");
						default:
							ModelState.AddModelError("", "Invalid role type.");
							return View(model);
					}
				}
				else
				{
					ModelState.AddModelError("", "Invalid username or password.");
					ViewData["Error"] = "Invalid username or password.";
					return View(model);
				}
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			HttpContext.Session.Clear();

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Index", "Login");
		}

		[HttpGet]
		public IActionResult Logout(string returnUrl = null)
		{
			return RedirectToAction("Index", "Login");
		}

		public IActionResult ForgotPassword()
		{
			return  View("ForgotPassword");
		}
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

				if (user == null)
				{
					ModelState.AddModelError("", "No user found with this email address.");
					return View(model);
				}

				var token = Guid.NewGuid().ToString();
				user.PasswordResetToken = token;
				user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);
				await _context.SaveChangesAsync();

				var resetLink = Url.Action("ResetPassword", "Login", new { token = token }, protocol: Request.Scheme);
				var emailBody = $"Please click <a href='{resetLink}'>here</a> to reset your password.";

				await _emailSender.SendEmailAsync(model.Email, "Password Reset Request", emailBody);

				return RedirectToAction("ForgotPasswordConfirmation");
			}

			return View(model);
		}
	}
}