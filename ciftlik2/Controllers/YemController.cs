using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class YemController : Controller
    {
        private readonly CiftlikDbContext _context;

        public YemController(CiftlikDbContext context)
        {
            _context = context;
        }

        // --- 1. LİSTELEME (INDEX) ---
        public async Task<IActionResult> Index()
        {
            var yemAlimlari = await _context.YemAlimlari
                                     .OrderByDescending(y => y.AlisTarihi)
                                     .ToListAsync();
            return View(yemAlimlari);
        }

        // --- 2. YENİ ALIM FORMU (GET) ---
        public IActionResult Create()
        {
            return View(new YemAlisi());
        }

        // --- 3. YENİ ALIM KAYDETME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("YemTuru,BirimFiyat,Miktar,AlisTarihi")] YemAlisi yemAlisi)
        {
            // DİKKAT: ToplamFiyat'ı Bind işleminden çıkardım, burada hesaplayacağız.

            if (ModelState.IsValid)
            {
                // --- OTOMATİK HESAPLAMA ---
                // Toplam = Birim Fiyat * Miktar
                yemAlisi.ToplamFiyat = yemAlisi.BirimFiyat * (decimal)yemAlisi.Miktar;

                _context.Add(yemAlisi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yemAlisi);
        }

        // --- 4. DÜZENLEME FORMU (GET) ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var yemAlisi = await _context.YemAlimlari.FindAsync(id);
            if (yemAlisi == null) return NotFound();

            return View(yemAlisi);
        }

        // --- 5. DÜZENLEME KAYDETME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,YemTuru,BirimFiyat,Miktar,AlisTarihi")] YemAlisi yemAlisi)
        {
            if (id != yemAlisi.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // --- GÜNCELLEMEDE DE HESAPLA ---
                    yemAlisi.ToplamFiyat = yemAlisi.BirimFiyat * (decimal)yemAlisi.Miktar;

                    _context.Update(yemAlisi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.YemAlimlari.Any(e => e.Id == yemAlisi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(yemAlisi);
        }

        // --- 6. SİLME (DELETE) ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var yemAlisi = await _context.YemAlimlari.FirstOrDefaultAsync(m => m.Id == id);
            if (yemAlisi == null) return NotFound();
            return View(yemAlisi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yemAlisi = await _context.YemAlimlari.FindAsync(id);
            if (yemAlisi != null)
            {
                _context.YemAlimlari.Remove(yemAlisi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}