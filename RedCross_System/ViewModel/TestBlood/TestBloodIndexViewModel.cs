using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.TestBlood
{
	public class TestBloodIndexViewModel
	{
		public int Id { get; set; }
		public decimal Quantity { get; set; }
		public string Donor { get; set; }
		public string Donation { get; set; }
		public string TestName { get; set; }
		public string? BagNumber { get; set; }
		public string Status { get; set; }

	}
}
