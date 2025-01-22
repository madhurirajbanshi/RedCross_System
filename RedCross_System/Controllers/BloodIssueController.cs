using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.BloodIssue;
using RedCross_System.ViewModel.TestBlood;

namespace RedCross_System.Controllers
{
	public class BloodIssueController : Controller
	{
		private readonly ApplicationDbContext _context;

		public BloodIssueController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{

			var donations = await _context.Donations.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.BagNumber
			}).ToListAsync();

			var donors = await _context.Donors.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.Name
			}).ToListAsync();

			var viewModel = new BloodIssueAddViewModel
			{
				Donations = donations,
				Donors = donors
			};

			return View(viewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(BloodIssueAddViewModel model)
		{
			if (ModelState.IsValid)
			{
				model.Total = model.Charge - (model.Charge * model.Discount / 100);
				model.Donations = await _context.Donations.Select(d => new SelectListItem
				{
					Value = d.Id.ToString(),
					Text = d.BagNumber
				}).ToListAsync();

				model.Donors = await _context.Donors.Select(d => new SelectListItem
				{
					Value = d.Id.ToString(),
					Text = d.Name
				}).ToListAsync();

				var donation = await _context.Donations.FindAsync(int.Parse(model.Donation));
				var donor = await _context.Donors.FindAsync(int.Parse(model.Donor));
				if (donation == null || donor == null)
				{
					ModelState.AddModelError("", "Invalid  Donation selected.");
					return View(model);
				}

				var bloodIssue = new BloodIssue
				{
					ReceiverName = model.ReceiverName,
					CreatedDate = model.CreatedDate,
					Charge = model.Charge,
					Discount = model.Discount,
					Total = model.Total,
					Status = model.Status,
					Donation = donation,
					Donor = donor
				};

				_context.BloodIssues.Add(bloodIssue);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			var bloodissue = await _context.BloodIssues.FindAsync(id);
			if (bloodissue == null) throw new Exception("Branch Not Found");

			bloodissue.Status = bloodissue.Status == "Active" ? "Inactive" : "Active";

			_context.BloodIssues.Update(bloodissue);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Index()
		{
			var bloodIssues = await _context.BloodIssues
					.Include(b => b.Donation)
					.Include(b => b.Donor)
					.ToListAsync();

			var viewModel = bloodIssues.Select(b => new BloodIssueIndexViewModel
			{
				ReceiverName = b.ReceiverName,
				CreatedDate = b.CreatedDate,
				Charge = b.Charge,
				Discount = b.Discount,
				Total = b.Total,
				Status = b.Status,
				Donation = b.Donation.BagNumber,
				Donor = b.Donor.Name
			}).ToList();

			return View(viewModel);
		}


		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var bloodIssue = await _context.BloodIssues
					.Include(b => b.Donation)
					.Include(b => b.Donor)
					.FirstOrDefaultAsync(b => b.Id == id);

			if (bloodIssue == null)
			{
				return NotFound();
			}

			var donations = await _context.Donations.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.BagNumber
			}).ToListAsync();

			var donors = await _context.Donors.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.Name
			}).ToListAsync();

			var viewModel = new BloodIssueUpdateViewModel
			{
				Id=bloodIssue.Id,
				ReceiverName = bloodIssue.ReceiverName,
				CreatedDate = bloodIssue.CreatedDate,
				Charge = bloodIssue.Charge,
				Discount = bloodIssue.Discount,
				Total = bloodIssue.Total,
				Status = bloodIssue.Status,
				Donation = bloodIssue.Donation.Id.ToString(),
				Donor = bloodIssue.Donor.Id.ToString(),
				Donations = donations,
				Donors = donors
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id, BloodIssueUpdateViewModel model)
		{
			if (id != model.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				model.Total = model.Charge - (model.Charge * model.Discount / 100);

				var bloodIssue = await _context.BloodIssues.FindAsync(id);
				if (bloodIssue == null)
				{
					return NotFound();
				}

				var donation = await _context.Donations.FindAsync(int.Parse(model.Donation));
				var donor = await _context.Donors.FindAsync(int.Parse(model.Donor));

				if (donation == null || donor == null)
				{
					ModelState.AddModelError("", "Invalid Donation or Donor selected.");
					return View(model);
				}

				bloodIssue.ReceiverName = model.ReceiverName;
				bloodIssue.CreatedDate = model.CreatedDate;
				bloodIssue.Charge = model.Charge;
				bloodIssue.Discount = model.Discount;
				bloodIssue.Total = model.Total;
				bloodIssue.Status = model.Status;
				bloodIssue.Donation = donation;
				bloodIssue.Donor = donor;

				_context.BloodIssues.Update(bloodIssue);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			return View(model);
		}
	}

	}

