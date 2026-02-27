namespace ciftlik2.Models
{
    public class FinansRaporViewModel
    {
        // Kartlar için Özetler
        public decimal ToplamGelir { get; set; }
        public decimal ToplamGider { get; set; }
        public decimal NetKar { get; set; }

        // Grafik Verileri (JSON formatında View'a gidecek)
        public decimal GiderYem { get; set; }
        public decimal GiderTedavi { get; set; }
        public decimal GiderHayvanAlim { get; set; }
        public decimal GiderGenel { get; set; } // Fatura, maaş vb.

        public decimal GelirSut { get; set; }
        public decimal GelirHayvanSatis { get; set; }
        public decimal GelirGenel { get; set; }

        // Son Hareketler Listesi (Karma)
        public List<FinansSatiri> SonHareketler { get; set; }
    }

    // Listede göstermek için basit yardımcı sınıf
    public class FinansSatiri
    {
        public DateTime Tarih { get; set; }
        public string Tur { get; set; } // "Süt Satışı", "Yem Alımı" vs.
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public bool GelirMi { get; set; } // Renklendirme için (Yeşil/Kırmızı)
    }
}