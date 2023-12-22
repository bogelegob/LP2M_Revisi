namespace LP2M_Revisi.Models
{
    public class Detailsurat
    {
        public string Idsurat { get; set; } = null!;

        public string Idpengguna { get; set; } = null!;

        public virtual Surattuga IdbukuNavigation { get; set; } = null!;

        public virtual Pengguna IdpenggunaNavigation { get; set; } = null!;
    }
}
