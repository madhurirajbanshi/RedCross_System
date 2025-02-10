namespace RedCross_System.ViewModel.Campaign.CampaignApi
{
	public class CampaignApiResponseRequest
	{
		public int Id { get; set; }	
		public string Name { get; set; }
		public string Address { get; set; }
		public DateTime StartDate { get; set; } = DateTime.UtcNow;
		public DateTime EndDate { get; set; } = DateTime.UtcNow;
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public String Status { get; set; }

		public string Branch { get; set; }
	}
}
