using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel; // Excel için gerekli kütüphane

namespace ciftlik2.Controllers
{
    public class AsiController : Controller
    {
        private readonly CiftlikDbContext _context;

        public AsiController(CiftlikDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME (INDEX)
        public async Task<IActionResult> Index()
        {
            var asilar = await _context.AsiKayitlari
                                    .Include(a => a.Hayvan)
                                    .OrderByDescending(a => a.AsiTarihi)
                                    .ToListAsync();
            return View(asilar);
        }

        // 2. YENİ KAYIT (GET)
        public IActionResult Create()
        {
            HayvanListesiniYukle();
            return View();
        }

        // 3. YENİ KAYIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HayvanId,AsiIsmi,AsiTarihi")] AsiKaydi asiKaydi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asiKaydi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            HayvanListesiniYukle();
            return View(asiKaydi);
        }

        // 4. TOPLU AŞI (GET)
        public async Task<IActionResult> TopluAsi()
        {
            // Sadece aktif hayvanları listeye gönder
            var aktifHayvanlar = await _context.Hayvanlar
                .Where(h => h.Aktif)
                .OrderBy(h => h.KupeNumarasi)
                .ToListAsync();

            ViewBag.HayvanListesi = aktifHayvanlar;
            return View();
        }

        // 5. TOPLU AŞI (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopluAsi(TopluAsiViewModel model, List<int> secilenHayvanIdleri)
        {
            if (secilenHayvanIdleri == null || !secilenHayvanIdleri.Any())
            {
                ModelState.AddModelError("", "Lütfen en az bir hayvan seçiniz.");
            }

            if (ModelState.IsValid)
            {
                // Seçilen her hayvan için yeni bir kayıt oluştur
                foreach (var id in secilenHayvanIdleri)
                {
                    var yeniKayit = new AsiKaydi
                    {
                        HayvanId = id,
                        AsiIsmi = model.AsiIsmi,
                        AsiTarihi = model.AsiTarihi
                    };
                    _context.Add(yeniKayit);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa listeyi tekrar yükle
            ViewBag.HayvanListesi = await _context.Hayvanlar
                .Where(h => h.Aktif)
                .OrderBy(h => h.KupeNumarasi)
                .ToListAsync();

            return View(model);
        }

        // 6. DÜZENLEME (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var asiKaydi = await _context.AsiKayitlari.FindAsync(id);
            if (asiKaydi == null) return NotFound();

            HayvanListesiniYukle(asiKaydi.HayvanId);
            return View(asiKaydi);
        }

        // 7. DÜZENLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HayvanId,AsiIsmi,AsiTarihi")] AsiKaydi asiKaydi)
        {
            if (id != asiKaydi.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asiKaydi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AsiKayitlari.Any(e => e.Id == asiKaydi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            HayvanListesiniYukle(asiKaydi.HayvanId);
            return View(asiKaydi);
        }

        // 8. SİLME (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var asiKaydi = await _context.AsiKayitlari
                .Include(a => a.Hayvan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asiKaydi == null) return NotFound();
            return View(asiKaydi);
        }

        // 9. SİLME (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asiKaydi = await _context.AsiKayitlari.FindAsync(id);
            if (asiKaydi != null)
            {
                _context.AsiKayitlari.Remove(asiKaydi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 10. EXCEL'E AKTAR (ClosedXML Kullanarak)
        public async Task<IActionResult> ExportToExcel()
        {
            // Veriyi Çek
            var veriler = await _context.AsiKayitlari
                .Include(a => a.Hayvan)
                .OrderByDescending(a => a.AsiTarihi)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Aşı Listesi");

                // --- BAŞLIKLAR ---
                worksheet.Cell(1, 1).Value = "Tarih";
                worksheet.Cell(1, 2).Value = "Küpe Numarası";
                worksheet.Cell(1, 3).Value = "Hayvan Adı";
                worksheet.Cell(1, 4).Value = "Aşı İsmi";

                // Başlık Stili
                var baslikRange = worksheet.Range("A1:D1");
                baslikRange.Style.Font.Bold = true;
                baslikRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                baslikRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // --- VERİLERİ DOLDUR ---
                int row = 2;
                foreach (var item in veriler)
                {
                    worksheet.Cell(row, 1).Value = item.AsiTarihi.HasValue ? item.AsiTarihi.Value.ToString("dd.MM.yyyy") : "-";
                    worksheet.Cell(row, 2).Value = item.Hayvan?.KupeNumarasi ?? "-";
                    worksheet.Cell(row, 3).Value = item.Hayvan?.Ad ?? "-";
                    worksheet.Cell(row, 4).Value = item.AsiIsmi;

                    // Hücreleri ortala
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    row++;
                }

                // Sütun genişliklerini içeriğe göre ayarla
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string dosyaAdi = $"Asi_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dosyaAdi);
                }
            }
        }

        // YARDIMCI METOT: Dropdown Listesini Doldurur
        private void HayvanListesiniYukle(object? selectedId = null)
        {
            var liste = _context.Hayvanlar
                .Where(h => h.Aktif)
                .OrderBy(h => h.KupeNumarasi)
                .Select(h => new {
                    Id = h.Id,
                    Gorunen = h.KupeNumarasi + (h.Ad != null ? " - " + h.Ad : "")
                }).ToList();

            ViewBag.HayvanListesi = new SelectList(liste, "Id", "Gorunen", selectedId);
        }
    }
}