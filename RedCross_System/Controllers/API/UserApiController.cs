using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models.Domain;
using RedCross_System.ViewModel.User;
using RedCross_System.ViewModel.User.UserApi;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserApiController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly SessionHelper _sessionHelper;

		public UserApiController(ApplicationDbContext context, SessionHelper sessionHelper)
		{
			_context = context;
			_sessionHelper = sessionHelper;
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserIndexApi>>> GetAllUsers()
		{
			var users = await _context.Users
							.Include(u => u.Role)
							.Include(u => u.BloodType)
							.Include(u => u.Donations) 
							.Select(u => new UserIndexApi
							{
								Id = u.Id,
								Name = u.Name,
								Email = u.Email,
								Role = u.Role != null ? u.Role.Name : "Unknown",
								RoleId = u.Role != null ? u.RoleId.ToString() : "0",
								BloodType = u.BloodType != null ? u.BloodType.Name : "Unknown",
								BloodTypeId = u.BloodType != null ? u.BloodTypeId.ToString() : "0",
					     Quantity=u.TotalAmount,
								DonationCount = u.Donations.Count,
								LastDonationDate = u.Donations.Count > 0
								? u.Donations.OrderByDescending(d => d.DonationDate).FirstOrDefault().DonationDate
								: DateTime.MinValue
							})
							.ToListAsync();

			return Ok(users);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserIndexApi>> GetUserById(int id)
		{
			var user = await _context.Users
					.Include(u => u.Role)
					.Include(u=>u.BloodType)
					.Include(u=>u.Donations)
					.Where(u => u.Id == id)
					.Select(u => new UserIndexApi
					{
						Id = u.Id,
						Name = u.Name,
						Email = u.Email,
						Role = u.Role != null ? u.Role.Name : "Unknown",
						RoleId = u.Role != null ? u.RoleId.ToString() : "0",
						BloodType = u.BloodType != null ? u.BloodType.Name : "Unknown",
						BloodTypeId = u.BloodType != null ? u.BloodTypeId.ToString() : "0",
					 Quantity=u.TotalAmount,
						DonationCount = u.Donations.Count,
						LastDonationDate = u.Donations.Count > 0
								? u.Donations.OrderByDescending(d => d.DonationDate).FirstOrDefault().DonationDate
								: DateTime.MinValue

					})
					.FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound(new { Message = $"User with ID {id} not found." });
			}

			return Ok(user);
		}

		[HttpPost]
		public async Task<ActionResult<UserApiResponseRequest>> AddUser([FromBody] UserApiAddRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var role = await _context.Roles
						.FirstOrDefaultAsync(r => r.Name == request.Role);

				if (role == null)
				{
					return BadRequest(new { Message = $"Role '{request.Role}' not found." });
				}

				var bloodType = await _context.BloodTypes
					 .FirstOrDefaultAsync(bt => bt.Name == request.BloodType);

				if (bloodType == null)
				{
					return BadRequest(new { Message = $"Blood type '{request.BloodType}' not found." });
				}

				if (await _context.Users.AnyAsync(u => u.Email == request.Email))
				{
					return BadRequest(new { Message = "Email already registered." });
				}

				var user = new User
				{
					Name = request.Name,
					Email = request.Email,
					Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
					Phone = request.Phone,
					RoleId = role.Id,
					BloodTypeId=bloodType.Id,
				
				};

				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				var response = new UserApiResponseRequest
				{
					Id = user.Id,
					Name = user.Name,
					Email = user.Email,
					Role = role.Name,
					Phone = user.Phone,
					BloodType=bloodType.Name,
				};

				return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = "An error occurred while creating the user." });
			}
		}
		

	[HttpPut("{id}")]
		public async Task<ActionResult<UserApiResponseRequest>> UpdateUser(int id, [FromBody] UserApiUpdateRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var user = await _context.Users
						.Include(u => u.Role)
						.Include(u=>u.BloodType)
						.FirstOrDefaultAsync(u => u.Id == id);

				if (user == null)
				{
					return NotFound(new { Message = $"User with ID {id} not found." });
				}

				if (request.Email != null && request.Email != user.Email)
				{
					if (await _context.Users.AnyAsync(u => u.Email == request.Email))
					{
						return BadRequest(new { Message = "Email already registered." });
					}
					user.Email = request.Email;
				}

				if (!string.IsNullOrEmpty(request.Role))
				{
					var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
					if (newRole == null)
					{
						return BadRequest(new { Message = $"Role '{request.Role}' not found." });
					}
					user.RoleId = newRole.Id;
				}
				if (!string.IsNullOrEmpty(request.BloodType))
				{
					var newbloodtype = await _context.BloodTypes.FirstOrDefaultAsync(r => r.Name == request.BloodType);
					if (newbloodtype == null)
					{
						return BadRequest(new { Message = $"BloodType '{request.BloodType}' not found." });
					}
					user.BloodTypeId = newbloodtype.Id;
				}

				if (!string.IsNullOrEmpty(request.Name))
					user.Name = request.Name;

				if (!string.IsNullOrEmpty(request.Phone))
					user.Phone = request.Phone;

				await _context.SaveChangesAsync();

				var response = new UserApiResponseRequest
				{
					Id = user.Id,
					Name = user.Name,
					Email = user.Email,
					Role = user.Role.Name,
					BloodType=user.BloodType.Name,
					Phone = user.Phone
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = "An error occurred while updating the user." });
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			try
			{
				var user = await _context.Users.FindAsync(id);

				if (user == null)
				{
					return NotFound(new { Message = $"User with ID {id} not found." });
				}

				_context.Users.Remove(user);
				await _context.SaveChangesAsync();

				return Ok(new { Message = $"User with ID {id} was successfully deleted." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = "An error occurred while deleting the user." });
			}
		}

	}
	}




