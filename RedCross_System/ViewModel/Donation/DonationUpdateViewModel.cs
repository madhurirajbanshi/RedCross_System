using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.ViewModel.Donation
{
	public class DonationUpdateViewModel
	{
		[Key]
		public int Id { get; set; }
		public decimal Quantity { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string Status { get; set; } = "active";
		public List<SelectListItem>? Donors { get; set; }
		public string Donor { get; set; }
		public List<SelectListItem> ?Branches { get; set; }
		public string Branch { get; set; }

		public List<SelectListItem>?Campaigns { get; set; }
		public string? Campaign { get; set; }
		public DateTime DonationDate { get; set; }	
		public DateTime ExpiryDate { get; set; }
		public string BagNumber { get; set; }



	}
}
