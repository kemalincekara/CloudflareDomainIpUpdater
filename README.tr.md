# Cloudflare Domain IP Updater


*For the English version, please see [README.md](README.md).*

Bu uygulama, Cloudflare API'sini kullanarak Cloudflare hesabınızdaki DNS kayıtlarını güncelleyen basit bir komut satırı aracıdır. Belirtilen "Eski_IP" değeriyle eşleşen tüm DNS kayıtları, "Yeni_IP" ile güncellenir.

## Özellikler

- **Otomatik DNS Güncelleme:** Cloudflare hesabınızdaki tüm zone'larda bulunan DNS kayıtlarını tarar.
- **IP Değişikliği:** Eğer bir DNS kaydının içeriği belirtilen "Eski_IP" ile eşleşiyorsa, kayıt "Yeni_IP" ile güncellenir.
- **Hata Yönetimi:** API çağrılarında oluşabilecek hatalar konsol üzerinden anlaşılır bir şekilde bildirilir.
- **Kullanıcı Dostu:** Yetersiz parametre girildiğinde, kullanım bilgilerini ekrana yazarak kullanıcıyı bilgilendirir.

## Gereksinimler

- [.NET 9 veya daha yeni bir .NET sürümü](https://dotnet.microsoft.com/download)
- [CloudFlare.Client](https://www.nuget.org/packages/CloudFlare.Client/) NuGet paketi
- Geçerli bir Cloudflare hesabı ve API erişim bilgileri (E-posta adresi ve Global API Key)

## Kurulum

1. Projeyi klonlayın veya indirin:
   ```bash
   git clone https://github.com/kemalincekara/CloudflareDomainIpUpdater.git
   ```
2. Proje dizinine geçin:
   ```bash
   cd CloudflareDomainIpUpdater
   ```
3. Gerekli NuGet paketlerini yükleyin:
   ```bash
   dotnet restore
   ```
4. Projeyi derleyin:
   ```bash
   dotnet build
   ```

## Kullanım

Uygulamayı çalıştırmak için aşağıdaki parametreleri komut satırında girmeniz gerekmektedir:

```bash
CloudflareDomainIpUpdater.exe <E_Posta> <Global_API_Key> <Eski_IP> <Yeni_IP>
```

**Parametre Açıklamaları:**

- `<E_Posta>`: Cloudflare hesabınıza ait e-posta adresi.
- `<Global_API_Key>`: Cloudflare Global API Anahtarınız.
- `<Eski_IP>`: Güncelleme yapılacak DNS kayıtlarında aranacak eski IP adresi.
- `<Yeni_IP>`: Bulunan DNS kayıtlarının güncelleneceği yeni IP adresi.

### Örnek

Aşağıdaki örnekte, `192.168.1.67` değeri `10.0.0.67` olarak güncellenecektir:

```bash
CloudflareDomainIpUpdater.exe user@example.com abcdef1234567890oldapikey 192.168.1.67 10.0.0.67
```

## Kod Açıklaması

- **Giriş Kontrolü:** Program başlangıcında, komut satırı argümanlarının sayısı kontrol edilir. Yeterli parametre girilmemişse, kullanım bilgileri gösterilir.
- **API Bağlantısı:** `CloudFlareClient` nesnesi, e-posta ve API anahtarı kullanılarak oluşturulur.
- **Zone ve DNS Kayıtları İşlemleri:** 
  - Cloudflare hesabındaki tüm zone'lar çekilir.
  - Her zone için, tüm DNS kayıtları sorgulanır.
  - Kayıt içeriği belirtilen eski IP ile eşleşiyorsa, yeni IP ile güncelleme yapılır.
- **Hata Yönetimi:** API çağrıları sırasında hata oluşursa, detaylı hata mesajları konsola yazdırılır.

## Katkıda Bulunma

Her türlü katkı, hata bildirimi veya geliştirme önerileri memnuniyetle karşılanır. Lütfen bir pull request açmadan önce ilgili konuyu tartışmak üzere bir issue oluşturun.

## Lisans

Bu proje MIT Lisansı kapsamında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına göz atabilirsiniz.