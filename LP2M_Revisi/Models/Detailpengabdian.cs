namespace LP2M_Revisi.Models
{
    public class Detailpengabdian
    {
        public string Idpengabdian { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Pengabdianmasyarakat IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
