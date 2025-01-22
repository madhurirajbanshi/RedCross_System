using System.ComponentModel.DataAnnotations;

namespace RedCross_System.ViewModel.Login

{
	public class IndexViewModel
	{

		[Required(ErrorMessage = "Username is required")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
