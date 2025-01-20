namespace RedCross_System.ViewModel.Donor.DonorApi
{
	public class DonorApiAddRequest
	{
		public string Name { get; set; }
		public string TemporaryAddress { get; set; }
		public string PermanentAddress { get; set; }

		public string MobileNumber { get; set; }
		public string SecondaryNumber { get; set; }
		public string Email { get; set; }

		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		public string Status { get; set; } = "active";
		public string BloodType { get; set; }

	}
}