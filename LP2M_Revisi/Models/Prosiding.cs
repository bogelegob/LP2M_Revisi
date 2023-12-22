using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Prosiding
{
    public string Id { get; set; } = null!;

    public string? Judulprogram { get; set; }

    public string? Judulpaper { get; set; }

    public string? Kategori { get; set; }

    public string? Penyelenggara { get; set; }

    public DateTime? Waktuterbit { get; set; }

    public string? Tempatpelaksanaan { get; set; }

    public string? Keterangan { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }

    public DateTime? Inputdate { get; set; }

    public string? Editby { get; set; }

    public DateTime? Editdate { get; set; }

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }
    public virtual ICollection<Pengaduan> ProsidingPengaduanNavigations { get; set; } = new List<Pengaduan>();
    public virtual ICollection<Detailprosiding> Detailprosidings { get; set; } = new List<Detailprosiding>();
}
