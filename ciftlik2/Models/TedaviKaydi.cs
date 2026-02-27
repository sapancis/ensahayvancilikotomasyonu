using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ciftlik2.Models
{
    public class TedaviKaydi
    {
        public int Id { get; set; }
        public string? Teshis { get; set; }
        public string? KullanilanIlac { get; set; }
        public DateTime? SutArinmaTarihi { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Tedavi Maliyeti (İlaç+Hekim)")]
        public decimal TedaviUcreti { get; set; } = 0;
        public string? ReceteyiYazanVeteriner { get; set; }
        public string? DenetleyenVeteriner { get; set; }


        public int HayvanId { get; set; }
        [ForeignKey("HayvanId")]
        public Hayvan? Hayvan { get; set; } // '?' EKLENDİ
                                            // ...
    }
}