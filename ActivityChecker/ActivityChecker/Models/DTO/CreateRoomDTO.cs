using System.ComponentModel.DataAnnotations;

namespace ActivityChecker.Models.DTO
{
	public class CreateRoomDTO
	{
		public string name { get; set; }
		public int limit { get; set; } = 20;
		public bool closed { get; set; } = false;
	}
}
