using ActivityChecker.Components;
using ActivityChecker.Models.Services;
using ActivityChecker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
	options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(10); 
	options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
})
.AddHubOptions(options =>
{
	options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
	options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
//builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();

builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PasswordResetService>();


//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7016") });
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp =>
{
	var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
	var cookies = httpContextAccessor.HttpContext?.Request.Cookies;

	// Retrieve the token from the cookies
	string token = cookies?["authToken"] ?? "";

	var authHandler = new AuthenticationHeaderHandler(token)
	{
	InnerHandler = new HttpClientHandler()
	};

	if (builder.Environment.IsDevelopment())
	{
		return new HttpClient(authHandler)
		{
			//BaseAddress = new Uri("http://localhost:8000/")
			BaseAddress = new Uri("https://localhost:7016/")
		};
	}
	else
	{
		return new HttpClient(authHandler)
		{
			BaseAddress = new Uri("http://ActivityCheckerApi:8080/")
		};
	}
	//return new HttpClient(authHandler)
	//{
	//	BaseAddress = new Uri("http://ActivityCheckerApi:8080/")
	//};
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = "localhost",
			ValidAudience = "localhost",
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("averylongsecretkeythatisrequiredtobeused"))
		};
		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				if (context.Request.Cookies.ContainsKey("authToken"))
				{
					context.Token = context.Request.Cookies["authToken"];
				}
				return Task.CompletedTask;
			}
		};
	});

//builder.Services.AddAuthenticationCore();

builder.Services.AddAuthorizationCore();
//builder.Services.AddCascadingAuthenticationState();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();




app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
