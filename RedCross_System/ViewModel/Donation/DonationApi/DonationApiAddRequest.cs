namespace RedCross_System.ViewModel.Donation.DonationApi
{
	public class DonationApiAddRequest
	{
		public decimal Quantity { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string CreatedBy { get; set; }
		public string Status { get; set; }
		public string Donor { get; set; }
		public string Branch { get; set; }
		public string Campaign { get; set; }
    public DateTime DonationDate { get; set; } = DateTime.UtcNow;
		public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;
		public string BagNumber { get; set; }
		public DateTime? ScheduledDate { get; set; }
	}

}
