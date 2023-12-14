using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Detailbuku
{
    public string Idbuku { get; set; } = null!;

    public string Idpengguna { get; set; } = null!;

    public string? Status { get; set; }

    public virtual Buku IdbukuNavigation { get; set; } = null!;

    public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
}
