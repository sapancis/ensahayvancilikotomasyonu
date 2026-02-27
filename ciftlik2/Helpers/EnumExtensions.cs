using System.ComponentModel.DataAnnotations;
using System.Reflection; // Bu using gerekli

namespace ciftlik2.Helpers
{
    // Enum'ların [Display] etiketini okumak için yardımcı sınıf
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                                .GetMember(enumValue.ToString())
                                .First() // Üyeyi al
                                         // [Display] etiketini al
                                .GetCustomAttribute<DisplayAttribute>()
                                // 'Name' özelliğini oku
                                ?.Name
                                // Eğer [Display] etiketi yoksa, enum'ın kendi adını döndür
                                ?? enumValue.ToString();
            }
            catch (Exception)
            {
                // Bir hata olursa (örn: enum üyesi bulunamazsa)
                // en azından ham veriyi göster
                return enumValue.ToString();
            }
        }
    }
}