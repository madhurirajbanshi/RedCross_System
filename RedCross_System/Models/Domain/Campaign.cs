namespace RedCross_System.Models.Domain;

	public class Campaign
	{

		public int Id { get; set; }
		public string Name { get; set; }	
		public string Address {  get; set; }
		public DateTime StartDate { get; set; }	= DateTime.Now;
		public DateTime EndDate { get; set; } = DateTime.UtcNow;

		public DateTime CreatedDate {  get; set; } = DateTime.UtcNow;
		public User CreatedBy { get; set; }
	public string Status { get; set; } = "Active";
		public Branch Branch { get; set; }
		
	}
