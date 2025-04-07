using System.ComponentModel.DataAnnotations;

namespace ActivityChecker.Models.Form
{
	public class ResetPasswordModel
	{
		[Required]
		public  string ResetToken { get; set; }
		[Required]
		public  string Username { get; set; }
		[Required]
		public  string Password { get; set; }
	}
}
