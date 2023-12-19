using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Buku
{
    public string Id { get; set; } = null!;
    [MaxLength(200)]
    public string? Judulbuku { get; set; }

    public string? Isbn { get; set; }

    public string? Penerbit { get; set; }

    public int? Tahun { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }
    [DisplayFormat(DataFormatString = "{0:yyyy-MMM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Inputdate { get; set; }
    public string? Editby { get; set; }
    [DisplayFormat(DataFormatString = "{0:yyyy-MMM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Editdate { get; set; }

    public virtual ICollection<Detailbuku> Detailbukus { get; set; } = new List<Detailbuku>();
    public virtual ICollection<Pengaduan> BukuPengaduanNavigations { get; set; } = new List<Pengaduan>();

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }
}
