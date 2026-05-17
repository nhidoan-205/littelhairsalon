using System;
using System.Collections.Generic;

namespace LittleHairSalon.Models;

public partial class Loai
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Hang> Hangs { get; set; } = new List<Hang>();
}
