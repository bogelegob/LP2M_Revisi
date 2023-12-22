namespace LP2M_Revisi.Models
{
    public class Detailprosiding
    {
        public string Idprosiding { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Prosiding IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
