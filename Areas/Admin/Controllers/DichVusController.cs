using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;
using System.IO;

namespace LittleHairSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DichVusController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public DichVusController(LittleHairSalonContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách dịch vụ
        public async Task<IActionResult> Index()
        {
            var items = await _context.DichVus.ToListAsync();
            return View(items);
        }

        // 2. Trang thêm mới dịch vụ (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. Xử lý lưu dịch vụ mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDichVu,MoTa,Gia,ThoiGianUocTinh")] DichVu dichVu, IFormFile fAnh)
        {
            // Loại bỏ kiểm tra lỗi cho trường ảnh và ID tự tăng
            ModelState.Remove("AnhDichVu");
            ModelState.Remove("MaDichVu");

            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh nếu có file được chọn
                if (fAnh != null && fAnh.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fAnh.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dichvu", fileName);

                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await fAnh.CopyToAsync(stream);
                    }
                    dichVu.AnhDichVu = fileName;
                }
                else
                {
                    dichVu.AnhDichVu = "https://images.unsplash.com/photo-1560066984-138dadb4c035?w=400";
                }

                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dichVu);
        }

        // 4. Hiển thị trang chỉnh sửa (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null) return NotFound();

            return View(dichVu);
        }

        // 5. Xử lý lưu thông tin chỉnh sửa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDichVu,TenDichVu,MoTa,Gia,ThoiGianUocTinh,AnhDichVu")] DichVu dichVu, IFormFile? fAnh)
        {
            if (id != dichVu.MaDichVu) return NotFound();

            ModelState.Remove("AnhDichVu");

            if (ModelState.IsValid)
            {
                try
                {
                    if (fAnh != null && fAnh.Length > 0)
                    {
                        // Xử lý ảnh mới
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fAnh.FileName);
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dichvu", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await fAnh.CopyToAsync(stream);
                        }
                        dichVu.AnhDichVu = fileName;
                    }
                    else
                    {
                        // Giữ lại ảnh cũ nếu không tải ảnh mới
                        var currentDichVu = await _context.DichVus.AsNoTracking().FirstOrDefaultAsync(x => x.MaDichVu == id);
                        if (currentDichVu != null)
                        {
                            dichVu.AnhDichVu = currentDichVu.AnhDichVu;
                        }
                    }

                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DichVuExists(dichVu.MaDichVu)) return NotFound();
                    else throw;
                }
            }
            return View(dichVu);
        }

        // 6. Xử lý xóa dịch vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu != null)
            {
                // Gợi ý: Xóa file ảnh vật lý để tiết kiệm bộ nhớ server
                if (!string.IsNullOrEmpty(dichVu.AnhDichVu))
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dichvu", dichVu.AnhDichVu);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }

                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVus.Any(e => e.MaDichVu == id);
        }
    }
}