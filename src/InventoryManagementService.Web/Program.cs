using InventoryManagementService.Application;
using InventoryManagementService.Infrastructure;
using InventoryManagementService.Infrastructure.Data;
//using InventoryManagementService.Web.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection; // 1. Buni qo'shdik

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Data Protection - Shifrlash kalitlarini fayl tizimida saqlash (Muhim!)
// Bu orqali "Antiforgery token could not be decrypted" xatosi hal bo'ladi
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys"));

// 3. Proxy serverlar orqali HTTPS ni to'g'ri aniqlash
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 4. Authentication (Google)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        // Render'dagi "Authentication__Google__ClientId" ni o'qish uchun:
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ??
                           builder.Configuration["Authentication__Google__ClientId"]!;

        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ??
                               builder.Configuration["Authentication__Google__ClientSecret"]!;
    });

// Cookie sozlamalari
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS majburiy
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// 5. Forwarded Headers ni eng tepada ishlatish
app.UseForwardedHeaders();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.SeedAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapHub<CommentHub>("/hubs/comments");

app.Run();