using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ciftlik2.Models
{
    public class Tohumlanma
    {
        [Key]
        public int Id { get; set; }

        // --- MEVCUT ALANLAR ---
        [Required(ErrorMessage = "Dişi hayvan seçimi zorunludur.")]
        [Display(Name = "Dişi Hayvan")]
        public int DisiHayvanId { get; set; }

        [ForeignKey("DisiHayvanId")]
        [ValidateNever]
        public Hayvan? DisiHayvan { get; set; }

        [Display(Name = "Erkek Hayvan")]
        public int? ErkekHayvanId { get; set; }

        [ForeignKey("ErkekHayvanId")]
        [ValidateNever]
        public Hayvan? ErkekHayvan { get; set; }

        [Required(ErrorMessage = "Tohumlanma tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Tohumlanma Tarihi")]
        public DateTime TohumlanmaTarihi { get; set; }

        [Required(ErrorMessage = "Tohumlanma türü seçimi zorunludur.")]
        [Display(Name = "Tohumlanma Türü")]
        public TohumlanmaTuru TohumlanmaTuru { get; set; }

        [Display(Name = "Gebelik Testi Sonucu")]
        public GebelikSonucu? GebelikTestiSonucu { get; set; }

        [Display(Name = "Gebelik Test Durumu")]
        public TestDurumu GebelikTestDurumu { get; set; }

        // ============================================================
        // YENİ EKLENEN AKILLI HESAPLAMALAR (Veritabanında yer tutmaz)
        // ============================================================

        // 1. Tahmini Doğum Tarihi: İneklerde gebelik ortalama 280 gün sürer.
        [NotMapped]
        public DateTime TahminiDogumTarihi => TohumlanmaTarihi.AddDays(280);

        // 2. Kuruya Alma Tarihi: Doğumdan yaklaşık 60 gün önce (220. gün)
        [NotMapped]
        public DateTime KuruyaAlmaTarihi => TohumlanmaTarihi.AddDays(220);

        // 3. Doğuma Kalan Gün Sayısı
        [NotMapped]
        public int DogumaKalanGun => (TahminiDogumTarihi - DateTime.Today).Days;

        // 4. Kuruya Almaya Kalan Gün Sayısı
        [NotMapped]
        public int KuruyaAlmayaKalanGun => (KuruyaAlmaTarihi - DateTime.Today).Days;

        // 5. Durum Mesajı (Bildirim için)
        [NotMapped]
        public string DurumBilgisi
        {
            get
            {
                if (GebelikTestiSonucu != GebelikSonucu.Gebe) return "Beklemede/Negatif";

                if (KuruyaAlmayaKalanGun == 1) return "⚠️ DİKKAT: Yarın kuruya alınacak!";
                if (KuruyaAlmayaKalanGun <= 0 && DogumaKalanGun > 0) return "🟢 Şu an Kuruda (Doğum Bekleniyor)";
                if (DogumaKalanGun <= 20 && DogumaKalanGun > 0) return $"🔥 KRİTİK: Doğuma {DogumaKalanGun} gün kaldı!";
                if (DogumaKalanGun <= 0) return "❗ Doğum Tarihi Geldi/Geçti!";

                return "Gebe - Süreç Normal";
            }
        }
        // ... diğer propertylerin altına ...
        public GebelikSonlanmaTuru SonlanmaTuru { get; set; } = GebelikSonlanmaTuru.DevamEdiyor;
    }
}