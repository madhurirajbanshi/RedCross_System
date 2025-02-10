using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Data;
using RedCross_System.Models.Domain;
using System;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class BloodTypeApiController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public BloodTypeApiController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BloodType>>> GetBloodTypes()
		{
			return await _context.BloodTypes.ToListAsync();
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<BloodType>> GetBloodType(int id)
		{
			var bloodType = await _context.BloodTypes.FindAsync(id);

			if (bloodType == null)
			{
				return NotFound();
			}

			return bloodType;
		}

	

	

		[HttpPost]
		public async Task<ActionResult<BloodType>> PostBloodType(BloodType bloodType)
		{
			_context.BloodTypes.Add(bloodType);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetBloodType", new { id = bloodType.Id }, bloodType);
		}

		

	}
}
