using ActivityChecker.Models.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
	private const string TokenKey = "authToken";
	private IJSRuntime _jsRuntime;


	public TokenAuthenticationStateProvider(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}


	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Initialize JSRuntime if needed
		await InitializeJsRuntime();

		string token = null;

		try
		{
			token = await CookieHelper.GetCookieAsync(_jsRuntime, TokenKey);
		}
		catch (InvalidOperationException)
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		if(string.IsNullOrEmpty(token)) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(token);

		var expiration = jwt.ValidTo;
		if (expiration < DateTime.UtcNow)
		{
			await MarkUserAsLoggedOut();
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		var claims = ParseClaimsFromJwt(token);

		var identity = new ClaimsIdentity(claims, "Bearer");
		var user = new ClaimsPrincipal(identity);

		Console.WriteLine($"Authentication Type: {identity.AuthenticationType}");
		Console.WriteLine($"Is Authenticated: {identity.IsAuthenticated}");

		return new AuthenticationState(user);
	}

	public async Task MarkUserAsAuthenticated(string token)
	{
		await CookieHelper.SetCookieAsync(_jsRuntime, TokenKey, token);
		var task = this.GetAuthenticationStateAsync();
		this.NotifyAuthenticationStateChanged(task);
	}

	public async Task MarkUserAsLoggedOut()
	{
		await CookieHelper.DeleteCookieAsync(_jsRuntime, TokenKey);
		var task = this.GetAuthenticationStateAsync();
		this.NotifyAuthenticationStateChanged(task);
	}

	private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
	{
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(jwt);
		var claims = token.Claims.ToList();

		var updatedClaims = new List<Claim>();
		foreach (var claim in claims)
		{
			if (claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
			{
				updatedClaims.Add(new Claim(ClaimTypes.Role, claim.Value));
			}
			else
			{
				updatedClaims.Add(claim);
			}
		}

		return updatedClaims;
	}

	public async Task InitializeJsRuntime()
	{
		if (_jsRuntime == null)
		{
			throw new InvalidOperationException("JSRuntime is not initialized.");
		}

		await Task.CompletedTask;  
	}
}
