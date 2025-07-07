# Takvim Uygulaması (Calendar Application)

Modern, minimalist tasarımlı WPF (.NET 8) takvim uygulaması. Pastel renk paleti, karanlık mod desteği ve Windows bildirimleri ile.

## Özellikler

### 📅 Takvim Görünümleri
- **Günlük Görünüm**: Seçilen günün detaylı etkinlik listesi
- **Haftalık Görünüm**: 7 günlük zaman çizelgesi görünümü
- **Aylık Görünüm**: Geleneksel aylık takvim görünümü

### ✨ Etkinlik Yönetimi
- Etkinlik ekleme, düzenleme ve silme
- Başlık, açıklama, tarih/saat bilgileri
- Kategori seçimi (İş, Kişisel, Toplantı, vb.)
- Pastel renk seçenekleri
- Tüm gün etkinlikleri desteği

### 🔔 Bildirim Sistemi
- Windows toast bildirimleri
- Özelleştirilebilir hatırlatma süreleri (5 dk - 1 gün)
- Otomatik bildirim zamanlaması
- Bildirim testi özelliği

### 💾 Veri Saklama
- JSON dosyasına otomatik kaydetme
- `%AppData%/CalendarAppWPF/events.json` konumunda
- Uygulama kapatılıp açıldığında veriler korunur

### 🎨 Modern Tasarım
- Pastel renk paleti (Mavi, Pembe, Yeşil, Sarı, Mor, Turuncu)
- Karanlık/Aydınlık mod desteği
- Material Design ve MahApps.Metro entegrasyonu
- Responsive tasarım
- Minimalist ve kullanıcı dostu arayüz

## Teknolojiler

- **.NET 8.0** - Modern .NET framework
- **WPF** - Windows Presentation Foundation
- **MVVM Pattern** - Model-View-ViewModel mimarisi
- **CommunityToolkit.Mvvm** - MVVM yardımcı kütüphanesi
- **MahApps.Metro** - Modern UI bileşenleri
- **MaterialDesignThemes** - Material Design stilleri
- **Microsoft.Toolkit.Uwp.Notifications** - Windows bildirimleri
- **Newtonsoft.Json** - JSON serileştirme

## Kurulum

### Gereksinimler
- Windows 10/11
- .NET 8.0 Runtime

### Derleme
```bash
git clone [repository-url]
cd CalendarAppWPF
dotnet restore
dotnet build
dotnet run --project CalendarAppWPF
```

## Kullanım

### Etkinlik Ekleme
1. **"+ Etkinlik"** butonuna tıklayın veya **Dosya > Yeni Etkinlik** menüsünü kullanın
2. Etkinlik bilgilerini doldurun:
   - Başlık (zorunlu)
   - Açıklama (isteğe bağlı)
   - Başlangıç ve bitiş tarihi/saati
   - Kategori seçimi
   - Renk seçimi
   - Hatırlatma ayarları
3. **"Kaydet"** butonuna tıklayın

### Görünüm Değiştirme
- **Günlük**: Tek günün detaylı görünümü
- **Haftalık**: 7 günlük zaman çizelgesi
- **Aylık**: Geleneksel takvim görünümü

### Karanlık Mod
- Sağ üst köşedeki **🌙** butonuna tıklayın
- Veya **Görünüm > Karanlık Mod** menüsünü kullanın

### Bildirimler
- Etkinlik eklerken hatırlatma seçeneğini aktifleştirin
- Bildirim zamanını seçin (5 dk - 1 gün öncesi)
- **Araçlar > Bildirim Testi** ile test edebilirsiniz

## Dosya Yapısı

```
CalendarAppWPF/
├── Models/
│   └── Event.cs                 # Etkinlik veri modeli
├── Services/
│   ├── FileService.cs           # JSON dosya işlemleri
│   └── NotificationService.cs   # Windows bildirimleri
├── ViewModels/
│   └── CalendarViewModel.cs     # Ana takvim mantığı
├── Views/
│   ├── CalendarView.xaml        # Ana takvim görünümü
│   ├── AddEventWindow.xaml      # Etkinlik ekleme penceresi
│   └── WindowView.xaml          # Ana pencere
├── Resources/
│   └── Styles.xaml              # Özel stiller ve renkler
└── Application/
    ├── App.xaml                 # Uygulama kaynakları
    └── App.xaml.cs              # Uygulama başlatma
```

## Özelleştirme

### Renk Paleti
`Resources/Styles.xaml` dosyasında pastel renkler tanımlanmıştır:
```xml
<SolidColorBrush x:Key="PastelBlue" Color="#E8F4FD"/>
<SolidColorBrush x:Key="PastelPink" Color="#FFE8E8"/>
<!-- Diğer renkler... -->
```

### Yeni Kategori Ekleme
`Views/AddEventWindow.xaml` dosyasında kategori listesini genişletebilirsiniz:
```xml
<ComboBoxItem Content="Yeni Kategori"/>
```

### Bildirim Süreleri
`Views/AddEventWindow.xaml` dosyasında hatırlatma sürelerini değiştirebilirsiniz:
```xml
<ComboBoxItem Content="2 saat önce" Tag="120"/>
```

## Katkıda Bulunma

1. Repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

## Lisans

Bu proje açık kaynak kodludur ve MIT lisansı altında dağıtılmaktadır.

## Geliştirici

**Calendar Developer**  
© 2025 Tüm hakları saklıdır.

## Sürüm Geçmişi

### v1.0.0
- İlk sürüm
- Temel takvim işlevleri
- Etkinlik yönetimi
- Windows bildirimleri
- Karanlık mod desteği
- JSON veri saklama

## Bilinen Sorunlar

- Bildirimler Windows 10/11'de çalışır
- Uygulama sadece Windows platformunda desteklenir

## Gelecek Özellikler

- [ ] Etkinlik tekrarı (günlük, haftalık, aylık)
- [ ] Takvim içe/dışa aktarma (iCal formatı)
- [ ] Çoklu takvim desteği
- [ ] Etkinlik arama ve filtreleme
- [ ] Yazdırma özelliği
- [ ] Tema özelleştirme
- [ ] Çoklu dil desteği
