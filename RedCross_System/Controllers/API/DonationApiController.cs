using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Donation;
using RedCross_System.ViewModel.Donation.DonationApi;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class DonationApiController : ControllerBase
	{
		
			private readonly ApplicationDbContext _applicationDbContext;
			private readonly SessionHelper _sessionHelper;
		public DonationApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_applicationDbContext = context;
			_sessionHelper = sessionHelper;
		}

			[HttpGet]
			public async Task<ActionResult<IEnumerable<DonationIndexViewModel>>> GetAllDonations()
			{
				var donations = await _applicationDbContext.Donations
						.Include(d => d.Branch)
						.Include(d => d.Campaign)
						.Include(d => d.Donor)
						.Include(x => x.CreatedBy)
						.Select(d => new DonationIndexViewModel
						{
							Id = d.Id,
							Quantity = d.Quantity,
							CreatedDate = d.CreatedDate,
							CreatedBy = d.CreatedBy.Name,
							Status = d.Status,
							Donor = d.Donor != null ? d.Donor.Name : "No Donor",
							Branch = d.Branch != null ? d.Branch.BranchName : "No Branch",
							Campaign = d.Campaign != null ? d.Campaign.Name : "No Campaign",
							DonationDate = d.DonationDate,
							ExpiryDate = d.ExpiryDate,
							BagNumber = d.BagNumber
						})
						.ToListAsync();

				return Ok(donations);
			}

		[HttpGet("{id}")]
		public async Task<ActionResult<DonationIndexViewModel>> GetDonationById(int id)
		{
			var donation = await _applicationDbContext.Donations
					.Include(d => d.Branch)
					.Include(d => d.Campaign)
					.Include(d => d.Donor)
					.Include(x => x.CreatedBy)
					.Where(d => d.Id == id)
					.Select(d => new DonationIndexViewModel
					{
						Id = d.Id,
						Quantity = d.Quantity,
						CreatedDate = d.CreatedDate,
						CreatedBy = d.CreatedBy.Name,
						Status = d.Status,
						Donor = d.Donor != null ? d.Donor.Name : "No Donor",
						Branch = d.Branch != null ? d.Branch.BranchName : "No Branch",
						Campaign = d.Campaign != null ? d.Campaign.Name : "No Campaign",
						DonationDate = d.DonationDate,
						ExpiryDate = d.ExpiryDate,
						BagNumber = d.BagNumber
					})
					.FirstOrDefaultAsync();

			if (donation == null)
			{
				return NotFound(new { Message = $"Donation with ID {id} not found." });
			}

			return Ok(donation);
		}
		[HttpPost]
		public async Task<ActionResult<DonationApiResponseRequest>> AddDonation([FromBody] DonationApiAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var currentUser = await _sessionHelper.CurrentUser();

				if (currentUser == null)
				{
					return Unauthorized(new { message = "User session is invalid. Please log in again." });
				}

				var donor = await _applicationDbContext.Donors.FirstOrDefaultAsync(d => d.Name== request.Donor);
				if (donor == null)
				{
					return BadRequest(new { message = $"Donor with ID {request.Donor} not found." });
				}

				var branch = await _applicationDbContext.Branches.FirstOrDefaultAsync(b => b.BranchName == request.Branch);
				if (branch == null)
				{
					return BadRequest(new { message = $"Branch with ID {request.Branch} not found." });
				}

				var campaign = await _applicationDbContext.Campaigns.FirstOrDefaultAsync(c => c.Name == request.Campaign);
				if (campaign == null)
				{
					return BadRequest(new { message = $"Campaign with ID {request.Campaign} not found." });
				}

				var donation = new Donation
				{
					Quantity = request.Quantity,
					CreatedDate = DateTime.UtcNow,
					CreatedBy = currentUser,
					Status = request.Status,
					Donor = donor,
					Branch = branch,
					Campaign = campaign,
					DonationDate = request.DonationDate,
					ExpiryDate = request.ExpiryDate,
					BagNumber = request.BagNumber
				};

				_applicationDbContext.Donations.Add(donation);
				await _applicationDbContext.SaveChangesAsync();

				var response = new DonationApiResponseRequest
				{
					Id = donation.Id,
					Quantity = donation.Quantity,
					CreatedDate = donation.CreatedDate,
					CreatedBy = currentUser.Name,
					Status = donation.Status,
					Donor = donor.Name,
					Branch = branch.BranchName,
					Campaign = campaign.Name,
					DonationDate = donation.DonationDate,
					ExpiryDate = donation.ExpiryDate,
					BagNumber = donation.BagNumber
				};

				return CreatedAtAction(nameof(GetDonationById), new { id = donation.Id }, response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while creating the donation: {ex.Message}" });
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateDonation(int id, [FromBody] DonationApiUpdateRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var donation = await _applicationDbContext.Donations
						.Include(d => d.Donor)
						.Include(d => d.Branch)
						.Include(d => d.Campaign)
						.FirstOrDefaultAsync(d => d.Id == id);

				if (donation == null)
				{
					return NotFound(new { message = $"Donation with ID {id} not found." });
				}

				var donor = await _applicationDbContext.Donors.FirstOrDefaultAsync(d => d.Name == request.Donor);
				if (donor == null)
				{
					return BadRequest(new { message = $"Donor with name '{request.Donor}' not found." });
				}

				var branch = await _applicationDbContext.Branches.FirstOrDefaultAsync(b => b.BranchName == request.Branch);
				if (branch == null)
				{
					return BadRequest(new { message = $"Branch with name '{request.Branch}' not found." });
				}

				var campaign = await _applicationDbContext.Campaigns.FirstOrDefaultAsync(c => c.Name == request.Campaign);
				if (campaign == null)
				{
					return BadRequest(new { message = $"Campaign with name '{request.Campaign}' not found." });
				}

				donation.Quantity = request.Quantity;
				donation.Status = request.Status;
				donation.Donor = donor;
				donation.Branch = branch;
				donation.Campaign = campaign;
				donation.DonationDate = request.DonationDate;
				donation.ExpiryDate = request.ExpiryDate;
				donation.BagNumber = request.BagNumber;

				await _applicationDbContext.SaveChangesAsync();

				return Ok(new { message = "Donation updated successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while updating the donation: {ex.Message}" });
			}
		}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteDonation(int id)
		{
			try
			{
				var donation = await _applicationDbContext.Donations.FirstOrDefaultAsync(d => d.Id == id);

				if (donation == null)
				{
					return NotFound(new { message = $"Donation with ID {id} not found." });
				}

				_applicationDbContext.Donations.Remove(donation);
				await _applicationDbContext.SaveChangesAsync();

				return Ok(new { message = "Donation deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while deleting the donation: {ex.Message}" });
			}
		}


	}
}

		
		