using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LittleHairSalon.Models;
using System.Linq;

namespace LittleHairSalon.Controllers
{
    public class DatLichController : Controller
    {
        private readonly LittleHairSalonContext _context;
        public DatLichController(LittleHairSalonContext context)
        {
            _context = context;
        }

        // ✅ GET: giữ nguyên, KHÔNG có tham số thừa
        public IActionResult Index()
        {
            var dsDichVu = _context.DichVus.Select(d => new {
                MaDichVu = d.MaDichVu,
                HienThi = d.TenDichVu + " - [" + d.Gia.ToString() + " VND]"
            }).ToList();
            ViewBag.MaDichVu = new SelectList(dsDichVu, "MaDichVu", "HienThi");

            var thoDangLam = _context.NhanViens.Where(n => n.TrangThai == 1).ToList();
            ViewBag.MaNhanVien = new SelectList(thoDangLam, "MaNhanVien", "TenNhanVien");

            return View(new LichDat());
        }

        // ✅ POST: thêm 2 tham số mới vào đây
        [HttpPost]
        public IActionResult Index(LichDat model, string? TenKhachVangLai, string? SdtKhachVangLai)
        {
            ModelState.Remove("KhachHang");
            ModelState.Remove("DichVu");
            ModelState.Remove("NhanVien");
            ModelState.Remove("GhiChu");

            // ✅ Kiểm tra khách chưa đăng nhập phải nhập tên & sdt
            var tenDangNhap = HttpContext.Session.GetString("TenDangNhap");
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                if (string.IsNullOrWhiteSpace(TenKhachVangLai))
                    ModelState.AddModelError("", "⚠️ Vui lòng nhập họ tên của bạn.");
                if (string.IsNullOrWhiteSpace(SdtKhachVangLai))
                    ModelState.AddModelError("", "⚠️ Vui lòng nhập số điện thoại.");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng giờ
                var trungGio = _context.LichDats.Any(l =>
                    l.NgayDat == model.NgayDat &&
                    l.GioDat == model.GioDat &&
                    l.MaNhanVien == model.MaNhanVien &&
                    l.TrangThai != 3); // 3 = đã hủy

                if (trungGio)
                {
                    ModelState.AddModelError("", "⚠️ Khung giờ này đã có người đặt! Vui lòng chọn giờ khác.");
                    NapLaiViewBag();
                    return View(model);
                }

                // ✅ Gán MaKhachHang nếu đã đăng nhập
                if (!string.IsNullOrEmpty(tenDangNhap))
                {
                    var khach = _context.KhachHangs.FirstOrDefault(k =>
                        k.TenDangNhap == tenDangNhap || k.HoTen == tenDangNhap);
                    if (khach != null) model.MaKhachHang = khach.MaKh;
                }

                // ✅ Nếu chưa đăng nhập → lưu tên & sdt vào GhiChu
                if (string.IsNullOrEmpty(tenDangNhap))
                {
                    var thongTinKhach = $"[Khách vãng lai] Tên: {TenKhachVangLai} | SĐT: {SdtKhachVangLai}";
                    model.GhiChu = string.IsNullOrWhiteSpace(model.GhiChu)
                        ? thongTinKhach
                        : thongTinKhach + " | " + model.GhiChu;
                }

                model.TrangThai = 0;
                model.NgayTaoLich = DateTime.Now;
                _context.LichDats.Add(model);
                _context.SaveChanges();
                return RedirectToAction("ThanhCong");
            }

            NapLaiViewBag();
            return View(model);
        }

        public IActionResult ThanhCong()
        {
            return View();
        }

        // ✅ Hàm phụ tránh lặp code
        private void NapLaiViewBag()
        {
            ViewBag.MaDichVu = new SelectList(
                _context.DichVus.Select(d => new {
                    MaDichVu = d.MaDichVu,
                    HienThi = d.TenDichVu + " - [" + d.Gia.ToString() + " VND]"
                }).ToList(), "MaDichVu", "HienThi");

            ViewBag.MaNhanVien = new SelectList(
                _context.NhanViens.Where(n => n.TrangThai == 1).ToList(),
                "MaNhanVien", "TenNhanVien");
        }
    }
}