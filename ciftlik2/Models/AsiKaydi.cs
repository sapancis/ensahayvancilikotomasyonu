using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    public class AsiKaydi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Aşı ismi zorunludur.")]
        [Display(Name = "Aşı İsmi")]
        public string? AsiIsmi { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [Display(Name = "Uygulama Tarihi")]
        public DateTime? AsiTarihi { get; set; }

        // Doz alanı silindi

        public int HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }
    }
}