using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Branch;

namespace RedCross_System.Controllers;
public class BranchController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly SessionHelper _sessionHelper;

	public BranchController(ApplicationDbContext context, SessionHelper sessionHelper)
	{
		_context = context;
		_sessionHelper = sessionHelper;


	}
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var branches = await _context.Branches
			.Include(x => x.Province)
			.ThenInclude(x => x.Country)
			.Include(x => x.CreatedBy)
			.Select(x => new BranchIndexViewModel()
			{
				BranchId = x.BranchId,
				CreatedBy = x.CreatedBy.Name,
				CreatedDate = x.CreatedDate,
				Location = x.Location,
				BranchName = x.BranchName,
				Province = x.Province.Name,
				Country = x.Province.Country.Name,
				Status = x.Status,
			}).ToListAsync();
		return View(branches);
	}

	[HttpPost]
	public async Task<IActionResult> ToggleStatus(int id)
	{
		var branch = await _context.Branches.FindAsync(id);
		if (branch == null) throw new Exception("Branch Not Found");

		branch.Status = branch.Status == "Active" ? "Inactive" : "Active";

		_context.Branches.Update(branch);
		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}


	[HttpGet]
	public async Task<IActionResult> Add()
	{
		var provinces = await _context.Provinces.Select(x => new SelectListItem()
		{
			Value = x.Id.ToString(),
			Text = x.Name,
		}).ToListAsync();

		var countries = await _context.Countries.Select(x => new SelectListItem()
		{
			Value = x.Id.ToString(),
			Text = x.Name,
		}).ToListAsync();

		var vm = new BranchAddViewModel();
		vm.Provinces = provinces;
		vm.Countries = countries;
		return View(vm);
	}


	[HttpPost]
	public async Task<IActionResult> Add(BranchAddViewModel vm)
	{
		if (!ModelState.IsValid)
		{
			var provinces = await _context.Provinces.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			var countries = await _context.Countries.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();


			vm.Provinces = provinces;
			vm.Countries = countries;
			return View(vm);
		}

		var currentUser = await _sessionHelper.CurrentUser();
		if (currentUser is null) throw new Exception("Current User Not FOund");

		var province = await _context.Provinces.FindAsync(int.Parse(vm.Province));
		if (province is null) throw new Exception("Province not found");

		var country = await _context.Countries.FindAsync(int.Parse(vm.Country));
		if (country is null) throw new Exception("Country not found");

		Branch branch = new()
		{
			BranchName = vm.BranchName,
			CreatedBy = currentUser,
			Location = vm.Location,
			Province = province,
			Country = country,
		};
		await _context.Branches.AddAsync(branch);
		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> Update(int id)
	{
		var branch = await _context.Branches
			.Include(x => x.Province)
			.FirstOrDefaultAsync(x => x.BranchId == id);

		if (branch == null)
		{
			return NotFound("Branch not found");
		}

		var provinces = await _context.Provinces.Select(x => new SelectListItem()
		{
			Value = x.Id.ToString(),
			Text = x.Name,
		}).ToListAsync();

		var countries = await _context.Countries.Select(x => new SelectListItem()
		{
			Value = x.Id.ToString(),
			Text = x.Name,
		}).ToListAsync();

		var vm = new BranchUpdateViewModel()
		{
			BranchId = branch.BranchId,
			BranchName = branch.BranchName,
			Location = branch.Location,
			Provinces = provinces,
			Status = branch.Status,
			Countries = countries
		};
		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Update(BranchUpdateViewModel vm)
	{
		if (!ModelState.IsValid)
		{
			vm.Provinces = await _context.Provinces.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			vm.Countries = await _context.Countries.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			return View(vm);
		}

		var branch = await _context.Branches.FindAsync(vm.BranchId);

		if (branch == null)
		{
			return NotFound("Branch not found");
		}

		var province = await _context.Provinces.FindAsync(int.Parse(vm.Province));
		if (province == null)
		{
			ModelState.AddModelError(nameof(vm.Province), "Invalid Province Selected");

			vm.Provinces = await _context.Provinces.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			vm.Countries = await _context.Countries.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			return View(vm);
		}

		var country = await _context.Countries.FindAsync(int.Parse(vm.Country));
		if (country == null)
		{
			ModelState.AddModelError(nameof(vm.Country), "Invalid Country Selected");

			vm.Provinces = await _context.Provinces.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			vm.Countries = await _context.Countries.Select(x => new SelectListItem()
			{
				Value = x.Id.ToString(),
				Text = x.Name,
			}).ToListAsync();

			return View(vm);
		}

		branch.BranchName = vm.BranchName;
		branch.Location = vm.Location;
		branch.Status = vm.Status;
		branch.Province = province;
		branch.Country = country;

		_context.Branches.Update(branch);
		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}


}


