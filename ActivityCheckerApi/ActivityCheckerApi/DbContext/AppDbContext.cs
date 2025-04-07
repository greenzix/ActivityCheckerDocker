namespace ActivityCheckerApi.DbContext
{
	using ActivityCheckerApi.Models.Entities;
	using Microsoft.EntityFrameworkCore;


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
		public DbSet<Activity> Activities { get; set; }
		public DbSet<RoomOrder> RoomOrders { get; set; }
		public DbSet<RoomPresence> RoomPresences { get; set; }

	}
}
