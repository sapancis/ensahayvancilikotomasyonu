// Models/Enums/TohumlanmaTuru.cs
using System.ComponentModel.DataAnnotations;

namespace ciftlik2.Models.Enums
{
    public enum TohumlanmaTuru
    {
        [Display(Name = "Doğal Aşım")]
        DogalAsim = 0,
        [Display(Name = "Suni Tohumlama")]
        SuniTohumlama = 1
    }
}