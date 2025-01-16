using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.Models.Domain;

public class Branch
{

	[Key]
	public int BranchId { get; set; }

	public string BranchName { get; set; }

	public string Location { get; set; }
	public User CreatedBy { get; set; }

	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

	public string Status { get; set; } = "Active";

	public Province Province { get; set; }
	public Country Country { get; set; }
}
