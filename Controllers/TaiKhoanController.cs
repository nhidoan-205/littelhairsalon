using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LittleHairSalon.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace LittleHairSalon.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly LittleHairSalonContext _context;
        public TaiKhoanController(LittleHairSalonContext context) => _context = context;

        // ================= ĐĂNG NHẬP THƯỜNG =================
        [HttpGet]
        public IActionResult DangNhap() => View();

        [HttpPost]
        public IActionResult DangNhap(string TenDangNhap, string MatKhau)
        {
            // 1. Kiểm tra Admin trong bảng NhanViens (chỉ dùng Email + ChucVu, không có MatKhau)
            var admin = _context.NhanViens.FirstOrDefault(n => n.Email == TenDangNhap && n.ChucVu == "Admin");
            if (admin != null)
            {
                HttpContext.Session.SetString("TenDangNhap", admin.TenNhanVien ?? "Admin");
                HttpContext.Session.SetString("VaiTro", "Admin");
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            // 2. Kiểm tra trong bảng KhachHangs
            var user = _context.KhachHangs.FirstOrDefault(k => k.TenDangNhap == TenDangNhap && k.MatKhau == MatKhau);
            if (user != null)
            {
                HttpContext.Session.SetString("TenDangNhap", user.HoTen ?? user.TenDangNhap);
                HttpContext.Session.SetString("VaiTro", user.VaiTro ?? "User");

                if (user.VaiTro == "Admin")
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });

                return Redirect("/TrangChu");
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        // ================= ĐĂNG KÝ =================
        [HttpGet]
        public IActionResult DangKy() => View();

        [HttpPost]
        public IActionResult DangKy(string HoTen, string TenDangNhap, string Email, string DienThoai, string MatKhau, string XacNhanMatKhau)
        {
            if (MatKhau != XacNhanMatKhau)
            {
                TempData["Message"] = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            var existed = _context.KhachHangs.FirstOrDefault(k => k.TenDangNhap == TenDangNhap);
            if (existed != null)
            {
                TempData["Message"] = "Tên đăng nhập đã được sử dụng, vui lòng chọn tên khác!";
                return View();
            }

            var emailExisted = _context.KhachHangs.FirstOrDefault(k => k.Email == Email);
            if (emailExisted != null)
            {
                TempData["Message"] = "Email này đã được đăng ký, vui lòng dùng email khác!";
                return View();
            }

            var newUser = new KhachHang
            {
                HoTen = HoTen,
                TenDangNhap = TenDangNhap,
                Email = Email,
                DienThoai = DienThoai,
                MatKhau = MatKhau,
                VaiTro = "User"
            };

            _context.KhachHangs.Add(newUser);
            _context.SaveChanges();

            TempData["Message"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        // ================= GOOGLE AUTH =================
        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "TaiKhoan", null, Request.Scheme)
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("DangNhap");

            var email = result.Principal?.FindFirstValue(ClaimTypes.Email);
            var hoTen = result.Principal?.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Không lấy được thông tin email từ Google.";
                return RedirectToAction("DangNhap");
            }

            // Ưu tiên 1: Kiểm tra bảng NhanViens
            var nhanVien = _context.NhanViens.FirstOrDefault(n => n.Email == email);
            if (nhanVien != null)
            {
                HttpContext.Session.SetString("TenDangNhap", nhanVien.TenNhanVien ?? "Admin");
                HttpContext.Session.SetString("VaiTro", nhanVien.ChucVu ?? "Admin");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            // Ưu tiên 2: Kiểm tra / tạo mới KhachHang
            var user = _context.KhachHangs.FirstOrDefault(k => k.Email == email);
            if (user == null)
            {
                user = new KhachHang
                {
                    HoTen = hoTen ?? "User",
                    Email = email,
                    TenDangNhap = email,
                    MatKhau = "",
                    VaiTro = "User"
                };
                _context.KhachHangs.Add(user);
                _context.SaveChanges();
            }

            HttpContext.Session.SetString("TenDangNhap", user.HoTen ?? user.TenDangNhap);
            HttpContext.Session.SetString("VaiTro", user.VaiTro ?? "User");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (user.VaiTro == "Admin")
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });

            return Redirect("/TrangChu");
        }
        // ================= FACEBOOK AUTH =================
        [HttpGet]
        public IActionResult LoginWithFacebook()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse", "TaiKhoan", null, Request.Scheme)
            };
            return Challenge(properties, "Facebook");
        }

        [HttpGet]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("DangNhap");

            var email = result.Principal?.FindFirstValue(ClaimTypes.Email);
            var hoTen = result.Principal?.FindFirstValue(ClaimTypes.Name);
            var facebookId = result.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            // Nếu không có email thì dùng FacebookID làm định danh
            var identifier = !string.IsNullOrEmpty(email) ? email : $"fb_{facebookId}@facebook.com";

            var user = _context.KhachHangs.FirstOrDefault(k => k.Email == identifier);
            if (user == null)
            {
                user = new KhachHang
                {
                    HoTen = hoTen ?? "Facebook User",
                    Email = identifier,
                    TenDangNhap = identifier,
                    MatKhau = "",
                    VaiTro = "User"
                };
                _context.KhachHangs.Add(user);
                _context.SaveChanges();
            }

            HttpContext.Session.SetString("TenDangNhap", user.HoTen ?? user.TenDangNhap);
            HttpContext.Session.SetString("VaiTro", user.VaiTro ?? "User");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (user.VaiTro == "Admin")
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });

            return Redirect("/TrangChu");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }
}
