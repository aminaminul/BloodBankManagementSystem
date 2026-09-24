using System;
using System.Threading.RateLimiting;
using BBDMS.Repository.Data;
using BBDMS.Repository.Interfaces;
using BBDMS.Repository.Repositories;
using BBDMS.Service.Interfaces;
using BBDMS.Service.Services;
using BBDMS.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration with null check
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in configuration.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Dependency Injection - Repository & Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IBloodGroupService, BloodGroupService>();
builder.Services.AddScoped<IHospitalService, HospitalService>();
builder.Services.AddScoped<IBloodBankService, BloodBankService>();
builder.Services.AddScoped<IAmbulanceService, AmbulanceHelpService>();
builder.Services.AddScoped<IOxygenService, OxygenServiceImplementation>();

// Hardened Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Hardened Anti-CSRF Cookie
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Built-in Rate Limiting (.NET 10)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    // Auth endpoints: 10 attempts per minute
    options.AddFixedWindowLimiter("authLimiter", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    // Emergency submission endpoints: 15 per minute
    options.AddFixedWindowLimiter("emergencyLimiter", opt =>
    {
        opt.PermitLimit = 15;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

// SignalR Real-Time Alerts
builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed initial database data and upgrade plain-text passwords
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Security Response Headers Middleware (Clickjacking, MIME-sniffing, Referrer Policy)
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(self)");
    await next();
});

app.UseRouting();
app.UseRateLimiter();
app.UseSession();
app.UseAuthorization();

// SignalR Hub Endpoint
app.MapHub<EmergencyHub>("/emergencyHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
