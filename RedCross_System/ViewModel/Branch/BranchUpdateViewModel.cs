using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.Branch;

	public class BranchUpdateViewModel
	{
		public int BranchId { get; set; }
		public string BranchName { get; set; }
		public string Location { get; set; }

		public List<SelectListItem>? Provinces { get; set; }
		public string Province { get; set; }
		public DateTime CreatedDate { get; set; }
		public int Id { get; set; }
	  public List<SelectListItem>?Countries { get; set; }
    public string Country { get; set; }
	public String Status { get; set; } = "Active";


}
