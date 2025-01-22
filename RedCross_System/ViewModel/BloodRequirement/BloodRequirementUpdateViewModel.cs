namespace RedCross_System.ViewModel.BloodRequirement
{
	public class BloodRequirementUpdateViewModel
	{
		public int Id {  get; set; }	
		public string Name { get; set; }
		public string Purpose { get; set; }
		public decimal Quantity { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string Status { get; set; } = "Active";
		public IFormFile Document { get; set; }
	}
}
