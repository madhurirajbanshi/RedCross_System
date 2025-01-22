using RedCross_System.Models.Domain;

namespace RedCross_System.ViewModels
{
	public class DonationProfileViewModel
	{
		public Donor Donor { get; set; }
		public ICollection<Donation> Donations { get; set; }
	}
}
