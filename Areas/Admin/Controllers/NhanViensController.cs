using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;
using Microsoft.AspNetCore.Http; // Thêm để dùng IFormFile
using System.IO;

namespace LittleHairSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanViensController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public NhanViensController(LittleHairSalonContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách nhân viên
        public async Task<IActionResult> Index()
        {
            var nhanViens = await _context.NhanViens.ToListAsync();
            return View(nhanViens);
        }

        // 2. Trang thêm mới nhân viên (Dành cho link Admin/NhanViens/Create)
        // GET: Admin/NhanViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // 3. Xử lý lưu nhân viên mới (Có upload ảnh)
        // POST: Admin/NhanViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenNhanVien,Email,ChucVu,KinhNghiem,TrangThai")] NhanVien nhanVien, IFormFile fAnh)
        {
            // Bỏ qua kiểm tra các trường không nhập từ Form hoặc tự tăng
            ModelState.Remove("MaNhanVien");
            ModelState.Remove("AnhDaiDien");

            if (ModelState.IsValid)
            {
                try
                {
                    if (fAnh != null && fAnh.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fAnh.FileName);
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "nhanvien", fileName);

                        // Đảm bảo thư mục tồn tại trước khi lưu
                        var directory = Path.GetDirectoryName(path);
                        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await fAnh.CopyToAsync(stream);
                        }
                        nhanVien.AnhDaiDien = fileName;
                    }

                    _context.Add(nhanVien);
                    // Đây là nơi thường xảy ra lỗi Timeout nếu kết nối DB yếu
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Thêm lỗi vào ModelState để hiển thị ra View thay vì crash trang
                    ModelState.AddModelError("", "Không thể kết nối đến cơ sở dữ liệu: " + ex.Message);
                }
            }
            return View(nhanVien);
        }

        // 4. Hiển thị trang chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null) return NotFound();
            return View(nhanVien);
        }

        // 5. Xử lý lưu thông tin chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNhanVien,TenNhanVien,Email,ChucVu,AnhDaiDien,KinhNghiem,TrangThai")] NhanVien nhanVien, IFormFile? fAnh)
        {
            if (id != nhanVien.MaNhanVien) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (fAnh != null && fAnh.Length > 0)
                    {
                        // Xử lý lưu ảnh mới như cũ
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fAnh.FileName);
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "nhanvien", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await fAnh.CopyToAsync(stream);
                        }
                        nhanVien.AnhDaiDien = fileName;
                    }
                    else
                    {
                        // CỰC KỲ QUAN TRỌNG: Nếu không chọn ảnh mới, lấy lại tên ảnh cũ từ Database
                        // (Trường hợp thẻ <input type="hidden" asp-for="AnhDaiDien" /> ở View bị lỗi)
                        var currentNv = await _context.NhanViens.AsNoTracking().FirstOrDefaultAsync(x => x.MaNhanVien == id);
                        if (currentNv != null)
                        {
                            nhanVien.AnhDaiDien = currentNv.AnhDaiDien;
                        }
                    }

                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNhanVien)) return NotFound();
                    else throw;
                }
            }
            return View(nhanVien);
        }

        private bool NhanVienExists(int maNhanVien)
        {
            return _context.NhanViens.Any(e => e.MaNhanVien == maNhanVien);
        }

        // 6. Thay đổi trạng thái nhanh (Ẩn/Hiện)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThayDoiTrangThai(int id)
        {
            var nv = await _context.NhanViens.FindAsync(id);
            if (nv != null)
            {
                nv.TrangThai = (nv.TrangThai == 1) ? 0 : 1;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                // Gợi ý: Bạn có thể xóa file ảnh trong thư mục wwwroot tại đây trước khi xóa db
                _context.NhanViens.Remove(nhanVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}