using ActivityCheckerApi.DbContext;
using ActivityCheckerApi.Models.Background_Service;
using ActivityCheckerApi.Models.Entities;
using ActivityCheckerApi.Models.Services;
using ActivityCheckerApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddHostedService<RoomCodeUpdateService>();
builder.Services.AddHostedService<RoomSessionCleanupService>();

builder.Services.AddScoped<RoomViewModel>();
builder.Services.AddScoped<UserViewModel>();
builder.Services.AddScoped<ActivityViewModel>();
builder.Services.AddScoped<TeacherViewModel>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PasswordResetTokenGenerator>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	if(builder.Environment.IsDevelopment())
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("DebugConnection"));
	}
	else
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
	}
});

var configuration = new ConfigurationBuilder()
	.SetBasePath(builder.Environment.ContentRootPath)
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.Build();



builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,	
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["JwtSettings:Issuer"],
			ValidAudience = configuration["JwtSettings:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
		};
	});

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "JWTToken_Auth_API",
		Version = "v1"
	});
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
		{
			new OpenApiSecurityScheme {
				Reference = new OpenApiReference {
					Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
	options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
	options.AddPolicy("TabletPolicy", policy =>
	{
		policy.RequireAssertion(context =>
			context.User.IsInRole("Tablet") || context.User.IsInRole("Admin"));
	});
	options.AddPolicy("TeacherPolicy", policy => 
	{
		policy.RequireAssertion(context =>
			context.User.IsInRole("Teacher") || context.User.IsInRole("Admin"));
	});
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins", policy =>
	{
		policy.AllowAnyOrigin()  
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();


app.MapControllers();

app.Run();
