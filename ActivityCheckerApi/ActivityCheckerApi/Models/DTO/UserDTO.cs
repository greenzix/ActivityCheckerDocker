using System.ComponentModel.DataAnnotations;

namespace ActivityCheckerApi.Models.DTO
{
	public class UserDTO
	{
		[Required(ErrorMessage = "Username is required.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
