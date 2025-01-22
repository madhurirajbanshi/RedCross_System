using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.ViewModel.Donation
{
	public class DonationAddViewModel
	{
		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
		public decimal Quantity { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		[Required]
		public string Status { get; set; } = "active";

		public List<SelectListItem>? Donors { get; set; }
		[Required]
		public string Donor { get; set; }

		public List<SelectListItem>? Branches { get; set; }
		[Required]
		public string Branch { get; set; }

		public List<SelectListItem>? Campaigns { get; set; }
		public string? Campaign { get; set; }

		[Required]
		public DateTime DonationDate { get; set; } = DateTime.UtcNow;

		[Required]
		public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;

		[Required]
		[StringLength(50, ErrorMessage = "Bag number can't be longer than 50 characters.")]
		public string BagNumber { get; set; }
	}
}
