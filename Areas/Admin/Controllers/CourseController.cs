using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models; 
using System.Security.Claims;
using LittleHairSalon.Services;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace LittleHairSalon.Controllers // SỬA NAMESPACE
{
    public class CourseController : Controller
    {
        private readonly LittleHairSalonContext _context; // DÙNG CONTEXT CỦA LÁ BEAUTY

        public CourseController(LittleHairSalonContext context)
        {
            _context = context;
        }

        // Đã sửa lại để lấy Session đúng chuẩn của Lá Beauty ("TenDangNhap")
        private string? GetCurrentUserEmail()
        {
            // Lấy từ Google Auth (nếu đăng nhập bằng Google)
            var email = User?.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email)) return email;

            // Lấy từ Session (nếu đăng nhập thủ công)
            return HttpContext.Session.GetString("TenDangNhap");
        }

        // US-009: Xem danh sách khóa học / Dịch vụ
        public async Task<IActionResult> Index(string searchString)
        {
            var courses = from c in _context.Courses
                          select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c =>
                  c.Title.Contains(searchString) ||
                  c.Description.Contains(searchString)
                );
            }

            return View(await courses.ToListAsync());
        }

        public IActionResult Details(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Lưu ý: Đảm bảo bạn có class ViewModel và EmailService trong project Lá Beauty
        public async Task<IActionResult> Register([FromForm] LittleHairSalon.ViewModels.RegisterCourseViewModel model, [FromServices] EmailService emailService)
        {
            if (!ModelState.IsValid)
            {
                TempData["RegisterError"] = "Dữ liệu đăng ký không hợp lệ.";
                return RedirectToAction("Details", new { id = model.CourseId });
            }

            var course = await _context.Courses.FindAsync(model.CourseId);
            if (course == null) return NotFound();

            var reg = new Registration // Chắc chắn bạn đã tạo model Registration
            {
                CourseId = model.CourseId,
                FullName = model.FullName,
                Phone = model.Phone,
                Email = model.Email,
                Role = model.Role,
                Location = model.Location,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

            try
            {
                _context.Registrations.Add(reg);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("log.txt", ex.ToString());
                TempData["RegisterError"] = "Lỗi khi lưu đăng ký: " + ex.Message;
                return RedirectToAction("Details", new { id = model.CourseId });
            }

            // Lá Beauty dùng "TenDangNhap" để lưu session user
            HttpContext.Session.SetString("TenDangNhap", reg.Email);

            try
            {
                var body = $"Cảm ơn {reg.FullName} đã đăng ký {course.Title}. Lá Beauty sẽ liên hệ bạn sớm.";
                await emailService.SendEmailAsync(reg.Email, "Xác nhận đăng ký Lá Beauty", body);
            }
            catch
            {
                // Bỏ qua lỗi gửi mail nếu có
            }

            TempData["RegisterSuccess"] = "Đăng ký thành công. Chúng tôi sẽ liên hệ với bạn sớm.";
            return RedirectToAction("Details", new { id = model.CourseId });
        }

        public class CancelRegistrationRequest
        {
            public int RegistrationId { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRegistration([FromForm] int registrationId)
        {
            if (registrationId <= 0)
            {
                TempData["Error"] = "Yêu cầu không hợp lệ.";
                return RedirectToAction("Index");
            }

            var reg = await _context.Registrations.FindAsync(registrationId);
            if (reg == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin đăng ký.";
                return RedirectToAction("Index");
            }

            var username = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Details", new { id = reg.CourseId });
            }

            // SỬA LẠI THEO BẢNG KhachHang VÀ CỘT VaiTro CỦA LÁ BEAUTY
            var currentUser = _context.KhachHangs.FirstOrDefault(u => u.TenDangNhap == username || u.Email == username);
            var isOwner = reg.Email == username;
            var isAdmin = currentUser != null && currentUser.VaiTro == "Admin"; // Đổi Role thành VaiTro

            if (!isOwner && !isAdmin)
            {
                TempData["Error"] = "Bạn không có quyền thực hiện hành động này.";
                return RedirectToAction("Details", new { id = reg.CourseId });
            }

            var regsToRemove = _context.Registrations.Where(r => r.CourseId == reg.CourseId && r.Email == reg.Email).ToList();
            if (regsToRemove.Any())
            {
                _context.Registrations.RemoveRange(regsToRemove);
                await _context.SaveChangesAsync();
            }
            TempData["CancelSuccess"] = "Đã hủy đăng ký thành công.";
            return RedirectToAction("Details", new { id = reg.CourseId });
        }

        public class UpdateRegistrationRequest
        {
            public int RegistrationId { get; set; }
            public string? FullName { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Role { get; set; }
            public string? Location { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistration(UpdateRegistrationRequest req)
        {
            if (req == null || req.RegistrationId <= 0)
            {
                TempData["Error"] = "Yêu cầu không hợp lệ.";
                return RedirectToAction("Index");
            }

            var reg = await _context.Registrations.FindAsync(req.RegistrationId);
            if (reg == null)
            {
                TempData["Error"] = "Không tìm thấy đăng ký.";
                return RedirectToAction("Index");
            }

            var username = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Details", new { id = reg.CourseId });
            }

            // SỬA LẠI THEO BẢNG KhachHang VÀ CỘT VaiTro CỦA LÁ BEAUTY
            var currentUser = _context.KhachHangs.FirstOrDefault(u => u.TenDangNhap == username || u.Email == username);
            var isOwner = reg.Email == username;
            var isAdmin = currentUser != null && currentUser.VaiTro == "Admin";

            if (!isOwner && !isAdmin)
            {
                TempData["Error"] = "Bạn không có quyền thực hiện hành động này.";
                return RedirectToAction("Details", new { id = reg.CourseId });
            }

            reg.FullName = req.FullName ?? reg.FullName;
            reg.Phone = req.Phone ?? reg.Phone;
            reg.Email = req.Email ?? reg.Email;
            reg.Role = req.Role ?? reg.Role;
            reg.Location = req.Location ?? reg.Location;

            await _context.SaveChangesAsync();

            TempData["UpdateSuccess"] = "Cập nhật thông tin đăng ký thành công.";
            return RedirectToAction("Details", new { id = reg.CourseId });
        }
    }
}
