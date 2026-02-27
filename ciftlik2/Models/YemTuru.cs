using System.ComponentModel.DataAnnotations; // [Display] etiketi için bu gerekli

namespace ciftlik2.Models
{
    public enum YemTuru
    {
        [Display(Name = "Yaş Yonca")]
        YasYonca = 1,

        [Display(Name = "Kuru Ot")]
        KuruOt = 2,

        [Display(Name = "Silaj Mısır")]
        SilajMisir = 3,

        [Display(Name = "Hayvan Pancarı")]
        HayvanPancari = 4,

        [Display(Name = "Dane Yem")]
        DaneYem = 5,

        [Display(Name = "Arpa")]
        Arpa = 6,

        [Display(Name = "TMR (Tam Rasyon)")]
        TMR = 7
    }
}