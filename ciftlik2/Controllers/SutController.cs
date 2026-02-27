using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class SutController : Controller
    {
        private readonly CiftlikDbContext _context;

        public SutController(CiftlikDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. ÜRETİM (GİRİŞ) İŞLEMLERİ (MEVCUT)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // Sadece üretim kayıtları
            var kayitlar = await _context.SutKayitlari
                .OrderByDescending(s => s.Tarih)
                .ThenBy(s => s.Vakit)
                .ToListAsync();
            return View(kayitlar);
        }

        public IActionResult Create()
        {
            return View(new SutKaydi { Tarih = DateTime.Today, Miktar = 0, SagilanHayvanSayisi = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tarih,SagilanHayvanSayisi,Miktar,Vakit")] SutKaydi sutKaydi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sutKaydi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sutKaydi);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sutKaydi = await _context.SutKayitlari.FindAsync(id);
            if (sutKaydi == null) return NotFound();
            return View(sutKaydi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tarih,SagilanHayvanSayisi,Miktar,Vakit")] SutKaydi sutKaydi)
        {
            if (id != sutKaydi.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(sutKaydi); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!_context.SutKayitlari.Any(e => e.Id == id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(sutKaydi);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var sutKaydi = await _context.SutKayitlari.FirstOrDefaultAsync(m => m.Id == id);
            if (sutKaydi == null) return NotFound();
            return View(sutKaydi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sutKaydi = await _context.SutKayitlari.FindAsync(id);
            if (sutKaydi != null) { _context.SutKayitlari.Remove(sutKaydi); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 2. TÜKETİM / SATIŞ (ÇIKIŞ) İŞLEMLERİ (YENİ)
        // ==========================================

        // A. ÇIKIŞ LİSTESİ
        public async Task<IActionResult> CikisListesi()
        {
            var cikislar = await _context.SutCikislari
                .OrderByDescending(s => s.Tarih)
                .ToListAsync();
            return View(cikislar);
        }

        // B. ÇIKIŞ EKLE (GET)
        public IActionResult CreateCikis()
        {
            return View(new SutCikisi());
        }

        // C. ÇIKIŞ EKLE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCikis([Bind("Tarih,CikisTuru,Miktar,BirimFiyat,Aciklama")] SutCikisi cikis)
        {
            if (ModelState.IsValid)
            {
                // Eğer "Satış" değilse fiyatı 0'la, yoksa hesaplama hatası olmasın
                if (cikis.CikisTuru != SutCikisTuru.Satis)
                {
                    cikis.BirimFiyat = 0;
                }

                // Toplam Tutar Hesabı: Litre * Fiyat
                cikis.ToplamTutar = (decimal)cikis.Miktar * cikis.BirimFiyat;

                _context.Add(cikis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CikisListesi));
            }
            return View(cikis);
        }

        // D. ÇIKIŞ SİL (GET)
        public async Task<IActionResult> DeleteCikis(int? id)
        {
            if (id == null) return NotFound();
            var kayit = await _context.SutCikislari.FirstOrDefaultAsync(m => m.Id == id);
            if (kayit == null) return NotFound();
            return View(kayit);
        }

        // E. ÇIKIŞ SİL (POST)
        [HttpPost, ActionName("DeleteCikis")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCikisConfirmed(int id)
        {
            var kayit = await _context.SutCikislari.FindAsync(id);
            if (kayit != null)
            {
                _context.SutCikislari.Remove(kayit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(CikisListesi));
        }
    }
}