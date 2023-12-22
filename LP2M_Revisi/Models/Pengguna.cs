using System;
using System.Collections.Generic;

namespace LP2M_Revisi.Models;

public partial class Pengguna
{
    public string Id { get; set; } = null!;

    public string? Nama { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? Email { get; set; }

    public string? Notelepon { get; set; }

    public int? Prodi { get; set; }

    public virtual ICollection<Buku> BukuEditbyNavigations { get; set; } = new List<Buku>();

    public virtual ICollection<Buku> BukuInputbyNavigations { get; set; } = new List<Buku>();

    public virtual ICollection<Detailbuku> Detailbukus { get; set; } = new List<Detailbuku>();
    public virtual ICollection<Detailhakcipta> Detailhakciptas { get; set; } = new List<Detailhakcipta>();
    public virtual ICollection<Detailhakpaten> Detailhakpatens { get; set; } = new List<Detailhakpaten>();
    public virtual ICollection<Detailjurnal> Detailjurnals { get; set; } = new List<Detailjurnal>();
    public virtual ICollection<Detailpengabdian> Detailpengabdians { get; set; } = new List<Detailpengabdian>();
    public virtual ICollection<Detailprosiding> Detailprosidings { get; set; } = new List<Detailprosiding>();
    public virtual ICollection<Detailseminar> Detailseminars { get; set; } = new List<Detailseminar>();
    public virtual ICollection<Detailsurat> Detailsurats { get; set; } = new List<Detailsurat>();

    public virtual ICollection<Hakciptum> HakciptumEditbyNavigations { get; set; } = new List<Hakciptum>();

    public virtual ICollection<Hakciptum> HakciptumInputbyNavigations { get; set; } = new List<Hakciptum>();

    public virtual ICollection<Hakpaten> HakpatenEditbyNavigations { get; set; } = new List<Hakpaten>();

    public virtual ICollection<Hakpaten> HakpatenInputbyNavigations { get; set; } = new List<Hakpaten>();

    public virtual ICollection<Jurnal> JurnalEditbyNavigations { get; set; } = new List<Jurnal>();

    public virtual ICollection<Jurnal> JurnalInputbyNavigations { get; set; } = new List<Jurnal>();

    public virtual ICollection<Pengabdianmasyarakat> PengabdianmasyarakatEditbyNavigations { get; set; } = new List<Pengabdianmasyarakat>();

    public virtual ICollection<Pengabdianmasyarakat> PengabdianmasyarakatInputbyNavigations { get; set; } = new List<Pengabdianmasyarakat>();

    public virtual Prodi? ProdiNavigation { get; set; }

    public virtual ICollection<Prosiding> ProsidingEditbyNavigations { get; set; } = new List<Prosiding>();

    public virtual ICollection<Prosiding> ProsidingInputbyNavigations { get; set; } = new List<Prosiding>();

    public virtual ICollection<Seminar> SeminarEditbyNavigations { get; set; } = new List<Seminar>();

    public virtual ICollection<Seminar> SeminarInputbyNavigations { get; set; } = new List<Seminar>();

    public virtual ICollection<Surattuga> SurattugaEditbyNavigations { get; set; } = new List<Surattuga>();

    public virtual ICollection<Surattuga> SurattugaInputbyNavigations { get; set; } = new List<Surattuga>();

   /* public virtual ICollection<Prosiding> IdProsidings { get; set; } = new List<Prosiding>();

    public virtual ICollection<Hakciptum> Idhakcipta { get; set; } = new List<Hakciptum>();

    public virtual ICollection<Hakpaten> Idhakpatens { get; set; } = new List<Hakpaten>();

    public virtual ICollection<Jurnal> Idjurnals { get; set; } = new List<Jurnal>();

    public virtual ICollection<Pengabdianmasyarakat> Idpengabdians { get; set; } = new List<Pengabdianmasyarakat>();

    public virtual ICollection<Seminar> Idseminars { get; set; } = new List<Seminar>();

    public virtual ICollection<Surattuga> Idsurattugas { get; set; } = new List<Surattuga>();*/
    public virtual ICollection<Pengaduan> PenggunaPengaduanNavigations { get; set; } = new List<Pengaduan>();
}
