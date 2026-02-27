using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ciftlik2.Controllers
{
    public class IstatistikController : Controller
    {
        private readonly CiftlikDbContext _context;

        public IstatistikController(CiftlikDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // 1. FİNANS DURUMU (Bu metot aynı kalacak, sadece tarih aralığına bakar)
        [HttpGet]
        public async Task<JsonResult> GetFinansDurumu(DateTime? baslangic, DateTime? bitis)
        {
            var start = baslangic ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var end = bitis ?? DateTime.Today.AddDays(1).AddTicks(-1);

            var sutGeliri = await _context.SutCikislari
                .Where(s => s.Tarih >= start && s.Tarih <= end && s.CikisTuru == SutCikisTuru.Satis)
                .SumAsync(s => s.ToplamTutar);

            var genelGelir = await _context.FinansIslemleri
                .Where(f => f.Tarih >= start && f.Tarih <= end && f.Tur == IslemTuru.Gelir)
                .SumAsync(f => f.Tutar);

            var toplamGelir = sutGeliri + genelGelir;

            var yemGideri = await _context.YemAlimlari
                .Where(y => y.AlisTarihi >= start && y.AlisTarihi <= end)
                .SumAsync(y => y.ToplamFiyat);

            var tedaviGideri = await _context.TedaviKayitlari.SumAsync(t => t.TedaviUcreti);

            var genelGider = await _context.FinansIslemleri
                .Where(f => f.Tarih >= start && f.Tarih <= end && f.Tur == IslemTuru.Gider)
                .SumAsync(f => f.Tutar);

            var toplamGider = yemGideri + tedaviGideri + genelGider;

            return Json(new
            {
                labels = new[] { "Toplam Gelir", "Toplam Gider" },
                data = new[] { toplamGelir, toplamGider }
            });
        }

        // =================================================================================
        // DÜZELTİLEN METOT 1: İNEK BAŞINA VERİM (FREKANS BAZLI)
        // =================================================================================
        [HttpGet]
        public async Task<JsonResult> GetVerimlilikAnalizi(DateTime? baslangic, DateTime? bitis, string frekans = "gun")
        {
            var start = baslangic ?? DateTime.Today.AddDays(-30);
            var end = bitis ?? DateTime.Today;

            var query = _context.SutKayitlari.Where(s => s.Tarih >= start && s.Tarih <= end);
            var culture = new CultureInfo("tr-TR");

            // --- AYLIK GRUPLAMA ---
            if (frekans == "ay")
            {
                var grouped = await query
                    .GroupBy(x => new { x.Tarih.Year, x.Tarih.Month })
                    .Select(g => new {
                        Yil = g.Key.Year,
                        Ay = g.Key.Month,
                        TotalSut = g.Sum(x => x.Miktar),
                        TotalInek = g.Sum(x => x.SagilanHayvanSayisi)
                    })
                    .OrderBy(x => x.Yil).ThenBy(x => x.Ay)
                    .ToListAsync();

                // Etiket Formatı: "Ocak 2024"
                return Json(new
                {
                    labels = grouped.Select(x => culture.DateTimeFormat.GetMonthName(x.Ay) + " " + x.Yil),
                    data = grouped.Select(x => x.TotalInek > 0 ? Math.Round(x.TotalSut / x.TotalInek, 1) : 0)
                });
            }
            // --- YILLIK GRUPLAMA ---
            else if (frekans == "yil")
            {
                var grouped = await query
                    .GroupBy(x => x.Tarih.Year)
                    .Select(g => new {
                        Yil = g.Key,
                        TotalSut = g.Sum(x => x.Miktar),
                        TotalInek = g.Sum(x => x.SagilanHayvanSayisi)
                    })
                    .OrderBy(x => x.Yil)
                    .ToListAsync();

                // Etiket Formatı: "2024"
                return Json(new
                {
                    labels = grouped.Select(x => x.Yil.ToString()),
                    data = grouped.Select(x => x.TotalInek > 0 ? Math.Round(x.TotalSut / x.TotalInek, 1) : 0)
                });
            }
            // --- GÜNLÜK GRUPLAMA (Varsayılan) ---
            else
            {
                var grouped = await query
                    .GroupBy(x => x.Tarih.Date)
                    .Select(g => new {
                        Tarih = g.Key,
                        TotalSut = g.Sum(x => x.Miktar),
                        TotalInek = g.Sum(x => x.SagilanHayvanSayisi)
                    })
                    .OrderBy(x => x.Tarih)
                    .ToListAsync();

                // Etiket Formatı: "15 Şub"
                return Json(new
                {
                    labels = grouped.Select(x => x.Tarih.ToString("dd MMM", culture)),
                    data = grouped.Select(x => x.TotalInek > 0 ? Math.Round(x.TotalSut / x.TotalInek, 1) : 0)
                });
            }
        }

        // 3. SÜRÜ DURUMU (Aynı kalacak)
        [HttpGet]
        public async Task<JsonResult> GetSuruDurumu()
        {
            var hayvanlar = await _context.Hayvanlar.Where(h => h.Aktif).ToListAsync();
            int sagmal = 0, kuru = 0, genc = 0, erkekBesi = 0;

            foreach (var h in hayvanlar)
            {
                var yasAy = h.DogumTarihi.HasValue ? (DateTime.Today - h.DogumTarihi.Value).TotalDays / 30 : 0;
                if (h.Cinsiyet == Cinsiyet.Erkek) erkekBesi++;
                else { if (yasAy < 15) genc++; else { if (h.LaktasyonSayisi > 0) sagmal++; else kuru++; } }
            }
            return Json(new { labels = new[] { "Sağmal İnek", "Kuru / Düve", "Buzağı / Genç", "Erkek (Besi)" }, data = new[] { sagmal, kuru, genc, erkekBesi } });
        }

        // 4. GEBELİK BAŞARISI (Aynı kalacak)
        [HttpGet]
        public async Task<JsonResult> GetGebelikBasarisi(DateTime? baslangic, DateTime? bitis)
        {
            var start = baslangic ?? new DateTime(DateTime.Today.Year, 1, 1);
            var end = bitis ?? DateTime.Today;
            var testler = await _context.Tohumlanmalar.Where(t => t.TohumlanmaTarihi >= start && t.TohumlanmaTarihi <= end && t.GebelikTestDurumu == TestDurumu.Yapildi).ToListAsync();
            return Json(new { labels = new[] { "Gebe (Başarılı)", "Boş (Tekrar)" }, data = new[] { testler.Count(t => t.GebelikTestiSonucu == GebelikSonucu.Gebe), testler.Count(t => t.GebelikTestiSonucu == GebelikSonucu.GebeDegil) } });
        }

        // =================================================================================
        // DÜZELTİLEN METOT 2: TOPLAM SÜT ÜRETİMİ (FREKANS BAZLI)
        // =================================================================================
        [HttpGet]
        public async Task<JsonResult> GetSutUretimGrafigi(DateTime? baslangic, DateTime? bitis, string frekans = "gun")
        {
            var start = baslangic ?? DateTime.Today.AddDays(-30);
            var end = bitis ?? DateTime.Today;

            var query = _context.SutKayitlari.Where(s => s.Tarih >= start && s.Tarih <= end);
            var culture = new CultureInfo("tr-TR");

            // --- AYLIK GRUPLAMA ---
            if (frekans == "ay")
            {
                var grouped = await query
                    .GroupBy(x => new { x.Tarih.Year, x.Tarih.Month })
                    .Select(g => new {
                        Yil = g.Key.Year,
                        Ay = g.Key.Month,
                        Toplam = g.Sum(x => x.Miktar)
                    })
                    .OrderBy(x => x.Yil).ThenBy(x => x.Ay)
                    .ToListAsync();

                return Json(new
                {
                    labels = grouped.Select(x => culture.DateTimeFormat.GetMonthName(x.Ay) + " " + x.Yil), // "Mart 2024"
                    data = grouped.Select(x => x.Toplam)
                });
            }
            // --- YILLIK GRUPLAMA ---
            else if (frekans == "yil")
            {
                var grouped = await query
                    .GroupBy(x => x.Tarih.Year)
                    .Select(g => new {
                        Yil = g.Key,
                        Toplam = g.Sum(x => x.Miktar)
                    })
                    .OrderBy(x => x.Yil)
                    .ToListAsync();

                return Json(new
                {
                    labels = grouped.Select(x => x.Yil.ToString()), // "2024", "2025"
                    data = grouped.Select(x => x.Toplam)
                });
            }
            // --- GÜNLÜK GRUPLAMA ---
            else
            {
                var grouped = await query
                    .GroupBy(x => x.Tarih.Date)
                    .Select(g => new {
                        Tarih = g.Key,
                        Toplam = g.Sum(x => x.Miktar)
                    })
                    .OrderBy(x => x.Tarih)
                    .ToListAsync();

                return Json(new
                {
                    labels = grouped.Select(x => x.Tarih.ToString("dd MMM", culture)), // "14 Ara"
                    data = grouped.Select(x => x.Toplam)
                });
            }
        }
    }
}