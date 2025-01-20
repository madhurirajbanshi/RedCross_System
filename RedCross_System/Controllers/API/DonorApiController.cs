using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.Donor;
using RedCross_System.ViewModel.Donor.DonorApi;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class DonorApiController : ControllerBase
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly SessionHelper _sessionHelper;

		public DonorApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_applicationDbContext = context;
			_sessionHelper = sessionHelper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<DonorIndexViewModel>>> GetAllDonors()
		{
			var donors = await _applicationDbContext.Donors
					.Include(d => d.CreatedBy)
					.Include(d => d.BloodType)
					.Select(d => new DonorIndexViewModel
					{
						Id = d.Id,
						Name = d.Name,
						TemporaryAddress = d.TemporaryAddress,
						PermanentAddress = d.PermanentAddress,
						MobileNumber = d.MobileNumber,
						SecondaryNumber = d.SecondaryNumber,
						Email = d.Email,
						CreatedBy = d.CreatedBy != null ? d.CreatedBy.Name : "Unknown",
						CreatedDate = d.CreatedDate,
						Status = d.Status,
						BloodType = d.BloodType != null ? d.BloodType.Name : "Unknown"
					})
					.ToListAsync();

			return Ok(donors);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<DonorIndexViewModel>> GetDonorById(int id)
		{
			var donor = await _applicationDbContext.Donors
					.Include(d => d.CreatedBy)
					.Include(d => d.BloodType)
					.Where(d => d.Id == id)
					.Select(d => new DonorIndexViewModel
					{
						Id = d.Id,
						Name = d.Name,
						TemporaryAddress = d.TemporaryAddress,
						PermanentAddress = d.PermanentAddress,
						MobileNumber = d.MobileNumber,
						SecondaryNumber = d.SecondaryNumber,
						Email = d.Email,
						PhotoBase64 = d.Photo != null ? Convert.ToBase64String(d.Photo) : null,
						CreatedBy = d.CreatedBy != null ? d.CreatedBy.Name : "Unknown",
						CreatedDate = d.CreatedDate,
						Status = d.Status,
						BloodType = d.BloodType != null ? d.BloodType.Name : "Unknown"
					})
					.FirstOrDefaultAsync();

			if (donor == null)
			{
				return NotFound(new { Message = $"Donor with ID {id} not found." });
			}

			return Ok(donor);
		}

		[HttpPost]
		public async Task<ActionResult<DonorApiResponseRequest>> AddDonor([FromBody] DonorApiAddRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { Message = "Invalid data." });
			}
			try
			{
				var currentUser = await _sessionHelper.CurrentUser();

				if (currentUser == null)
				{
					return Unauthorized(new { message = "User session is invalid. Please log in again." });
				}

				var bloodType = await _applicationDbContext.BloodTypes
						.FirstOrDefaultAsync(bt => bt.Name == request.BloodType);

				if (bloodType == null)
				{
					return BadRequest(new { Message = "Invalid Blood Type." });
				}

				var donor = new Donor
				{
					Name = request.Name,
					TemporaryAddress = request.TemporaryAddress,
					PermanentAddress = request.PermanentAddress,
					MobileNumber = request.MobileNumber,
					SecondaryNumber = request.SecondaryNumber,
					Email = request.Email,
					CreatedBy = currentUser,
					CreatedDate = request.CreatedDate,
					Status = request.Status,
					BloodType = bloodType
				};

				_applicationDbContext.Donors.Add(donor);
				await _applicationDbContext.SaveChangesAsync();

				var response = new DonorApiResponseRequest
				{
					Id = donor.Id,
					Name = donor.Name,
					TemporaryAddress = donor.TemporaryAddress,
					PermanentAddress = donor.PermanentAddress,
					MobileNumber = donor.MobileNumber,
					SecondaryNumber = donor.SecondaryNumber,
					Email = donor.Email,
					CreatedBy = donor.CreatedBy.Name,
					CreatedDate = donor.CreatedDate,
					Status = donor.Status,
					BloodType = donor.BloodType.Name
				};

				return CreatedAtAction(nameof(GetDonorById), new { id = donor.Id }, response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while creating the donation: {ex.Message}" });
			}
		}
		[HttpPut("{id}")]
		public async Task<ActionResult<DonorApiResponseRequest>> UpdateDonor(int id, [FromBody] DonorApiUpdateRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { Message = "Invalid data." });
			}

			try
			{
				var donor = await _applicationDbContext.Donors
						.Include(d => d.BloodType)
						.FirstOrDefaultAsync(d => d.Id == id);

				if (donor == null)
				{
					return NotFound(new { Message = $"Donor with ID {id} not found." });
				}

				var bloodType = await _applicationDbContext.BloodTypes
						.FirstOrDefaultAsync(bt => bt.Name == request.BloodType);

				if (bloodType == null)
				{
					return BadRequest(new { Message = "Invalid Blood Type." });
				}

				donor.Name = request.Name;
				donor.TemporaryAddress = request.TemporaryAddress;
				donor.PermanentAddress = request.PermanentAddress;
				donor.MobileNumber = request.MobileNumber;
				donor.SecondaryNumber = request.SecondaryNumber;
				donor.Email = request.Email;
				donor.Status = request.Status;
				donor.BloodType = bloodType;
				donor.CreatedDate = request.CreatedDate;

				_applicationDbContext.Donors.Update(donor);
				await _applicationDbContext.SaveChangesAsync();

				var response = new DonorApiResponseRequest
				{
					Id = donor.Id,
					Name = donor.Name,
					TemporaryAddress = donor.TemporaryAddress,
					PermanentAddress = donor.PermanentAddress,
					MobileNumber = donor.MobileNumber,
					SecondaryNumber = donor.SecondaryNumber,
					Email = donor.Email,
					CreatedBy = donor.CreatedBy != null ? donor.CreatedBy.Name : "Unknown",
					CreatedDate = donor.CreatedDate,
					Status = donor.Status,
					BloodType = donor.BloodType != null ? donor.BloodType.Name : "Unknown"
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while updating the donor: {ex.Message}" });
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteDonor(int id)
		{
			try
			{
				var donor = await _applicationDbContext.Donors
						.FirstOrDefaultAsync(d => d.Id == id);

				if (donor == null)
				{
					return NotFound(new { Message = $"Donor with ID {id} not found." });
				}

				_applicationDbContext.Donors.Remove(donor);
				await _applicationDbContext.SaveChangesAsync();

				return NoContent();  
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred while deleting the donor: {ex.Message}" });
			}
		}


	}
}
