using LittleHairSalon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LittleHairSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public ThongKeController(LittleHairSalonContext context)
        {
            _context = context;
        }

        public IActionResult DoanhThu()
        {
            var lichDat = _context.LichDats.ToList();

            // tính doanh thu từ giá dịch vụ
            ViewBag.TongDoanhThu = _context.LichDats
                .Where(x => x.DichVu != null)
                .Sum(x => x.DichVu.Gia);

            return View(lichDat);
        }
    }
}