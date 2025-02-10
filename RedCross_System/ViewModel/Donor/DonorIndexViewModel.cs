namespace RedCross_System.ViewModel.Donor
{
	public class DonorIndexViewModel
	{
		public int Id {  get; set; }
		public string Name { get; set; }
		public string TemporaryAddress { get; set; }
		public string PermanentAddress { get; set; }

		public string MobileNumber { get; set; }
		public string SecondaryNumber { get; set; }
		public string Email { get; set; }
		public string Photo { get; set; }

		public string CreatedBy { get; set; } 
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		public string Status { get; set; } = "active";
		public string BloodType { get; set; }
		public int BloodTypeId { get; set; }



	}
}
