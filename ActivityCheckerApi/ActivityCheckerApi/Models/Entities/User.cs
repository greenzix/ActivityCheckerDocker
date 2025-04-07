namespace ActivityCheckerApi.Models.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public DateTime JoinedDate { get; set; }
	}
}
