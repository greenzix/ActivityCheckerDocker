namespace ActivityCheckerApi.Models.Entities
{
	public class Room
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Limit { get; set; }
		public bool Closed { get; set; }
		public string Code { get; set; }
	}
}
