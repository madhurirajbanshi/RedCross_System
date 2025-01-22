using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.TestBlood;
using RedCross_System.ViewModel.TestBlood.TestBloodApi;
using System.Threading.Tasks;

namespace RedCross_System.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestBloodApiController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public TestBloodApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TestBloodIndexViewModel>>> GetTestBloods()
		{
			var testBloods = await _context.TestBloods
					.Include(t => t.Donation)
					.ThenInclude(d => d.Donor)
					.Select(t => new TestBloodIndexViewModel
					{
						Id = t.Id,
						Donor = t.Donor.Name,
						TestName = t.TestName,
						Donation = t.Donation.BagNumber,
						Status = t.Status,
					})
					.ToListAsync();

			return Ok(testBloods);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<TestBloodIndexViewModel>> GetTestBlood(int id)
		{
			var testBlood = await _context.TestBloods
							.Include(t => t.Donation)
							.ThenInclude(d => d.Donor)
							.FirstOrDefaultAsync(t => t.Id == id);

			if (testBlood == null)
			{
				return NotFound();
			}

			var viewModel = new TestBloodIndexViewModel
			{
				Id = testBlood.Id,
				Donor = testBlood.Donor.Name,
				TestName = testBlood.TestName,
				Donation = testBlood.Donation.BagNumber,
				Status = testBlood.Status
			};

			return Ok(viewModel);
		}
		[HttpPost]
		public async Task<ActionResult<TestBloodApiResponse>> AddTestBlood([FromBody] TestBloodApiAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var donation = await _context.Donations
						.Include(d => d.Donor)
						.FirstOrDefaultAsync(d => d.BagNumber == request.Donation);

				if (donation == null)
				{
					return BadRequest("Invalid donation selected");
				}

				var donor = await _context.Donors
						.FirstOrDefaultAsync(d => d.Name == request.Donor);

				if (donor == null)
				{
					return BadRequest("Invalid donor selected");
				}

				var currentUser = await _sessionHelper.CurrentUser();
				if (currentUser == null)
				{
					return Unauthorized(new { message = "User not logged in." });
				}

				var testBlood = new TestBlood
				{
					TestName = request.TestName,
					Donation = donation,
					Donor = donor,
					CreatedBy = currentUser
				};

				_context.TestBloods.Add(testBlood);
				await _context.SaveChangesAsync();

				var response = new TestBloodApiResponse
				{
					Id = testBlood.Id,
					TestName = testBlood.TestName,
					Donor = testBlood.Donor.Name,
					Donation = testBlood.Donation.BagNumber,
				};

				return CreatedAtAction(
						nameof(GetTestBlood),
						new { id = testBlood.Id },
						response
				);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}
		[HttpPut("{id}")]
		public async Task<ActionResult<TestBloodApiResponse>> UpdateTestBlood(int id, [FromBody] TestBloodApiAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var testBlood = await _context.TestBloods
						.Include(t => t.Donation)
						.ThenInclude(d => d.Donor)
						.FirstOrDefaultAsync(t => t.Id == id);

				if (testBlood == null)
				{
					return NotFound();
				}

				var donation = await _context.Donations
						.Include(d => d.Donor)
						.FirstOrDefaultAsync(d => d.BagNumber == request.Donation);

				if (donation == null)
				{
					return BadRequest("Invalid donation selected");
				}

				var donor = await _context.Donors
						.FirstOrDefaultAsync(d => d.Name == request.Donor);

				if (donor == null)
				{
					return BadRequest("Invalid donor selected");
				}

				testBlood.TestName = request.TestName;
				testBlood.Donation = donation;
				testBlood.Donor = donor;

				_context.TestBloods.Update(testBlood);
				await _context.SaveChangesAsync();

				var response = new TestBloodApiResponse
				{
					Id = testBlood.Id,
					TestName = testBlood.TestName,
					Donor = testBlood.Donor.Name,
					Donation = testBlood.Donation.BagNumber,
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}
		

			[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteTestBlood(int id)
		{
			try
			{
				var testBlood = await _context.TestBloods
						.FirstOrDefaultAsync(t => t.Id == id);

				if (testBlood == null)
				{
					return NotFound();
				}

				_context.TestBloods.Remove(testBlood);
				await _context.SaveChangesAsync();

				return NoContent(); 
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}
	}
}	

	
