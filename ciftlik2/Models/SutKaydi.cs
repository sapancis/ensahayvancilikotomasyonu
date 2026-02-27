using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    public class SutKaydi
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sağım Tarihi")]
        public DateTime Tarih { get; set; }

        [Required]
        [Display(Name = "Sağılan İnek Sayısı")]
        [Range(1, 10000, ErrorMessage = "En az 1 inek sağılmış olmalıdır.")]
        public int SagilanHayvanSayisi { get; set; }

        [Required]
        [Display(Name = "Toplam Miktar (Litre)")]
        [Range(0.1, 10000, ErrorMessage = "Geçerli bir miktar giriniz.")]
        public double Miktar { get; set; }

        [Display(Name = "Sağım Vakti")]
        public SagimVakti Vakit { get; set; }
    }

    public enum SagimVakti { Sabah = 1, Aksam = 2, Diger = 3 }
}