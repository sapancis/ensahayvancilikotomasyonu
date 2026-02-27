using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class AyarlarController : Controller
    {
        private readonly CiftlikDbContext _context;

        public AyarlarController(CiftlikDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // İlk kaydı getir, yoksa oluştur
            var ayar = await _context.Ayarlar.FirstOrDefaultAsync(a => a.Id == 1);
            if (ayar == null)
            {
                ayar = new SistemAyarlari(); // Varsayılan değerlerle gelir
                _context.Ayarlar.Add(ayar);
                await _context.SaveChangesAsync();
            }
            return View(ayar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SistemAyarlari gelenAyar)
        {
            if (ModelState.IsValid)
            {
                var mevcut = await _context.Ayarlar.FirstOrDefaultAsync(a => a.Id == 1);
                if (mevcut == null) return NotFound();

                // 1. Genel Ayarlar
                mevcut.CiftlikAdi = gelenAyar.CiftlikAdi;
                mevcut.CiftlikSahibi = gelenAyar.CiftlikSahibi;

                // 2. Süre Ayarları
                mevcut.StandartGebelikSuresi = gelenAyar.StandartGebelikSuresi;
                mevcut.KuruyaAlmaGunu = gelenAyar.KuruyaAlmaGunu;
                mevcut.KizginlikBaslangicGun = gelenAyar.KizginlikBaslangicGun;
                mevcut.KizginlikBitisGun = gelenAyar.KizginlikBitisGun;
                mevcut.GebelikTestiGun = gelenAyar.GebelikTestiGun;
                mevcut.BuzagiSuttenKesmeGunu = gelenAyar.BuzagiSuttenKesmeGunu;
                mevcut.DogumOncesiBildirimGunu = gelenAyar.DogumOncesiBildirimGunu;
                _context.Update(mevcut);
                await _context.SaveChangesAsync();

                TempData["Mesaj"] = "Sistem ayarları başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(gelenAyar);
        }
    }
}