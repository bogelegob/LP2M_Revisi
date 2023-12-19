using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LP2M_Revisi.Models;

public partial class Surattuga
{
    public string Id { get; set; } = null!;
    public string? Namafile { get; set; }
    public string? Namafilesurat { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? tanggalselesai { get; set; }
    [MaxLength(200)]
    [Required(ErrorMessage = "Nama Kegiatan harus diisi.")]
    public string? Namakegiatan { get; set; }
    public string? Keterangan { get; set; }
    [Required(ErrorMessage = "Masa pelaksanaan harus diisi.")]
    public string? Masapelaksanaan { get; set; }
    /*[MaxLength()]*/
    [Required(ErrorMessage = "Bukti Pendukung harus diisi.")]
    public byte[]? Buktipendukung { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Inputdate { get; set; }

    public string? Editby { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Editdate { get; set; }
    public byte[]? Surattugas { get; set; }

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }

    public virtual ICollection<Pengguna> Idpenggunas { get; set; } = new List<Pengguna>();
}
