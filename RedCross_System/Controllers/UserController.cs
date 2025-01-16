using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RedCross_System.Controllers
{
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;

		public UserController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			var roles = await _context.Roles.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			var vm = new UserAddViewModel
			{
				Roles = roles
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Add(UserAddViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
				foreach (var error in errors)
				{
					Console.WriteLine(error);
				}

				var roles = await _context.Roles.Select(x => new SelectListItem()
				{
					Value = x.Id.ToString(),
					Text = x.Name,
				}).ToListAsync();

				vm.Roles = roles;
				return View(vm);
			}

			if (await _context.Users.AnyAsync(u => u.Email == vm.Email))
			{
				ModelState.AddModelError("Email", "This email is already in use.");

				var roles = await _context.Roles.Select(x => new SelectListItem()
				{
					Value = x.Id.ToString(),
					Text = x.Name,
				}).ToListAsync();

				vm.Roles = roles;
				return View(vm);
			}

			var role = await _context.Roles.FindAsync(int.Parse(vm.Role));
			if (role == null)
			{
				ModelState.AddModelError("", "The selected role was not found.");
				return View(vm);
			}

			var user = new User()
			{
				Name = vm.Name,
				Email = vm.Email,
				Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
				Phone = vm.Phone,
				Role = role,
			};

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = await _context.Users
					.Include(x => x.Role)
					.Select(x => new UserIndexViewModel()
					{
						Id = x.Id,
						Name = x.Name,
						Email = x.Email,
						Role = x.Role.Name,
					})
					.ToListAsync();

			return View(users);
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var user = await _context.Users
					.Include(x => x.Role)
					.FirstOrDefaultAsync(x => x.Id == id);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var roles = await _context.Roles.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			var vm = new UserUpdateViewModel()
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				Password = user.Password,
				Role = user.Role.Id.ToString(),
				Roles = roles
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Update(UserUpdateViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				vm.Roles = await _context.Roles.Select(x => new SelectListItem()
				{
					Value = x.Id.ToString(),
					Text = x.Name,
				}).ToListAsync();

				return View(vm);
			}

			var user = await _context.Users.Include(x => x.Role)
					.FirstOrDefaultAsync(x => x.Id == vm.Id);

			if (user == null)
			{
				return NotFound("User not found");
			}

			var role = await _context.Roles.FindAsync(int.Parse(vm.Role));
			if (role == null)
			{
				ModelState.AddModelError(nameof(vm.Role), "Invalid role selected.");
				vm.Roles = await _context.Roles.Select(x => new SelectListItem()
				{
					Value = x.Id.ToString(),
					Text = x.Name,
				}).ToListAsync();

				return View(vm);
			}

			user.Name = vm.Name;
			user.Email = vm.Email;

			if (!string.IsNullOrEmpty(vm.Password))
			{
				user.Password = BCrypt.Net.BCrypt.HashPassword(vm.Password);
			}

			user.Role = role;

			_context.Users.Update(user);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> IsEmailUnique(string email)
		{
			var isUnique = !await _context.Users.AnyAsync(u => u.Email == email);
			return Json(isUnique);
		}
	}
}
