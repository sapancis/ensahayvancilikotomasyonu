using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class TestController : Controller
    {
        private readonly CiftlikDbContext _context;

        public TestController(CiftlikDbContext context)
        {
            _context = context;
        }

        // --- 1. LİSTELEME (INDEX) ---
        public async Task<IActionResult> Index()
        {
            var testKayitlari = await _context.TestKayitlari
                                    .Include(t => t.Hayvan)
                                    .OrderByDescending(t => t.TestTarihi)
                                    .ToListAsync();
            return View(testKayitlari);
        }

        // --- 2. YENİ TEST FORMU (GET) ---
        public IActionResult Create()
        {
            HayvanListesiniYukle();
            return View();
        }

        // --- 3. YENİ TEST KAYDETME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HayvanId,TestIsmi,TestTarihi,Sonuc")] TestKaydi testKaydi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testKaydi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            HayvanListesiniYukle();
            return View(testKaydi);
        }

        // --- 4. DÜZENLEME FORMU (GET) ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var testKaydi = await _context.TestKayitlari.FindAsync(id);
            if (testKaydi == null) return NotFound();

            HayvanListesiniYukle(testKaydi.HayvanId);
            return View(testKaydi);
        }

        // --- 5. DÜZENLEME KAYDETME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HayvanId,TestIsmi,TestTarihi,Sonuc")] TestKaydi testKaydi)
        {
            if (id != testKaydi.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testKaydi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TestKayitlari.Any(e => e.Id == testKaydi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            HayvanListesiniYukle(testKaydi.HayvanId);
            return View(testKaydi);
        }

        // --- 6. SİLME ONAY SAYFASI (GET) ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var testKaydi = await _context.TestKayitlari
                .Include(t => t.Hayvan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (testKaydi == null) return NotFound();

            return View(testKaydi);
        }

        // --- 7. SİLME İŞLEMİ (POST) ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testKaydi = await _context.TestKayitlari.FindAsync(id);
            if (testKaydi != null)
            {
                _context.TestKayitlari.Remove(testKaydi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- YARDIMCI METOT ---
        // SADECE AKTİF HAYVANLARI LİSTELER
        private void HayvanListesiniYukle(object? selectedId = null)
        {
            var hayvanListesi = _context.Hayvanlar
                .Where(h => h.Aktif) // <-- FİLTRE EKLENDİ
                .OrderBy(h => h.KupeNumarasi)
                .Select(h => new
                {
                    Id = h.Id,
                    GorunenDeger = h.KupeNumarasi + " - " + h.Ad
                }).ToList();

            ViewBag.HayvanListesi = new SelectList(hayvanListesi, "Id", "GorunenDeger", selectedId);
        }
    }
}