using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class FinansController : Controller
    {
        private readonly CiftlikDbContext _context;

        public FinansController(CiftlikDbContext context)
        {
            _context = context;
        }

        // FİLTRE PARAMETRESİ EKLENDİ (Varsayılan: aylik)
        public async Task<IActionResult> Index(string filter = "aylik")
        {
            var model = new FinansRaporViewModel();
            var hareketler = new List<FinansSatiri>();

            // 1. TARİH ARALIĞINI BELİRLE
            DateTime baslangicTarihi;
            DateTime bitisTarihi = DateTime.Now;

            if (filter == "yillik")
            {
                // Bu yılın başı (1 Ocak)
                baslangicTarihi = new DateTime(DateTime.Today.Year, 1, 1);
                ViewBag.FilterText = "Bu Yıl";
            }
            else // Varsayılan: Aylık
            {
                // Bu ayın başı (1'i)
                baslangicTarihi = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                ViewBag.FilterText = "Bu Ay";
            }

            ViewBag.ActiveFilter = filter; // Butonları boyamak için View'a gönderiyoruz

            // --- VERİTABANI SORGULARI (TARİH FİLTRELİ) ---

            // 2. SÜT GELİRLERİ
            var sutSatislar = await _context.SutCikislari
                .Where(s => s.CikisTuru == SutCikisTuru.Satis && s.Tarih >= baslangicTarihi)
                .ToListAsync();

            model.GelirSut = sutSatislar.Sum(x => x.ToplamTutar);
            hareketler.AddRange(sutSatislar.Select(x => new FinansSatiri
            {
                Tarih = x.Tarih,
                Tur = "Süt Satışı",
                Aciklama = $"{x.Miktar} Lt - {x.Aciklama}",
                Tutar = x.ToplamTutar,
                GelirMi = true
            }));

            // 3. YEM GİDERLERİ
            var yemAlimlari = await _context.YemAlimlari
                .Where(y => y.AlisTarihi >= baslangicTarihi)
                .ToListAsync();

            model.GiderYem = yemAlimlari.Sum(x => x.ToplamFiyat);
            hareketler.AddRange(yemAlimlari.Select(x => new FinansSatiri
            {
                Tarih = x.AlisTarihi,
                Tur = "Yem Alımı",
                Aciklama = $"{x.YemTuru} ({x.Miktar} Birim)",
                Tutar = x.ToplamFiyat,
                GelirMi = false
            }));

            // 4. HAYVAN ALIM (Gider)
            // Not: Hayvan satışı için 'Çıkış Tarihi' tutmadığımızdan mecburen 'Sürüden Çıkış' işlemini o an olmuş sayarak dahil edemiyoruz.
            // Ancak Alış Tarihi var, onu filtreleyebiliriz.
            var hayvanAlimlar = await _context.Hayvanlar
                .Where(h => h.GirisSekli == GirisSekli.SatinAlma && h.AlisFiyati.HasValue && h.AlisTarihi >= baslangicTarihi)
                .ToListAsync();

            model.GiderHayvanAlim = hayvanAlimlar.Sum(x => x.AlisFiyati.Value);
            hareketler.AddRange(hayvanAlimlar.Select(x => new FinansSatiri
            {
                Tarih = x.AlisTarihi ?? DateTime.MinValue,
                Tur = "Hayvan Alımı",
                Aciklama = $"{x.KupeNumarasi} - {x.Cins}",
                Tutar = x.AlisFiyati.Value,
                GelirMi = false
            }));

            // Hayvan Satışı (Geçici Çözüm: Tarih tutmadığımız için filtresiz getiriyoruz veya pas geçiyoruz. 
            // Doğrusu Hayvan tablosuna 'CikisTarihi' eklemektir. Şimdilik pas geçiyorum veya tümünü ekleyebiliriz.)
            // Şimdilik sadece manuel eklenen FinansIslemi üzerinden satış girilmesi daha sağlıklı olur.

            // 5. TEDAVİ GİDERLERİ (Tarih olmadığı için ID veya SutArinma referans alınabilir ama en doğrusu FinansIslemi'ne girmektir)
            // Şimdilik Tedavi maliyetlerini filtreye dahil etmiyoruz (Tarih kolonu eksik olduğu için).

            // 6. GENEL FİNANS İŞLEMLERİ (Fatura, Maaş vb.)
            var genelIslemler = await _context.FinansIslemleri
                .Where(f => f.Tarih >= baslangicTarihi)
                .ToListAsync();

            model.GiderGenel = genelIslemler.Where(x => x.Tur == IslemTuru.Gider).Sum(x => x.Tutar);
            model.GelirGenel = genelIslemler.Where(x => x.Tur == IslemTuru.Gelir).Sum(x => x.Tutar);

            hareketler.AddRange(genelIslemler.Select(x => new FinansSatiri
            {
                Tarih = x.Tarih,
                Tur = x.Kategori.ToString(),
                Aciklama = x.Aciklama ?? "-",
                Tutar = x.Tutar,
                GelirMi = (x.Tur == IslemTuru.Gelir)
            }));

            // --- TOPLAMLAR ---
            model.ToplamGelir = model.GelirSut + model.GelirHayvanSatis + model.GelirGenel;
            model.ToplamGider = model.GiderYem + model.GiderHayvanAlim + model.GiderTedavi + model.GiderGenel;
            model.NetKar = model.ToplamGelir - model.ToplamGider;

            // Listeyi tarihe göre sırala
            model.SonHareketler = hareketler.OrderByDescending(x => x.Tarih).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinansIslemi islem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(islem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(islem);
        }
    }
}