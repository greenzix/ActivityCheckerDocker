namespace ActivityCheckerApi.Models.Services
{
	public class TimeHelper
	{
		private static readonly TimeSpan StartOfDay = new TimeSpan(7, 15, 0);
		private static readonly TimeSpan PeriodDuration = TimeSpan.FromMinutes(45);
		private static readonly TimeSpan BreakDuration = TimeSpan.FromMinutes(5);

		public static DateTime GetNextPeriodEnd(DateTime currentTime)
		{
			var today = currentTime.Date;
			var currentDayStart = today.Add(StartOfDay);

			var elapsed = currentTime - currentDayStart;

			if (elapsed < TimeSpan.Zero)
			{

				return currentDayStart.Add(PeriodDuration);
			}

			var cycleDuration = PeriodDuration + BreakDuration;
			int cyclesCompleted = (int)(elapsed.TotalMinutes / cycleDuration.TotalMinutes);

			DateTime currentCycleStart = currentDayStart.Add(cycleDuration.Multiply(cyclesCompleted));
			DateTime periodEnd = currentCycleStart + PeriodDuration;
			DateTime breakEnd = currentCycleStart + cycleDuration;

			if (currentTime < periodEnd)
			{
				return periodEnd;
			}
			else if (currentTime < breakEnd)
			{
				DateTime nextCycleStart = breakEnd; // Start of next period
				return nextCycleStart + PeriodDuration;
			}
			else
			{
				// Handle edge case where currentTime is beyond calculated cycles (shouldn't typically occur)
				DateTime nextCycleStart = currentDayStart.Add(cycleDuration.Multiply(cyclesCompleted + 1));
				return nextCycleStart + PeriodDuration;
			}
		}
	}

	public static class TimeSpanExtensions
	{
		public static TimeSpan Multiply(this TimeSpan timeSpan, int factor)
		{
			return TimeSpan.FromTicks(timeSpan.Ticks * factor);
		}
	}
}