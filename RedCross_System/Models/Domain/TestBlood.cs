namespace RedCross_System.Models.Domain
{
	public class TestBlood
	{

		public int Id { get; set; }
		public string TestName { get; set; }
		public Donor Donor { get; set; }
		public Donation Donation { get; set; }
		public string Status { get; set; } = "Active";
		public decimal Quantity { get; set; }
		public User CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }= DateTime.UtcNow;
    

	}
}
