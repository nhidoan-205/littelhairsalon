using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;
using OfficeOpenXml; // Thư viện EPPlus để đọc file Excel
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LittleHairSalon.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly LittleHairSalonContext _context;
        private readonly IWebHostEnvironment _env; // Biến môi trường để lấy đường dẫn thư mục wwwroot

        // Hàm khởi tạo nhận cả Context Database và Environment
        public HangHoaController(LittleHairSalonContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. TRANG TẤT CẢ SẢN PHẨM
        public IActionResult TatCa()
        {
            List<Hang> data = new List<Hang>();
            try
            {
                // SQL bật -> Lấy từ DB bình thường
                data = _context.Hangs.Include(h => h.MaLoaiNavigation).OrderBy(h => h.MaLoai).ThenBy(h => h.MaHang).ToList();
            }
            catch (Exception)
            {
                // SQL tắt -> Đọc full từ file Excel
                data = LayDanhSachHangTuExcel();
            }

            ViewBag.TieuDe = "Tất Cả Sản Phẩm";
            return View("DanhSach", data);
        }

        // 2. TRANG DẦU GỘI & DẦU XẢ (MaLoai == 1 hoặc MaLoai == 2)
        public IActionResult DauGoi()
        {
            List<Hang> data = new List<Hang>();
            try
            {
                data = _context.Hangs.Include(h => h.MaLoaiNavigation)
                    .Where(h => h.MaLoai == 1 || h.MaLoai == 2).OrderBy(h => h.MaHang).ToList();
            }
            catch (Exception)
            {
                // Lọc Excel theo MaLoai 1 hoặc 2
                data = LayDanhSachHangTuExcel()
                    .Where(h => h.MaLoai == 1 || h.MaLoai == 2).ToList();
            }

            ViewBag.TieuDe = "Dầu Gội & Dầu Xả";
            return View("DanhSach", data);
        }

        // 3. TRANG SÁP VUỐT TÓC (MaLoai == 3)
        public IActionResult SapVuoc()
        {
            List<Hang> data = new List<Hang>();
            try
            {
                data = _context.Hangs.Include(h => h.MaLoaiNavigation)
                    .Where(h => h.MaLoai == 3).OrderBy(h => h.MaHang).ToList();
            }
            catch (Exception)
            {
                // Lọc Excel theo MaLoai 3
                data = LayDanhSachHangTuExcel()
                    .Where(h => h.MaLoai == 3).ToList();
            }

            ViewBag.TieuDe = "Sáp Vuốt Tóc";
            return View("DanhSach", data);
        }

        // 4. TRANG THUỐC NHUỘM TÓC (MaLoai == 4)
        public IActionResult NhuomToc()
        {
            List<Hang> data = new List<Hang>();
            try
            {
                data = _context.Hangs.Include(h => h.MaLoaiNavigation)
                    .Where(h => h.MaLoai == 4).OrderBy(h => h.MaHang).ToList();
            }
            catch (Exception)
            {
                // Lọc Excel theo MaLoai 4
                data = LayDanhSachHangTuExcel()
                    .Where(h => h.MaLoai == 4).ToList();
            }

            ViewBag.TieuDe = "Thuốc Nhuộm Tóc";
            return View("DanhSach", data);
        }

        // 5. TRANG CHI TIẾT SẢN PHẨM
        public IActionResult ChiTiet(int id)
        {
            Hang sanPham = null;
            try
            {
                sanPham = _context.Hangs
                    .Include(h => h.MaLoaiNavigation)
                    .FirstOrDefault(h => h.MaHang == id);
            }
            catch (Exception)
            {
                // Tìm kiếm sản phẩm tương ứng theo ID từ danh sách Excel
                sanPham = LayDanhSachHangTuExcel().FirstOrDefault(h => h.MaHang == id);
            }

            if (sanPham == null) return NotFound();
            return View(sanPham);
        }

        // =========================================================================
        // HÀM DÙNG CHUNG: ĐỌC DỮ LIỆU TỪ TAB "SanPham" TRONG FILE EXCEL SALON_DATA.XLSX
        // =========================================================================
        private List<Hang> LayDanhSachHangTuExcel()
        {
            List<Hang> danhSach = new List<Hang>();
            string filePath = Path.Combine(_env.WebRootPath, "salon_data.xlsx");

            if (System.IO.File.Exists(filePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Xác nhận bản quyền EPPlus trực tiếp

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var sheetSanPham = package.Workbook.Worksheets["SanPham"]; // Đọc đúng tab SanPham
                    if (sheetSanPham != null)
                    {
                        int rowCount = sheetSanPham.Dimension?.Rows ?? 0;
                        int autoId = 1;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string tenHang = sheetSanPham.Cells[row, 2].Value?.ToString() ?? "";
                            if (string.IsNullOrEmpty(tenHang)) continue;

                            // Đọc mã hàng (Cột A)
                            if (!int.TryParse(sheetSanPham.Cells[row, 1].Value?.ToString(), out int maHang))
                            {
                                maHang = autoId;
                            }

                            // Đọc Giá sản phẩm (Cột C)
                            decimal.TryParse(sheetSanPham.Cells[row, 3].Value?.ToString(), out decimal giaSP);

                            // Đọc Đường dẫn hình ảnh (Cột D)
                            string hinhAnhSP = sheetSanPham.Cells[row, 4].Value?.ToString() ?? "";

                            // Đọc cột MaLoai trong file Excel (Cột số 5) để phân loại menu danh mục hàng hóa
                            int.TryParse(sheetSanPham.Cells[row, 5].Value?.ToString(), out int maLoai);

                            danhSach.Add(new Hang
                            {
                                MaHang = maHang,
                                TenHang = tenHang,
                                Gia = giaSP,
                                HinhAnh = hinhAnhSP,
                                MaLoai = maLoai // Gán mã loại để phục vụ tính năng lọc danh mục
                            });

                            autoId++;
                        }
                    }
                }
            }
            return danhSach;
        }
    }
}