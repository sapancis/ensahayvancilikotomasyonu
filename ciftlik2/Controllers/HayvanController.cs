using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class HayvanController : Controller
    {
        private readonly CiftlikDbContext _context;

        public HayvanController(CiftlikDbContext context)
        {
            _context = context;
        }

        private void IrkListesiniYukle()
        {
            var irklar = new List<string>()
            {
                "Holstein", "Simental", "Montofon", "Jersey", "Angus", "Hereford",
                "Yerli Kara", "Boz Irk", "Şarole", "Limousin", "Melez", "Diğer"
            };
            ViewBag.IrkListesi = new SelectList(irklar);
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.Hayvanlar.Where(h => h.Aktif == true);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(h =>
                    h.KupeNumarasi.Contains(searchString) ||
                    (h.Ad != null && h.Ad.Contains(searchString))
                );
            }

            var hamListe = await query.ToListAsync();
            var siraliListe = hamListe
                .OrderBy(h => h.HayvanGrubu)
                .ThenBy(h => h.KupeNumarasi)
                .ToList();

            return View(siraliListe);
        }

        // --- CREATE (GET) ---
        public IActionResult Create(string? anaKupe, string? babaKupe, int? refTohumlamaId)
        {
            IrkListesiniYukle();
            var yeniHayvan = new Hayvan();

            // Eğer gebeler sayfasından (doğum) gelindiyse verileri doldur
            if (refTohumlamaId.HasValue)
            {
                yeniHayvan.AnaKupeNumarasi = anaKupe;
                yeniHayvan.BabaKupeNumarasi = babaKupe;
                yeniHayvan.DogumTarihi = DateTime.Today; // Bugün doğdu
                yeniHayvan.GirisSekli = GirisSekli.Dogum;
                yeniHayvan.Ad = "Yeni Buzağı";

                // Annenin ırkını bulup yavruya da aynısını seçmeye çalışalım
                if (!string.IsNullOrEmpty(anaKupe))
                {
                    var anne = _context.Hayvanlar.FirstOrDefault(h => h.KupeNumarasi == anaKupe);
                    if (anne != null) yeniHayvan.Cins = anne.Cins;
                }

                // View'a bu işlemin bir gebelik sonlandırma (doğum) olduğunu bildirmek için
                ViewBag.RefTohumlamaId = refTohumlamaId;
            }

            return View(yeniHayvan);
        }

        // --- CREATE (POST) GÜNCELLENDİ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("KupeNumarasi,Ad,Cins,Cinsiyet,DogumTarihi,BabaKupeNumarasi,AnaKupeNumarasi," +
                  "GirisSekli,AlisFiyati,AlisTarihi,LaktasyonSayisi,Kisirlastirildi,Hikaye")] Hayvan hayvan, // <-- HİKAYE EKLENDİ
            int? RefTohumlamaId)
        {
            if (string.IsNullOrWhiteSpace(hayvan.KupeNumarasi))
            {
                ModelState.AddModelError("KupeNumarasi", "Küpe Numarası zorunludur.");
            }
            else
            {
                hayvan.KupeNumarasi = hayvan.KupeNumarasi.Trim();
                if (await _context.Hayvanlar.AnyAsync(h => h.KupeNumarasi == hayvan.KupeNumarasi))
                {
                    ModelState.AddModelError("KupeNumarasi", "Bu küpe numarası zaten kayıtlı.");
                }
            }

            if (hayvan.GirisSekli == GirisSekli.SatinAlma && !hayvan.AlisTarihi.HasValue)
            {
                ModelState.AddModelError("AlisTarihi", "Satın alma kaydı için alış tarihi zorunludur.");
            }

            if (ModelState.IsValid)
            {
                hayvan.Aktif = true;

                // --- 1. YAVRUYU KAYDET ---
                _context.Add(hayvan);

                // --- 2. DOĞUM İŞLEMLERİ (Gebelik Kapatma & Laktasyon Artışı) ---
                if (RefTohumlamaId.HasValue)
                {
                    var gebelikKaydi = await _context.Tohumlanmalar
                        .Include(t => t.DisiHayvan)
                        .FirstOrDefaultAsync(t => t.Id == RefTohumlamaId.Value);

                    if (gebelikKaydi != null)
                    {
                        // a) Gebeliği "Doğum (Canlı)" olarak kapat
                        gebelikKaydi.SonlanmaTuru = GebelikSonlanmaTuru.DogumCanli;
                        _context.Update(gebelikKaydi);

                        // b) Annenin laktasyon sayısını artır
                        if (gebelikKaydi.DisiHayvan != null)
                        {
                            gebelikKaydi.DisiHayvan.LaktasyonSayisi++;
                            _context.Update(gebelikKaydi.DisiHayvan);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            IrkListesiniYukle();
            ViewBag.RefTohumlamaId = RefTohumlamaId;
            return View(hayvan);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string kupeNo)
        {
            if (string.IsNullOrEmpty(kupeNo)) return NotFound();
            var hayvan = await _context.Hayvanlar
                .Include(h => h.AsiKayitlari)
                .Include(h => h.TestKayitlari)
                .Include(h => h.TedaviKayitlari)
                .Include(h => h.Tohumlanmalar).ThenInclude(t => t.ErkekHayvan)
                .FirstOrDefaultAsync(h => h.KupeNumarasi == kupeNo);
            if (hayvan == null) return NotFound();
            return View(hayvan);
        }

        public async Task<IActionResult> Edit(string kupeNo)
        {
            if (string.IsNullOrEmpty(kupeNo)) return NotFound();
            var hayvan = await _context.Hayvanlar.FirstOrDefaultAsync(h => h.KupeNumarasi == kupeNo);
            if (hayvan == null) return NotFound();
            IrkListesiniYukle();
            return View(hayvan);
        }

        // --- EDIT (POST) GÜNCELLENDİ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string kupeNo, Hayvan hayvan)
        {
            if (kupeNo != hayvan.KupeNumarasi)
            {
                if (await _context.Hayvanlar.AnyAsync(h => h.KupeNumarasi == hayvan.KupeNumarasi))
                {
                    ModelState.AddModelError("KupeNumarasi", "Bu numara kullanımda.");
                    IrkListesiniYukle();
                    return View(hayvan);
                }
            }

            var asil = await _context.Hayvanlar.FirstOrDefaultAsync(h => h.Id == hayvan.Id);
            if (asil == null) return NotFound();

            // Veritabanındaki asıl kaydı, formdan gelen yeni bilgilerle güncelliyoruz
            asil.KupeNumarasi = hayvan.KupeNumarasi;
            asil.Ad = hayvan.Ad;
            asil.Hikaye = hayvan.Hikaye; // <-- HİKAYE GÜNCELLEMESİ EKLENDİ
            asil.Cins = hayvan.Cins;
            asil.Cinsiyet = hayvan.Cinsiyet;
            asil.DogumTarihi = hayvan.DogumTarihi;
            asil.GirisSekli = hayvan.GirisSekli;
            asil.LaktasyonSayisi = hayvan.LaktasyonSayisi;
            asil.AlisTarihi = hayvan.AlisTarihi;
            asil.AlisFiyati = hayvan.AlisFiyati;
            asil.AnaKupeNumarasi = hayvan.AnaKupeNumarasi;
            asil.BabaKupeNumarasi = hayvan.BabaKupeNumarasi;
            asil.Kisirlastirildi = hayvan.Kisirlastirildi;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string kupeNo)
        {
            if (string.IsNullOrEmpty(kupeNo)) return NotFound();
            var hayvan = await _context.Hayvanlar.FirstOrDefaultAsync(h => h.KupeNumarasi == kupeNo);
            return View(hayvan);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string kupeNo, CikisNedeni CikisNedeni, decimal? SatisFiyati)
        {
            var hayvan = await _context.Hayvanlar.FirstOrDefaultAsync(h => h.KupeNumarasi == kupeNo);
            if (hayvan != null)
            {
                hayvan.Aktif = false;
                hayvan.CikisNedeni = CikisNedeni;

                if (CikisNedeni == CikisNedeni.Satis)
                {
                    hayvan.SatisFiyati = SatisFiyati ?? 0;
                }

                _context.Update(hayvan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}