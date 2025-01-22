using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Branch;
using RedCross_System.ViewModel.Branch.BranchApi;
using RedCross_System.ViewModel.Donation.DonationApi;
using RedCross_System.ViewModel.TestBlood;
using RedCross_System.ViewModel.TestBlood.TestBloodApi;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class BranchApiController : ControllerBase
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly SessionHelper _sessionHelper;

		public BranchApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_applicationDbContext = context;
			_sessionHelper = sessionHelper;
		}
		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<BranchIndexViewModel>>> GetTestBloods()
		{
			var branches = await _applicationDbContext.Branches
						.Include(x => x.Province)
						.ThenInclude(x => x.Country)
						.Include(x => x.CreatedBy)
						.Select(x => new BranchIndexViewModel()
						{
							BranchId = x.BranchId,
							CreatedBy = x.CreatedBy.Name,
							CreatedDate = x.CreatedDate,
							Location = x.Location,
							BranchName = x.BranchName,
							Province = x.Province.Name,
							Country = x.Province.Country.Name,
							Status = x.Status,
						}).ToListAsync();
			return (branches);

		}
		[HttpGet("{id}")]
		public async Task<ActionResult<BranchIndexViewModel>> GetBranchById(int id)
		{
			var branch = await _applicationDbContext.Branches
					.Include(x => x.Province)
					.ThenInclude(x => x.Country)
					.Include(x => x.CreatedBy)
					.Where(x => x.BranchId == id)
					.Select(x => new BranchIndexViewModel
					{
						BranchId = x.BranchId,
						CreatedBy = x.CreatedBy.Name,
						CreatedDate = x.CreatedDate,
						Location = x.Location,
						BranchName = x.BranchName,
						Province = x.Province.Name,
						Country = x.Province.Country.Name,
						Status = x.Status,
					})
					.FirstOrDefaultAsync();

			if (branch == null)
			{
				return NotFound(new { message = $"Branch with Id {id} not found." });
			}

			return Ok(branch);
		}
		[HttpPost]
		public async Task<ActionResult<BranchApiResponseRequest>> AddBranch([FromBody] BranchApiAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var currentUser = await _sessionHelper.CurrentUser();

				var province = await _applicationDbContext.Provinces
						.Include(p => p.Country)
						.FirstOrDefaultAsync(p => p.Name == request.Province && p.Country.Name == request.Country);

				if (province == null)
				{
					return BadRequest(new { message = $"Province '{request.Province}' in country '{request.Country}' not found." });
				}

				var branch = new Branch
				{
					BranchName = request.BranchName,
					Location = request.Location,
					Province = province,
					Country = province.Country,
					Status = request.Status,
					CreatedBy = currentUser,
					CreatedDate = DateTime.UtcNow
				};

				_applicationDbContext.Branches.Add(branch);
				await _applicationDbContext.SaveChangesAsync();

				var savedBranch = await _applicationDbContext.Branches
						.Include(b => b.Province)
						.ThenInclude(p => p.Country)
						.Include(b => b.CreatedBy)
						.FirstAsync(b => b.BranchId == branch.BranchId);

				var response = new BranchApiResponseRequest
				{
					BranchId = savedBranch.BranchId.ToString(),
					BranchName = savedBranch.BranchName,
					Location = savedBranch.Location,
					Province = savedBranch.Province.Name,
					Country = savedBranch.Country.Name,
					CreatedBy = savedBranch.CreatedBy.Name,
					CreatedDate = savedBranch.CreatedDate
				};

				return CreatedAtAction(
						nameof(GetBranchById),
						new { id = branch.BranchId },
						response
				);
			}
			catch (Exception ex) when (ex.Message == "Invalid Session")
			{
				return Unauthorized(new { message = "Please log in to create a branch." });
			}
			catch (Exception ex) when (ex.Message == "User Not Found")
			{
				return Unauthorized(new { message = "User account not found. Please log in again." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while creating the branch: {ex.Message}" });
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<BranchApiResponseRequest>> UpdateBranch(int id, [FromBody] BranchApiUpdateRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var branch = await _applicationDbContext.Branches
										.Include(b => b.Province)
										.ThenInclude(p => p.Country)
										.FirstOrDefaultAsync(b => b.BranchId == id);

				if (branch == null)
				{
					return NotFound(new { message = $"Branch with Id {id} not found." });
				}

				var province = await _applicationDbContext.Provinces
										.Include(p => p.Country)
										.FirstOrDefaultAsync(p => p.Name == request.Province && p.Country.Name == request.Country);

				if (province == null)
				{
					return BadRequest(new { message = $"Province '{request.Province}' in country '{request.Country}' not found." });
				}

				branch.BranchName = request.BranchName;
				branch.Location = request.Location;
				branch.Province = province;
				branch.Country = province.Country;
				branch.Status = request.Status;
				branch.CreatedDate = DateTime.UtcNow;

				_applicationDbContext.Branches.Update(branch);
				await _applicationDbContext.SaveChangesAsync();

				var updatedBranch = await _applicationDbContext.Branches
												.Include(b => b.Province)
												.ThenInclude(p => p.Country)
												.Include(b => b.CreatedBy)
												.FirstAsync(b => b.BranchId == branch.BranchId);

				var response = new BranchApiResponseRequest
				{
					BranchId = updatedBranch.BranchId.ToString(),
					BranchName = updatedBranch.BranchName,
					Location = updatedBranch.Location,
					Province = updatedBranch.Province.Name,
					Country = updatedBranch.Country.Name,
					CreatedBy = updatedBranch.CreatedBy.Name,
					CreatedDate = updatedBranch.CreatedDate
				};

				return Ok(response);
			}
			catch (Exception ex) when (ex.Message == "Invalid Session")
			{
				return Unauthorized(new { message = "Please log in to update the branch." });
			}
			catch (Exception ex) when (ex.Message == "User Not Found")
			{
				return Unauthorized(new { message = "User account not found. Please log in again." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while updating the branch: {ex.Message}" });
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteBranch(int id)
		{
			try
			{
				var branch = await _applicationDbContext.Branches
										.Include(b => b.Province)
										.ThenInclude(p => p.Country)
										.Include(b => b.CreatedBy)
										.FirstOrDefaultAsync(b => b.BranchId == id);

				if (branch == null)
				{
					return NotFound(new { message = $"Branch with Id {id} not found." });
				}

				_applicationDbContext.Branches.Remove(branch);
				await _applicationDbContext.SaveChangesAsync();

				return NoContent(); 
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while deleting the branch: {ex.Message}" });
			}
		}

		}


	}

		
	
	

