
using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.Services;
using Microsoft.EntityFrameworkCore;

namespace ActivityCheckerApi.Models.Background_Service
{
	public class RoomCodeUpdateService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public RoomCodeUpdateService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while(!stoppingToken.IsCancellationRequested)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
					var rooms = await _context.Rooms.ToListAsync();

					foreach(var room in rooms)
					{
						room.Code = CodeGenerator.RandomString(4);
					}
					_context.Rooms.UpdateRange(rooms);
					await _context.SaveChangesAsync();
				}
				var nextPeriodEnd = TimeHelper.GetNextPeriodEnd(DateTime.Now);
				var delay = nextPeriodEnd - DateTime.Now;

				if (delay > TimeSpan.Zero)
				{
					await Task.Delay(delay, stoppingToken);
				}
				else
				{
					// Handle edge case where delay is invalid
					Console.WriteLine("Next period end is in the past or delay is invalid. Skipping delay.");
					await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Fallback delay
				}
			}
		}
	}
}	
