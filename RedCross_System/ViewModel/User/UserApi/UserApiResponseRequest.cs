namespace RedCross_System.ViewModel.User.UserApi
{
	public class UserApiResponseRequest
	{
		public int Id { get; set; }	
		public string Name { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public string? Phone { get; set; }
	}
}
