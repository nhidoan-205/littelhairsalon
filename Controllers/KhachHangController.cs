using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;

namespace WebMyPham.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public KhachHangController(LittleHairSalonContext context)
        {
            _context = context;
        }

        // 1. Action hiển thị danh sách đơn hàng (Sửa lỗi 404 của bạn)
        public async Task<IActionResult> DonHangCuaToi()
        {
            var tenDangNhap = HttpContext.Session.GetString("TenDangNhap");

            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("DangNhap", "TaiKhoan");

            // Tìm khách hàng đang đăng nhập
            var khach = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TenDangNhap == tenDangNhap
                                       || k.HoTen == tenDangNhap
                                       || k.Email == tenDangNhap);

            if (khach == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            // Lọc đơn hàng theo tên hoặc số điện thoại của khách
            var danhSachDonHang = await _context.DonHangs
     .Where(d => d.MaKh == khach.MaKh)
     .OrderByDescending(d => d.NgayDat)
     .ToListAsync();

            return View(danhSachDonHang);
        }

        // 2. Action hiển thị chi tiết một đơn hàng cụ thể
        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            // Tìm đơn hàng theo ID, nạp kèm các ChiTietDon và thông tin Hàng Hóa
            var donHang = await _context.DonHangs
        .Include(d => d.ChiTietDons)
            .ThenInclude(c => c.MaHangNavigation)
        .FirstOrDefaultAsync(d => d.MaDon == id); // <--- Đổi thành MaDon

            if (donHang == null)
            {
                // Thay vì hiện trang trắng, mình trả về thông báo để dễ debug
                return Content($"Không tìm thấy đơn hàng có mã: {id}");
            }

            return View(donHang);
        }
    }
}