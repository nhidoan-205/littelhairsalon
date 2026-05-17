using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;
using LittleHairSalon.Filters;

namespace LittleHairSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize] // Đảm bảo Filter này kiểm tra Role == "Admin"
    [Route("Admin/[controller]/[action]")]
    public class HomeAdminController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public HomeAdminController(LittleHairSalonContext context) => _context = context;

        // 1. DASHBOARD - Trang chủ quản trị
        [Route("/Admin")]
        [Route("/Admin/HomeAdmin")]
        public async Task<IActionResult> Index()
        {
            ViewBag.TongSanPham = await _context.Hangs.CountAsync();
            ViewBag.TongDonHang = await _context.DonHangs.CountAsync();
            ViewBag.DoanhThu = await _context.DonHangs.AnyAsync()
                ? await _context.DonHangs.SumAsync(x => x.TongTien)
                : 0;
            return View();
        }

        // 2. DANH SÁCH SẢN PHẨM
        [HttpGet]
        public async Task<IActionResult> QuanLySanPham(string searchString)
        {
            var query = _context.Hangs.Include(h => h.MaLoaiNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.TenHang.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var ds = await query.OrderByDescending(h => h.MaHang).ToListAsync();
            return View(ds);
        }

        // 3. THÊM SẢN PHẨM
        [HttpGet]
        public IActionResult ThemSanPham()
        {
            ViewBag.MaLoai = new SelectList(_context.Loais, "MaLoai", "TenLoai");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham(Hang sanPhamMoi, IFormFile fHinh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu tên file ảnh nếu có upload
                if (fHinh != null)
                {
                    sanPhamMoi.HinhAnh = fHinh.FileName;
                    // Bạn nên thêm code copy file vào wwwroot/images tại đây
                }

                _context.Hangs.Add(sanPhamMoi);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(QuanLySanPham));
            }
            ViewBag.MaLoai = new SelectList(_context.Loais, "MaLoai", "TenLoai", sanPhamMoi.MaLoai);
            return View(sanPhamMoi);
        }

        // 4. SỬA SẢN PHẨM
        [HttpGet]
        public async Task<IActionResult> SuaSanPham(int id)
        {
            var sp = await _context.Hangs.FindAsync(id);
            if (sp == null) return NotFound();

            ViewBag.MaLoai = new SelectList(_context.Loais, "MaLoai", "TenLoai", sp.MaLoai);
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaSanPham(Hang sanPham, IFormFile fHinh)
        {
            if (ModelState.IsValid)
            {
                if (fHinh != null)
                {
                    sanPham.HinhAnh = fHinh.FileName;
                }

                _context.Hangs.Update(sanPham);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(QuanLySanPham));
            }
            ViewBag.MaLoai = new SelectList(_context.Loais, "MaLoai", "TenLoai", sanPham.MaLoai);
            return View(sanPham);
        }

        // 5. XÓA SẢN PHẨM
        [HttpPost]
        public async Task<IActionResult> XoaSanPham(int id)
        {
            var sp = await _context.Hangs.FindAsync(id);
            if (sp != null)
            {
                _context.Hangs.Remove(sp);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã xóa sản phẩm!";
            }
            return RedirectToAction(nameof(QuanLySanPham));
        }
    }
}