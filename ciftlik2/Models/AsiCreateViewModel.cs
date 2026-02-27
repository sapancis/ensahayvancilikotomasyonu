using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    // Aşı ekleme formu için özel ViewModel
    public class AsiCreateViewModel
    {
        // Kullanıcı bu alanı dolduracak
        [Required(ErrorMessage = "Küpe Numarası zorunludur.")]
        [Display(Name = "Hayvan Küpe Numarası")]
        public string? KupeNumarasi { get; set; }

        // AsiKaydi modelindeki diğer alanlar
        [Required(ErrorMessage = "Aşı Adı zorunludur.")]
        [Display(Name = "Aşı Adı")]
        public string? AsiIsmi { get; set; }

        [Display(Name = "Aşı Tarihi")]
        public DateTime? AsiTarihi { get; set; }

        public string? Dozu { get; set; }
    }
}