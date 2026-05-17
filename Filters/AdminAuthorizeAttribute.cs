using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace LittleHairSalon.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var session = http.Session;
            var vaiTro = session.GetString("VaiTro");

            if (string.IsNullOrEmpty(vaiTro) || !string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { area = string.Empty });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
