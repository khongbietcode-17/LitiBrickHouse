// --- Thêm các thư viện cần thiết ---
using LitiBrickHouse.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Thêm các Dịch vụ (Services) ---

// Kết nối CSDL (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm dịch vụ Lỗi Database (cho Development)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- SỬA LỖI: Chỉ dùng AddIdentity (để hỗ trợ Roles) ---
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>(); // Phải kết thúc bằng ;

// Thêm dịch vụ cho Controller & View
builder.Services.AddControllersWithViews();
// Thêm dịch vụ cho trang Đăng nhập/Đăng ký
builder.Services.AddRazorPages();

// Thêm dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- 2. Xây dựng Ứng dụng ---
var app = builder.Build();

// --- 3. Cấu hình "Đường ống" HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages(); // Dùng cho các trang Đăng nhập/Đăng ký

// --- 4. Gieo mầm Dữ liệu (SỬA LỖI LOGIC & ASYNC) ---
// Gọi hàm gieo mầm (được định nghĩa ở dưới)
await SeedDataAsync(app);

// --- 5. Chạy Ứng dụng ---
app.Run();


// (Trong file Program.cs)
// XÓA HẲN hàm SeedDataAsync CŨ VÀ THAY BẰNG HÀM NÀY:

// ===================================================================
// === HÀM GIEO MẦM (HELPER METHOD) ===
// ===================================================================
async Task SeedDataAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<LitiBrickHouse.Data.ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // --- SỬA LỖI Ở ĐÂY ---
        // Dùng MigrateAsync() (Chạy migration) thay vì EnsureCreated() (Bỏ qua migration)
        // Điều này đảm bảo cột "Size" sẽ được tạo.
        await context.Database.MigrateAsync();

        // === Gieo mầm ProductTypes ===
        if (!context.ProductTypes.Any())
        {
            context.ProductTypes.AddRange(
                new LitiBrickHouse.Models.ProductType { Name = "Mẫu 1 Lego", Description = "Một nhân vật lego tùy chỉnh", BasePrice = 239000 },
                new LitiBrickHouse.Models.ProductType { Name = "Mẫu 2 Lego", Description = "Một cặp nhân vật lego tùy chỉnh.", BasePrice = 259000 },
                new LitiBrickHouse.Models.ProductType { Name = "Mẫu Lego Thể Thao", Description = "Chọn nhân vật thể thao bạn yêu thích", BasePrice = 199000 }
            );
            await context.SaveChangesAsync();
        }

        // === Gieo mầm Categories & Options ===
        if (!context.OptionCategories.Any())
        {
            // 1. Gieo các Danh mục (Category)
            var categoryBg1 = new LitiBrickHouse.Models.OptionCategory { Name = "Background (1 Lego)" };
            var categoryBg2 = new LitiBrickHouse.Models.OptionCategory { Name = "Background (2 Lego)" };
            var categoryBgSport = new LitiBrickHouse.Models.OptionCategory { Name = "Background (Sport)" };
            var categoryPlayer = new LitiBrickHouse.Models.OptionCategory { Name = "Cầu thủ" };
            var categoryHair = new LitiBrickHouse.Models.OptionCategory { Name = "Tóc" };
            var categoryFace = new LitiBrickHouse.Models.OptionCategory { Name = "Mặt" };
            var categoryClothes = new LitiBrickHouse.Models.OptionCategory { Name = "Áo" };
            var categoryPants = new LitiBrickHouse.Models.OptionCategory { Name = "Quần" };
            var categoryAcc = new LitiBrickHouse.Models.OptionCategory { Name = "Phụ kiện" };

            context.OptionCategories.AddRange(
                categoryBg1, categoryBg2, categoryBgSport,
                categoryPlayer, categoryHair, categoryFace,
                categoryClothes, categoryPants, categoryAcc
            );

            // 2. Gieo một vài Tùy chọn (CustomOption) mẫu
            context.CustomOptions.AddRange(
                // Backgrounds (đã thêm Size)
                new LitiBrickHouse.Models.CustomOption { Name = "Nền Đơn sắc (Trắng)", Size = "21x15", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", Quantity = null, OptionCategory = categoryBg1 },
                new LitiBrickHouse.Models.CustomOption { Name = "Nền Cặp đôi (Trái tim)", Size = "21x15", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", Quantity = null, OptionCategory = categoryBg2 },
                new LitiBrickHouse.Models.CustomOption { Name = "Nền Sân cỏ", Size = "21x15", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", AdditionalPrice = 15000, Quantity = null, OptionCategory = categoryBgSport },
                // Cầu thủ
                new LitiBrickHouse.Models.CustomOption { Name = "Cầu thủ Messi", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", AdditionalPrice = 20000, Quantity = 50, OptionCategory = categoryPlayer },
                // Tóc
                new LitiBrickHouse.Models.CustomOption { Name = "Tóc Xoăn (Nam)", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", AdditionalPrice = 5000, Quantity = 100, OptionCategory = categoryHair, Gender = LitiBrickHouse.Enums.GenderType.Male },
                new LitiBrickHouse.Models.CustomOption { Name = "Tóc Dài (Nữ)", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", AdditionalPrice = 5000, Quantity = 100, OptionCategory = categoryHair, Gender = LitiBrickHouse.Enums.GenderType.Female },
                // Mặt
                new LitiBrickHouse.Models.CustomOption { Name = "Mặt Cười (Nam)", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", Quantity = 100, OptionCategory = categoryFace, Gender = LitiBrickHouse.Enums.GenderType.Male },
                // Áo
                new LitiBrickHouse.Models.CustomOption { Name = "Áo Sơ mi", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", Quantity = 100, OptionCategory = categoryClothes, Gender = LitiBrickHouse.Enums.GenderType.Unisex },
                // Quần
                new LitiBrickHouse.Models.CustomOption { Name = "Quần Jean", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", Quantity = 100, OptionCategory = categoryPants, Gender = LitiBrickHouse.Enums.GenderType.Unisex },
                // Phụ kiện
                new LitiBrickHouse.Models.CustomOption { Name = "Mũ Lưỡi trai", ImageUrl = "/images/placeholder.png", ThumbnailUrl = "/images/placeholder.png", AdditionalPrice = 10000, Quantity = 100, OptionCategory = categoryAcc, Gender = LitiBrickHouse.Enums.GenderType.Unisex }
            );

            await context.SaveChangesAsync();
        }

        // === Gieo mầm Admin & Roles ===
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
        var adminEmail = "admin@litibrickhouse.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, "Password123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}