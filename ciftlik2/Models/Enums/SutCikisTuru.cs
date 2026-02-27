using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models.Enums
{
    public enum SutCikisTuru
    {
        [Display(Name = "Süt Satışı")]
        Satis = 1,

        [Display(Name = "Buzağı Besleme")]
        BuzagiBesleme = 2,

        [Display(Name = "Ev Tüketimi / Personel")]
        EvTuketimi = 3,

        [Display(Name = "Peynir/Yoğurt Yapımı")]
        Isleme = 4,

        [Display(Name = "Zayi / Dökülen")]
        Zayi = 5
    }
}