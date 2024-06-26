using Microsoft.EntityFrameworkCore;
using MahediBookStore.DataAccess.Data;
using MahediBookStore.DataAccess.Repository;
using MahediBookStore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using MahediBookStore.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using MahediBookStore.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IEmailSender, EmailSender>(); // Add IEmailSender service

// overriding some default paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Adding the session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);

    // to avoid client side script accessing, 
    // its true to make it only accessible from the server
    options.Cookie.HttpOnly = true;

    // this ensure the session cookie will be sent with the request always even when no cookies are sent and regardless the cookie consent settings of the user's browser
    options.Cookie.IsEssential = true;
});

// Injecting Stripe secret and publishable key to StripeSetting properties
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<IUnitOfWorks, UnitOfWork>();

// Add the IDbInitializer to the Service
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Stripe configuration
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// adding session in the request pipeline
app.UseSession();

// Initialize database pending migration, create role and admin (if not exists)
SeedDatabase();

app.MapRazorPages();



app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();



void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}