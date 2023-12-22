using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Hakpaten
{
    public string Id { get; set; } = null!;

    public string? Judul { get; set; }

    public string? Nopermohonan { get; set; }

    public DateTime? TanggalPenerimaan { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }

    public DateTime? Inputdate { get; set; }

    public string? Editby { get; set; }

    public DateTime? Editdate { get; set; }

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }
    public virtual ICollection<Pengaduan> HakPatenPengaduanNavigations { get; set; } = new List<Pengaduan>();
    public virtual ICollection<Detailhakpaten> Detailhakpatens { get; set; } = new List<Detailhakpaten>();
}
