﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LP2M_Revisi.Models;

public partial class Pengabdianmasyarakat
{
    public string Id { get; set; } = null!;

    public string? Namakegiatan { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? Waktupelaksanaan { get; set; }

    public int? Jumlahpenerima { get; set; }

    public byte[]? Surattugas { get; set; }

    public byte[]? Laporan { get; set; }

    public byte[]? Buktipendukung { get; set; }

    public string? MahasiswaProdiNim { get; set; }
    public string? Namafilesurat { get; set; }
    public string? Namafilebukti { get; set; }
    public string? Namafilelaporan { get; set; }

    public int? Status { get; set; }

    public string? Inputby { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Inputdate { get; set; }

    public string? Editby { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Editdate { get; set; }

    public virtual Pengguna? EditbyNavigation { get; set; }

    public virtual Pengguna? InputbyNavigation { get; set; }

    public virtual ICollection<Pengguna> Idpenggunas { get; set; } = new List<Pengguna>();
}
