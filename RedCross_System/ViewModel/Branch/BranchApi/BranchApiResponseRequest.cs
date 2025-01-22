namespace RedCross_System.ViewModel.Branch.BranchApi
{
	public class BranchApiResponseRequest
	{
		public string BranchId { get; set; }
		public string BranchName { get; set; }
		public string Location { get; set; }
		public string Province { get; set; }
		public string Country { get; set; }
		public string CreatedBy {  get; set; }	
		public DateTime CreatedDate { get; set; }
	}
}
