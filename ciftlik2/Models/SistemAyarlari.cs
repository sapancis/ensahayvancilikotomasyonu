using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    public class SistemAyarlari
    {
        public int Id { get; set; }

        // --- 1. GENEL ÇİFTLİK AYARLARI ---
        [Display(Name = "Çiftlik Adı")]
        [Required(ErrorMessage = "Çiftlik adı boş bırakılamaz.")]
        [MaxLength(50)]
        public string CiftlikAdi { get; set; } = "Çiftlik Yönetim Sistemi";

        [Display(Name = "Çiftlik Sahibi / Yönetici")]
        [MaxLength(50)]
        public string? CiftlikSahibi { get; set; }

        // --- 2. ÜREME VE DOĞUM AYARLARI ---
        [Display(Name = "Gebelik Süresi (Gün)")]
        [Range(260, 300)]
        public int StandartGebelikSuresi { get; set; } = 280; // İnekler için ortalama

        [Display(Name = "Kuruya Alma Zamanı (Gün)")]
        public int KuruyaAlmaGunu { get; set; } = 220; // 280-60
        [Display(Name = "Doğum Uyarı Başlangıcı (Gün)")]
        [Range(1, 60)]
        public int DogumOncesiBildirimGunu { get; set; } = 15; // Varsayılan: 15 gün kala uyar
        // --- 3. KIZGINLIK AYARLARI ---
        [Display(Name = "Kızgınlık Başlangıç (Gün)")]
        public int KizginlikBaslangicGun { get; set; } = 18;

        [Display(Name = "Kızgınlık Bitiş (Gün)")]
        public int KizginlikBitisGun { get; set; } = 23;

        // --- 4. KONTROL AYARLARI ---
        [Display(Name = "Gebelik Testi Zamanı (Gün)")]
        public int GebelikTestiGun { get; set; } = 40;
        [Display(Name = "Buzağı Sütten Kesme (Gün)")]
        public int BuzagiSuttenKesmeGunu { get; set; } = 70; // Varsayılan 70 gün
    }
}