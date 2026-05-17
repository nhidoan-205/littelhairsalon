namespace LittleHairSalon.Models
{
    public class GioHangItem
    {
        public int MaHang { get; set; }
        public string TenHang { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}