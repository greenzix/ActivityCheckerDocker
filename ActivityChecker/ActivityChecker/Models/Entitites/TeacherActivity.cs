using ActivityChecker.Models;
using Action = ActivityChecker.Models.Action;

namespace ActivityCheckerApi.Models.Entities
{
	public class TeacherActivity
	{
		public int id { get; set; }
		public int userId { get; set; }
		public string? username { get; set; }
		public int roomId { get; set; }
		public string? roomName { get; set; }
		public Action action { get; set; }
		public DateTime date { get; set; }
	}
}
