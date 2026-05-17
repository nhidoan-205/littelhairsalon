using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleHairSalon.Models
{
    [Table("DonHang")]
    public partial class DonHang
    {
        [Key]

        [Column("MaDH")]
        public int MaDon { get; set; }
        public DateTime? NgayDat { get; set; }

        public string? NguoiNhan { get; set; }
        public string? Sdt { get; set; }
        public string? DiaChi { get; set; }
        public decimal? TongTien { get; set; }

        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public string? PTThanhToan { get; set; }
        public int? MaKh { get; set; }

        [NotMapped]
        public virtual ICollection<ChiTietDon> ChiTietDons { get; set; } = new List<ChiTietDon>();
        public int MaDonHang { get; internal set; }
    }
}