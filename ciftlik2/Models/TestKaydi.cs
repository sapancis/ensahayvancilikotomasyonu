using System.ComponentModel.DataAnnotations.Schema;

namespace ciftlik2.Models
{
    public class TestKaydi
    {
        public int Id { get; set; }
        public string? TestIsmi { get; set; }
        public DateTime? TestTarihi { get; set; }
        public string? Sonuc { get; set; } // örn: "Pozitif"

        // --- İlişki: Hangi hayvana ait? ---
        // ...
        public int HayvanId { get; set; }
        [ForeignKey("HayvanId")]
        public Hayvan? Hayvan { get; set; } // '?' EKLENDİ
                                            // ...        [ForeignKey("HayvanId")]

    }
}