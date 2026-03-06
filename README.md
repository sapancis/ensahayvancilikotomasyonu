# 🚜 Ensa Hayvancılık Otomasyonu 
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

<img width="1397" height="901" alt="wvftZk" src="https://github.com/user-attachments/assets/70e48c8a-511f-4c46-bb6c-2a63c4d277c6" /><img width="1286" height="769" alt="wvf8Nc" src="https://github.com/user-attachments/assets/1adc131b-914f-40ec-83eb-45a664860a66" />
<img width="1304" height="810" alt="wvfe8y" src="https://github.com/user-attachments/assets/57a3b24f-a413-4706-bd10-8ffe8801703f" />
<img width="1341" height="749" alt="wvf39H" src="https://github.com/user-attachments/assets/156e985a-a5fc-4e40-a31f-80994bb62a0a" />
<img width="1294" height="851" alt="wvf0Hb" src="https://github.com/user-attachments/assets/b053048f-a92e-4037-8177-0eb9507156d0" />
<img width="1268" height="664" alt="wvf596" src="https://github.com/user-attachments/assets/f21f121e-b904-4fc8-a793-ede3deac251e" />
<img width="1306" height="679" alt="wvfsT8" src="https://github.com/user-attachments/assets/3db5a6b1-9706-40fd-b298-5f7ff154cc74" />



