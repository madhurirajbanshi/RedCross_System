using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.BloodIssue
{
	public class BloodIssueUpdateViewModel
	{
		public int Id { get; set; }
		public string ReceiverName { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public decimal Charge { get; set; }
		public decimal Discount { get; set; }
		public decimal Total { get; set; }
		public string Status { get; set; } = "Active";
		public string Donation { get; set; }
		public List<SelectListItem>? Donations { get; set; }
		public List<SelectListItem>? Donors { get; set; }
		public string Donor { get; set; }
		public List<SelectListItem>? BloodRequirements { get; set; }
		public string BloodRequirement { get; set; }
	}
}
