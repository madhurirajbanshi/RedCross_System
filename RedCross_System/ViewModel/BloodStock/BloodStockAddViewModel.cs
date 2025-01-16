namespace RedCross_System.ViewModel.BloodStock
{
	using RedCross_System.Models.Domain;
	public class BloodStockAddViewModel
	{
		public List<Donor> CampaignDonors { get; set; }
		public List<Donor> NonCampaignDonors { get; set; }


		public List<int> CampaignDonorIds { get; set; }
		public List<int> NonCampaignDonorIds { get; set; }
	}
}
