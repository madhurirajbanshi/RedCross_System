using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Campaign;

namespace RedCross_System.Controllers
{
	public class CampaignController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public CampaignController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			this._sessionHelper = sessionHelper;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var campaigns = await _context.Campaigns
					.Include(x => x.Branch)
					.Include(x=>x.CreatedBy)
					.Select(x => new CampaignIndexViewModel
					{
						Id = x.Id,
						Name = x.Name,
						Address = x.Address,
						StartDate = x.StartDate,
						EndDate = x.EndDate,
						CreatedDate = x.CreatedDate,
						CreatedBy = x.CreatedBy.Name,
						Branch = x.Branch.BranchName,
						Status = x.Status
					}).ToListAsync();

			return View(campaigns);
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			var campaign = await _context.Campaigns.FindAsync(id);
			if (campaign== null) throw new Exception("Branch Not Found");

			campaign.Status = campaign.Status == "Active" ? "Inactive" : "Active";

			_context.Campaigns.Update(campaign);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			var branches = await _context.Branches.Select(x => new SelectListItem
			{
				Value = x.BranchId.ToString(),
				Text = x.BranchName
			}).ToListAsync();

			var vm = new CampaignAddViewModel
			{
				Branches = branches
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Add(CampaignAddViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.Branches = await _context.Branches.Select(x => new SelectListItem
				{
					Value = x.BranchId.ToString(),
					Text = x.BranchName
				}).ToListAsync();

				return View(viewModel);
			}
			var currentUser = await _sessionHelper.CurrentUser();
			if (currentUser == null)
			{
				ModelState.AddModelError("", "User not logged in.");
				viewModel.Branches = await _context.Branches.Select(x => new SelectListItem
				{
					Value = x.BranchId.ToString(),
					Text = x.BranchName
				}).ToListAsync();
				return View(viewModel);
			}
			var branch = await _context.Branches.FindAsync(int.Parse(viewModel.Branch));
			if (branch == null)
			{
				ModelState.AddModelError(nameof(viewModel.Branch), "Invalid branch selected.");
				viewModel.Branches = await _context.Branches.Select(x => new SelectListItem
				{
					Value = x.BranchId.ToString(),
					Text = x.BranchName
				}).ToListAsync();
				return View(viewModel);
			}


			var campaign = new Campaign
			{
				Name = viewModel.Name,
				Address = viewModel.Address,
				StartDate = viewModel.StartDate.ToUniversalTime(),
				EndDate = viewModel.EndDate.ToUniversalTime(),
				CreatedDate = viewModel.CreatedDate.ToUniversalTime(),
				CreatedBy = currentUser,
				Branch = branch
			};

			await _context.Campaigns.AddAsync(campaign);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var campaign = await _context.Campaigns
					.Include(x => x.Branch)
					.FirstOrDefaultAsync(x => x.Id == id);

			if (campaign == null)
			{
				return NotFound("Campaign not found");
			}

			var branches = await _context.Branches.Select(x => new SelectListItem
			{
				Value = x.BranchId.ToString(),
				Text = x.BranchName
			}).ToListAsync();

			var vm = new CampaignUpdateViewModel
			{
				Id = campaign.Id,
				Name = campaign.Name,
				Address = campaign.Address,
				StartDate = campaign.StartDate.ToUniversalTime(),
				EndDate = campaign.EndDate.ToUniversalTime(),
				CreatedDate = campaign.CreatedDate.ToUniversalTime(),
				CreatedBy = campaign.CreatedBy?.Name??"Unknown",
				Branch = campaign.Branch?.BranchId.ToString(),
				Branches = branches,
				Status = campaign.Status
			};

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Update(CampaignUpdateViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.Branches = await _context.Branches.Select(x => new SelectListItem
				{
					Value = x.BranchId.ToString(),
					Text = x.BranchName
				}).ToListAsync();
				return View(viewModel);
			}

			var campaign = await _context.Campaigns
					.Include(x => x.Branch)
					.FirstOrDefaultAsync(x => x.Id == viewModel.Id);

			if (campaign == null)
			{
				return NotFound("Campaign not found");
			}

			if (string.IsNullOrWhiteSpace(viewModel.Branch) || !int.TryParse(viewModel.Branch, out int branchId))
			{
				ModelState.AddModelError(nameof(viewModel.Branch), "Invalid branch selected");
				viewModel.Branches = await _context.Branches.Select(x => new SelectListItem
				{
					Value = x.BranchId.ToString(),
					Text = x.BranchName
				}).ToListAsync();
				return View(viewModel);
			}

			var branch = await _context.Branches.FindAsync(branchId);
			if (branch == null)
			{
				ModelState.AddModelError(nameof(viewModel.Branch), "Branch not found");
				viewModel.Branches = await _context.Branches.Select(x => new SelectListItem
				{
					Value = x.BranchId.ToString(),
					Text = x.BranchName
				}).ToListAsync();
				return View(viewModel);
			}

			campaign.Name = viewModel.Name;
			campaign.Address = viewModel.Address;
			campaign.StartDate = viewModel.StartDate.ToUniversalTime();
			campaign.EndDate = viewModel.EndDate.ToUniversalTime();
			campaign.CreatedDate = viewModel.CreatedDate.ToUniversalTime();
			campaign.Branch = branch;
			campaign.Status = viewModel.Status;

			_context.Campaigns.Update(campaign);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}


	}
}
