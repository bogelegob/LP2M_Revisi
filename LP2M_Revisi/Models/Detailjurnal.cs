namespace LP2M_Revisi.Models
{
    public class Detailjurnal
    {
        public string Idjurnal { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Jurnal IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
