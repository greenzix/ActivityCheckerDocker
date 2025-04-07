namespace ActivityChecker.Models.Entities
{
	public class User
	{
		public int id { get; set; }
		public string name { get; set; }
		public string password { get; set; }
		public List<string> roles { get; set; } = new List<string>();
		public DateTime joinedDate { get; set; }
	}
}
