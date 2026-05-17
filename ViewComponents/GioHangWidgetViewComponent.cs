using Microsoft.AspNetCore.Mvc;
using LittleHairSalon.Models;
using LittleHairSalon.Helpers; // <--- Phải có dòng này để nó hiểu lệnh .Get

namespace LittleHairSalon.ViewComponents
{
    public class GioHangWidgetViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Bây giờ chữ .Get sẽ hết bị gạch đỏ
            var data = HttpContext.Session.Get<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();

            int totalQuantity = data.Sum(p => p.SoLuong);

            return View(totalQuantity);
        }
    }
}