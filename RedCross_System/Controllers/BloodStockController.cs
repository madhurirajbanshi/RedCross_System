using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.BloodStock;
using RedCross_System.ViewModel.TestBlood;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCross_System.Controllers;

	public class BloodStockController : Controller
	{
		private readonly ApplicationDbContext _context;

		public BloodStockController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var campaignDonors = await _context.Donations
					.Where(d => d.Campaign != null)
					.Select(d => d.Donor)
					.Distinct()
					.ToListAsync();

			var nonCampaignDonors = await _context.Donations
					.Where(d => d.Campaign == null)
					.Select(d => d.Donor)
					.Distinct()
					.ToListAsync();

			var viewModel = new BloodStockAddViewModel
			{
				CampaignDonors = campaignDonors ?? new List<Donor>(),  
				NonCampaignDonors = nonCampaignDonors ?? new List<Donor>()  
			};

			return View(viewModel);
		}
		

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(BloodStockAddViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var selectedCampaignDonorIds = viewModel.CampaignDonorIds;
			var selectedNonCampaignDonorIds = viewModel.NonCampaignDonorIds;

			if (selectedCampaignDonorIds != null && selectedCampaignDonorIds.Any())
			{
				foreach (var donorId in selectedCampaignDonorIds)
				{
					var campaignDonor = await _context.Donors.FindAsync(donorId);
				}
			}

			if (selectedNonCampaignDonorIds != null && selectedNonCampaignDonorIds.Any())
			{
				foreach (var donorId in selectedNonCampaignDonorIds)
				{
					var nonCampaignDonor = await _context.Donors.FindAsync(donorId);
				}
			}

			return RedirectToAction("Index");
		}






}
