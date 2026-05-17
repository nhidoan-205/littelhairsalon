using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm thư viện này

namespace LittleHairSalon.Models
{
    [Table("DichVus")] // Khớp chính xác với tên bảng trong SQL của bạn
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [MaxLength(100)]
        public string TenDichVu { get; set; }

        public string? MoTa { get; set; } // Cho phép null để tránh lỗi nếu DB để trống

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public decimal Gia { get; set; }

        public int ThoiGianUocTinh { get; set; }

        public string? AnhDichVu { get; set; } // Cho phép null
    }
}