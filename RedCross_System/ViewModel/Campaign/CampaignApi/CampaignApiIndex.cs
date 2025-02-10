namespace RedCross_System.ViewModel.Campaign.CampaignApi
{
	public class CampaignApiIndex
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }

		public String Status { get; set; }

		public string Branch { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
	
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string CreatedBy { get; set; } = "SuperAdmin";

	}
}
