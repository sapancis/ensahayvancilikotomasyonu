using ciftlik2.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ciftlik2.Models
{
    public class FinansIslemi
    {
        public int Id { get; set; }

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Today;

        public string? Aciklama { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Tutar { get; set; }

        public IslemTuru Tur { get; set; } // Gelir mi Gider mi?
        public FinansKategorisi Kategori { get; set; }
    }
}