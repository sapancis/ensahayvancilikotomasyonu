using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class TedaviController : Controller
    {
        private readonly CiftlikDbContext _context;

        public TedaviController(CiftlikDbContext context)
        {
            _context = context;
        }

        // --- 1. LİSTELEME (INDEX) ---
        public async Task<IActionResult> Index()
        {
            var tedaviKayitlari = await _context.TedaviKayitlari
                                    .Include(t => t.Hayvan)
                                    .OrderByDescending(t => t.Id)
                                    .ToListAsync();
            return View(tedaviKayitlari);
        }

        // --- 2. YENİ TEDAVİ FORMU (GET) ---
        public IActionResult Create()
        {
            // Listeyi doldur ve View'a gönder
            HayvanListesiniYukle();
            return View();
        }

        // --- 3. YENİ TEDAVİ KAYDETME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("HayvanId,Teshis,KullanilanIlac,SutArinmaTarihi,ReceteyiYazanVeteriner,DenetleyenVeteriner")] TedaviKaydi tedaviKaydi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tedaviKaydi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Hata varsa listeyi tekrar yükle
            HayvanListesiniYukle(tedaviKaydi.HayvanId);
            return View(tedaviKaydi);
        }

        // --- 4. DÜZENLEME FORMU (GET) ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tedaviKaydi = await _context.TedaviKayitlari.FindAsync(id);
            if (tedaviKaydi == null) return NotFound();

            HayvanListesiniYukle(tedaviKaydi.HayvanId);
            return View(tedaviKaydi);
        }

        // --- 5. DÜZENLEME KAYDETME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,HayvanId,Teshis,KullanilanIlac,SutArinmaTarihi,ReceteyiYazanVeteriner,DenetleyenVeteriner")] TedaviKaydi tedaviKaydi)
        {
            if (id != tedaviKaydi.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tedaviKaydi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TedaviKayitlari.Any(e => e.Id == tedaviKaydi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            HayvanListesiniYukle(tedaviKaydi.HayvanId);
            return View(tedaviKaydi);
        }

        // --- 6. SİLME (DELETE) ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tedaviKaydi = await _context.TedaviKayitlari
                .Include(t => t.Hayvan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tedaviKaydi == null) return NotFound();
            return View(tedaviKaydi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tedaviKaydi = await _context.TedaviKayitlari.FindAsync(id);
            if (tedaviKaydi != null)
            {
                _context.TedaviKayitlari.Remove(tedaviKaydi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- YARDIMCI METOT: Dropdown Listesini Hazırlar ---
        private void HayvanListesiniYukle(object? selectedId = null)
        {
            // Sadece AKTİF hayvanları getiriyoruz
            var hayvanListesi = _context.Hayvanlar
                .Where(h => h.Aktif)
                .OrderBy(h => h.KupeNumarasi)
                .Select(h => new
                {
                    Id = h.Id,
                    // Görünecek Metin: "105 - Sarıkız" gibi
                    GorunenDeger = h.KupeNumarasi + (string.IsNullOrEmpty(h.Ad) ? "" : " - " + h.Ad)
                }).ToList();

            ViewBag.HayvanListesi = new SelectList(hayvanListesi, "Id", "GorunenDeger", selectedId);
        }
    }
}