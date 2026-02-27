using Microsoft.EntityFrameworkCore;
namespace ciftlik2.Models
{
    public class CiftlikDbContext : DbContext
    {
        public CiftlikDbContext(DbContextOptions<CiftlikDbContext> options) : base(options) { }

        public DbSet<Hayvan> Hayvanlar { get; set; }
        public DbSet<YemAlisi> YemAlimlari { get; set; }
        public DbSet<AsiKaydi> AsiKayitlari { get; set; }
        public DbSet<TestKaydi> TestKayitlari { get; set; }
        public DbSet<TedaviKaydi> TedaviKayitlari { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Tohumlanma> Tohumlanmalar { get; set; }
        public DbSet<SutKaydi> SutKayitlari { get; set; }
        public DbSet<SutCikisi> SutCikislari { get; set; }
        public DbSet<FinansIslemi> FinansIslemleri { get; set; }
        public DbSet<SistemAyarlari> Ayarlar { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Hayvan>(entity =>
            {
                entity.HasIndex(h => h.KupeNumarasi).IsUnique();
                entity.Property(h => h.KupeNumarasi).IsRequired();
            });
            modelBuilder.Entity<SistemAyarlari>().HasData(
                new SistemAyarlari
                {
                    Id = 1,
                    KizginlikBaslangicGun = 18,
                    KizginlikBitisGun = 23,
                    GebelikTestiGun = 40
                }
            );
        }
    }
}