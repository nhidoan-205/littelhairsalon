using Microsoft.AspNetCore.Mvc;
using LittleHairSalon.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Linq;

namespace LittleHairSalon.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public ThanhToanController(LittleHairSalonContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            var session = HttpContext.Session;
            string jsoncart = session.GetString("GioHang");
            if (string.IsNullOrEmpty(jsoncart))
            {
                return RedirectToAction("Index", "TrangChu");
            }

            var gioHang = JsonSerializer.Deserialize<List<GioHangItem>>(jsoncart);
            decimal tongTienHang = gioHang.Sum(item => item.ThanhTien);

            ViewBag.TongTienHang = tongTienHang;
            ViewBag.PhiShip = 30000;
            ViewBag.TongThanhToan = tongTienHang + 30000;

            var username = HttpContext.Session.GetString("TenDangNhap");
            var khach = _context.KhachHangs.FirstOrDefault(k => k.TenDangNhap == username);
            if (khach != null)
            {
                ViewBag.KhachHang = khach;
            }

            return View();
        }

        [HttpPost]
        public IActionResult DatHang(string hoTen, string sdt, string diaChi, string ghiChu, string loaiThanhToan)
        {
            var username = HttpContext.Session.GetString("TenDangNhap");
            if (username == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var session = HttpContext.Session;
            string jsoncart = session.GetString("GioHang");
            if (string.IsNullOrEmpty(jsoncart)) return RedirectToAction("Index", "TrangChu");

            var gioHang = JsonSerializer.Deserialize<List<GioHangItem>>(jsoncart);
            decimal tongTienHang = gioHang.Sum(x => x.ThanhTien);
            decimal tongThanhToan = tongTienHang + 30000;

            // 1. Tạo đối tượng đơn hàng
            var khach = _context.KhachHangs.FirstOrDefault(k => k.TenDangNhap == username || k.HoTen == username || k.Email == username);

            var donHang = new DonHang
            {
                NguoiNhan = hoTen,
                Sdt = sdt,
                DiaChi = diaChi,
                GhiChu = ghiChu,
                NgayDat = DateTime.Now,
                TongTien = tongThanhToan,
                MaKh = khach?.MaKh,  
                TrangThai = loaiThanhToan == "VNPAY" ? "Chờ thanh toán VNPAY" :
            loaiThanhToan == "NGANHANG" ? "Chờ chuyển khoản" :
            loaiThanhToan == "MOMO" ? "Chờ thanh toán MoMo" :
            "Mới đặt (COD)",
                PTThanhToan = loaiThanhToan
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges(); // SQL sẽ sinh ra MaDon tại đây

            // 2. Lưu chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTiet = new ChiTietDon
                {
                    MaDon = donHang.MaDon, // MaDon lúc này đã có giá trị từ SQL
                    MaHang = item.MaHang,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                };
                _context.ChiTietDons.Add(chiTiet);
            }
            _context.SaveChanges();

            // 3. Xóa giỏ hàng
            HttpContext.Session.Remove("GioHang");

            if (loaiThanhToan == "VNPAY")
            {
                return RedirectToAction("VnPayDemo", new { maDon = donHang.MaDon, soTien = tongThanhToan });
            }

            if (loaiThanhToan == "MOMO")
            {
                return RedirectToAction("MomoDemo", new { maDon = donHang.MaDon, soTien = tongThanhToan });
            }

            return RedirectToAction("DatHangThanhCong", new { maDon = donHang.MaDon, loaiTT = loaiThanhToan });
        }
            [HttpGet]
        public IActionResult DatHangThanhCong(int maDon, string loaiTT)
        {
            var donHang = _context.DonHangs.FirstOrDefault(x => x.MaDon == maDon);

            if (donHang == null)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            ViewBag.LoaiThanhToan = loaiTT;
            ViewBag.TongTien = donHang.TongTien;

            return View(donHang); // Truyền model là donHang
        }
        [HttpGet]
        public IActionResult MomoDemo(int maDon, decimal soTien)
        {
            ViewBag.MaDon = maDon;
            ViewBag.SoTien = soTien;
            return View();
        }

        [HttpGet]
        public IActionResult VnPayDemo(int maDon, decimal soTien)
        {
            ViewBag.MaDon = maDon;
            ViewBag.SoTien = soTien;
            return View();
        }

        public IActionResult XuLyVnPay(int maDon, bool thanhCong)
        {
            if (thanhCong)
            {
                var donHang = _context.DonHangs.FirstOrDefault(x => x.MaDon == maDon);
                if (donHang != null)
                {
                    donHang.TrangThai = "Đã thanh toán qua VNPAY";
                    _context.SaveChanges();
                }
                return RedirectToAction("DatHangThanhCong", new { maDon = maDon, loaiTT = "VNPAY" });
            }
            return RedirectToAction("Index", "TrangChu");
        }
    }
}