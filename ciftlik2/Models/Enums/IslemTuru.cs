using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models.Enums
{
    public enum IslemTuru
    {
        [Display(Name = "Gelir")]
        Gelir = 1,
        [Display(Name = "Gider")]
        Gider = 2
    }
}