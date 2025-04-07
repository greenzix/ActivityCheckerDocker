using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace ActivityCheckerApi.Models.Background_Service
{
	public class RoomSessionCleanupService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public RoomSessionCleanupService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using (var scope = _serviceProvider.CreateScope())
					{
						var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
						var now = DateTime.Now;

						const int batchSize = 100;
						List<RoomPresence> expiredSessions;

						do
						{
							expiredSessions = await dbContext.RoomPresences
								.Where(rp => rp.ExpiresAt < now)
								.OrderBy(rp => rp.Id)
								.Take(batchSize)
								.ToListAsync(stoppingToken);


							foreach (var session in expiredSessions)
							{
								dbContext.RoomPresences.Remove(session);
								dbContext.Activities.Add(new Activity
								{
									UserId = session.UserId,
									RoomId = session.RoomId,
									Action = Action.Leave,
									Date = now
								});
							}

							if (expiredSessions.Any())
							{
								await dbContext.SaveChangesAsync(stoppingToken);
							}

						} while (expiredSessions.Count == batchSize && !stoppingToken.IsCancellationRequested);
					}

					await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error in cleanup service: {ex.Message}");
				}
			}
		}
	}
}
