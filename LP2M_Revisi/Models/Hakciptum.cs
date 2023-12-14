using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Hakciptum
{
    public string Id { get; set; } = null!;

    public string? Judul { get; set; }

    public string? Noaplikasi { get; set; }

    public string? Nosertifikat { get; set; }

    public string? Keterangan { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }

    public DateTime? Inputdate { get; set; }

    public string? Editby { get; set; }

    public DateTime? Editdate { get; set; }

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }

    public virtual ICollection<Pengguna> Idpenggunas { get; set; } = new List<Pengguna>();
}
