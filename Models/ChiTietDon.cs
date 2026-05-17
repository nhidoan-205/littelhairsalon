using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleHairSalon.Models;

public partial class ChiTietDon
{
    public int MaChiTiet { get; set; }

    [Column("MaDH")]
    public int? MaDon { get; set; }

    public int? MaHang { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public virtual DonHang? MaDonNavigation { get; set; }

    public virtual Hang? MaHangNavigation { get; set; }
}
