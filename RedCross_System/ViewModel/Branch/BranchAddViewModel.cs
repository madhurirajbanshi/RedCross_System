using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.ViewModel.Branch;

public class BranchAddViewModel
{
	[Required(ErrorMessage = "Branch name is required.")]
	[StringLength(100, ErrorMessage = "Branch name cannot be longer than 100 characters.")]
	public string BranchName { get; set; }

	[Required(ErrorMessage = "Location is required.")]
	[StringLength(200, ErrorMessage = "Location cannot be longer than 200 characters.")]
	public string Location { get; set; }

	public string Province { get; set; }

	public List<SelectListItem>? Provinces { get; set; }
	public List<SelectListItem>? Countries { get; set; }

	public string Country { get; set; }




}
