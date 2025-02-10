using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Campaign.CampaignApi;
using RedCross_System.ViewModel.Campaign;
using Microsoft.AspNetCore.Authorization;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CampaignApiController : ControllerBase
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly SessionHelper _sessionHelper;

		public CampaignApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_applicationDbContext = context;
			_sessionHelper = sessionHelper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CampaignApiIndex>>> GetAllCampaigns()
		{
			var campaigns = await _applicationDbContext.Campaigns
				      .Include(c=>c.Branch)
					.Select(c => new CampaignApiIndex
					{
						Id = c.Id,
						Name = c.Name,
						Address = c.Address,
						Status = c.Status,
						StartDate = c.StartDate,
						EndDate = c.EndDate,
						StartTime=c.StartTime,
						EndTime=c.EndTime,
						CreatedDate = c.CreatedDate,
						Branch = c.Branch != null ? c.Branch.BranchName : "No Branch"
					})
					.ToListAsync();

			return Ok(campaigns);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CampaignApiIndex>> GetCampaign(int id)
		{
			var campaign = await _applicationDbContext.Campaigns
				  .Include(c=>c.Branch)
					.FirstOrDefaultAsync(c => c.Id == id);

			if (campaign == null)
			{
				return NotFound(new { message = $"Campaign with Id {id} not found." });
			}

			var viewModel = new CampaignApiIndex
			{
				Id = campaign.Id,
				Name = campaign.Name,
				Address = campaign.Address,
				Status = campaign.Status,
				StartDate = campaign.StartDate,
				EndDate = campaign.EndDate,
				StartTime=campaign.StartTime,
				EndTime=campaign.EndTime,
				CreatedDate = campaign.CreatedDate,
				Branch = campaign.Branch != null ? campaign.Branch.BranchName : "No Branch"
			};

			return Ok(viewModel);
		}
		[HttpPost]
		public async Task<ActionResult<CampaignApiResponseRequest>> AddCampaign([FromBody] CampaignApiAddRequest request)
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
					return Unauthorized(new { message = "User not logged in." });
				}

				
				var branch = await _applicationDbContext.Branches
																								.FirstOrDefaultAsync(b => b.BranchName == request.Branch);
				if (branch == null)
				{
					return BadRequest(new { message = "Invalid Branch ID." });
				}

				var campaign = new Campaign
				{
					Name = request.Name,
					Address = request.Address,
					StartDate = request.StartDate,
					EndDate = request.EndDate,
					StartTime=request.StartTime,
					EndTime=request.EndTime,

					CreatedDate = DateTime.UtcNow,
					Status = request.Status,
					CreatedBy = currentUser,
					Branch = branch 
				};

				_applicationDbContext.Campaigns.Add(campaign);
				await _applicationDbContext.SaveChangesAsync();
				var savedCampaign = await _applicationDbContext.Campaigns
						.Include(b => b.Branch)
						.FirstOrDefaultAsync(c => c.Id == campaign.Id);


				var response = new CampaignApiResponseRequest
				{
					Id = savedCampaign.Id,
					Name = savedCampaign.Name,
					Address = savedCampaign.Address,
					Status = savedCampaign.Status,
					StartDate = savedCampaign.StartDate,
					EndDate = savedCampaign.EndDate,
					StartTime = savedCampaign.StartTime,
					EndTime = savedCampaign.EndTime,
					CreatedDate = savedCampaign.CreatedDate,
					Branch = savedCampaign.Branch != null ? savedCampaign.Branch.BranchName : "No Branch",  

				};

				return CreatedAtAction(
								nameof(GetCampaign),
								new { id = campaign.Id },
								response
				);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}


		[HttpPut("{id}")]
		public async Task<ActionResult<CampaignApiResponseRequest>> UpdateCampaign(int id, [FromBody] CampaignApiAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var campaign = await _applicationDbContext.Campaigns
					.Include(c=>c.Branch)
						.FirstOrDefaultAsync(c => c.Id == id);

				if (campaign == null)
				{
					return NotFound(new { message = $"Campaign with Id {id} not found." });
				}

				campaign.Name = request.Name;
				campaign.Address = request.Address;
				campaign.StartDate = request.StartDate;
				campaign.EndDate = request.EndDate;
				campaign.Status = request.Status;
				campaign.CreatedDate = DateTime.UtcNow;
				campaign.StartTime = request.StartTime;
				campaign.EndTime = request.EndTime;

				_applicationDbContext.Campaigns.Update(campaign);
				await _applicationDbContext.SaveChangesAsync();

				var response = new CampaignApiResponseRequest
				{
					Id = campaign.Id,
					Name = campaign.Name,
					Address = campaign.Address,
					Status = campaign.Status,
					StartDate = campaign.StartDate,
					EndDate = campaign.EndDate,
					StartTime=campaign.StartTime,
					EndTime=campaign.EndTime,
					CreatedDate = campaign.CreatedDate,
					Branch = campaign.Branch != null ? campaign.Branch.BranchName : "No Branch" 
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCampaign(int id)
		{
			try
			{
				var campaign = await _applicationDbContext.Campaigns
						.FirstOrDefaultAsync(c => c.Id == id);

				if (campaign == null)
				{
					return NotFound(new { message = $"Campaign with Id {id} not found." });
				}

				_applicationDbContext.Campaigns.Remove(campaign);
				await _applicationDbContext.SaveChangesAsync();

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}
	}
}


	

