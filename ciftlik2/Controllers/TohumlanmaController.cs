using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class TohumlanmalarController : Controller
    {
        private readonly CiftlikDbContext _context;

        public TohumlanmalarController(CiftlikDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. İŞLEM BEKLEYENLER (INDEX)
        // ============================================================
        // Sadece henüz test sonucu girilmemiş kayıtları gösterir.
        public async Task<IActionResult> Index()
        {
            var ayarlar = await _context.Ayarlar.FirstOrDefaultAsync() ?? new SistemAyarlari();
            ViewBag.Ayarlar = ayarlar;

            var tohumlanmalar = await _context.Tohumlanmalar
                .Include(t => t.DisiHayvan)
                .Include(t => t.ErkekHayvan)
                .Where(t => t.GebelikTestDurumu == TestDurumu.Yapilmadi) // Sadece Yapılmayanlar
                .OrderByDescending(t => t.TohumlanmaTarihi)
                .ToListAsync();

            return View(tohumlanmalar);
        }

        // ============================================================
        // 2. GENEL ARŞİV (BOŞ ÇIKANLAR + DOĞUM YAPANLAR + DÜŞÜKLER)
        // ============================================================
        public async Task<IActionResult> TohumlanmaArsivi()
        {
            var arsiv = await _context.Tohumlanmalar
                .Include(t => t.DisiHayvan)
                .Include(t => t.ErkekHayvan)
                .Where(t =>
                    // 1. Test yapılmış ve BOŞ çıkmışsa arşive girer
                    (t.GebelikTestDurumu == TestDurumu.Yapildi && t.GebelikTestiSonucu == GebelikSonucu.GebeDegil)
                    ||
                    // 2. Gebelik bitmişse (Doğum veya Düşük) arşive girer
                    (t.SonlanmaTuru != GebelikSonlanmaTuru.DevamEdiyor)
                )
                .OrderByDescending(t => t.TohumlanmaTarihi)
                .ToListAsync();

            return View(arsiv);
        }

        // ============================================================
        // 3. YENİ KAYIT (CREATE)
        // ============================================================
        public IActionResult Create()
        {
            ListeleriYukle();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisiHayvanId,ErkekHayvanId,TohumlanmaTarihi,TohumlanmaTuru")] Tohumlanma tohumlanma)
        {
            if (ModelState.IsValid)
            {
                // Varsayılan değerler
                tohumlanma.GebelikTestDurumu = TestDurumu.Yapilmadi;
                tohumlanma.GebelikTestiSonucu = null;
                tohumlanma.SonlanmaTuru = GebelikSonlanmaTuru.DevamEdiyor;

                _context.Add(tohumlanma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ListeleriYukle(tohumlanma.DisiHayvanId, tohumlanma.ErkekHayvanId);
            return View(tohumlanma);
        }

        // ============================================================
        // 4. DÜZENLEME (EDIT)
        // ============================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tohumlanma = await _context.Tohumlanmalar.FindAsync(id);
            if (tohumlanma == null) return NotFound();

            ListeleriYukle(tohumlanma.DisiHayvanId, tohumlanma.ErkekHayvanId);
            return View(tohumlanma);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DisiHayvanId,ErkekHayvanId,TohumlanmaTarihi,TohumlanmaTuru,GebelikTestDurumu,GebelikTestiSonucu,SonlanmaTuru")] Tohumlanma tohumlanma)
        {
            if (id != tohumlanma.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tohumlanma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tohumlanmalar.Any(e => e.Id == tohumlanma.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ListeleriYukle(tohumlanma.DisiHayvanId, tohumlanma.ErkekHayvanId);
            return View(tohumlanma);
        }

        // ============================================================
        // 5. SİLME (DELETE)
        // ============================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tohumlanma = await _context.Tohumlanmalar
                .Include(t => t.DisiHayvan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tohumlanma == null) return NotFound();
            return View(tohumlanma);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tohumlanma = await _context.Tohumlanmalar.FindAsync(id);
            if (tohumlanma != null)
            {
                _context.Tohumlanmalar.Remove(tohumlanma);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 6. TEST SONUCU GÜNCELLEME (KRİTİK MANTIK BURADA)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestSonucuGuncelle(int id, int sonuc)
        {
            var tohumlanma = await _context.Tohumlanmalar.FindAsync(id);
            if (tohumlanma == null) return NotFound();

            // Durumları Güncelle
            tohumlanma.GebelikTestiSonucu = (GebelikSonucu)sonuc;
            tohumlanma.GebelikTestDurumu = TestDurumu.Yapildi;

            _context.Update(tohumlanma);
            await _context.SaveChangesAsync();

            // YÖNLENDİRME MANTIĞI:
            // Eğer GEBE ise -> Gebeler Listesine git
            // Eğer BOŞ ise  -> Arşive git
            if (tohumlanma.GebelikTestiSonucu == GebelikSonucu.Gebe)
            {
                return RedirectToAction(nameof(Gebeler));
            }
            else
            {
                return RedirectToAction(nameof(TohumlanmaArsivi));
            }
        }

        // ============================================================
        // 7. RAPOR: GEBE HAYVANLAR (AKTİF GEBELİKLER)
        // ============================================================
        public async Task<IActionResult> Gebeler()
        {
            var ayarlar = await _context.Ayarlar.FirstOrDefaultAsync() ?? new SistemAyarlari();
            ViewBag.Ayarlar = ayarlar;

            var gebeler = await _context.Tohumlanmalar
                .Include(t => t.DisiHayvan)
                .Include(t => t.ErkekHayvan)
                .Where(t => t.GebelikTestDurumu == TestDurumu.Yapildi &&
                            t.GebelikTestiSonucu == GebelikSonucu.Gebe &&
                            t.DisiHayvan.Aktif &&
                            t.SonlanmaTuru == GebelikSonlanmaTuru.DevamEdiyor) // Sadece devam edenler
                .OrderBy(t => t.TohumlanmaTarihi)
                .ToListAsync();

            return View(gebeler);
        }

        // ============================================================
        // 8. RAPOR: KURUDAKİLER
        // ============================================================
        public async Task<IActionResult> Kurudakiler()
        {
            var ayarlar = await _context.Ayarlar.FirstOrDefaultAsync() ?? new SistemAyarlari();
            ViewBag.Ayarlar = ayarlar;

            var tumGebeler = await _context.Tohumlanmalar
                .Include(t => t.DisiHayvan)
                .Where(t => t.GebelikTestDurumu == TestDurumu.Yapildi &&
                            t.GebelikTestiSonucu == GebelikSonucu.Gebe &&
                            t.DisiHayvan.Aktif &&
                            t.SonlanmaTuru == GebelikSonlanmaTuru.DevamEdiyor)
                .ToListAsync();

            // Hesaplama
            var kurudakiler = tumGebeler
                .Where(t =>
                {
                    var kuruyaAlmaTarihi = t.TohumlanmaTarihi.AddDays(ayarlar.KuruyaAlmaGunu);
                    var dogumTarihi = t.TohumlanmaTarihi.AddDays(ayarlar.StandartGebelikSuresi);
                    var bugun = DateTime.Today;

                    // Kuruya alma zamanı gelmiş VE henüz doğurmamış
                    return kuruyaAlmaTarihi <= bugun && dogumTarihi > bugun;
                })
                .OrderBy(t => t.TohumlanmaTarihi)
                .ToList();

            return View(kurudakiler);
        }

        // ============================================================
        // 9. ESKİ GEBELİK ARŞİVİ (Link uyumluluğu için bıraktım, 
        // ama artık TohumlanmaArsivi'ni kullanacağız)
        // ============================================================
        public async Task<IActionResult> GebelikArsivi()
        {
            return RedirectToAction(nameof(TohumlanmaArsivi));
        }

        // ============================================================
        // 10. GEBELİK SONLANDIR (Düşük/Ölüm)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GebelikSonlandir(int id, int durum)
        {
            var tohumlanma = await _context.Tohumlanmalar.FindAsync(id);
            if (tohumlanma == null) return NotFound();

            // Durumu güncelle (Örn: Düşük)
            tohumlanma.SonlanmaTuru = (GebelikSonlanmaTuru)durum;

            _context.Update(tohumlanma);
            await _context.SaveChangesAsync();

            // Gebelik bittiği için artık aktif gebelerde işi yok, arşive veya listeye dönebiliriz.
            // Genellikle işlem yapılan sayfaya dönmek iyidir.
            return RedirectToAction(nameof(Gebeler));
        }

        // ============================================================
        // YARDIMCI METOTLAR
        // ============================================================
        private void ListeleriYukle(object? seciliDisiId = null, object? seciliErkekId = null)
        {
            var yetiskinTarihi = DateTime.Today.AddYears(-1);

            var disiQuery = _context.Hayvanlar
                .Where(h => h.Aktif && h.Cinsiyet == Cinsiyet.Disi && h.DogumTarihi.HasValue && h.DogumTarihi.Value <= yetiskinTarihi)
                .OrderBy(h => h.KupeNumarasi)
                .Select(h => new { Id = h.Id, GorunenAd = $"{h.KupeNumarasi} - {h.Ad ?? ""} ({h.HayvanGrubu})" })
                .ToList();

            var erkekQuery = _context.Hayvanlar
                .Where(h => h.Aktif && h.Cinsiyet == Cinsiyet.Erkek && h.Kisirlastirildi == false && h.DogumTarihi.HasValue && h.DogumTarihi.Value <= yetiskinTarihi)
                .OrderBy(h => h.KupeNumarasi)
                .Select(h => new { Id = h.Id, GorunenAd = $"{h.KupeNumarasi} - {h.Ad ?? ""} ({h.HayvanGrubu})" })
                .ToList();

            ViewBag.DisiHayvanId = new SelectList(disiQuery, "Id", "GorunenAd", seciliDisiId);
            ViewBag.ErkekHayvanId = new SelectList(erkekQuery, "Id", "GorunenAd", seciliErkekId);
        }
    }
}