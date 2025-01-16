using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedCross_System.Data;
using RedCross_System.Models.Domain;

namespace RedCross_System.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class DonorApiController : ControllerBase
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public DonorApiController(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		[HttpGet("getalldonor")]
		public List<Donor> GetAllDonor()
		{
			var donor = _applicationDbContext.Donors.ToList();
			return donor;
		}
	}
}
