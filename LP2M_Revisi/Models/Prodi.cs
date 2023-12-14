using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Prodi
{
    public int Id { get; set; }

    public string? Nama { get; set; }

    public virtual ICollection<Pengguna> Penggunas { get; set; } = new List<Pengguna>();
}
