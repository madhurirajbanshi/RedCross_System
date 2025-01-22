using RedCross_System.Models.Domain;

namespace RedCross_System.ViewModels
{
	public class DonorProfileViewModel
	{
		public Donor Donor { get; set; }
		public ICollection<Donation> Donations { get; set; }
	}
}