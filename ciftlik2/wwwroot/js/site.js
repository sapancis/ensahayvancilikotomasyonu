/* ============================================================
   ÇİFTLİK YÖNETİM SİSTEMİ - GLOBAL SCRIPTS
   (Vanilla JS - jQuery Bağımlılığı Yoktur)
   ============================================================ */

document.addEventListener("DOMContentLoaded", function () {

    // 1. MOBİL MENÜ AÇMA / KAPAMA
    // ------------------------------------------------------------
    const menuToggle = document.querySelector(".menu-toggle");
    const navMenu = document.querySelector(".nav-menu");

    if (menuToggle && navMenu) {
        menuToggle.addEventListener("click", function () {
            navMenu.classList.toggle("active");
        });
    }

    // 2. MOBİL DROPDOWN (Açılır Menü)
    // ------------------------------------------------------------
    // Sadece ekran genişliği 992px ve altındaysa tıklama ile çalışır.
    const dropdownToggles = document.querySelectorAll(".dropdown-toggle");

    dropdownToggles.forEach(function (btn) {
        btn.addEventListener("click", function (e) {
            // CSS'teki @media (max-width: 992px) ile uyumlu olmalı
            if (window.innerWidth <= 992) {
                e.preventDefault(); // Linke gitmeyi engelle, menüyü aç

                // Tıklanan butonun ebeveynine (li.nav-item) active sınıfı ekle/çıkar
                const parentItem = this.parentElement;
                parentItem.classList.toggle("active");
            }
        });
    });

    // 3. PENCERE BOYUTLANDIRMA (RESIZE) KONTROLÜ
    // ------------------------------------------------------------
    // Masaüstü görünümüne geçildiğinde mobil menü kalıntılarını temizler.
    window.addEventListener("resize", function () {
        if (window.innerWidth > 992) {
            // Ana menüyü kapat
            if (navMenu) navMenu.classList.remove("active");

            // Açık olan dropdownları kapat
            document.querySelectorAll(".nav-item.active").forEach(function (item) {
                item.classList.remove("active");
            });
        }
    });

    // 4. AKORDİYON BAŞLANGIÇ AYARI
    // ------------------------------------------------------------
    // Sayfada akordiyon varsa, ilkini otomatik açık hale getirir.
    const firstAccordion = document.querySelector('.accordion-item');
    if (firstAccordion) {
        firstAccordion.classList.add('active');
    }

    // 5. DELETE ONAYI (İsteğe Bağlı Global Kontrol)
    // ------------------------------------------------------------
    // data-confirm attribute'u olan formlarda submit öncesi onay ister.
    const confirmForms = document.querySelectorAll('form[data-confirm]');
    confirmForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const message = this.getAttribute('data-confirm') || 'Bu işlemi yapmak istediğinize emin misiniz?';
            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });
});

/* ============================================================
   GLOBAL FONKSİYONLAR (HTML içinden onclick="" ile çağrılanlar)
   ============================================================ */

/**
 * Akordiyon başlığına tıklandığında içeriği açar/kapatır.
 * @param {HTMLElement} header - Tıklanan başlık elementi
 */
function toggleAccordion(header) {
    const item = header.parentElement;

    // İsteğe bağlı: Diğerlerini kapatmak isterseniz bu satırı açın:
    // document.querySelectorAll('.accordion-item').forEach(el => el !== item && el.classList.remove('active'));

    item.classList.toggle('active');
}

/**
 * Gebelik Testi Modalı için ID ve Küpe Numarası taşır.
 * (Tohumlanma/Index sayfasında kullanılır)
 */
function setTestId(id, kupeNo) {
    const hiddenInput = document.getElementById('hiddenTohumlanmaId');
    const infoText = document.getElementById('modalHayvanBilgisi');

    if (hiddenInput) hiddenInput.value = id;
    if (infoText) infoText.innerText = kupeNo;
}