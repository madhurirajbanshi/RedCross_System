using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.TestBlood;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCross_System.Controllers
{
	public class TestBloodController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public TestBloodController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;
		}

		public async Task<IActionResult> Index()
		{
			var testBloods = await _context.TestBloods
							.Include(t => t.Donation)
							.ThenInclude(d => d.Donor)
							.ToListAsync();

			var viewModel = testBloods.Select(t => new TestBloodIndexViewModel
			{
				Id = t.Id,
				TestName = t.TestName,
				Donation = t.Donation.BagNumber,
				Status = t.Status,
			}).ToList();

			return View(viewModel);
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

			var viewModel = new TestBloodAddViewModel
			{
				Donations = donations,
				Donors = donors
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(TestBloodAddViewModel model)
		{
			if (!ModelState.IsValid)
			{
			
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

				return View(model);
			}

			var donation = await _context.Donations.FindAsync(int.Parse(model.Donation));
			var donor = await _context.Donors.FindAsync(int.Parse(model.Donor));
			if ( donation == null|| donor==null)
			{
				ModelState.AddModelError("", "Invalid  Donation selected.");
				return View(model);
			}
			var currentUser = await _sessionHelper.CurrentUser();
			if (currentUser == null)
			{
				ModelState.AddModelError("", "User not logged in.");
				return View(model);
			}

			var testBlood = new TestBlood
			{
				TestName = model.TestName,
				Quantity = model.Quantity,
				CreatedBy = currentUser,
				Donation = donation,
				Donor=donor,
			};

			_context.TestBloods.Add(testBlood);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			var testblood = await _context.TestBloods.FindAsync(id);
			if (testblood == null) throw new Exception("TestBlood Not Found");

			testblood.Status = testblood.Status == "Active" ? "Inactive" : "Active";

			_context.TestBloods.Update(testblood);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var testBlood = await _context.TestBloods
											.Include(t => t.Donation)
											.ThenInclude(d => d.Donor)
											.FirstOrDefaultAsync(t => t.Id == id);

			if (testBlood == null || testBlood.Donation == null || testBlood.Donation.Donor == null)
			{
				return NotFound();
			}

			var donors = await _context.Donors.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.Name
			}).ToListAsync();

			var donations = await _context.Donations.Select(d => new SelectListItem
			{
				Value = d.Id.ToString(),
				Text = d.BagNumber
			}).ToListAsync();

			var viewModel = new TestBloodUpdateViewModel
			{
				Id = testBlood.Id,
				TestName = testBlood.TestName,
				Donor = testBlood.Donation.Donor.Id.ToString(),
				Donation = testBlood.Donation.Id.ToString(),
				Donors = donors,
				Donations = donations
			};

			return View(viewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(TestBloodUpdateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Donors = await _context.Donors.Select(d => new SelectListItem
				{
					Value = d.Id.ToString(),
					Text = d.Name
				}).ToListAsync();

				model.Donations = await _context.Donations.Select(d => new SelectListItem
				{
					Value = d.Id.ToString(),
					Text = d.BagNumber
				}).ToListAsync();

				return View(model);
			}

			if (string.IsNullOrEmpty(model.Donor) || string.IsNullOrEmpty(model.Donation))
			{
				ModelState.AddModelError("", "Donor and Donation must be selected.");
				return View(model);
			}

			var testBlood = await _context.TestBloods
																			 .Include(t => t.Donation)
																			 .ThenInclude(d => d.Donor)
																			 .FirstOrDefaultAsync(t => t.Id == model.Id);

			if (testBlood == null)
			{
				return NotFound();
			}

			var donor = await _context.Donors.FindAsync(int.Parse(model.Donor));
			var donation = await _context.Donations.FindAsync(int.Parse(model.Donation));
		

			if (donor == null || donation == null)
			{
				ModelState.AddModelError("", "Invalid Donor or Donation selected.");
				return View(model);
			}

			testBlood.TestName = model.TestName;
			testBlood.Donation = donation;

			_context.TestBloods.Update(testBlood);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}


		[HttpGet]
		public async Task<IActionResult> Report()
		{
			var donations = await _context.Donations
							.Include(d => d.Donor)
							.ThenInclude(d=>d.BloodType)
							.Include(d => d.Campaign)
							.Include(d => d.Branch)
							.ToListAsync();
			var reportData = donations.Select(donation => new TestBloodReportViewModel
			{
				TestName = _context.TestBloods.FirstOrDefault(tb => tb.Donation.Id == donation.Id)?.TestName ?? "Unknown",
				DonorName = donation.Donor?.Name ?? "Unknown", 
				DonationDate = donation.DonationDate,
				Quantity = donation.Quantity,
				BloodType = donation.Donor?.BloodType?.Name ?? "Unknown",
				Status = donation.Status
			}).ToList();

			return View(reportData);
		}
		[HttpPost]
		public async Task<IActionResult> ExportReport()
		{
			var donations = await _context.Donations
											.Include(d => d.Donor)
											.Include(d => d.Campaign)
											
											.Include(d => d.Branch)
											.ToListAsync();

			var reportData = donations.Select(donation => new TestBloodReportViewModel
			{
				TestName = _context.TestBloods.FirstOrDefault(tb => tb.Donation.Id == donation.Id)?.TestName ?? "Unknown",
				DonorName = donation.Donor?.Name ?? "Unknown",
				DonationDate = donation.DonationDate,
				Quantity = donation.Quantity,
				BloodType = donation.Donor?.BloodType?.Name ?? "Unknown",
				Status = donation.Status
			}).ToList();

			using var stream = new MemoryStream();

			using (var writer = new iText.Kernel.Pdf.PdfWriter(stream))
			{
				var pdfDoc = new iText.Kernel.Pdf.PdfDocument(writer);
				var document = new iText.Layout.Document(pdfDoc);

				var title = new iText.Layout.Element.Paragraph("Test Blood Report")
						.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
						.SetFontSize(20);
					
				document.Add(title);

				var table = new iText.Layout.Element.Table(new float[] { 3, 3, 2, 1, 2, 2 })
						.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

				table.AddHeaderCell("Test Name");
				table.AddHeaderCell("Donor Name");
				table.AddHeaderCell("Donation Date");
				table.AddHeaderCell("Quantity");
				table.AddHeaderCell("Blood Type");
				table.AddHeaderCell("Status");

				foreach (var item in reportData)
				{
					table.AddCell(item.TestName);
					table.AddCell(item.DonorName);
					table.AddCell(item.DonationDate.ToString("yyyy-MM-dd"));
					table.AddCell(item.Quantity.ToString());
					table.AddCell(item.BloodType);
					table.AddCell(item.Status);
				}

				document.Add(table);

				document.Close();
			}

			var fileName = "TestBloodReport.pdf";
			return File(stream.ToArray(), "application/pdf", fileName);
		}


	}
}
