using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.Campaign
{
	public class CampaignUpdateViewModel
	{

		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public DateTime StartDate { get; set; } = DateTime.UtcNow;
		public DateTime EndDate { get; set; } = DateTime.UtcNow;

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string CreatedBy { get; set; } 
		public String Status { get; set; } 
		public List<SelectListItem> ?Branches { get; set; }
		public string Branch { get; set; }

	

	}
}
