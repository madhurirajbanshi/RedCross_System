namespace RedCross_System.ViewModel.User.UserApi
{
	public class UserIndexApi
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public string RoleId { get; set; }
		public string BloodType { get; set; }
		public string BloodTypeId { get; set; }
		public int DonationCount { get; set; }
		public decimal Quantity { get; set; }
		public DateTime LastDonationDate { get; set; } 
	}
}
