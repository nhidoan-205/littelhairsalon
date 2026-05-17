using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleHairSalon.Models
{
    public class LichDat
    {
        [Key]
        public int MaLichDat { get; set; }

        // Liên kết với người đặt (Sửa lại tên KhachHang cho đúng với Model cũ của bạn)
        public int? MaKhachHang { get; set; }
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; }

        // Liên kết với Dịch vụ khách chọn
        public int MaDichVu { get; set; }
        [ForeignKey("MaDichVu")]
        public virtual DichVu DichVu { get; set; }

        // Liên kết với Thợ (Có thể null nếu khách vào chọn bừa, không yêu cầu thợ)
        public int? MaNhanVien { get; set; }
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien NhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        [DataType(DataType.Date)]
        public DateTime? NgayDat { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ")]
        [DataType(DataType.Time)]
        public TimeSpan? GioDat { get; set; }

        // Quy ước: 0 - Chờ xác nhận, 1 - Đã xác nhận, 2 - Hoàn thành, 3 - Đã hủy
        public int TrangThai { get; set; } = 0;

        [MaxLength(255)]
        public string GhiChu { get; set; }

        public DateTime NgayTaoLich { get; set; } = DateTime.Now;
    }
}