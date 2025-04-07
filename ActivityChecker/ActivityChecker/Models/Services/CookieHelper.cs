using Microsoft.JSInterop;

namespace ActivityChecker.Models.Services
{
	public static class CookieHelper
	{
		public static async Task SetCookieAsync(IJSRuntime jsRuntime, string key, string value, int days = 7)
		{
			var expirationDate = DateTime.UtcNow.AddDays(days);
			var cookieValue = $"{key}={value}; expires={expirationDate:R}; path=/";
			await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{cookieValue}\"");
		}

		public static async Task<string> GetCookieAsync(IJSRuntime jsRuntime, string key)
		{
			string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
			var cookie = cookies.Split(';').FirstOrDefault(c => c.Trim().StartsWith($"{key}="));
			return cookie?.Split('=')[1]?.Trim();
		}

		public static async Task DeleteCookieAsync(IJSRuntime jsRuntime, string key)
		{
			await SetCookieAsync(jsRuntime, key, string.Empty, -1); 
		}
	}
}
