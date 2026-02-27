using ciftlik2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ciftlik2.Models
{
    public class Hayvan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Küpe Numarası zorunludur.")]
        [Display(Name = "Küpe Numarası")]
        public string KupeNumarasi { get; set; }

        [NotMapped]
        public string TamKupeNumarasi => KupeNumarasi;

        public string? Ad { get; set; }
        public string? Cins { get; set; }

        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        public DateTime? DogumTarihi { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
        public Cinsiyet Cinsiyet { get; set; }

        [Display(Name = "Laktasyon Sayısı")]
        public int LaktasyonSayisi { get; set; } = 0;

        [Display(Name = "Kısırlaştırıldı mı?")]
        public bool Kisirlastirildi { get; set; } = false;

        // --- YAŞ BİLGİSİ (SADECE AY CİNSİNDEN) ---
        [NotMapped]
        public string YasBilgisi
        {
            get
            {
                if (!DogumTarihi.HasValue) return "-";

                var bugun = DateTime.Today;
                // Toplam ay farkını hesapla
                var ayFarki = ((bugun.Year - DogumTarihi.Value.Year) * 12) + bugun.Month - DogumTarihi.Value.Month;

                // Gün henüz dolmadıysa 1 ay düş
                if (bugun.Day < DogumTarihi.Value.Day) ayFarki--;

                if (ayFarki < 0) ayFarki = 0;

                return $"{ayFarki} Aylık";
            }
        }

        // --- GRUPLAMA MANTIĞI ---
        [NotMapped]
        public string HayvanGrubu
        {
            get
            {
                if (!DogumTarihi.HasValue) return "Bilinmiyor";

                var yasAy = (DateTime.Today - DogumTarihi.Value).TotalDays / 30.44;

                if (yasAy < 6) return Cinsiyet == Cinsiyet.Disi ? "Dişi Buzağı" : "Erkek Buzağı";
                if (yasAy >= 6 && yasAy < 12) return "Dana";

                if (Cinsiyet == Cinsiyet.Disi)
                {
                    if (LaktasyonSayisi == 0) return "Düve";
                    return "İnek";
                }
                else
                {
                    if (Kisirlastirildi && yasAy >= 12) return "Öküz";
                    if (yasAy >= 12 && yasAy < 24) return "Tosun";
                    return "Boğa";
                }
            }
        }

        public GirisSekli GirisSekli { get; set; }
        public string? BabaKupeNumarasi { get; set; }
        public string? AnaKupeNumarasi { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AlisFiyati { get; set; }
        public DateTime? AlisTarihi { get; set; }
        // Mevcut property'lerin altına ekle:
        [Display(Name = "Hikaye / Notlar")]
        public string? Hikaye { get; set; }

        // GeldigiYer alanı SİLİNDİ.

        public bool Aktif { get; set; }
        public CikisNedeni? CikisNedeni { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Satış Fiyatı")]
        public decimal? SatisFiyati { get; set; } // Sürüden çıkarken kaça satıldı?

        public ICollection<AsiKaydi> AsiKayitlari { get; set; }
        public ICollection<TestKaydi> TestKayitlari { get; set; }
        public ICollection<TedaviKaydi> TedaviKayitlari { get; set; }

        [InverseProperty("DisiHayvan")]
        public ICollection<Tohumlanma> Tohumlanmalar { get; set; }

        public Hayvan()
        {
            Aktif = true;
            GirisSekli = GirisSekli.Dogum;
            AsiKayitlari = new HashSet<AsiKaydi>();
            TestKayitlari = new HashSet<TestKaydi>();
            TedaviKayitlari = new HashSet<TedaviKaydi>();
            Tohumlanmalar = new HashSet<Tohumlanma>();
        }
    }
}