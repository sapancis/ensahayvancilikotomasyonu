using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models.Enums
{
    public enum FinansKategorisi
    {
        [Display(Name = "Diğer / Genel")]
        Diger = 0,
        [Display(Name = "Elektrik Faturası")]
        Elektrik = 1,
        [Display(Name = "Su Faturası")]
        Su = 2,
        [Display(Name = "Personel Maaşı")]
        Maas = 3,
        [Display(Name = "Mazot / Yakıt")]
        Yakit = 4,
        [Display(Name = "Bakım Onarım")]
        Bakim = 5,
        [Display(Name = "Teşvik / Hibe (Gelir)")]
        Tesvik = 6,
        [Display(Name = "Gübre Satışı (Gelir)")]
        GubreSatisi = 7
    }
}