using Microsoft.AspNetCore.Mvc;
using LittleHairSalon.Models;
using System.Linq;

namespace LittleHairSalon.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly LittleHairSalonContext _context;
        public DanhGiaController(LittleHairSalonContext context) => _context = context;

        [HttpPost]
        public IActionResult GuiDanhGia(string HoTen, string NoiDung, int SoSao)
        {
            var tenDangNhap = HttpContext.Session.GetString("TenDangNhap");
            var danhGia = new DanhGia
            {
                HoTen = string.IsNullOrEmpty(tenDangNhap) ? HoTen : tenDangNhap,
                NoiDung = NoiDung,
                SoSao = SoSao,
                TrangThai = "Hien thi"
            };
            _context.DanhGias.Add(danhGia);
            _context.SaveChanges();
            TempData["DanhGiaOK"] = "Cam on ban da danh gia!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult DanhSach()
        {
            var data = _context.DanhGias
                .Where(d => d.TrangThai == "Hien thi")
                .OrderByDescending(d => d.NgayDang)
                .ToList();
            return Json(data);
        }
    }
}