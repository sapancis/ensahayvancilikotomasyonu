using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Bu satırı ekleyin

namespace ciftlik2.Controllers
{
    public class AccountController : Controller
    {
        // 1. Veritabanı bağlantısı için context'i ekleyin
        private readonly CiftlikDbContext _context;

        // 2. Constructor'ı güncelleyin (DbContext'i enjekte edin)
        public AccountController(CiftlikDbContext context)
        {
            _context = context;
        }

        // --- Login sayfasını göster (GET) ---
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // --- Formdan gelen veriyi işle (POST) ---
        [HttpPost]
        public async Task<IActionResult> Login(Kullanici model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // --- VERİTABANI KONTROLÜ ---
                // Kullanıcıyı, kullanıcı adına ve şifreye göre veritabanında ara
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi &&
                                            k.Sifre == model.Sifre);

                // Eğer 'kullanici' null değilse (yani bulunduysa)
                if (kullanici != null)
                {
                    // !!! BURAYA GİRİŞ SESSION VEYA COOKIE KODU EKLENMELİ !!!
                    // (Şimdilik sadece yönlendirme yapıyoruz)

                    return RedirectToAction("Index", "Home"); // Başarılı, Home'a yönlendir
                }

                // Kullanıcı bulunamadıysa hata göster
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View(model);
            }

            // Model geçerli değilse formu tekrar göster
            return View(model);
        }
    }
}