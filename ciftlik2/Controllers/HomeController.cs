using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ciftlik2.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly CiftlikDbContext _context;

        public HomeController(CiftlikDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- 1. DASHBOARD ÜST BANNER VERİLERİ ---
        [HttpGet]
        public async Task<JsonResult> GetKpiBanner()
        {
            try
            {
                var today = DateTime.Today;
                var buAyBasi = new DateTime(today.Year, today.Month, 1);

                // Aktif hayvan sayısı
                var aktifHayvan = await _context.Hayvanlar.CountAsync(h => h.Aktif);

                // Bugün üretilen süt
                var gunlukSut = await _context.SutKayitlari
                    .Where(s => s.Tarih == today)
                    .SumAsync(s => s.Miktar);

                // Bugün işlem yapılan (sağılan) inek sayısı
                var sagilanHayvanSayisi = await _context.SutKayitlari
                    .Where(s => s.Tarih == today)
                    .SumAsync(s => s.SagilanHayvanSayisi);

                // Bu ayki toplam süt
                var aylikSut = await _context.SutKayitlari
                    .Where(s => s.Tarih >= buAyBasi)
                    .SumAsync(s => s.Miktar);

                return Json(new
                {
                    aktifHayvan = aktifHayvan,
                    gunlukSut = gunlukSut.ToString("N1") + " Lt",
                    sagilanSayisi = sagilanHayvanSayisi + " Adet",
                    aylikSut = aylikSut.ToString("N0") + " Lt"
                });
            }
            catch (Exception)
            {
                return Json(new { aktifHayvan = 0, gunlukSut = "0 Lt", sagilanSayisi = "0", aylikSut = "0 Lt" });
            }
        }

        // --- 2. AKILLI BİLDİRİM SİSTEMİ ---
        [HttpGet]
        public async Task<JsonResult> GetBildirimler()
        {
            try
            {
                var today = DateTime.Today;
                var bildirimler = new List<object>();

                // 1. AYARLARI ÇEK (Yönetim Panelinden Ayarlanan Değerler)
                // Eğer veritabanında ayar yoksa varsayılan değerleri kullan
                var ayar = await _context.Ayarlar.FirstOrDefaultAsync()
                           ?? new SistemAyarlari
                           {
                               KizginlikBaslangicGun = 18,
                               KizginlikBitisGun = 23,
                               GebelikTestiGun = 40,
                               BuzagiSuttenKesmeGunu = 70,
                               StandartGebelikSuresi = 280,
                               DogumOncesiBildirimGunu = 15
                           };

                // A) KIZGINLIK KONTROLÜ (Dinamik Aralık)
                // Formül: Bugün - Bitiş <= TohumlamaTarihi <= Bugün - Başlangıç
                var kizginlikRiski = await _context.Tohumlanmalar
                    .Include(t => t.DisiHayvan)
                    .Where(t => t.GebelikTestDurumu == TestDurumu.Yapilmadi &&
                                t.DisiHayvan.Aktif &&
                                t.TohumlanmaTarihi >= today.AddDays(-ayar.KizginlikBitisGun) &&
                                t.TohumlanmaTarihi <= today.AddDays(-ayar.KizginlikBaslangicGun))
                    .ToListAsync();

                foreach (var item in kizginlikRiski)
                {
                    var gecen = (today - item.TohumlanmaTarihi).Days;
                    bildirimler.Add(new
                    {
                        tur = "warning",
                        ikon = "fa-fire",
                        baslik = "Kızgınlık Kontrolü!",
                        mesaj = $"{item.DisiHayvan?.KupeNumarasi} tohumlanalı {gecen} gün oldu. ({ayar.KizginlikBaslangicGun}-{ayar.KizginlikBitisGun}. gün aralığında)",
                        url = $"/Tohumlanmalar/Edit/{item.Id}", // Direkt düzenlemeye git
                        btnMetni = "Kontrol Et"
                    });
                }

                // B) GEBELİK TESTİ ZAMANI (Dinamik Gün)
                var testBekleyenler = await _context.Tohumlanmalar
                    .Include(t => t.DisiHayvan)
                    .Where(t => t.GebelikTestDurumu == TestDurumu.Yapilmadi &&
                                t.DisiHayvan.Aktif &&
                                t.TohumlanmaTarihi <= today.AddDays(-ayar.GebelikTestiGun))
                    .ToListAsync();

                foreach (var item in testBekleyenler)
                {
                    var gecen = (today - item.TohumlanmaTarihi).Days;
                    bildirimler.Add(new
                    {
                        tur = "info",
                        ikon = "fa-flask",
                        baslik = "Gebelik Testi",
                        mesaj = $"{item.DisiHayvan?.KupeNumarasi} için {gecen} gün geçti. Test yapılmalı.",
                        url = $"/Tohumlanmalar/Edit/{item.Id}",
                        btnMetni = "Sonuç Gir"
                    });
                }

                // C) İLAÇLI SÜT UYARISI (Tedavi Kayıtlarına Göre)
                var riskliSutler = await _context.TedaviKayitlari
                    .Include(t => t.Hayvan)
                    .Where(t => t.Hayvan.Aktif &&
                                t.SutArinmaTarihi.HasValue &&
                                t.SutArinmaTarihi >= today)
                    .OrderBy(t => t.SutArinmaTarihi)
                    .ToListAsync();

                foreach (var item in riskliSutler)
                {
                    var kalan = (item.SutArinmaTarihi.Value - today).Days;
                    string msj = kalan == 0 ? "BUGÜN SON!" : $"{kalan} gün daha dökülmeli.";

                    bildirimler.Add(new
                    {
                        tur = "danger",
                        ikon = "fa-ban",
                        baslik = "İlaçlı Süt!",
                        mesaj = $"{item.Hayvan?.KupeNumarasi} ({item.KullanilanIlac}). {msj}",
                        url = $"/Tedavi/Edit/{item.Id}",
                        btnMetni = "Detay"
                    });
                }

                // D) BUZAĞI SÜTTEN KESME (Dinamik Gün)
                // Son 6 ayda doğanlar
                var buzagilar = await _context.Hayvanlar
                    .Where(h => h.Aktif && h.DogumTarihi.HasValue && h.DogumTarihi >= today.AddDays(-180))
                    .ToListAsync();

                foreach (var h in buzagilar)
                {
                    var yasGun = (today - h.DogumTarihi.Value).Days;
                    var hedefGun = ayar.BuzagiSuttenKesmeGunu;

                    // Eğer tam gününde veya ertesi günse (2 günlük uyarı)
                    if (yasGun >= hedefGun && yasGun < hedefGun + 2)
                    {
                        bildirimler.Add(new
                        {
                            tur = "info", // Mavi
                            ikon = "fa-cow",
                            baslik = "Sütten Kesme Zamanı!",
                            mesaj = $"{h.KupeNumarasi} nolu buzağı {yasGun} günlük oldu. (Hedef: {hedefGun})",
                            url = "/Buzagi/Index",
                            btnMetni = "Takip Paneli"
                        });
                    }
                }

                // E) DOĞUM YAKLAŞTI UYARISI (Dinamik Gebelik Süresi ve Uyarı Aralığı)
                var dogumYaklasanlar = await _context.Tohumlanmalar
                    .Include(t => t.DisiHayvan)
                    .Where(t => t.GebelikTestDurumu == TestDurumu.Yapildi &&
                                t.GebelikTestiSonucu == GebelikSonucu.Gebe &&
                                t.DisiHayvan.Aktif &&
                                t.SonlanmaTuru == GebelikSonlanmaTuru.DevamEdiyor)
                    .ToListAsync();

                foreach (var item in dogumYaklasanlar)
                {
                    // Tahmini Doğum = Tohumlama + Standart Süre (örn: 280)
                    var tahminiDogum = item.TohumlanmaTarihi.AddDays(ayar.StandartGebelikSuresi);
                    var kalanGun = (tahminiDogum - today).Days;

                    // Ayarlanan uyarı gününe (örn: 15 gün kala) girdiyse
                    if (kalanGun <= ayar.DogumOncesiBildirimGunu && kalanGun >= 0)
                    {
                        string aciliyet = kalanGun <= 3 ? "KRİTİK" : "Yaklaşıyor";
                        string renk = kalanGun <= 3 ? "danger" : "warning"; // Son 3 gün kırmızı

                        bildirimler.Add(new
                        {
                            tur = renk,
                            ikon = "fa-baby-carriage",
                            baslik = $"Doğum {aciliyet}!",
                            mesaj = $"{item.DisiHayvan?.KupeNumarasi} doğuma {kalanGun} gün kaldı. Hazırlıkları yapın.",
                            url = "/Tohumlanmalar/Gebeler",
                            btnMetni = "Görüntüle"
                        });
                    }
                    // Eğer gün geçmişse
                    else if (kalanGun < 0)
                    {
                        bildirimler.Add(new
                        {
                            tur = "danger",
                            ikon = "fa-exclamation-triangle",
                            baslik = "Doğum Gecikti!",
                            mesaj = $"{item.DisiHayvan?.KupeNumarasi} tahmini tarihi {Math.Abs(kalanGun)} gün geçti. Kontrol edin!",
                            url = "/Tohumlanmalar/Gebeler",
                            btnMetni = "Kontrol Et"
                        });
                    }
                }

                return Json(bildirimler);
            }
            catch (Exception)
            {
                // Hata durumunda boş liste dön, sistemi kilitleme
                return Json(new List<object>());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}