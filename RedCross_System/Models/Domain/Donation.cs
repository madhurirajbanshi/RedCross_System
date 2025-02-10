using System.ComponentModel.DataAnnotations;

namespace RedCross_System.Models.Domain
{
	public class Donation
	{
		[Key]
		public int Id { get; set; }
		public decimal Quantity { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public User CreatedBy { get; set; }
		public string Status { get; set; } = "Active";
		public int DonorId { get; set; }
		public Donor Donor { get; set; }
		public Branch Branch { get; set; }
		public Campaign? Campaign { get; set; }
	
		public DateTime DonationDate { get; set; } = DateTime.UtcNow;	
		public DateTime ExpiryDate { get; set; }=DateTime.UtcNow;
		public string BagNumber { get; set; }
		public DateTime? ScheduledDate { get; set; }
	}
}
