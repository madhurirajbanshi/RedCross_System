namespace RedCross_System.Models.Domain
{
	public class BloodIssue
	{
		public int Id { get; set; }
		public string ReceiverName { get; set; }
		public DateTime CreatedDate { get; set; }	= DateTime.UtcNow;
		public decimal Charge { get; set; }
		public decimal Discount { get; set; }
		public decimal Total { get; set; }
		public string Status { get; set; } = "Active";
		public Donation Donation { get; set; }
		public Donor Donor { get; set; }	
		public BloodRequirement BloodRequirement { get; set; }
	}
}
