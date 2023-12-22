namespace LP2M_Revisi.Models
{
    public class Detailhakpaten
    {
        public string Idhakpaten { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Hakpaten IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
