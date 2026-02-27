// Models/Enums/GebelikSonucu.cs
using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models.Enums
{
    public enum GebelikSonucu
    {
        [Display(Name = "Gebe Değil")]
        GebeDegil = 0,
        [Display(Name = "Gebe")]
        Gebe = 1
    }
}