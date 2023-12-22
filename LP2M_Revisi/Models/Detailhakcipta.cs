namespace LP2M_Revisi.Models
{
    public class Detailhakcipta
    {
        public string Idhakcipta { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Hakciptum IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
