using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.TestBlood
{
	public class TestBloodUpdateViewModel
	{
		public int Id { get; set; }
		public List<SelectListItem>? Donors { get; set; }
		public string Donor { get; set; }
		public List<SelectListItem>? Donations { get; set; }
		public string Donation { get; set; }
		public string TestName { get; set; }

	}
}
