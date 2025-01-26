using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RedCross_System.Data;
using RedCross_System.Helpers;
using RedCross_System.Models;
using RedCross_System.Models.Domain;

namespace RedCross_System.Controllers
{
    public class HomeController : Controller
    {
    private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
			      _context=context;
		}
		public IActionResult Index()
		{
			var controllers = new List<ControllerInfo>
		{
				new ControllerInfo {Name="Branch",Controller="Branch",Count = _context.Branches.Count() },
				new ControllerInfo { Name = "Campaign", Controller = "Campaign",Count = _context.Campaigns.Count() },
				new ControllerInfo { Name = "Donor", Controller = "Donor" ,Count = _context.Donors.Count() },
				new ControllerInfo { Name = "User", Controller = "User",Count = _context.Users.Count()  },
				new ControllerInfo { Name = "Donation", Controller = "Donation",Count = _context.Donations.Count()  },
				new ControllerInfo {Name="TestBlood",Controller="TestBlood",Count = _context.TestBloods.Count() }

		};

			return View(controllers); 
		}

		public IActionResult Privacy()
        {
            return View();
        }

	

	}












}
