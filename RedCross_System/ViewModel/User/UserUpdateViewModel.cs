using Microsoft.AspNetCore.Mvc.Rendering;

namespace RedCross_System.ViewModel.User
{
	public class UserUpdateViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public List<SelectListItem>? Roles { get; set; }
		public string Role { get; set; }

		public List<SelectListItem>? BloodTypes { get; set; }
		public string BloodType { get; set; }
	}
}
