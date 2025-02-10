namespace RedCross_System.Models.Domain
{
	public class Donor
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string TemporaryAddress {  get; set; }
		public string PermanentAddress {  get; set; }
		public decimal Quantity { get; set; }
		public string MobileNumber {  get; set; }
		public string SecondaryNumber { get; set; }
		public string Email { get; set; }
		public string Photo { get; set; }
		public User CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string Status { get; set; } = "active";
		public BloodType BloodType { get; set; }
		public int BloodTypeId { get; set; }
	}
}
