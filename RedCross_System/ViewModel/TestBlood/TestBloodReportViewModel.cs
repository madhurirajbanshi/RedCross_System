namespace RedCross_System.ViewModel.TestBlood
{
	public class TestBloodReportViewModel
	{
		public string TestName { get; set; }
		public string? BagNumber { get; set; }

		public string MobileNumber {  get; set; }


		public string DonorName { get; set; }
		public string BranchName { get; set; }
		public DateTime DonationDate { get; set; }
		public decimal Quantity { get; set; }
		public string BloodType { get; set; }
		public string Status { get; set; }
	}
}
