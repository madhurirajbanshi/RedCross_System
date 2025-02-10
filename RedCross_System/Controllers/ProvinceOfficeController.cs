using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedCross_System.CoreModule.ProvinceFeature;
using RedCross_System.CoreModule.ProvinceOfficeFeature;
using RedCrossSystem.Core.src.ProvinceFeature;

namespace RedCross_System.Controllers
{
	public class ProvinceOfficeController : Controller
	{
		

		private readonly ProvinceService _provinceService;

		public ProvinceOfficeController(ProvinceService provinceService)
		{
			_provinceService = provinceService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var provinces = await _provinceService.GetAll();
			return View(provinces);
		}


		[HttpGet]
		public IActionResult Add()
		{
			return View(); 
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(ProvinceCreateDto dto)
		{
			if (!ModelState.IsValid)
			{
				return View(dto); 
			}

			await _provinceService.Create(dto);
			return RedirectToAction("Index"); 
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			try
			{
				await _provinceService.ToggleStatus(id);
			}
			catch (ProvinceNotFountException)
			{
				return NotFound();
			}

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var province = await _provinceService.GetAll();  

			var provinceToUpdate = province.FirstOrDefault(p => p.Id == id);

			if (provinceToUpdate == null)
			{
				return NotFound(); 
			}


			var dto = new ProvinceUpdateDto
			{
				Id = provinceToUpdate.Id,
				Name = provinceToUpdate.Name,
				Description = provinceToUpdate.Description
			};

			return View(dto); 
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(ProvinceUpdateDto dto)
		{
			if (!ModelState.IsValid)
			{
				return View(dto); 
			}

			try
			{
				await _provinceService.Update(dto);
			}
			catch (ProvinceNotFountException)
			{
				return NotFound(); 
			}

			return RedirectToAction("Index"); 
		}
	}


}

