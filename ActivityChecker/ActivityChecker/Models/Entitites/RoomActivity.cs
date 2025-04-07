namespace ActivityChecker.Models.Entities
{
	public class RoomActivity
	{
		public int id { get; set; }
		public int userId { get; set; }
		public int roomId { get; set; }
		public string? username { get; set; }
		public Action action { get; set; }
		public DateTime date { get; set; }
	}
}
