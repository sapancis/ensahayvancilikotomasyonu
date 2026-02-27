using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    public class Kullanici
    {
        public int Id { get; set; }

        [Required]
        public string? KullaniciAdi { get; set; }

        [Required]
        public string? Sifre { get; set; }
    }
}