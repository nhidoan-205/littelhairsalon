using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm dòng này để dùng thẻ [Table]

namespace LittleHairSalon.Models
{
    [Table("DanhGia")] 
    public class DanhGia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int MaDanhGia { get; set; }

        public int? MaKhachHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
        public string NoiDung { get; set; } = "";

        [Range(1, 5)]
        public int SoSao { get; set; } = 5;

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public string TrangThai { get; set; } = "Hien thi";
    }
}