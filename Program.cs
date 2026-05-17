using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using LittleHairSalon.Models;

var builder = WebApplication.CreateBuilder(args);


// 1. Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("LittleHairSalonContext") ?? "";
builder.Services.AddDbContext<LittleHairSalonContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));
// 2. Cấu hình Session & Caching
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".LittleHairSalon.Session"; // Đặt tên để tránh xung đột
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<LittleHairSalon.Services.EmailService>();

// 3. Cấu hình Authentication (Google, Facebook, Cookie)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // Để mặc định là Cookie để không bị tự động văng sang trang Google khi chưa đăng nhập
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.LoginPath = "/TaiKhoan/DangNhap";
    options.AccessDeniedPath = "/Home/Error"; // Trang báo lỗi khi không có quyền Admin
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Ghi nhớ đăng nhập 7 ngày
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Facebook:AppId"] ?? "";
    options.AppSecret = builder.Configuration["Facebook:AppSecret"] ?? "";
});

System.Environment.SetEnvironmentVariable("EPPLUS_LICENSE_CONTEXT", "NonCommercial"); var app = builder.Build();

// Cấu hình Pipeline xử lý HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// THỨ TỰ QUAN TRỌNG: Session -> Authentication -> Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 4. Cấu hình Route (Hoàn thiện phần Areas)
app.UseEndpoints(endpoints =>
{
    // Cấu hình Area Admin: PHẢI đặt phía trên default route
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}"
    );

    // Cấu hình mặc định cho trang chủ
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=TrangChu}/{action=Index}/{id?}");
});

app.Run();