using InventoryManagementService.Application;
using InventoryManagementService.Infrastructure;
using InventoryManagementService.Infrastructure.Data;
using InventoryManagementService.Web.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Authentication (Google + Facebook) – secretlarni env var dan oling
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });
    //.AddFacebook(options =>
    //{
    //    options.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
    //    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    //});

// Cookie sozlamalari
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Controllers + Views + SignalR
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Auto-migrate va seed – HAR QANDAY muhitda (Development va Production ham)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Migrations ni qo‘llash (agar kerak bo‘lsa)
    await db.Database.MigrateAsync();

    // Seed data (test userlar yaratish)
    await DbInitializer.SeedAsync(app.Services);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // HTTPS Strict Transport Security
}

// Middleware’lar (HTTPS redirection ni o‘chirib qo‘ydik, Render o‘zi hal qiladi)
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Routes va SignalR hub
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<CommentHub>("/hubs/comments");

// Ilovani ishga tushirish
app.Run();