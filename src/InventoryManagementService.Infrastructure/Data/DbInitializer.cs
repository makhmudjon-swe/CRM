using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementService.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure roles exist
        string[] roles = ["Admin", "Creator", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // --- Users (50) ---
        var userIds = new List<string>();

        // 3 default accounts
        var adminId = await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName = "admin@inventory.com",
            Email = "admin@inventory.com",
            DisplayName = "Default Admin",
            EmailConfirmed = true,
            CanManageInventories = true,
            CreatedAt = DateTime.UtcNow
        }, "admin123", "Admin");
        userIds.Add(adminId);

        var creatorId = await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName = "creator@inventory.com",
            Email = "creator@inventory.com",
            DisplayName = "Default Creator",
            EmailConfirmed = true,
            CanManageInventories = true,
            CreatedAt = DateTime.UtcNow
        }, "creator123", "Creator");
        userIds.Add(creatorId);

        var regularUserId = await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName = "user@inventory.com",
            Email = "user@inventory.com",
            DisplayName = "Default User",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "user123", "User");
        userIds.Add(regularUserId);

        // 47 more users
        var firstNames = new[] { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack",
            "Karen", "Leo", "Mia", "Nathan", "Olivia", "Paul", "Quinn", "Rachel", "Sam", "Tina",
            "Ulysses", "Vera", "Walter", "Xena", "Yuri", "Zara", "Amber", "Blake", "Chloe", "Derek",
            "Elena", "Felix", "Gina", "Hugo", "Irene", "Jake", "Kira", "Liam", "Monica", "Nick",
            "Opal", "Peter", "Rosa", "Steve", "Tara", "Uma", "Victor" };

        for (int i = 0; i < 47; i++)
        {
            var name = firstNames[i];
            var email = $"{name.ToLower()}@inventory.com";
            var id = await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = name + " " + (i % 10 == 0 ? "Smith" : i % 5 == 0 ? "Johnson" : i % 3 == 0 ? "Williams" : i % 2 == 0 ? "Brown" : "Davis"),
                EmailConfirmed = true,
                CanManageInventories = i < 10,
                CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365)),
                PreferredTheme = i % 3 == 0 ? "dark" : "light",
                PreferredLanguage = i % 5 == 0 ? "uz" : "en"
            }, "password123", i < 5 ? "Creator" : "User");
            userIds.Add(id);
        }

        // If inventories already seeded, skip the rest
        if (await context.Inventories.AnyAsync())
            return;

        // --- Categories ---
        var categoryNames = new Dictionary<string, string>
        {
            ["Electronics"] = "Electronic devices and components",
            ["Books"] = "Books and publications",
            ["Tools"] = "Tools and equipment",
            ["Collectibles"] = "Collectible items",
            ["Office Supplies"] = "Office and stationery supplies",
            ["Furniture"] = "Furniture and home items",
            ["Vehicles"] = "Vehicles and parts",
            ["Other"] = "Miscellaneous items",
            ["Clothing"] = "Clothing and apparel",
            ["Sports"] = "Sports and outdoor equipment",
            ["Art"] = "Art and craft supplies",
            ["Music"] = "Musical instruments and accessories"
        };
        var existingCategoryNames = await context.Categories.Select(c => c.Name).ToListAsync();
        var newCategories = categoryNames
            .Where(kv => !existingCategoryNames.Contains(kv.Key))
            .Select(kv => new Category { Name = kv.Key, Description = kv.Value })
            .ToList();
        if (newCategories.Any())
        {
            context.Categories.AddRange(newCategories);
            await context.SaveChangesAsync();
        }

        var allCategories = await context.Categories.ToListAsync();
        var catCount = allCategories.Count;

        // --- Tags (50) ---
        if (!await context.Tags.AnyAsync())
        {
            var tagNames = new[]
            {
                "vintage", "rare", "new", "used", "refurbished", "limited-edition", "handmade", "imported",
                "organic", "premium", "budget", "professional", "beginner", "advanced", "portable",
                "wireless", "bluetooth", "usb-c", "waterproof", "durable", "lightweight", "heavy-duty",
                "compact", "full-size", "mini", "xl", "eco-friendly", "recyclable", "antique", "modern",
                "classic", "retro", "custom", "standard", "deluxe", "basic", "pro", "enterprise",
                "home-use", "office-use", "outdoor", "indoor", "seasonal", "everyday", "travel",
                "educational", "decorative", "functional", "ergonomic", "smart"
            };
            var tags = tagNames.Select(n => new Tag { Name = n }).ToArray();
            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
        }

        var allTags = await context.Tags.ToListAsync();

        // --- Inventories (50) ---
        if (!await context.Inventories.AnyAsync())
        {
            var inventoryData = new (string Name, string Desc, int CatIdx, bool IsPublic, bool EnableStr, string? StrName, bool EnableInt, string? IntName, bool EnableBool, string? BoolName)[]
            {
                ("Laptop Collection", "A curated **collection** of laptops from various brands and eras.", 0, true, true, "Brand", true, "Year", true, "Still Working"),
                ("Office Chairs", "Ergonomic and standard office chairs for the workplace.", 5, true, true, "Material", true, "Weight (kg)", false, null),
                ("Science Fiction Library", "All the sci-fi books you could ever want. _Includes first editions._", 1, true, true, "Author", true, "Pages", true, "First Edition"),
                ("Power Tools Inventory", "Workshop power tools - drills, saws, grinders and more.", 2, false, true, "Manufacturer", true, "Wattage", true, "Cordless"),
                ("Stamp Collection", "Rare and valuable postage stamps from around the world.", 3, true, true, "Country", true, "Year Issued", true, "Mint Condition"),
                ("Stationery Supplies", "Pens, paper, notebooks and everything for the office.", 4, true, true, "Brand", true, "Quantity", false, null),
                ("Vintage Cameras", "Film cameras from the golden age of photography.", 3, false, true, "Brand", true, "Year", true, "Functional"),
                ("Gaming Consoles", "Retro and modern gaming consoles.", 0, true, true, "Manufacturer", true, "Release Year", true, "Complete in Box"),
                ("Standing Desks", "Adjustable and fixed standing desks.", 5, false, true, "Material", true, "Height (cm)", true, "Electric"),
                ("Programming Books", "Software development and CS textbooks.", 1, true, true, "Author", true, "Edition", true, "Has Exercises"),
                ("Wrenches & Spanners", "Complete set of wrenches for automotive and home use.", 2, false, true, "Size (mm)", true, "Set Count", true, "Metric"),
                ("Coin Collection", "Coins from different countries and historical periods.", 3, true, true, "Country", true, "Year", true, "Proof Grade"),
                ("Printer Supplies", "Toner, ink cartridges, and paper stock.", 4, true, true, "Compatible Model", true, "Page Yield", false, null),
                ("Dining Tables", "Wooden, glass, and composite dining tables.", 5, false, true, "Material", true, "Seats", true, "Extendable"),
                ("Motorcycle Parts", "OEM and aftermarket motorcycle components.", 6, false, true, "Bike Model", true, "Part Number", true, "OEM"),
                ("Board Games", "Strategy, party, and family board games.", 7, true, true, "Publisher", true, "Players Max", true, "Expansion"),
                ("Smartphones Archive", "A history of smartphones from 2007 to present.", 0, true, true, "Brand", true, "Release Year", true, "Working"),
                ("Poetry Collection", "Poetry anthologies and single-author collections.", 1, false, true, "Poet", true, "Pages", true, "Signed"),
                ("Garden Tools", "Hand tools and powered equipment for gardening.", 2, true, true, "Type", true, "Weight (g)", true, "Electric"),
                ("Baseball Cards", "Trading cards from MLB players past and present.", 3, true, true, "Player", true, "Year", true, "Graded"),
                ("Envelopes & Mailers", "Various sizes of envelopes and packaging.", 4, false, true, "Size", true, "Pack Quantity", false, null),
                ("Bookshelves", "Freestanding and wall-mounted bookshelves.", 5, true, true, "Material", true, "Shelves", true, "Wall Mounted"),
                ("Car Tires", "Summer, winter, and all-season tires.", 6, false, true, "Size Code", true, "Tread Depth (mm)", true, "Run-Flat"),
                ("Vinyl Records", "LP and EP records across all genres.", 7, true, true, "Artist", true, "Year", true, "Original Press"),
                ("Server Rack Equipment", "Rack-mount servers, switches, and UPS units.", 0, false, true, "Manufacturer", true, "Rack Units", true, "Redundant PSU"),
                ("Cookbooks", "Recipes from around the world.", 1, true, true, "Chef/Author", true, "Recipes Count", true, "Vegetarian Friendly"),
                ("Measuring Instruments", "Calipers, micrometers, and gauges.", 2, false, true, "Brand", true, "Accuracy (mm)", true, "Digital"),
                ("Vintage Posters", "Movie, travel, and propaganda posters.", 3, true, true, "Era", true, "Year", true, "Framed"),
                ("Sticky Notes & Labels", "Adhesive notes and label rolls.", 4, true, true, "Color", true, "Pack Size", false, null),
                ("Office Desks", "Executive and student desks.", 5, false, true, "Material", true, "Width (cm)", true, "With Drawers"),
                ("Bicycle Parts", "Frames, wheels, gears and accessories.", 6, true, true, "Compatible Bike", true, "Weight (g)", true, "Carbon Fiber"),
                ("Action Figures", "Superhero and anime action figures.", 3, true, true, "Franchise", true, "Height (cm)", true, "In Package"),
                ("Audio Equipment", "Headphones, speakers, and amplifiers.", 0, true, true, "Brand", true, "Impedance (ohm)", true, "Wireless"),
                ("Children's Books", "Picture books and early readers.", 1, false, true, "Author", true, "Age Range", true, "Hardcover"),
                ("Plumbing Supplies", "Pipes, fittings, and valves.", 2, false, true, "Material", true, "Diameter (mm)", true, "Lead-Free"),
                ("Movie Memorabilia", "Props and signed items from films.", 3, false, true, "Movie", true, "Year", true, "Authenticated"),
                ("Ink & Toner", "Printer consumables warehouse stock.", 4, true, true, "Printer Model", true, "Page Yield", false, null),
                ("Outdoor Benches", "Park and garden seating.", 5, true, true, "Material", true, "Length (cm)", true, "Weather Resistant"),
                ("Truck Accessories", "Bed liners, covers, and racks.", 6, false, true, "Truck Model", true, "Weight (kg)", true, "Universal Fit"),
                ("Puzzle Collection", "Jigsaw puzzles from 100 to 5000 pieces.", 7, true, true, "Brand", true, "Piece Count", true, "Complete"),
                ("Raspberry Pi Projects", "SBCs and components for maker projects.", 0, true, true, "Model", true, "RAM (MB)", true, "With Case"),
                ("History Books", "World history, wars, and civilizations.", 1, true, true, "Author", true, "Year Published", true, "Illustrated"),
                ("Welding Equipment", "MIG, TIG, and stick welding gear.", 2, false, true, "Type", true, "Amperage", true, "Portable"),
                ("Comic Books", "Marvel, DC, and indie comics.", 3, true, true, "Publisher", true, "Issue Number", true, "Graded"),
                ("Binders & Folders", "Ring binders and file folders.", 4, false, true, "Size", true, "Ring Count", false, null),
                ("Bean Bags & Loungers", "Casual seating for home and office.", 5, true, true, "Fill Material", true, "Diameter (cm)", true, "Washable Cover"),
                ("Boat Accessories", "Marine parts and boating gear.", 6, false, true, "Boat Type", true, "Weight (kg)", true, "Saltwater Safe"),
                ("T-Shirts", "Cotton and blend t-shirts in various sizes.", 8, true, true, "Size", true, "Stock Qty", true, "Organic Cotton"),
                ("Running Shoes", "Athletic footwear for runners.", 9, true, true, "Brand", true, "Size (EU)", true, "Waterproof"),
                ("Painting Supplies", "Canvases, paints, and brushes for artists.", 10, true, true, "Medium", true, "Quantity", true, "Professional Grade"),
            };

            var inventories = new List<Inventory>();
            var rng = new Random(42); // deterministic seed

            for (int i = 0; i < inventoryData.Length; i++)
            {
                var d = inventoryData[i];
                var ownerIdx = i % userIds.Count;
                inventories.Add(new Inventory
                {
                    Name = d.Name,
                    Description = d.Desc,
                    CategoryId = allCategories[d.CatIdx % catCount].Id,
                    OwnerId = userIds[ownerIdx],
                    IsPublic = d.IsPublic,
                    CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(10, 300)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-rng.Next(0, 10)),
                    CustomString1Enabled = d.EnableStr,
                    CustomString1Name = d.StrName,
                    CustomInt1Enabled = d.EnableInt,
                    CustomInt1Name = d.IntName,
                    CustomBool1Enabled = d.EnableBool,
                    CustomBool1Name = d.BoolName,
                });
            }

            context.Inventories.AddRange(inventories);
            await context.SaveChangesAsync();

            // --- InventoryTags (assign 2-4 tags per inventory) ---
            var inventoryTags = new List<InventoryTag>();
            foreach (var inv in inventories)
            {
                var tagCount = rng.Next(2, 5);
                var selectedTags = allTags.OrderBy(_ => rng.Next()).Take(tagCount).ToList();
                foreach (var tag in selectedTags)
                {
                    inventoryTags.Add(new InventoryTag { InventoryId = inv.Id, TagId = tag.Id });
                }
            }
            context.Set<InventoryTag>().AddRange(inventoryTags);
            await context.SaveChangesAsync();

            // --- Items (3-5 per inventory, contextually matched) ---
            (string Name, string? Str1, int? Int1, bool? Bool1)[] I(params (string, string?, int?, bool?)[] items) => items;
            var itemsPerInventory = new Dictionary<int, (string Name, string? Str1, int? Int1, bool? Bool1)[]>
            {
                // 0: Laptop Collection (Brand, Year, Still Working)
                [0] = I(("ThinkPad X1 Carbon Gen 11", "Lenovo", 2023, true),
                    ("MacBook Air M2", "Apple", 2022, true),
                    ("Dell XPS 15 9530", "Dell", 2023, true),
                    ("HP Spectre x360 14", "HP", 2022, true),
                    ("Asus ROG Zephyrus G14", "Asus", 2021, false)),
                // 1: Office Chairs (Material, Weight, -)
                [1] = I(("Herman Miller Aeron Remastered", "Mesh", 12, null),
                    ("Steelcase Leap V2", "Fabric", 18, null),
                    ("Secretlab Titan Evo 2022", "Leather", 32, null),
                    ("IKEA Markus", "Mesh", 20, null),
                    ("Autonomous ErgoChair Pro+", "Mesh", 14, null)),
                // 2: Science Fiction Library (Author, Pages, First Edition)
                [2] = I(("Dune", "Frank Herbert", 412, true),
                    ("Neuromancer", "William Gibson", 271, false),
                    ("Foundation", "Isaac Asimov", 244, true),
                    ("Snow Crash", "Neal Stephenson", 440, false),
                    ("The Left Hand of Darkness", "Ursula K. Le Guin", 286, true)),
                // 3: Power Tools Inventory (Manufacturer, Wattage, Cordless)
                [3] = I(("DCD771C2 Compact Drill/Driver", "DeWalt", 300, true),
                    ("XRJ05Z Reciprocating Saw", "Makita", 750, true),
                    ("GWS13-50 Angle Grinder", "Bosch", 1300, false),
                    ("M18 FUEL Impact Driver", "Milwaukee", 400, true),
                    ("P261 Impact Wrench", "Ryobi", 350, true)),
                // 4: Stamp Collection (Country, Year Issued, Mint Condition)
                [4] = I(("Penny Black", "United Kingdom", 1840, true),
                    ("Inverted Jenny", "United States", 1918, false),
                    ("British Guiana 1c Magenta", "British Guiana", 1856, true),
                    ("Treskilling Yellow", "Sweden", 1855, true),
                    ("Basel Dove", "Switzerland", 1845, false)),
                // 5: Stationery Supplies (Brand, Quantity, -)
                [5] = I(("Pilot G2 0.7mm Blue (12-pack)", "Pilot", 12, null),
                    ("Moleskine Classic Ruled Notebook", "Moleskine", 1, null),
                    ("Staedtler Triplus Fineliner Set", "Staedtler", 20, null),
                    ("Leuchtturm1917 A5 Dotted", "Leuchtturm", 1, null)),
                // 6: Vintage Cameras (Brand, Year, Functional)
                [6] = I(("Leica M3 Double Stroke", "Leica", 1954, true),
                    ("Nikon F Photomic", "Nikon", 1959, true),
                    ("Canon AE-1 Program", "Canon", 1981, true),
                    ("Pentax K1000", "Pentax", 1976, false),
                    ("Olympus OM-1", "Olympus", 1972, true)),
                // 7: Gaming Consoles (Manufacturer, Release Year, Complete in Box)
                [7] = I(("Nintendo Switch OLED", "Nintendo", 2021, true),
                    ("PlayStation 5 Digital", "Sony", 2020, true),
                    ("Xbox Series X", "Microsoft", 2020, false),
                    ("Steam Deck 512GB", "Valve", 2022, true),
                    ("Sega Genesis Model 1", "Sega", 1988, true)),
                // 8: Standing Desks (Material, Height cm, Electric)
                [8] = I(("UPLIFT V2 Commercial", "Bamboo", 127, true),
                    ("FlexiSpot E7 Pro", "MDF", 130, true),
                    ("Vari Electric", "Laminate", 125, true),
                    ("Branch Standing Desk", "Birch", 122, true)),
                // 9: Programming Books (Author, Edition, Has Exercises)
                [9] = I(("Clean Code", "Robert C. Martin", 1, true),
                    ("SICP", "H. Abelson & G. Sussman", 2, true),
                    ("Design Patterns", "Gang of Four", 1, false),
                    ("The Pragmatic Programmer", "D. Thomas & A. Hunt", 2, true),
                    ("Introduction to Algorithms (CLRS)", "Cormen, Leiserson, Rivest, Stein", 4, true)),
                // 10: Wrenches & Spanners (Size mm, Set Count, Metric)
                [10] = I(("Combination Wrench Set 8-19mm", "8-19", 12, true),
                    ("Adjustable Wrench 250mm", "250", 1, true),
                    ("Torque Wrench 1/2 Drive", "12.7", 1, true),
                    ("Ratcheting Spanner Set SAE", "1/4-7/8", 8, false)),
                // 11: Coin Collection (Country, Year, Proof Grade)
                [11] = I(("1909-S VDB Lincoln Cent", "United States", 1909, false),
                    ("1933 Double Eagle", "United States", 1933, true),
                    ("1943 Copper Penny", "United States", 1943, false),
                    ("Krugerrand 1oz Gold", "South Africa", 1967, true),
                    ("Maple Leaf 1oz Silver", "Canada", 2020, true)),
                // 12: Printer Supplies (Compatible Model, Page Yield, -)
                [12] = I(("HP 61XL Black Ink Cartridge", "HP DeskJet 2540", 480, null),
                    ("Canon PG-245XL Black", "Canon PIXMA MG2520", 300, null),
                    ("Brother TN-760 Toner", "Brother HL-L2350DW", 3000, null),
                    ("Epson 502 Ink Bottle", "Epson EcoTank ET-2720", 7500, null)),
                // 13: Dining Tables (Material, Seats, Extendable)
                [13] = I(("Scandinavian Oak Dining Table", "Solid Oak", 6, true),
                    ("Tempered Glass Round Table", "Glass", 4, false),
                    ("Industrial Walnut Farmhouse", "Walnut Wood", 8, true),
                    ("Marble Top Dining Table", "Marble", 6, false)),
                // 14: Motorcycle Parts (Bike Model, Part Number, OEM)
                [14] = I(("Front Brake Rotor 300mm", "Yamaha YZF-R6", 25810, true),
                    ("Chain & Sprocket Kit", "Honda CBR600RR", 40530, true),
                    ("LED Headlight Assembly", "Kawasaki Z900", 23004, false),
                    ("Aftermarket Exhaust Slip-On", "Ducati Monster 821", 96480, false)),
                // 15: Board Games (Publisher, Players Max, Expansion)
                [15] = I(("Catan", "Kosmos", 4, false),
                    ("Ticket to Ride", "Days of Wonder", 5, false),
                    ("Pandemic", "Z-Man Games", 4, true),
                    ("Wingspan", "Stonemaier Games", 5, true),
                    ("Terraforming Mars", "FryxGames", 5, true)),
                // 16: Smartphones Archive (Brand, Release Year, Working)
                [16] = I(("iPhone 15 Pro Max", "Apple", 2023, true),
                    ("Galaxy S24 Ultra", "Samsung", 2024, true),
                    ("Pixel 8 Pro", "Google", 2023, true),
                    ("Original iPhone 2G", "Apple", 2007, false),
                    ("Nokia 3310 (original)", "Nokia", 2000, true)),
                // 17: Poetry Collection (Poet, Pages, Signed)
                [17] = I(("Leaves of Grass", "Walt Whitman", 400, false),
                    ("The Waste Land & Other Poems", "T.S. Eliot", 96, true),
                    ("Ariel", "Sylvia Plath", 85, false),
                    ("Milk and Honey", "Rupi Kaur", 208, true)),
                // 18: Garden Tools (Type, Weight g, Electric)
                [18] = I(("Fiskars Bypass Pruner", "Pruner", 210, false),
                    ("Stihl HSA 56 Hedge Trimmer", "Hedge Trimmer", 3100, true),
                    ("Gardena Comfort Spade", "Spade", 1800, false),
                    ("Black+Decker Leaf Blower", "Leaf Blower", 2700, true),
                    ("Haws Watering Can 8.8L", "Watering Can", 900, false)),
                // 19: Baseball Cards (Player, Year, Graded)
                [19] = I(("Honus Wagner T206", "Honus Wagner", 1909, true),
                    ("Mickey Mantle 1952 Topps", "Mickey Mantle", 1952, true),
                    ("Mike Trout 2011 Topps Update", "Mike Trout", 2011, true),
                    ("Babe Ruth 1933 Goudey #53", "Babe Ruth", 1933, false)),
                // 20: Envelopes & Mailers (Size, Pack Quantity, -)
                [20] = I(("#10 Business Envelopes", "#10 (4.125x9.5)", 500, null),
                    ("6x9 Kraft Mailers", "6x9", 100, null),
                    ("Padded Bubble Mailers 8.5x11", "8.5x11", 50, null),
                    ("A7 Invitation Envelopes", "A7 (5.25x7.25)", 250, null)),
                // 21: Bookshelves (Material, Shelves, Wall Mounted)
                [21] = I(("IKEA Billy Bookcase", "Particle Board", 5, false),
                    ("Floating Wall Shelf Set (3)", "Solid Pine", 3, true),
                    ("Industrial Pipe Bookshelf", "Iron & Wood", 5, true),
                    ("Corner Ladder Shelf", "Bamboo", 4, false)),
                // 22: Car Tires (Size Code, Tread Depth mm, Run-Flat)
                [22] = I(("Michelin Pilot Sport 4S", "255/35ZR19", 8, false),
                    ("Continental WinterContact TS 860", "205/55R16", 8, false),
                    ("Bridgestone Potenza RE980AS+", "225/45R17", 7, false),
                    ("Pirelli P Zero Run-Flat", "245/40R18", 8, true)),
                // 23: Vinyl Records (Artist, Year, Original Press)
                [23] = I(("Dark Side of the Moon", "Pink Floyd", 1973, true),
                    ("Rumours", "Fleetwood Mac", 1977, true),
                    ("Abbey Road", "The Beatles", 1969, true),
                    ("Kind of Blue", "Miles Davis", 1959, false),
                    ("Thriller", "Michael Jackson", 1982, true)),
                // 24: Server Rack Equipment (Manufacturer, Rack Units, Redundant PSU)
                [24] = I(("PowerEdge R750 Server", "Dell", 2, true),
                    ("Catalyst 9300 Switch", "Cisco", 1, false),
                    ("Smart-UPS 3000VA", "APC", 2, true),
                    ("ProLiant DL380 Gen10+", "HPE", 2, true)),
                // 25: Cookbooks (Chef/Author, Recipes Count, Vegetarian Friendly)
                [25] = I(("Mastering the Art of French Cooking", "Julia Child", 524, false),
                    ("Salt, Fat, Acid, Heat", "Samin Nosrat", 120, true),
                    ("The Food Lab", "J. Kenji Lopez-Alt", 300, true),
                    ("Ottolenghi Simple", "Yotam Ottolenghi", 130, true)),
                // 26: Measuring Instruments (Brand, Accuracy mm, Digital)
                [26] = I(("Digital Caliper 150mm", "Mitutoyo", 1, true),
                    ("Outside Micrometer 0-25mm", "Starrett", 1, false),
                    ("Dial Indicator 0.01mm", "Mitutoyo", 1, false),
                    ("Laser Distance Meter 50m", "Bosch", 2, true)),
                // 27: Vintage Posters (Era, Year, Framed)
                [27] = I(("Casablanca Movie Poster", "1940s", 1942, true),
                    ("Visit Paris TWA Travel Poster", "1950s", 1955, true),
                    ("Rosie the Riveter", "1940s", 1943, false),
                    ("Woodstock Festival Poster", "1960s", 1969, true)),
                // 28: Sticky Notes & Labels (Color, Pack Size, -)
                [28] = I(("Post-it Super Sticky 3x3", "Yellow", 12, null),
                    ("Avery Address Labels 1x2.625", "White", 750, null),
                    ("Post-it Flags 1 inch", "Assorted", 200, null),
                    ("Dymo LabelWriter Labels", "White", 350, null)),
                // 29: Office Desks (Material, Width cm, With Drawers)
                [29] = I(("Executive L-Shaped Desk", "Walnut Veneer", 180, true),
                    ("Student Writing Desk", "White Laminate", 100, true),
                    ("Corner Computer Desk", "MDF", 140, true),
                    ("Minimalist Floating Desk", "Solid Oak", 120, false)),
                // 30: Bicycle Parts (Compatible Bike, Weight g, Carbon Fiber)
                [30] = I(("Shimano 105 Groupset R7000", "Road Bike", 2430, false),
                    ("Continental GP5000 Tire 700x25c", "Road Bike", 200, false),
                    ("ENVE SES 3.4 Wheelset", "Road Bike", 1380, true),
                    ("Fizik Arione R3 Saddle", "Road/Gravel", 205, true)),
                // 31: Action Figures (Franchise, Height cm, In Package)
                [31] = I(("Spider-Man No Way Home Figure", "Marvel", 30, true),
                    ("Goku Ultra Instinct SH Figuarts", "Dragon Ball", 15, true),
                    ("Batman Arkham Knight Statue", "DC", 45, false),
                    ("Optimus Prime Masterpiece MP-44", "Transformers", 28, true),
                    ("Link Breath of the Wild Nendoroid", "Zelda", 10, true)),
                // 32: Audio Equipment (Brand, Impedance ohm, Wireless)
                [32] = I(("Sennheiser HD 600", "Sennheiser", 300, false),
                    ("Sony WH-1000XM5", "Sony", 48, true),
                    ("KEF LS50 Meta Speakers", "KEF", 8, false),
                    ("Apple AirPods Pro 2", "Apple", 32, true),
                    ("Schiit Magni Heresy Amp", "Schiit", 16, false)),
                // 33: Children's Books (Author, Age Range, Hardcover)
                [33] = I(("The Very Hungry Caterpillar", "Eric Carle", 3, true),
                    ("Where the Wild Things Are", "Maurice Sendak", 4, true),
                    ("Goodnight Moon", "Margaret Wise Brown", 2, true),
                    ("Diary of a Wimpy Kid", "Jeff Kinney", 8, true)),
                // 34: Plumbing Supplies (Material, Diameter mm, Lead-Free)
                [34] = I(("1/2 inch Copper Pipe (10ft)", "Copper", 15, true),
                    ("PVC 90-Degree Elbow", "PVC", 50, true),
                    ("Brass Ball Valve 3/4 inch", "Brass", 20, true),
                    ("Stainless Steel Flex Hose 24in", "Stainless Steel", 12, true)),
                // 35: Movie Memorabilia (Movie, Year, Authenticated)
                [35] = I(("Lightsaber Prop Replica", "Star Wars", 1977, true),
                    ("One Ring Gold Replica", "Lord of the Rings", 2001, true),
                    ("Signed Poster by Harrison Ford", "Indiana Jones", 1981, true),
                    ("DeLorean Model 1:18 Scale", "Back to the Future", 1985, false)),
                // 36: Ink & Toner (Printer Model, Page Yield, -)
                [36] = I(("HP 206X High Yield Black Toner", "HP LaserJet Pro M283", 3150, null),
                    ("Canon 055H Magenta Toner", "Canon MF743Cdw", 5900, null),
                    ("Epson 522 Cyan Ink Bottle", "Epson EcoTank ET-4760", 7500, null),
                    ("Brother TN-227BK Black", "Brother MFC-L3770CDW", 3000, null)),
                // 37: Outdoor Benches (Material, Length cm, Weather Resistant)
                [37] = I(("Classic Park Bench", "Cast Iron & Wood", 150, true),
                    ("Modern Garden Bench", "Recycled Plastic", 120, true),
                    ("Teak Patio Bench", "Teak", 180, true),
                    ("Wrought Iron Garden Bench", "Wrought Iron", 130, true)),
                // 38: Truck Accessories (Truck Model, Weight kg, Universal Fit)
                [38] = I(("Tri-Fold Tonneau Cover", "Ford F-150", 18, false),
                    ("LED Light Bar 52 inch", "Universal", 5, true),
                    ("Drop-In Bed Liner", "RAM 1500", 25, false),
                    ("Bull Bar Grille Guard", "Toyota Tacoma", 22, false)),
                // 39: Puzzle Collection (Brand, Piece Count, Complete)
                [39] = I(("Starry Night 1000pc", "Ravensburger", 1000, true),
                    ("World Map 2000pc", "Educa", 2000, true),
                    ("Harry Potter Hogwarts 3000pc", "Aquarius", 3000, false),
                    ("Great Wave 500pc", "Galison", 500, true),
                    ("New York Skyline 1500pc", "Clementoni", 1500, true)),
                // 40: Raspberry Pi Projects (Model, RAM MB, With Case)
                [40] = I(("Raspberry Pi 5 8GB Kit", "Pi 5", 8192, true),
                    ("Raspberry Pi 4B 4GB", "Pi 4B", 4096, true),
                    ("Raspberry Pi Zero 2 W", "Pi Zero 2 W", 512, false),
                    ("Raspberry Pi Pico W", "Pico W", 264, false)),
                // 41: History Books (Author, Year Published, Illustrated)
                [41] = I(("Sapiens: A Brief History", "Yuval Noah Harari", 2011, false),
                    ("Guns, Germs, and Steel", "Jared Diamond", 1997, false),
                    ("The Silk Roads", "Peter Frankopan", 2015, true),
                    ("A People's History of the US", "Howard Zinn", 1980, false)),
                // 42: Welding Equipment (Type, Amperage, Portable)
                [42] = I(("Lincoln MIG 180 Welder", "MIG", 180, false),
                    ("Miller Dynasty 210 TIG", "TIG", 210, false),
                    ("ESAB Rogue ES 180i Stick", "Stick", 180, true),
                    ("Hobart Handler 140 MIG", "MIG", 140, true)),
                // 43: Comic Books (Publisher, Issue Number, Graded)
                [43] = I(("Amazing Spider-Man #300", "Marvel", 300, true),
                    ("Batman: The Killing Joke", "DC", 1, true),
                    ("Saga #1", "Image", 1, true),
                    ("X-Men #1 (1991)", "Marvel", 1, false),
                    ("Watchmen #1", "DC", 1, true)),
                // 44: Binders & Folders (Size, Ring Count, -)
                [44] = I(("Avery Heavy Duty 3-Ring Binder", "Letter", 3, null),
                    ("Smead Manila File Folders (100pk)", "Letter", 0, null),
                    ("Five Star Zipper Binder", "Letter", 3, null),
                    ("Pendaflex Hanging Folders (25pk)", "Letter", 0, null)),
                // 45: Bean Bags & Loungers (Fill Material, Diameter cm, Washable Cover)
                [45] = I(("Fatboy Original Bean Bag", "EPS Beads", 140, true),
                    ("Big Joe Milano Bean Chair", "UltimaX Foam", 81, true),
                    ("Yogibo Max Lounger", "EPS Beads", 170, true),
                    ("Chill Sack 5ft Memory Foam", "Memory Foam", 150, true)),
                // 46: Boat Accessories (Boat Type, Weight kg, Saltwater Safe)
                [46] = I(("Lowrance HDS-9 Fish Finder", "All Boats", 2, true),
                    ("Minn Kota Riptide Trolling Motor", "Saltwater Boats", 28, true),
                    ("Marine Battery Switch", "All Boats", 1, true),
                    ("Fender Vinyl Boat Bumper (4pk)", "Pontoon", 3, true)),
                // 47: T-Shirts (Size, Stock Qty, Organic Cotton)
                [47] = I(("Classic Crew Neck Black", "M", 150, true),
                    ("V-Neck Heather Gray", "L", 200, false),
                    ("Graphic Print Vintage Logo", "S", 75, true),
                    ("Polo Collar Navy Blue", "XL", 120, false),
                    ("Oversized Streetwear White", "L", 90, true)),
                // 48: Running Shoes (Brand, Size EU, Waterproof)
                [48] = I(("Nike Air Zoom Pegasus 40", "Nike", 42, false),
                    ("Adidas Ultraboost 23", "Adidas", 43, false),
                    ("ASICS Gel-Kayano 30", "ASICS", 44, false),
                    ("Brooks Ghost 15", "Brooks", 42, false),
                    ("Salomon Speedcross 6 GTX", "Salomon", 43, true)),
                // 49: Painting Supplies (Medium, Quantity, Professional Grade)
                [49] = I(("Winsor & Newton Oil Paint Set", "Oil", 12, true),
                    ("Liquitex Acrylic Basics 24-Tube", "Acrylic", 24, false),
                    ("Arches Watercolor Paper 140lb", "Watercolor", 20, true),
                    ("Princeton Velvetouch Brush Set", "Mixed", 8, true)),
            };

            var items = new List<Item>();
            for (int invIdx = 0; invIdx < inventories.Count; invIdx++)
            {
                var inv = inventories[invIdx];
                if (!itemsPerInventory.TryGetValue(invIdx, out var invItems)) continue;
                foreach (var (name, str1, int1, bool1) in invItems)
                {
                    items.Add(new Item
                    {
                        Name = name,
                        InventoryId = inv.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 200)),
                        UpdatedAt = DateTime.UtcNow.AddDays(-rng.Next(0, 5)),
                        CustomString1Value = inv.CustomString1Enabled ? str1 : null,
                        CustomInt1Value = inv.CustomInt1Enabled ? int1 : null,
                        CustomBool1Value = inv.CustomBool1Enabled ? bool1 : null,
                    });
                }
            }
            context.Items.AddRange(items);
            await context.SaveChangesAsync();

            // --- Comments (50 — spread across items) ---
            var commentTexts = new[]
            {
                "Great item, love the condition!", "Where did you find this?", "I have a similar one!",
                "How much did you pay for it?", "Amazing collection!", "This is in perfect shape.",
                "Would you consider trading?", "I need one of these!", "Incredible find!",
                "The details on this are superb.", "My favorite in the collection.", "Jealous!",
                "Been looking for one like this for years.", "Excellent quality.", "Nice addition!",
                "What year is this from?", "Can you share more photos?", "This is museum quality.",
                "How do you store these?", "Very well preserved.", "Stunning piece!",
                "Do you have the original packaging?", "This brings back memories.", "Wow, just wow.",
                "I collect these too!", "Solid choice.", "Any scratches or defects?",
                "Top notch!", "This must be worth a fortune.", "Beautiful craftsmanship.",
                "Does it still work?", "Where can I get one?", "Such a rare find!",
                "Love the patina on this.", "Mint condition indeed.", "Outstanding item!",
                "My grandfather had one of these.", "The color is gorgeous.", "A true classic.",
                "How long have you had this?", "Remarkable specimen.", "I'm impressed!",
                "Would love to see it in person.", "Keep these coming!", "Fantastic addition!",
                "This made my day.", "Pure nostalgia!", "A gem in any collection.",
                "Five stars from me.", "You have excellent taste!",
            };

            var comments = new List<Comment>();
            for (int i = 0; i < 50; i++)
            {
                var itemIdx = i % items.Count;
                var authorIdx = (i * 3 + 7) % userIds.Count;
                comments.Add(new Comment
                {
                    Content = commentTexts[i],
                    ItemId = items[itemIdx].Id,
                    AuthorId = userIds[authorIdx],
                    CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(0, 60)).AddHours(-rng.Next(0, 24)),
                });
            }
            context.Comments.AddRange(comments);
            await context.SaveChangesAsync();

            // --- Likes (50 — spread across items, unique per user-item) ---
            var likes = new List<Like>();
            var likeSet = new HashSet<(int, string)>();
            int likeCount = 0;
            int attempt = 0;
            while (likeCount < 50 && attempt < 200)
            {
                var itemIdx = rng.Next(0, items.Count);
                var userIdx = rng.Next(0, userIds.Count);
                var key = (items[itemIdx].Id, userIds[userIdx]);
                if (likeSet.Add(key))
                {
                    likes.Add(new Like
                    {
                        ItemId = items[itemIdx].Id,
                        UserId = userIds[userIdx],
                        CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(0, 90)),
                    });
                    likeCount++;
                }
                attempt++;
            }
            context.Likes.AddRange(likes);
            await context.SaveChangesAsync();

            // --- InventoryAccess (50 — grant access to various users on private inventories) ---
            var accesses = new List<InventoryAccess>();
            var accessSet = new HashSet<(int, string)>();
            int accessCount = 0;
            attempt = 0;
            while (accessCount < 50 && attempt < 300)
            {
                var invIdx = rng.Next(0, inventories.Count);
                var userIdx = rng.Next(0, userIds.Count);
                var inv = inventories[invIdx];
                // Don't grant access to the owner
                if (userIds[userIdx] == inv.OwnerId) { attempt++; continue; }
                var key = (inv.Id, userIds[userIdx]);
                if (accessSet.Add(key))
                {
                    accesses.Add(new InventoryAccess
                    {
                        InventoryId = inv.Id,
                        UserId = userIds[userIdx],
                        AccessLevel = (AccessLevel)rng.Next(0, 3),
                        GrantedAt = DateTime.UtcNow.AddDays(-rng.Next(0, 100)),
                    });
                    accessCount++;
                }
                attempt++;
            }
            context.InventoryAccesses.AddRange(accesses);
            await context.SaveChangesAsync();

            // --- CustomIdFormats (50 — one per inventory) ---
            var formats = new List<CustomIdFormat>();
            for (int i = 0; i < inventories.Count; i++)
            {
                formats.Add(new CustomIdFormat
                {
                    InventoryId = inventories[i].Id,
                    CurrentCounter = rng.Next(1, 100),
                });
            }
            context.Set<CustomIdFormat>().AddRange(formats);
            await context.SaveChangesAsync();

            // --- CustomIdElements (multiple per format: prefix + separator + sequence/date) ---
            var elements = new List<CustomIdElement>();
            for (int i = 0; i < formats.Count; i++)
            {
                var prefix = inventories[i].Name.Length >= 3 ? inventories[i].Name[..3].ToUpper() : inventories[i].Name.ToUpper();
                // Element 0: Fixed text prefix
                elements.Add(new CustomIdElement
                {
                    CustomIdFormatId = formats[i].Id,
                    ElementType = CustomIdElementType.FixedText,
                    Value = prefix + "-",
                    SortOrder = 0,
                });
                // Element 1: Date
                elements.Add(new CustomIdElement
                {
                    CustomIdFormatId = formats[i].Id,
                    ElementType = CustomIdElementType.DateTime,
                    DateFormat = "yyyyMMdd",
                    SortOrder = 1,
                });
                // Element 2: Separator
                elements.Add(new CustomIdElement
                {
                    CustomIdFormatId = formats[i].Id,
                    ElementType = CustomIdElementType.FixedText,
                    Value = "-",
                    SortOrder = 2,
                });
                // Element 3: Sequence
                elements.Add(new CustomIdElement
                {
                    CustomIdFormatId = formats[i].Id,
                    ElementType = CustomIdElementType.Sequence,
                    PaddingLength = 5,
                    SortOrder = 3,
                });
            }
            context.Set<CustomIdElement>().AddRange(elements);
            await context.SaveChangesAsync();
        }
    }

    private static async Task<string> SeedUserAsync(UserManager<ApplicationUser> userManager,
        ApplicationUser user, string password, string role)
    {
        var existing = await userManager.FindByEmailAsync(user.Email!);
        if (existing != null)
            return existing.Id;

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
        return user.Id;
    }
}
