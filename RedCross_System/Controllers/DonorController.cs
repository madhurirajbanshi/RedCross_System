using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Donor;
using RedCross_System.ViewModels;
using System.IO;
using OfficeOpenXml;
using System.Threading.Tasks;
using RedCross_System.Data;

namespace RedCross_System.Controllers;

	[Authorize]
	public class DonorController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public DonorController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			var bloodTypeList = await _context.BloodTypes.Select(x => new SelectListItem
			{
				Value = x.Id.ToString(),
				Text = x.Name
			}).ToListAsync();

			var vm = new DonorAddViewModel
			{
				BloodTypes = bloodTypeList
			};

			return View(vm);
		}

	[HttpPost]
	public async Task<IActionResult> ToggleStatus(int id)
	{
		var donor = await _context.Donors.FindAsync(id);
		if (donor == null) throw new Exception("Donation Not Found");

		donor.Status = donor.Status == "Active" ? "Inactive" : "Active";

		_context.Donors.Update(donor);
		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}

	[HttpPost]
		public async Task<IActionResult> Add(DonorAddViewModel donorAddView)
		{
			var createdBy = await _sessionHelper.CurrentUser();
			if (createdBy is null)
			{
				ModelState.AddModelError("", "User not logged in.");
				donorAddView.BloodTypes = await _context.BloodTypes.Select(x => new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToListAsync();
				return View(donorAddView);
			}

			if (!ModelState.IsValid)
			{
				donorAddView.BloodTypes = await _context.BloodTypes.Select(x => new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToListAsync();

				return View(donorAddView);
			}

            BloodType? bloodType = await _context.BloodTypes.FindAsync(int.Parse(donorAddView.BloodType));
			if (bloodType == null)
			{
				ModelState.AddModelError("", "Invalid Blood Type selected.");
				return View(donorAddView);
			}

			byte[] photoBytes = null;
			if (donorAddView.Photo != null && donorAddView.Photo.Length > 0)
			{
            using var memoryStream = new MemoryStream();
            await donorAddView.Photo.CopyToAsync(memoryStream);
            photoBytes = memoryStream.ToArray();
        }

			var donor = new Donor
			{
				Name = donorAddView.Name,
				TemporaryAddress = donorAddView.TemporaryAddress,
				PermanentAddress = donorAddView.PermanentAddress,
				MobileNumber = donorAddView.MobileNumber,
				SecondaryNumber = donorAddView.SecondaryNumber,
				Email = donorAddView.Email,
				Photo = photoBytes,
				CreatedBy = createdBy,
				Status = donorAddView.Status,
				BloodType = bloodType
			};

			_context.Add(donor);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var donors = await _context.Donors
				.Include(x => x.BloodType)
				.Select(x => new DonorIndexViewModel
				{
					Id = x.Id,
					Name = x.Name,
					TemporaryAddress = x.TemporaryAddress,
					PermanentAddress = x.PermanentAddress,
					MobileNumber = x.MobileNumber,
					SecondaryNumber = x.SecondaryNumber,
					Email = x.Email,
					Status = x.Status,
					PhotoBase64 = x.Photo != null ? Convert.ToBase64String(x.Photo) : null,
					BloodType = x.BloodType.Name,
					CreatedBy = x.CreatedBy.Name
				}).ToListAsync();

			return View(donors);
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var donor = await _context.Donors
				.Include(x => x.BloodType)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (donor == null)
			{
				return NotFound();
			}

			var bloodTypeList = await _context.BloodTypes.Select(x => new SelectListItem
			{
				Value = x.Id.ToString(),
				Text = x.Name
			}).ToListAsync();

			var vm = new DonorUpdateViewModel
			{
				Id = donor.Id,
				Name = donor.Name,
				TemporaryAddress = donor.TemporaryAddress,
				PermanentAddress = donor.PermanentAddress,
				MobileNumber = donor.MobileNumber,
				SecondaryNumber = donor.SecondaryNumber,
				Email = donor.Email,
				Status = donor.Status,
				PhotoBase64 = donor.Photo != null ? Convert.ToBase64String(donor.Photo) : null,
				BloodType = donor.BloodType.Id.ToString(),
				BloodTypes = bloodTypeList
			};

			return View(vm);
		}

	[HttpPost]
	public async Task<IActionResult> Update(DonorUpdateViewModel donorUpdateView)
	{
		
		if (!ModelState.IsValid)
		{
			donorUpdateView.BloodTypes = await _context.BloodTypes.Select(x => new SelectListItem
			{
				Value = x.Id.ToString(),
				Text = x.Name
			}).ToListAsync();

			return View(donorUpdateView);
		}

		var donor = await _context.Donors.Include(x => x.BloodType).FirstOrDefaultAsync(x => x.Id == donorUpdateView.Id);
		if (donor == null)
		{
			return NotFound();
		}

		var bloodType = await _context.BloodTypes.FindAsync(int.Parse(donorUpdateView.BloodType));
		if (bloodType == null)
		{
			ModelState.AddModelError("", "Invalid Blood Type selected.");
			return View(donorUpdateView);
		}

		if (donorUpdateView.Photo != null && donorUpdateView.Photo.Length > 0)
		{
			using var memoryStream = new MemoryStream();
			await donorUpdateView.Photo.CopyToAsync(memoryStream);
			donor.Photo = memoryStream.ToArray();
		}

		donor.Name = donorUpdateView.Name;
		donor.TemporaryAddress = donorUpdateView.TemporaryAddress;
		donor.PermanentAddress = donorUpdateView.PermanentAddress;
		donor.MobileNumber = donorUpdateView.MobileNumber;
		donor.SecondaryNumber = donorUpdateView.SecondaryNumber;
		donor.Email = donorUpdateView.Email;
		donor.Status = donorUpdateView.Status;
		donor.BloodType = bloodType;

		_context.Update(donor);
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
			 .Include(d => d.Campaign)
			 .Where(d => d.Id == id)
			 .OrderByDescending(d => d.DonationDate)
			 .ToListAsync();


		var viewModel = new DonorProfileViewModel
		{
			Donor = donor,
			Donations = donations
		};

		return View(viewModel);
	}
	[HttpPost]
	public async Task<IActionResult> ExportToExcel(int id)
	{
		var donor = await _context.Donors
						.Include(x => x.BloodType)
						.FirstOrDefaultAsync(x => x.Id == id);

		if (donor == null)
		{
			return NotFound();
		}

		var donations = await _context.Donations
						.Include(d => d.Campaign)
						.Where(d => d.Id == id)
						.OrderByDescending(d => d.DonationDate)
						.ToListAsync();

		using (var package = new ExcelPackage())
		{
			var worksheet = package.Workbook.Worksheets.Add("Donor Profile");

			worksheet.Cells[1, 1].Value = "Donor Name";
			worksheet.Cells[1, 2].Value = "Blood Type";
			worksheet.Cells[1, 3].Value = "Email";
			worksheet.Cells[1, 4].Value = "Status";
			worksheet.Cells[1, 5].Value = "Donation Date";
			worksheet.Cells[1, 6].Value = "Campaign Name";

			worksheet.Cells[2, 1].Value = donor.Name;
			worksheet.Cells[2, 2].Value = donor.BloodType.Name;
			worksheet.Cells[2, 3].Value = donor.Email;
			worksheet.Cells[2, 4].Value = donor.Status;

			int row = 2;
			foreach (var donation in donations)
			{
				row++;
				worksheet.Cells[row, 5].Value = donation.DonationDate.ToString("yyyy-MM-dd");
				worksheet.Cells[row, 6].Value = donation.Campaign.Name;
			}

			var stream = new MemoryStream();
			package.SaveAs(stream);
			stream.Position = 0;

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DonorProfile.xlsx");
		}
	}
}



