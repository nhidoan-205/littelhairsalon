using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;
using OfficeOpenXml; // Thư viện đọc file Excel (EPPlus)
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LittleHairSalon.Controllers
{
    public class TrangChuController : Controller
    {
        private readonly LittleHairSalonContext _context;
        private readonly IWebHostEnvironment _env; // Biến môi trường để lấy đường dẫn thư mục wwwroot

        // Hàm khởi tạo nhận cả Context Database và Environment
        public TrangChuController(LittleHairSalonContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var danhSachHang = _context.Hangs.Include(h => h.MaLoaiNavigation).ToList();
            return View(danhSachHang);
        }

        // HÀM DỊCH VỤ: TỰ ĐỘNG CHUYỂN SANG ĐỌC EXCEL KHI MẤT KẾT NỐI SQL SERVER
        public IActionResult DichVu()
        {
            List<DichVu> danhSachDichVu = new List<DichVu>();
            List<Hang> danhSachSanPhamGoiY = new List<Hang>();

            try
            {
                // TRƯỜNG HỢP 1: SQL SERVER BẬT -> Lấy dữ liệu từ Database bình thường
                danhSachDichVu = _context.DichVus.OrderBy(d => d.MaDichVu).ToList();
                danhSachSanPhamGoiY = _context.Hangs.OrderBy(h => h.MaHang).Take(4).ToList();
            }
            catch (Exception)
            {
                // TRƯỜNG HỢP 2: SQL SERVER TẮT -> Chuyển hướng sang đọc file Excel 'salon_data.xlsx'
                string filePath = Path.Combine(_env.WebRootPath, "salon_data.xlsx");

                if (System.IO.File.Exists(filePath))
                {
                    // Sử dụng cơ chế gán bản quyền trực tiếp bên trong luồng đọc file bằng lệnh chuỗi (String)
                    // Cách này giải quyết triệt để tình trạng sập Exception hoặc gạch đỏ biến hệ thống
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        // --- ĐỌC SHEET 1: Đọc danh sách Dịch Vụ ---
                        var sheetDichVu = package.Workbook.Worksheets["DichVu"];
                        if (sheetDichVu != null)
                        {
                            int rowCountDV = sheetDichVu.Dimension?.Rows ?? 0;
                            for (int row = 2; row <= rowCountDV; row++)
                            {
                                int.TryParse(sheetDichVu.Cells[row, 1].Value?.ToString(), out int maDV);
                                decimal.TryParse(sheetDichVu.Cells[row, 3].Value?.ToString(), out decimal giaDV);
                                int.TryParse(sheetDichVu.Cells[row, 6].Value?.ToString(), out int thoiGian);

                                danhSachDichVu.Add(new DichVu
                                {
                                    MaDichVu = maDV,
                                    TenDichVu = sheetDichVu.Cells[row, 2].Value?.ToString() ?? "",
                                    Gia = giaDV,
                                    MoTa = sheetDichVu.Cells[row, 4].Value?.ToString() ?? "",
                                    AnhDichVu = sheetDichVu.Cells[row, 5].Value?.ToString() ?? "",
                                    ThoiGianUocTinh = thoiGian
                                });
                            }
                        }

                        // --- ĐỌC SHEET 2: Đọc danh sách Sản Phẩm Gợi Ý ---
                        // Khớp chuẩn xác tên tab "SanPham" trong file Excel của bạn
                        var sheetSanPham = package.Workbook.Worksheets["SanPham"];
                        if (sheetSanPham != null)
                        {
                            int rowCountSP = sheetSanPham.Dimension?.Rows ?? 0;
                            int autoId = 1; // Tạo mã tự động dự phòng nếu ô ID bị trống

                            for (int row = 2; row <= rowCountSP; row++)
                            {
                                // Đọc giá trị ô Tên hàng trước để kiểm tra dữ liệu dòng
                                string tenHang = sheetSanPham.Cells[row, 2].Value?.ToString() ?? "";
                                if (string.IsNullOrEmpty(tenHang)) continue;

                                // Đọc mã hàng (Cột A - Id trong Excel)
                                if (!int.TryParse(sheetSanPham.Cells[row, 1].Value?.ToString(), out int maHang))
                                {
                                    maHang = autoId;
                                }

                                // Đọc Giá sản phẩm (Cột C - DonGia trong file Excel của bạn)
                                decimal.TryParse(sheetSanPham.Cells[row, 3].Value?.ToString(), out decimal giaSP);

                                // Đọc Đường dẫn hình ảnh (Cột D - Hinh trong file Excel của bạn)
                                string hinhAnhSP = sheetSanPham.Cells[row, 4].Value?.ToString() ?? "";

                                // Map chuẩn xác theo thuộc tính gốc (Gia, HinhAnh) của Model Hang
                                danhSachSanPhamGoiY.Add(new Hang
                                {
                                    MaHang = maHang,
                                    TenHang = tenHang,
                                    Gia = giaSP,
                                    HinhAnh = hinhAnhSP
                                });

                                autoId++;
                            }
                        }
                    }
                }
            }

            // Gửi cả 2 danh sách sang View thông qua ViewBag
            ViewBag.DanhSachDichVu = danhSachDichVu;
            ViewBag.DanhSachSanPham = danhSachSanPhamGoiY;

            return View();
        }

        public IActionResult LienHe()
        {
            return View();
        }
    }
}