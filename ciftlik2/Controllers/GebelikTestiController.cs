using ciftlik2.Models;
using ciftlik2.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class GebelikTestiController : Controller
    {
        private readonly CiftlikDbContext _context;

        public GebelikTestiController(CiftlikDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var yapilmamisTestler = await _context.Tohumlanmalar
                                                .Include(t => t.DisiHayvan).Include(t => t.ErkekHayvan)
                                                .Where(t => t.GebelikTestDurumu == TestDurumu.Yapilmadi)
                                                .OrderBy(t => t.TohumlanmaTarihi).ToListAsync();
            return View(yapilmamisTestler);
        }

        public async Task<IActionResult> Yap(int? id)
        {
            if (id == null) return NotFound();
            var tohumlanma = await _context.Tohumlanmalar
                                        .Include(t => t.DisiHayvan).Include(t => t.ErkekHayvan)
                                        .FirstOrDefaultAsync(m => m.Id == id && m.GebelikTestDurumu == TestDurumu.Yapilmadi);
            if (tohumlanma == null) return NotFound();
            return View(tohumlanma);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Yap(int id, [Bind("Id,GebelikTestiSonucu")] Tohumlanma gelenVeri)
        {
            if (id != gelenVeri.Id) return NotFound();

            var mevcutTohumlanma = await _context.Tohumlanmalar
                .Include(t => t.DisiHayvan).Include(t => t.ErkekHayvan)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (mevcutTohumlanma == null) return NotFound();

            if (gelenVeri.GebelikTestiSonucu != null)
            {
                mevcutTohumlanma.GebelikTestiSonucu = gelenVeri.GebelikTestiSonucu;
                mevcutTohumlanma.GebelikTestDurumu = TestDurumu.Yapildi;

                try { _context.Update(mevcutTohumlanma); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(mevcutTohumlanma);
        }
    }
}