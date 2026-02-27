namespace ciftlik2.Models
{
    public class IstatistikViewModel
    {
        // 1. Genel Sayımlar
        public int ToplamHayvan { get; set; }
        public int ToplamAsiKaydi { get; set; }
        public int ToplamTestKaydi { get; set; }
        public int ToplamTedaviKaydi { get; set; }

        // 2. Hayvan İstatistikleri
        public double OrtalamaYas { get; set; }
        public Dictionary<string, int>? CinslereGoreHayvanSayisi { get; set; }

        // 3. Sağlık İstatistikleri
        public string? EnSikKullanilanAsi { get; set; }
        public string? EnSikTeshis { get; set; }
    }
}