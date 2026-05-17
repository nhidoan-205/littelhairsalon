using System.ComponentModel.DataAnnotations;

namespace LittleHairSalon.Models
{
    public partial class NhanVien
    {
        [Key]//
        public int MaNhanVien { get; set; }
        public string? TenNhanVien { get; set; }

        // THÊM DÒNG NÀY VÀO
        public string? Email { get; set; }

        public string? ChucVu { get; set; }
        public string? AnhDaiDien { get; set; }
        public string? KinhNghiem { get; set; }

        public int TrangThai { get; set; }
    }
}