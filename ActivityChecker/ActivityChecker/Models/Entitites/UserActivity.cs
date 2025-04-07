namespace ActivityChecker.Models.Entities
{
	public class UserActivity
	{
		public int id { get; set; }
		public int userId { get; set; }
		public int roomId { get; set; }
		public string? roomName { get; set; }
		public Action action { get; set; }
		public DateTime date { get; set; }
	}
}	
