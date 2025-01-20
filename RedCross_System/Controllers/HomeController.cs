using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RedCross_System.Helpers;
using RedCross_System.Models;
using RedCross_System.Models.Domain;

namespace RedCross_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
		}
		public IActionResult Index()
		{
			var controllers = new List<ControllerInfo>
		{
				new ControllerInfo {Name="Branch",Controller="Branch"},
				new ControllerInfo { Name = "Campaign", Controller = "Campaign" },
				new ControllerInfo { Name = "Donor", Controller = "Donor" },
				new ControllerInfo { Name = "User", Controller = "User" },
				new ControllerInfo { Name = "Donation", Controller = "Donation" },
				new ControllerInfo {Name="TestBlood",Controller="TestBlood"}

		};

			return View(controllers); 
		}

		public IActionResult Privacy()
        {
            return View();
        }

	

	}












}
