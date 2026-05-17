using Microsoft.AspNetCore.Mvc;
using LittleHairSalon.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LittleHairSalon.Controllers
{
    public class GioHangController : Controller
    {
        private readonly LittleHairSalonContext _context;

        public GioHangController(LittleHairSalonContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(LayGioHang());
        }

        [HttpPost]
        public IActionResult ThemVaoGio(int maHang)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.MaHang == maHang);
            if (item == null)
            {
                var hang = _context.Hangs.FirstOrDefault(p => p.MaHang == maHang);
                if (hang != null)
                {
                    item = new GioHangItem
                    {
                        MaHang = hang.MaHang,
                        TenHang = hang.TenHang,
                        HinhAnh = hang.HinhAnh,
                        DonGia = hang.Gia ?? 0,
                        SoLuong = 1
                    };
                    gioHang.Add(item);
                }
            }
            else { item.SoLuong++; }
            LuuGioHang(gioHang);
            return Json(new { soLuong = gioHang.Sum(x => x.SoLuong) });
        }
        public IActionResult TangSoLuong(int maHang)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.MaHang == maHang);
            if (item != null)
            {
                item.SoLuong++; 
                LuuGioHang(gioHang);
            }
            return RedirectToAction("Index"); 
        }
        public IActionResult GiamSoLuong(int maHang)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.MaHang == maHang);
            if (item != null)
            {
                if (item.SoLuong > 1)
                {
                    item.SoLuong--; 
                }
                LuuGioHang(gioHang);
            }
            return RedirectToAction("Index");
        }
        public IActionResult XoaKhoiGio(int maHang)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.MaHang == maHang);
            if (item != null)
            {
                gioHang.Remove(item); 
                LuuGioHang(gioHang);
            }
            return RedirectToAction("Index");
        }
        private List<GioHangItem> LayGioHang()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString("GioHang");
            if (jsoncart != null) return JsonSerializer.Deserialize<List<GioHangItem>>(jsoncart);
            return new List<GioHangItem>();
        }

        private void LuuGioHang(List<GioHangItem> gioHang)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonSerializer.Serialize(gioHang);
            session.SetString("GioHang", jsoncart);
        }
    }
}