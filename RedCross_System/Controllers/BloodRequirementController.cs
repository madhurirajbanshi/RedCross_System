using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.BloodRequirement;

namespace RedCross_System.Controllers
{
	public class BloodRequirementController : Controller
	{
		private readonly ApplicationDbContext _context;
		public BloodRequirementController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(BloodRequirementAddViewModel model)
		{
			if (ModelState.IsValid)
			{
				string base64Document = null;

				if (model.File != null && model.File.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await model.File.CopyToAsync(memoryStream);
						byte[] fileBytes = memoryStream.ToArray();
						base64Document = Convert.ToBase64String(fileBytes);
					}
				}

				var bloodRequirement = new BloodRequirement
				{
					Name = model.Name,
					Purpose = model.Purpose,
					Quantity = model.Quantity,
					CreatedDate = model.CreatedDate,
					Status = model.Status,
					Document = base64Document
				};

				_context.BloodRequirements.Add(bloodRequirement);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			return View(model);

		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			var bloodrequirement = await _context.BloodRequirements.FindAsync(id);
			if (bloodrequirement == null) throw new Exception("Branch Not Found");

			bloodrequirement.Status = bloodrequirement.Status == "Active" ? "Inactive" : "Active";

			_context.BloodRequirements.Update(bloodrequirement);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Index()
		{
			var bloodRequirements = await _context.BloodRequirements.ToListAsync();

			var viewModel = bloodRequirements.Select(br => new BloodRequirementIndexViewModel
			{
				Id = br.Id,
				Name = br.Name,
				Purpose = br.Purpose,
				Quantity = br.Quantity,
				CreatedDate = br.CreatedDate,
				Status = br.Status,
				Document = !string.IsNullOrEmpty(br.Document)
									 ? $"data:application/octet-stream;base64,{br.Document}"
									 : null
			}).ToList();


			return View(viewModel);
		}

		public async Task<IActionResult> Update(int id)
		{
			var bloodRequirement = await _context.BloodRequirements.FindAsync(id);
			if (bloodRequirement == null)
			{
				return NotFound();
			}

			var viewModel = new BloodRequirementUpdateViewModel
			{
				Id = bloodRequirement.Id,
				Name = bloodRequirement.Name,
				Purpose = bloodRequirement.Purpose,
				Quantity = bloodRequirement.Quantity,
				CreatedDate = bloodRequirement.CreatedDate,
				Status = bloodRequirement.Status,
				Document = bloodRequirement.Document
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(BloodRequirementUpdateViewModel model)
		{
			if (ModelState.IsValid)
			{
				var bloodRequirement = await _context.BloodRequirements.FindAsync(model.Id);
				if (bloodRequirement == null)
				{
					return NotFound();
				}

				string base64Document = bloodRequirement.Document; 
				if (model.File != null && model.File.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await model.File.CopyToAsync(memoryStream);
						byte[] fileBytes = memoryStream.ToArray();
						base64Document = Convert.ToBase64String(fileBytes);
					}
				}

				bloodRequirement.Name = model.Name;
				bloodRequirement.Purpose = model.Purpose;
				bloodRequirement.Quantity = model.Quantity;
				bloodRequirement.CreatedDate = model.CreatedDate;
				bloodRequirement.Status = model.Status;
				bloodRequirement.Document = base64Document;

				_context.BloodRequirements.Update(bloodRequirement);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			return View(model);
		}

		
	}
}
