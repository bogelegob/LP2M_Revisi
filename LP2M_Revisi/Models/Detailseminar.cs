namespace LP2M_Revisi.Models
{
    public class Detailseminar
    {
        public string Idseminar { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Seminar IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
