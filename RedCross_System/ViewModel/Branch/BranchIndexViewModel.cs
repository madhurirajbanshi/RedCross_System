using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace RedCross_System.ViewModel.Branch;

public class BranchIndexViewModel
{

	public int BranchId { get; set; }
	public string BranchName { get; set; }
	public string Location { get; set; }
	public string Province { get; set; }
    public string Country {  get; set; }

	public string CreatedBy { get; set; }

	public DateTime CreatedDate { get; set; }

	public string Status { get; set; }
}
