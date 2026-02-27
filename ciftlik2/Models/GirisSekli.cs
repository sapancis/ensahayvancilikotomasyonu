using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models
{
    public enum GirisSekli
    {
        [Display(Name = "Bilinmiyor")]
        None = 0,

        [Display(Name = "Doğum")]
        Dogum = 1,

        [Display(Name = "Satın Alma")]
        SatinAlma = 2

        // Transfer = 3 satırı buradan kaldırıldı.
    }
}