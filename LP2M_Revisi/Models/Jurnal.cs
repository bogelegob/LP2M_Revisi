using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Jurnal
{
    public string Id { get; set; } = null!;
    [MaxLength(200)]
    [Required(ErrorMessage = "Judul Makalah harus diisi.")]
    public string? Judulmakalah { get; set; }

    [MaxLength(200)]
    [Required(ErrorMessage = "Nama Jurnal harus diisi.")]

    public string? Namajurnal { get; set; }
    [Required(ErrorMessage = "ISBN harus diisi.")]
    [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Format ISBN tidak valid.")]

    public string? Issn { get; set; }
    [Required(ErrorMessage = "Volume harus diisi.")]
    public int? Volume { get; set; }
    [Required(ErrorMessage = "Nomer harus diisi.")]
    public int? Nomor { get; set; }
    [Required(ErrorMessage = "Halaman Awal harus diisi.")]
    public int? Halamanawal { get; set; }
    [Required(ErrorMessage = "Halaman Akhir harus diisi.")]
    public int? Halamanakhir { get; set; }
    [Required(ErrorMessage = "Url harus diisi.")]
    public string? Url { get; set; }
    [Required(ErrorMessage = "Kategori harus pilih.")]
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
