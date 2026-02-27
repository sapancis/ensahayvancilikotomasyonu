# 🚜 Ensa Hayvancılık Otomasyonu (v2.0)

**Geliştirici:** [@sapancis](https://github.com/sapancis)  
**Teknoloji:** ASP.NET Core 8.0 MVC & Entity Framework Core

Bu proje; bir hayvancılık işletmesinin **finansal, biyolojik ve operasyonel** tüm süreçlerini tek bir merkezden yönetmek için geliştirilmiştir. Sadece veri kaydı tutmakla kalmaz, algoritmaları sayesinde çiftlik yönetimine kritik bildirimler sunar.

## 🚀 Öne Çıkan Gelişmiş Özellikler

### 📊 Finansal Zeka ve Raporlama (`FinansController`)
* **Net Kâr Analizi:** Süt satışları ve hayvan ticaretinden gelen gelirleri; yem, ilaç ve veteriner giderleriyle karşılaştırarak anlık kârlılık raporu sunar.
* **Dinamik Filtreleme:** Aylık ve yıllık bazda gelir-gider dökümü.

### 🥛 Akıllı Süt Yönetimi (`SutController` & `IstatistikController`)
* **Vardiyalı Takip:** Sabah ve akşam sağım miktarlarını ayrı ayrı kayıt altına alır.
* **Verim Grafikleri:** Chart.js entegrasyonu ile üretim trendlerini görselleştirir.
* **Süt Çıkış Yönetimi:** Sütün satış, buzağı besleme veya işletme içi tüketim yollarını takip eder.

### 👶 Biyolojik Evre ve Buzağı Takibi (`BuzagiController`)
* **Otomatik Sütten Kesme:** Sistem ayarlarındaki güne göre buzağıları otomatik olarak "Süt Emenler" ve "Sütten Kesilenler" olarak sınıflandırır.
* **Gelişim İzleme:** Doğumdan itibaren kritik 6 aylık periyodu özel bir panelde yönetir.

### 🩺 Üreme ve Sağlık Otomasyonu (`TohumlanmaController`)
* **Gebelik Algoritması:** Tohumlama tarihinden itibaren tahmini doğum tarihini (TDT) otomatik hesaplar.
* **Kritik Bildirimler:** Yaklaşan doğumlar ve yapılması gereken gebelik testleri için ana sayfada (Dashboard) akıllı uyarılar çıkarır.
* **Gıda Güvenliği:** Tedavi gören hayvanların ilaç arınma sürelerini (süt/et) takip ederek riskli ürün kullanımını engeller.

## 🛠️ Teknik Mimari
* **Backend:** C# / .NET 8 MVC
* **Veritabanı:** MS SQL Server & EF Core (Code First)
* **Frontend:** Responsive CSS (Grid/Flexbox), JavaScript, FontAwesome 6, Chart.js
* **Güvenlik:** Role-based Authorization & Authentication

## 💻 Kurulum Talimatları
1. Projeyi klonlayın: `git clone https://github.com/sapancis/ensahayvancilikotomasyonu.git`
2. `appsettings.json` dosyasındaki `DefaultConnection` bilgisini kendi SQL Server'ınıza göre güncelleyin.
3. Package Manager Console'da `Update-Database` komutunu çalıştırın.
4. `dotnet run` ile projeyi başlatın.

---
