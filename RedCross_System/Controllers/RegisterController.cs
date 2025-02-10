using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel;
using RedCross_System.ViewModel.Register;

public class RegisterController : Controller
{
	private readonly ApplicationDbContext _context;

	public RegisterController(ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public IActionResult Index()
	{
		var roles = _context.Roles.Select(r => new SelectListItem
		{
			Value = r.Id.ToString(),
			Text = r.Name
		}).ToList();

		var bloodtypes = _context.BloodTypes.Select(r => new SelectListItem
		{
			Value = r.Id.ToString(),
			Text = r.Name
		}).ToList();

		var model = new RegisterViewModel
		{
			Roles = roles,
			BloodTypes=bloodtypes
		};

		return View(model);
	}


	[HttpPost]
	public async Task<IActionResult> Index(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			if (model.Password != model.ConfirmPassword)
			{
				ModelState.AddModelError("", "Password confirmation doesn't match password.");
				return View(model);
			}

			var existingUser = await _context.Users
																				.FirstOrDefaultAsync(u => u.Name == model.UserName);

			if (existingUser != null)
			{
				ModelState.AddModelError("", "Username already exists. Please choose a different one.");
				return View(model);
			}

			var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
			var bloodtype = await _context.BloodTypes.FirstOrDefaultAsync(r => r.Id == model.BloodTypeId);

			if (role == null|| bloodtype==null)
			{
				ModelState.AddModelError("", "Invalid Role selected.");
				return View(model);
			}

			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

			var newUser = new User
			{
				Name = model.UserName,
				Password = hashedPassword,
				Email = model.Email,
				Phone = model.Phone,
				RoleId = model.RoleId,
				BloodTypeId = model.BloodTypeId
			};

			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();


			return RedirectToAction("Index", "Login");
		}
		ViewData["Error"] = "Please fix the errors below.";
		return View(model);
	}


}
