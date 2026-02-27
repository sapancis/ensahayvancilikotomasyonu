using ciftlik2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ciftlik2.Models
{
    public class SutCikisi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Çıkış türü seçilmelidir.")]
        [Display(Name = "Çıkış Türü")]
        public SutCikisTuru CikisTuru { get; set; }

        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(0.1, 10000, ErrorMessage = "Geçerli bir miktar giriniz.")]
        [Display(Name = "Miktar (Litre)")]
        public double Miktar { get; set; }

        // Sadece 'Satış' ise doldurulacak
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Litre Fiyatı (TL)")]
        public decimal BirimFiyat { get; set; } = 0;

        // Otomatik hesaplanacak (Miktar * BirimFiyat)
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Toplam Tutar")]
        public decimal ToplamTutar { get; set; } = 0;

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; } // Örn: "Mandıraya verildi", "Sabah beslemesi"

        public SutCikisi()
        {
            Tarih = DateTime.Today;
        }
    }
}