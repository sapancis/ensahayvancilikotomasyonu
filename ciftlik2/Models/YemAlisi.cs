using System.ComponentModel.DataAnnotations.Schema; // [Column] için

namespace ciftlik2.Models
{
    public class YemAlisi
    {
        public int Id { get; set; }

        // Enum'dan gelen yem türü
        public YemTuru YemTuru { get; set; }

        // Parasal değerler için 'decimal' kullanıyoruz
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BirimFiyat { get; set; }

        // Miktar (örn: kg, ton, adet)
        public double Miktar { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ToplamFiyat { get; set; }

        // Kaydın ne zaman eklendiğini bilmek her zaman iyidir
        public DateTime AlisTarihi { get; set; }

        public YemAlisi()
        {
            // Yeni kayıt oluşturulduğunda tarihi otomatik olarak ayarla
            AlisTarihi = DateTime.Now;
        }
    }
}