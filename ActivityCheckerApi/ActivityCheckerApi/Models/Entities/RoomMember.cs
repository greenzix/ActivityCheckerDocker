namespace ActivityCheckerApi.Models.Entities
{
	public class RoomMember
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string? Username { get; set; }
		public int RoomId { get; set; }
		public DateTime JoinedDate { get; set; }
		public Permission Permission { get; set; }
	}
}
