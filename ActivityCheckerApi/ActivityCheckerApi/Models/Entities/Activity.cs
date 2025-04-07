

namespace ActivityCheckerApi.Models.Entities
{
	public class Activity
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public Action Action { get; set; }
		public DateTime Date { get; set; }
	}
}
