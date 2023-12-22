using System.ComponentModel.DataAnnotations;

namespace LP2M_Revisi.Models
{
    public class Pengaduan
    {
        public int? Id { get; set; }
        public string? pengguna { get; set; }
        public string? buku { get; set; }
        public string? prosiding { get; set; }
        public string? hakcipta { get; set; }
        public string? hakpaten { get; set; }
        public string? jurnal { get; set; }
        public string? seminar { get; set; }
        public string? pengabdian { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? createdate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? updatedate { get; set; }
        public int? Status { get; set; }
        public string? Keterangan { get; set; }
        public virtual Pengguna? PenggunaNavigation { get; set; }
        public virtual Prosiding? ProsidingNavigation { get; set; }
        public virtual Seminar? SeminarNavigation { get; set; }
        public virtual Jurnal? JurnalNavigation { get; set; }
        public virtual Hakciptum? HakciptaNavigation { get; set; }
        public virtual Hakpaten? HakpatenNavigation { get; set; }
        public virtual Pengabdianmasyarakat? PengabdianmasyarakatNavigation { get; set; }
        public virtual Buku? BukuNavigation { get; set; }
        
    }
}
