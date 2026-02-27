using ciftlik2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ciftlik2.Controllers
{
    public class BuzagiController : Controller
    {
        private readonly CiftlikDbContext _context;

        public BuzagiController(CiftlikDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Ayarları Çek
            var ayar = await _context.Ayarlar.FirstOrDefaultAsync() ?? new SistemAyarlari();
            ViewBag.KesmeGunu = ayar.BuzagiSuttenKesmeGunu;

            // 2. Buzağıları Çek (0-6 Ay arası yani 180 gün)
            var limitTarih = DateTime.Today.AddDays(-180);

            var tumBuzagilar = await _context.Hayvanlar
                .Where(h => h.Aktif && h.DogumTarihi.HasValue && h.DogumTarihi >= limitTarih)
                .OrderByDescending(h => h.DogumTarihi)
                .ToListAsync();

            // 3. Listeyi İkiye Böl
            var model = new BuzagiViewModel
            {
                SutEmenler = new List<Hayvan>(),
                SuttenKesilenler = new List<Hayvan>()
            };

            foreach (var h in tumBuzagilar)
            {
                var gunlukYas = (DateTime.Today - h.DogumTarihi.Value).Days;
                if (gunlukYas < ayar.BuzagiSuttenKesmeGunu)
                {
                    model.SutEmenler.Add(h);
                }
                else
                {
                    model.SuttenKesilenler.Add(h);
                }
            }

            return View(model);
        }
    }

    // Basit ViewModel (Aynı dosyanın altına veya Models klasörüne koyabilirsin)
    public class BuzagiViewModel
    {
        public List<Hayvan> SutEmenler { get; set; }
        public List<Hayvan> SuttenKesilenler { get; set; }
    }
}