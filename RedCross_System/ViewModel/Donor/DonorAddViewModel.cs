using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.Donor
{
	public class DonorAddViewModel
	{
		public string Name { get; set; }
		public string TemporaryAddress { get; set; }
		public string PermanentAddress { get; set; }

		public string MobileNumber { get; set; }
		public string SecondaryNumber { get; set; }
		public string Email { get; set; }
		public IFormFile File { get; set; }

		public string Status { get; set; } = "active";

		public List<SelectListItem>? BloodTypes { get; set; }
		public string BloodType {  get; set; }
	}
}
