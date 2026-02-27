// Models/Enums/TestDurumu.cs
using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models.Enums
{
    public enum TestDurumu
    {
        [Display(Name = "Yapılmadı")]
        Yapilmadi = 0,
        [Display(Name = "Yapıldı")]
        Yapildi = 1
    }
}