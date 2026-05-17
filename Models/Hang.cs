using System;
using System.Collections.Generic;

namespace LittleHairSalon.Models;

public partial class Hang
{
    public int MaHang { get; set; }

    public string TenHang { get; set; } = null!;

    public decimal? Gia { get; set; }

    public string? HinhAnh { get; set; }

    public string? MoTa { get; set; }

    public int? MaLoai { get; set; }

    public virtual ICollection<ChiTietDon> ChiTietDons { get; set; } = new List<ChiTietDon>();

    public virtual Loai? MaLoaiNavigation { get; set; }
}
