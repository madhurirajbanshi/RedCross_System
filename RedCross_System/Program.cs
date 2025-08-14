using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RedCross_System.Controllers;
using RedCross_System.Helpers;
using OfficeOpenXml;
using RedCross_System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using RedCross_System.Service;
using RedCross_System.Models.Domain;
using RedCrossSystem.Core.src.ProvinceFeature;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using RedCross_System.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RedCross_System.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Authorization;

// TLS config
using System.Net;
using System.Security.Authentication;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use TLS 1.2 and TLS 1.3
builder.WebHost.ConfigureKestrel(options =>
{
	options.ConfigureHttpsDefaults(httpsOptions =>
	{
		httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
	});
});

// Ensure proper certificate validation (for production)
// Only for development purposes, validate self-signed certificates
ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
{
	if (sslPolicyErrors == SslPolicyErrors.None)
	{
		return true; // If no errors, the certificate is valid
	}
	else
	{
		// Log the SSL errors for better tracking and debugging
		Log.Error($"SSL Certificate error: {sslPolicyErrors}");
		return false; // Reject the certificate if errors are found
	}
};

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.File("Logs/redcross_logs.txt", rollingInterval: RollingInterval.Day) // Set log file path with daily rolling
		.CreateLogger();

builder.Host.UseSerilog();

// Set Excel Package License
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Add session helper to service
builder.Services.AddScoped<SessionHelper>();
builder.Services.AddScoped<ProvinceService>();
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		.AddCookie(options =>
		{
			options.LoginPath = "/Login/Index";  // Path to redirect if not authenticated
			options.LogoutPath = "/Login/Logout"; // Path to logout
			options.SlidingExpiration = true;    // Enable sliding expiration for cookies
			options.ExpireTimeSpan = TimeSpan.FromHours(1);  // Set expiration time
		});

builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout duration
});

builder.Services.AddScoped<JwtService>();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "RedCross API",
		Version = "v1",
		Description = "API documentation for the Red Cross Blood Management System"
	});

	// Configure JWT Authentication for Swagger
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter 'Bearer' [space] and your valid token.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
				{
						new OpenApiSecurityScheme
						{
								Reference = new OpenApiReference
								{
										Type = ReferenceType.SecurityScheme,
										Id = "Bearer"
								}
						},
						new string[] { }
				}
		});
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlite(builder.Configuration.GetConnectionString("Data Source=redcross.db"));
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],  // The issuer from configuration
		ValidAudience = builder.Configuration["Jwt:Audience"],  // The audience from configuration
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))  // Your secret key
	};
});

// Add the EmailSender service
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable session support
app.UseSession();

// Force HTTPS
app.UseHttpsRedirection();

// Serve static files (CSS, JS, images)
app.UseStaticFiles();

// Set up routing middleware
app.UseRouting();

// Use authentication middleware (this is required before authorization)
app.UseAuthentication();

// Use authorization middleware (this must be between UseRouting and UseEndpoints)
app.UseAuthorization();

// CORS configuration for all origins, methods, and headers
app.UseCors(builder => builder
		.AllowAnyOrigin()  // Allows requests from any domain
		.AllowAnyMethod()  // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
		.AllowAnyHeader()  // Allows all request headers
);

// Swagger UI setup (only in development environment)
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();  // Generate Swagger documentation
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedCross API V1");
		c.RoutePrefix = "swagger";  // Optional: Makes Swagger UI the default page
	});
}

// Error handling setup based on environment
if (app.Environment.IsDevelopment())
{
	app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");  // Custom error handling for development
}
else
{
	app.UseExceptionHandler("/Home/Error");  // Error handling page for production
	app.UseHsts();  // HTTP Strict Transport Security (for production)
}

// Set up the default route
app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Login}/{action=Index}/{id?}");  // Default route is Login/Index

app.Run();  // Run the application
