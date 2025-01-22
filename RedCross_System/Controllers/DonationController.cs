using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Donation;
using RedCross_System.ViewModels;

namespace RedCross_System.Controllers
{
	public class DonationController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public DonationController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var donations = await _context.Donations
											.Include(d => d.Donor)
											.Include(d => d.Branch)
											.Include(d => d.Campaign)
											.Include(d => d.CreatedBy)
											.Select(d => new DonationIndexViewModel
											{
												Id = d.Id,
												Quantity = d.Quantity,
												CreatedDate = d.CreatedDate,
												CreatedBy = d.CreatedBy.Name,
												Status = d.Status,
												Donor = d.Donor.Name,
												Branch = d.Branch.BranchName,
												Campaign = d.Campaign != null ? d.Campaign.Name : "No Campaign",
												DonationDate = d.DonationDate,
												ExpiryDate = d.ExpiryDate,
												BagNumber = d.BagNumber
											}).ToListAsync();

			return View(donations);
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			var donation = await _context.Donations.FindAsync(id);
			if (donation == null) throw new Exception("Donation Not Found");

			donation.Status = donation.Status == "Active" ? "Inactive" : "Active";

			_context.Donations.Update(donation);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}


		[HttpGet]
		public async Task<IActionResult> Add()
		{
			var donors = await _context.Donors.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.Name
			}).ToListAsync();

			var branches = await _context.Branches.Select(b => new SelectListItem
			{
				Value = b.BranchId.ToString(),
				Text = b.BranchName
			}).ToListAsync();

			var campaigns = await _context.Campaigns.Select(b => new SelectListItem
			{
				Value = b.Id.ToString(),
				Text = b.Name
			}).ToListAsync();

			campaigns.Insert(0, new SelectListItem { Value = "", Text = "-- Select Campaign (Optional) --" });

			var viewModel = new DonationAddViewModel
			{
				Donors = donors,
				Branches = branches,
				Campaigns = campaigns
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(DonationAddViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.Donors = await _context.Donors.Select(d => new SelectListItem
				{
					Value = d.Id.ToString(),
					Text = d.Name
				}).ToListAsync();

				viewModel.Branches = await _context.Branches.Select(b => new SelectListItem
				{
					Value = b.BranchId.ToString(),
					Text = b.BranchName
				}).ToListAsync();

				viewModel.Campaigns = await _context.Campaigns.Select(b => new SelectListItem
				{
					Value = b.Id.ToString(),
					Text = b.Name
				}).ToListAsync();

				viewModel.Campaigns.Insert(0, new SelectListItem { Value = "", Text = "-- Select Campaign (Optional) --" });

				return View(viewModel);
			}

			if (!int.TryParse(viewModel.Donor, out var donorId) || !int.TryParse(viewModel.Branch, out var branchId))
			{
				ModelState.AddModelError("", "Invalid Donor or Branch selected.");
				return View(viewModel);
			}

			var donor = await _context.Donors.FindAsync(donorId);
			var branch = await _context.Branches.FindAsync(branchId);
			Campaign? campaign = null;

			if (!string.IsNullOrEmpty(viewModel.Campaign))
			{
				if (int.TryParse(viewModel.Campaign, out var campaignId))
				{
					campaign = await _context.Campaigns.FindAsync(campaignId);
					if (campaign == null)
					{
						ModelState.AddModelError("", "Invalid Campaign selected.");
						return View(viewModel);
					}
				}
			}

			if (donor == null || branch == null)
			{
				ModelState.AddModelError("", "Invalid Donor or Branch selected.");
				return View(viewModel);
			}

			var currentUser = await _sessionHelper.CurrentUser();
			if (currentUser == null)
			{
				ModelState.AddModelError("", "User not logged in.");
				return View(viewModel);
			}

			string generatedBagNumber = "BAG-"  + new Random().Next(1000, 9999);
			var donation = new Donation
			{
				Quantity = viewModel.Quantity,
				CreatedDate = viewModel.CreatedDate,
				CreatedBy = currentUser,
				Status = viewModel.Status,
				Donor = donor,
				Branch = branch,
				Campaign = campaign,
				DonationDate = viewModel.DonationDate,
				ExpiryDate = viewModel.ExpiryDate,
				BagNumber = generatedBagNumber
			};

			_context.Donations.Add(donation);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var donation = await _context.Donations
											.Include(d => d.Donor)
											.Include(d => d.Branch)
											.Include(d => d.Campaign)
											.FirstOrDefaultAsync(d => d.Id == id);

			if (donation == null)
			{
				return NotFound("Donation not found.");
			}

			var donors = await _context.Donors.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.Name
			}).ToListAsync();

			var branches = await _context.Branches.Select(b => new SelectListItem
			{
				Value = b.BranchId.ToString(),
				Text = b.BranchName
			}).ToListAsync();

			var campaigns = await _context.Campaigns.Select(b => new SelectListItem
			{
				Value = b.Id.ToString(),
				Text = b.Name
			}).ToListAsync();

			campaigns.Insert(0, new SelectListItem { Value = "", Text = "-- Select Campaign (Optional) --" });

			var viewModel = new DonationUpdateViewModel
			{
				Id = donation.Id,
				Quantity = donation.Quantity,
				CreatedDate = donation.CreatedDate,

				Status = donation.Status,
				Donor = donation.Donor.Id.ToString(),
				Branch = donation.Branch.BranchId.ToString(),
				Campaign = donation.Campaign != null ? donation.Campaign.Id.ToString() : "",
				DonationDate = donation.DonationDate,
				ExpiryDate = donation.ExpiryDate,
				BagNumber = donation.BagNumber,
				Donors = donors,
				Branches = branches,
				Campaigns = campaigns
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(DonationUpdateViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.Donors = await _context.Donors.Select(d => new SelectListItem
				{
					Value = d.Id.ToString(),
					Text = d.Name
				}).ToListAsync();

				viewModel.Branches = await _context.Branches.Select(b => new SelectListItem
				{
					Value = b.BranchId.ToString(),
					Text = b.BranchName
				}).ToListAsync();

				viewModel.Campaigns = await _context.Campaigns.Select(b => new SelectListItem
				{
					Value = b.Id.ToString(),
					Text = b.Name
				}).ToListAsync();

				viewModel.Campaigns.Insert(0, new SelectListItem { Value = "", Text = "-- Select Campaign (Optional) --" });

				return View(viewModel);
			}

			var donation = await _context.Donations
											.Include(d => d.Donor)
											.Include(d => d.Branch)
											.Include(d => d.Campaign)
											.FirstOrDefaultAsync(d => d.Id == viewModel.Id);

			if (donation == null)
			{
				return NotFound("Donation not found.");
			}

			if (!int.TryParse(viewModel.Donor, out var donorId) || !int.TryParse(viewModel.Branch, out var branchId))
			{
				ModelState.AddModelError("", "Invalid Donor or Branch selected.");
				return View(viewModel);
			}

			var donor = await _context.Donors.FindAsync(donorId);
			var branch = await _context.Branches.FindAsync(branchId);
			Campaign? campaign = null;

			if (!string.IsNullOrEmpty(viewModel.Campaign))
			{
				if (int.TryParse(viewModel.Campaign, out var campaignId))
				{
					campaign = await _context.Campaigns.FindAsync(campaignId);
					if (campaign == null)
					{
						ModelState.AddModelError("", "Invalid Campaign selected.");
						return View(viewModel);
					}
				}
			}

			if (donor == null || branch == null)
			{
				ModelState.AddModelError("", "Invalid Donor or Branch selected.");
				return View(viewModel);
			}
			donation.Quantity = viewModel.Quantity;
			donation.CreatedDate = viewModel.CreatedDate;
			donation.Status = viewModel.Status;
			donation.Donor = donor;
			donation.Branch = branch;
			donation.Campaign = campaign;
			donation.DonationDate = viewModel.DonationDate;
			donation.ExpiryDate = viewModel.ExpiryDate;
			donation.BagNumber = viewModel.BagNumber;

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> ViewProfile(int id)
		{
			var donor = await _context.Donors
									.Include(x => x.BloodType)
									.FirstOrDefaultAsync(x => x.Id == id);

			if (donor == null)
			{
				return NotFound();
			}
			var donations = await _context.Donations
													.Include(x => x.Branch)
													.Include(x => x.Campaign)
													.Where(x => x.Donor.Id == id)
													.ToListAsync();

			var viewModel = new DonationProfileViewModel
			{
				Donor = donor,
				Donations = donations
			};

			return View(viewModel);
		}



	}
}
