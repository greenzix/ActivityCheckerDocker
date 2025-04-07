namespace ActivityCheckerApi.Models.Entities
{
	public class RoomPresence
	{
		public int id { get; set; }
		public int userId { get; set; }
		public int roomId { get; set; }
		public DateTime joinedAt { get; set; }
		public DateTime expiresAt { get; set; }
	}
}
