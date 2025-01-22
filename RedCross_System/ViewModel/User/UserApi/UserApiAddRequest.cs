namespace RedCross_System.ViewModel.User.UserApi
{
	public class UserApiAddRequest
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public string Password { get; set; }
		public string? Phone { get; set; }
	}
}
