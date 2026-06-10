using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Enums;

namespace WholesaleCRM.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        if (await context.ProductCategories.AnyAsync()) return;

        // Users
        var admin = new AppUser { UserName = "admin@crm.uz", Email = "admin@crm.uz", FullName = "Nodir Toshmatov", EmailConfirmed = true };
        var sales1 = new AppUser { UserName = "dilnoza@crm.uz", Email = "dilnoza@crm.uz", FullName = "Dilnoza Yusupova", EmailConfirmed = true };
        var sales2 = new AppUser { UserName = "jasur@crm.uz", Email = "jasur@crm.uz", FullName = "Jasur Karimov", EmailConfirmed = true };

        await userManager.CreateAsync(admin, "Admin@123!");
        await userManager.CreateAsync(sales1, "Sales@123!");
        await userManager.CreateAsync(sales2, "Sales@123!");

        // Product Categories
        var cats = new List<ProductCategory>
        {
            new() { Name = "Ko'ylaklar", Description = "Erkaklar va ayollar ko'ylaklari" },
            new() { Name = "Shimlar", Description = "Jins va rasmiy shimlar" },
            new() { Name = "Kurtka va Palto", Description = "Qishki va bahorgi kiyimlar" },
            new() { Name = "Ko'ylaklar (Ayollar)", Description = "Ayollar ko'ylaklari va liboslari" },
            new() { Name = "Bolalar Kiyimi", Description = "0-14 yosh bolalar uchun kiyimlar" },
            new() { Name = "Aksessuarlar", Description = "Kamar, kepka, sharf va boshqalar" }
        };
        context.ProductCategories.AddRange(cats);
        await context.SaveChangesAsync();

        // Products
        var products = new List<Product>
        {
            new() { Name = "Klassik Oq Ko'ylak", SKU = "SHT-001", ProductCategoryId = cats[0].Id, UnitPrice = 45000, StockQuantity = 500, IsActive = true },
            new() { Name = "Polo Ko'ylak (Rangli)", SKU = "SHT-002", ProductCategoryId = cats[0].Id, UnitPrice = 35000, StockQuantity = 800, IsActive = true },
            new() { Name = "Biznes Ko'ylak (Slim Fit)", SKU = "SHT-003", ProductCategoryId = cats[0].Id, UnitPrice = 65000, StockQuantity = 300, IsActive = true },
            new() { Name = "Casual T-Shirt", SKU = "SHT-004", ProductCategoryId = cats[0].Id, UnitPrice = 20000, StockQuantity = 1200, IsActive = true },
            new() { Name = "Klassik Jins (Straight)", SKU = "PNT-001", ProductCategoryId = cats[1].Id, UnitPrice = 75000, StockQuantity = 600, IsActive = true },
            new() { Name = "Rasmiy Shim (Qora)", SKU = "PNT-002", ProductCategoryId = cats[1].Id, UnitPrice = 65000, StockQuantity = 400, IsActive = true },
            new() { Name = "Kargo Shim", SKU = "PNT-003", ProductCategoryId = cats[1].Id, UnitPrice = 70000, StockQuantity = 350, IsActive = true },
            new() { Name = "Qishki Paxta Kurtka", SKU = "JKT-001", ProductCategoryId = cats[2].Id, UnitPrice = 150000, StockQuantity = 200, IsActive = true },
            new() { Name = "Bahorgi Kurtka (Sport)", SKU = "JKT-002", ProductCategoryId = cats[2].Id, UnitPrice = 90000, StockQuantity = 250, IsActive = true },
            new() { Name = "Fleece Hoodie", SKU = "JKT-003", ProductCategoryId = cats[2].Id, UnitPrice = 80000, StockQuantity = 400, IsActive = true },
            new() { Name = "Yozgi Gullik Ko'ylak", SKU = "DRS-001", ProductCategoryId = cats[3].Id, UnitPrice = 60000, StockQuantity = 300, IsActive = true },
            new() { Name = "Kechki Libos", SKU = "DRS-002", ProductCategoryId = cats[3].Id, UnitPrice = 120000, StockQuantity = 150, IsActive = true },
            new() { Name = "Bolalar T-Shirt To'plami (5 ta)", SKU = "KID-001", ProductCategoryId = cats[4].Id, UnitPrice = 55000, StockQuantity = 500, IsActive = true },
            new() { Name = "Bolalar Qishki Kurtka", SKU = "KID-002", ProductCategoryId = cats[4].Id, UnitPrice = 85000, StockQuantity = 200, IsActive = true },
            new() { Name = "Charm Kamar", SKU = "ACC-001", ProductCategoryId = cats[5].Id, UnitPrice = 25000, StockQuantity = 1000, IsActive = true },
            new() { Name = "Baseball Kepka", SKU = "ACC-002", ProductCategoryId = cats[5].Id, UnitPrice = 18000, StockQuantity = 800, IsActive = true },
            new() { Name = "Jun Sharf", SKU = "ACC-003", ProductCategoryId = cats[5].Id, UnitPrice = 22000, StockQuantity = 600, IsActive = true },
        };
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Customers
        var customers = new List<Customer>
        {
            new() { CompanyName = "Toshkent Textile LLC", Industry = "Ulgurji savdo", Email = "info@ttextile.uz", Phone = "+998712345678", City = "Toshkent", Country = "O'zbekiston", Status = CustomerStatus.Active, AssignedToId = admin.Id, Notes = "Katta hajmdagi doimiy mijoz", CreatedAt = DateTime.UtcNow.AddMonths(-8) },
            new() { CompanyName = "Samarqand Fashion House", Industry = "Kiyim ishlab chiqarish", Email = "contact@sfashion.uz", Phone = "+998662345678", City = "Samarqand", Country = "O'zbekiston", Status = CustomerStatus.Active, AssignedToId = sales1.Id, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new() { CompanyName = "Buxoro Clothes Market", Industry = "Chakana savdo", Email = "buxoro@clothes.uz", Phone = "+998652345678", City = "Buxoro", Country = "O'zbekiston", Status = CustomerStatus.Active, AssignedToId = sales1.Id, CreatedAt = DateTime.UtcNow.AddMonths(-5) },
            new() { CompanyName = "Andijon Garment Trading", Industry = "Ulgurji savdo", Email = "info@agarment.uz", Phone = "+998742345678", City = "Andijon", Country = "O'zbekiston", Status = CustomerStatus.Prospect, AssignedToId = sales2.Id, CreatedAt = DateTime.UtcNow.AddMonths(-2) },
            new() { CompanyName = "Farg'ona Style Shop", Industry = "Chakana savdo", Email = "farghona@style.uz", Phone = "+998732345678", City = "Farg'ona", Country = "O'zbekiston", Status = CustomerStatus.Active, AssignedToId = sales2.Id, CreatedAt = DateTime.UtcNow.AddMonths(-4) },
            new() { CompanyName = "Namangan Textiles Group", Industry = "Ulgurji savdo", Email = "info@namangan-t.uz", Phone = "+998692345678", City = "Namangan", Country = "O'zbekiston", Status = CustomerStatus.Active, AssignedToId = admin.Id, CreatedAt = DateTime.UtcNow.AddMonths(-7) },
            new() { CompanyName = "Navoiy Fashion House", Industry = "Kiyim savdosi", Email = "navoiy@fashion.uz", Phone = "+998792345678", City = "Navoiy", Country = "O'zbekiston", Status = CustomerStatus.Inactive, AssignedToId = sales1.Id, CreatedAt = DateTime.UtcNow.AddMonths(-10) },
            new() { CompanyName = "Qashqadaryo Clothes Center", Industry = "Chakana savdo", Email = "qashqa@clothes.uz", Phone = "+998752345678", City = "Qarshi", Country = "O'zbekiston", Status = CustomerStatus.Prospect, AssignedToId = sales2.Id, CreatedAt = DateTime.UtcNow.AddDays(-45) },
            new() { CompanyName = "Xorazm Textile Trading", Industry = "Ulgurji savdo", Email = "xorazm@textile.uz", Phone = "+998622345678", City = "Urganch", Country = "O'zbekiston", Status = CustomerStatus.Active, AssignedToId = admin.Id, CreatedAt = DateTime.UtcNow.AddMonths(-3) },
            new() { CompanyName = "Surxondaryo Fashion", Industry = "Kiyim ishlab chiqarish", Email = "surxon@fashion.uz", Phone = "+998762345678", City = "Termiz", Country = "O'zbekiston", Status = CustomerStatus.Prospect, AssignedToId = sales1.Id, CreatedAt = DateTime.UtcNow.AddDays(-20) },
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        // Contacts
        var contacts = new List<Contact>
        {
            new() { FirstName = "Alisher", LastName = "Nazarov", Email = "a.nazarov@ttextile.uz", Phone = "+998901234567", Position = "Direktori", CustomerId = customers[0].Id, IsPrimary = true },
            new() { FirstName = "Malika", LastName = "Umarova", Email = "m.umarova@ttextile.uz", Phone = "+998901234568", Position = "Xarid menejeri", CustomerId = customers[0].Id, IsPrimary = false },
            new() { FirstName = "Bobur", LastName = "Xolmatov", Email = "b.xolmatov@sfashion.uz", Phone = "+998912345678", Position = "Egasi", CustomerId = customers[1].Id, IsPrimary = true },
            new() { FirstName = "Shahlo", LastName = "Tursunova", Email = "sh.tursunova@bclothes.uz", Phone = "+998913456789", Position = "Savdo bo'yicha direktori", CustomerId = customers[2].Id, IsPrimary = true },
            new() { FirstName = "Ulugbek", LastName = "Qodirov", Email = "u.qodirov@agarment.uz", Phone = "+998914567890", Position = "Bosh direktor", CustomerId = customers[3].Id, IsPrimary = true },
            new() { FirstName = "Nilufar", LastName = "Hasanova", Email = "n.hasanova@fstyle.uz", Phone = "+998915678901", Position = "Marketing menejeri", CustomerId = customers[4].Id, IsPrimary = true },
            new() { FirstName = "Sardor", LastName = "Mirzayev", Email = "s.mirzayev@namangan-t.uz", Phone = "+998916789012", Position = "Xarid bo'yicha menejer", CustomerId = customers[5].Id, IsPrimary = true },
            new() { FirstName = "Feruza", LastName = "Ergasheva", Email = "f.ergasheva@xorazm.uz", Phone = "+998917890123", Position = "Direktor", CustomerId = customers[8].Id, IsPrimary = true },
        };
        context.Contacts.AddRange(contacts);
        await context.SaveChangesAsync();

        // Deals
        var now = DateTime.UtcNow;
        var deals = new List<Deal>
        {
            new() { Title = "Ko'ylaklar ulgurji buyurtmasi", CustomerId = customers[0].Id, AssignedToId = admin.Id, Status = DealStatus.Won, TotalAmount = 45000000, CreatedAt = now.AddMonths(-6), UpdatedAt = now.AddMonths(-5) },
            new() { Title = "Qishki kurtka kollekciyasi", CustomerId = customers[0].Id, AssignedToId = admin.Id, Status = DealStatus.Won, TotalAmount = 32000000, CreatedAt = now.AddMonths(-4), UpdatedAt = now.AddMonths(-3) },
            new() { Title = "Bahorgi assortiment shartnomasi", CustomerId = customers[1].Id, AssignedToId = sales1.Id, Status = DealStatus.Won, TotalAmount = 28000000, CreatedAt = now.AddMonths(-3), UpdatedAt = now.AddMonths(-2) },
            new() { Title = "Bolalar kiyimi partiyasi", CustomerId = customers[2].Id, AssignedToId = sales1.Id, Status = DealStatus.Negotiation, TotalAmount = 15000000, ExpectedCloseDate = now.AddDays(15), CreatedAt = now.AddDays(-30) },
            new() { Title = "Jins shimlar ulgurji buyurtmasi", CustomerId = customers[4].Id, AssignedToId = sales2.Id, Status = DealStatus.Proposal, TotalAmount = 22000000, ExpectedCloseDate = now.AddDays(20), CreatedAt = now.AddDays(-20) },
            new() { Title = "Yangi assortiment taklifi", CustomerId = customers[3].Id, AssignedToId = sales2.Id, Status = DealStatus.Qualified, TotalAmount = 18000000, ExpectedCloseDate = now.AddDays(30), CreatedAt = now.AddDays(-15) },
            new() { Title = "Yozgi kollekciya shartnomasi", CustomerId = customers[5].Id, AssignedToId = admin.Id, Status = DealStatus.Won, TotalAmount = 38000000, CreatedAt = now.AddMonths(-2), UpdatedAt = now.AddMonths(-1) },
            new() { Title = "Aksessuarlar buyurtmasi", CustomerId = customers[8].Id, AssignedToId = admin.Id, Status = DealStatus.Proposal, TotalAmount = 8000000, ExpectedCloseDate = now.AddDays(10), CreatedAt = now.AddDays(-10) },
            new() { Title = "To'liq assortiment uchun shartnoma", CustomerId = customers[5].Id, AssignedToId = admin.Id, Status = DealStatus.Lead, TotalAmount = 55000000, ExpectedCloseDate = now.AddDays(45), CreatedAt = now.AddDays(-5) },
            new() { Title = "Erkaklar kiyimi kollekciyasi", CustomerId = customers[1].Id, AssignedToId = sales1.Id, Status = DealStatus.Lost, TotalAmount = 12000000, CreatedAt = now.AddMonths(-5), UpdatedAt = now.AddMonths(-4) },
        };
        context.Deals.AddRange(deals);
        await context.SaveChangesAsync();

        // Activities
        var activities = new List<Activity>
        {
            new() { Type = ActivityType.Call, Subject = "Dastlabki tanishuv qo'ng'irog'i", CustomerId = customers[0].Id, UserId = admin.Id, ActivityDate = now.AddMonths(-8).AddDays(1), Notes = "Mijoz bilan birinchi marta aloqa o'rnatildi. Qiziqish bildirdi." },
            new() { Type = ActivityType.Meeting, Subject = "Ofis uchrashuvi - shartnoma muhokamasi", CustomerId = customers[0].Id, DealId = deals[0].Id, UserId = admin.Id, ActivityDate = now.AddMonths(-6).AddDays(2), Notes = "Narxlar va miqdorlar bo'yicha kelishildi." },
            new() { Type = ActivityType.Email, Subject = "Taklif xati yuborildi", CustomerId = customers[1].Id, DealId = deals[2].Id, UserId = sales1.Id, ActivityDate = now.AddMonths(-3).AddDays(-5), Notes = "Bahorgi assortiment taklifi PDF ko'rinishida yuborildi." },
            new() { Type = ActivityType.Call, Subject = "Narx muzokarasi", CustomerId = customers[2].Id, DealId = deals[3].Id, UserId = sales1.Id, ActivityDate = now.AddDays(-25), Notes = "10% chegirma so'randi, 5% kelishildi." },
            new() { Type = ActivityType.Meeting, Subject = "Namuna ko'rish uchrashuvi", CustomerId = customers[3].Id, UserId = sales2.Id, ActivityDate = now.AddDays(-14), Notes = "Yangi kollekciya namunalari ko'rsatildi." },
            new() { Type = ActivityType.Task, Subject = "Shartnoma hujjatlarini tayyorlash", CustomerId = customers[4].Id, DealId = deals[4].Id, UserId = sales2.Id, ActivityDate = now.AddDays(-18) },
            new() { Type = ActivityType.Call, Subject = "Yetkazib berish muddatini tasdiqlash", CustomerId = customers[5].Id, DealId = deals[6].Id, UserId = admin.Id, ActivityDate = now.AddMonths(-1).AddDays(5), Notes = "2 hafta ichida yetkazib beriladi." },
            new() { Type = ActivityType.Email, Subject = "Yangi narxlar ro'yxati yuborildi", CustomerId = customers[7].Id, UserId = sales2.Id, ActivityDate = now.AddDays(-8) },
            new() { Type = ActivityType.Note, Subject = "Mijoz qo'shimcha talablari", CustomerId = customers[8].Id, UserId = admin.Id, ActivityDate = now.AddDays(-7), Notes = "Maxsus rang sxemasi talab qilindi. Ishlab chiqaruvchi bilan ko'rib chiqish kerak." },
            new() { Type = ActivityType.Meeting, Subject = "Yangi mavsum taqdimoti", CustomerId = customers[9].Id, UserId = sales1.Id, ActivityDate = now.AddDays(-3), Notes = "Yangi katalog taqdim etildi, ijobiy javob olinadi deb kutilmoqda." },
            new() { Type = ActivityType.Call, Subject = "Follow-up qo'ng'iroq", CustomerId = customers[3].Id, DealId = deals[5].Id, UserId = sales2.Id, ActivityDate = now.AddDays(-1), Notes = "Qarorni keyingi haftaga qoldirdi." },
            new() { Type = ActivityType.Email, Subject = "Oylik hisobot yuborildi", CustomerId = customers[0].Id, UserId = admin.Id, ActivityDate = now.AddDays(-2), Notes = "Oxirgi oy statistikasi va keyingi buyurtma prognozi yuborildi." },
        };
        context.Activities.AddRange(activities);
        await context.SaveChangesAsync();
    }
}
