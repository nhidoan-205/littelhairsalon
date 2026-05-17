using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleHairSalon.Models
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        public int MaKh { get; set; }

        // Thêm dấu ? vào các trường có thể bị bỏ trống
        public string? HoTen { get; set; }

        public string? DienThoai { get; set; }

        public string? Email { get; set; }

        public string? DiaChi { get; set; }  // <-- Đây là thủ phạm chính gây lỗi Null

        // Những trường bắt buộc phải có dữ liệu thì KHÔNG thêm dấu ?
        [Required]
        public string TenDangNhap { get; set; }

        [Required]
        public string MatKhau { get; set; }

        public string? VaiTro { get; set; } // "Admin" hoặc "User"
    }
}