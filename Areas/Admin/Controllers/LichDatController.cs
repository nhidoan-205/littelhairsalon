using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LittleHairSalon.Models;

namespace LittleHairSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/LichDat/{action=Index}/{id?}")]
    public class LichDatController : Controller
    {
        private readonly LittleHairSalonContext _context;
        public LichDatController(LittleHairSalonContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lichDats = await _context.LichDats
                .Include(l => l.KhachHang)
                .Include(l => l.DichVu)
                .Include(l => l.NhanVien)
                .OrderByDescending(l => l.NgayDat)
                .ToListAsync();
            return View(lichDats);
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var lich = await _context.LichDats
                .Include(l => l.KhachHang)
                .Include(l => l.DichVu)
                .Include(l => l.NhanVien)
                .FirstOrDefaultAsync(l => l.MaLichDat == id);
            if (lich == null) return NotFound();
            return View(lich);
        }

        [HttpGet]
        public async Task<IActionResult> XacNhan(int id)
        {
            var lich = await _context.LichDats.FindAsync(id);
            if (lich != null)
            {
                lich.TrangThai = 1;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> HuyLich(int id)
        {
            var lich = await _context.LichDats.FindAsync(id);
            if (lich != null)
            {
                lich.TrangThai = 3;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}