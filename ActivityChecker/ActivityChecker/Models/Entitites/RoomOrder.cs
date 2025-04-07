namespace ActivityChecker.Models.Entities
{
	public class RoomOrder
	{
		public int id { get; set; }
		public int userId { get; set; }
		public int roomId { get; set; }
		public DateTime joinedDate { get; set; }
		public Permission permission { get; set; }
	}
}
