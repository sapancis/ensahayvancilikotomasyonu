using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    public class TopluAsiViewModel
    {
        [Required(ErrorMessage = "Aşı Adı zorunludur.")]
        [Display(Name = "Aşı Adı")]
        public string AsiIsmi { get; set; }

        [Required(ErrorMessage = "Aşı Tarihi zorunludur.")]
        [Display(Name = "Aşı Tarihi")]
        public DateTime? AsiTarihi { get; set; }

        // Dozu alanı buradan da silindi
    }
}