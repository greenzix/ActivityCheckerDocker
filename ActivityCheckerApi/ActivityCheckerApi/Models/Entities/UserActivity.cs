namespace ActivityCheckerApi.Models.Entities
{
	public class UserActivity
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public string? RoomName { get; set; }
		public Action Action { get; set; }
		public DateTime Date { get; set; }
	}
}
