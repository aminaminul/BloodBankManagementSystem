using BBDMS.Repository.Data;
using BBDMS.Repository.Interfaces;
using BBDMS.Repository.Repositories;
using BBDMS.Service.Interfaces;
using BBDMS.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IBloodGroupService, BloodGroupService>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
