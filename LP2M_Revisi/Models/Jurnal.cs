using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Jurnal
{
    public string Id { get; set; } = null!;

    public string? Judulmakalah { get; set; }

    public string? Namajurnal { get; set; }

    public string? Issn { get; set; }

    public int? Volume { get; set; }

    public int? Nomor { get; set; }

    public int? Halamanawal { get; set; }

    public int? Halamanakhir { get; set; }

    public string? Url { get; set; }

    public string? Kategori { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }

    public DateTime? Inputdate { get; set; }

    public string? Editby { get; set; }

    public DateTime? Editdate { get; set; }

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }

    public virtual ICollection<Pengguna> Idpenggunas { get; set; } = new List<Pengguna>();
    public virtual ICollection<Pengaduan> JurnalPengaduanNavigations { get; set; } = new List<Pengaduan>();
}
