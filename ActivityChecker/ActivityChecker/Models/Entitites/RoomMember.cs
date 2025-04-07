namespace ActivityChecker.Models.Entities
{
	public class RoomMember
	{
		public int id { get; set; }
		public int userId { get; set; }
		public string? username { get; set; }
		public int roomId { get; set; }
		public DateTime joinedDate { get; set; }
		public Permission permission { get; set; }
	}
}
