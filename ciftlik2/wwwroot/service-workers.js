const CACHE_NAME = 'ciftlik-v1';
const urlsToCache = [
    '/',
    '/Home/Index',
    '/css/site.css',
    '/js/site.js',
    '/images/app-icon-192.png',
    '/images/app-icon-512.png',
    '/manifest.json'
];

// Yükleme (Install)
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('Cache açıldı');
                return cache.addAll(urlsToCache);
            })
    );
});

// İstek Yakalama (Fetch)
self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                // Cache'de varsa döndür, yoksa internetten çek
                if (response) {
                    return response;
                }
                return fetch(event.request);
            })
    );
});

// Güncelleme (Activate)
self.addEventListener('activate', event => {
    const cacheWhitelist = [CACHE_NAME];
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheWhitelist.indexOf(cacheName) === -1) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});