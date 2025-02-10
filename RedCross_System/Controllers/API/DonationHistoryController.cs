using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Donation.DonationApi;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class DonationHistoryController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;


		public DonationHistoryController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;

		}
		[HttpGet("history")]
		public async Task<ActionResult<IEnumerable<DonationApiIndex>>> GetDonationHistory(int donorId)
		{
			var donationHistory = await _context.Donations
					.Include(d => d.Branch)
					.Include(d => d.Campaign)
					.Include(d => d.Donor)
					.Include(d => d.CreatedBy)
						.Where(d => d.Donor != null && d.Donor.Id == donorId)
					.Select(d => new DonationApiIndex
					{
						Id = d.Id,
						Quantity = d.Quantity,
						CreatedDate = d.CreatedDate,
						CreatedBy = d.CreatedBy != null ? d.CreatedBy.Name : "Unknown", 
						Status = d.Status,
						Donor = d.Donor != null ? d.Donor.Name : "Unknown", 
						Branch = d.Branch != null ? d.Branch.BranchName : "Unknown",
						Campaign = d.Campaign != null ? d.Campaign.Name : "No Campaign",
						DonationDate = d.DonationDate,
						ExpiryDate = d.ExpiryDate,
						BagNumber = d.BagNumber
					})
					.ToListAsync();

			if (donationHistory == null || !donationHistory.Any())
			{
				return NotFound("No donation history found for this donor.");
			}

			return Ok(donationHistory);
		}



		
	}
}
