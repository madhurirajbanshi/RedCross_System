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

var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Add session helper to service
builder.Services.AddScoped<SessionHelper>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseMySQL(builder.Configuration.GetConnectionString("MysqlConnection"));
});


// Configure authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		.AddCookie(options =>
		{
			options.LoginPath = "/Login/Index";  // Redirect to login if not authenticated
			options.LogoutPath = "/Login/Logout"; // Redirect to logout path
			options.ExpireTimeSpan = TimeSpan.FromHours(24);  // Set cookie expiration time
			options.SlidingExpiration = true;  // Enable sliding expiration
			options.Cookie.HttpOnly = true;  // Ensure the session cookie is only accessible via HTTP requests
			options.Cookie.IsEssential = true;  // Cookie essential for authentication
		});

// Session configuration for storing session data (e.g., username)
builder.Services.AddDistributedMemoryCache();  // Store session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session timeout duration
	options.Cookie.HttpOnly = true;  // Prevent JavaScript from accessing session cookies
	options.Cookie.IsEssential = true;  // Ensure session cookie is always sent
});

// Add the EmailSender service
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");  // Error handling page
	app.UseHsts();  // HTTP Strict Transport Security
}

app.UseSession();  // Enable session middleware
app.UseHttpsRedirection();  // Redirect HTTP requests to HTTPS
app.UseStaticFiles();  // Serve static files (CSS, JS, images)

app.UseRouting();  // Set up routing middleware

app.UseAuthentication();  // Enable authentication middleware
app.UseAuthorization();  // Enable authorization middleware

app.UseCors(builder => builder
		.AllowAnyOrigin()  // Allows requests from any domain
		.AllowAnyMethod()  // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
		.AllowAnyHeader()  // Allows all request headers
);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();  // Generate Swagger documentation
	app.UseSwaggerUI(c =>  // Serve the Swagger UI
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedCross API V1");
		c.RoutePrefix = "swagger";  // Optional: Makes Swagger UI the default page
	});
}

// Set up the default route
app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Login}/{action=Index}/{id?}");  // Default route is Login/Index

	app.Run();  // Run the application
