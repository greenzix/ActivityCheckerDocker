namespace ActivityCheckerApi.Models.Entities
{
	public class RoomPresence
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public DateTime JoinedAt { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}
