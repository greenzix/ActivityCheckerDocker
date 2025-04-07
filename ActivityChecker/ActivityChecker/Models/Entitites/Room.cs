namespace ActivityChecker.Models.Entities
{
	public class Room
	{
		public int id { get; set; }
		public string name { get; set; }
		public int limit { get; set; }
		public bool closed { get; set; }
		public string code { get; set; }
	}
}
