# 🐾 PetTrack - Akıllı Evcil Hayvan Takip Sistemi

## 📘 Proje Özeti
**PetTrack**, evcil hayvan sahiplerinin dostlarını daha kolay takip edebilmesi için geliştirilmiş bir .NET 8 tabanlı sistemdir.  
Proje, **katmanlı mimari (Clean Architecture)** yapısıyla geliştirilmiştir.  
Amaç: evcil hayvanların **aktivite, sağlık, konum** ve **acil durum** bilgilerini kaydetmek, analiz etmek ve raporlamaktır.

---

## 🏗️ Mimarinin Genel Yapısı
Proje, **bağımlılıkları azaltmak ve yönetilebilirliği artırmak** amacıyla 4 katmanlı olarak tasarlanmıştır:

| Katman | Açıklama |
|--------|-----------|
| 🧠 **Domain** | Temel varlıkların (Entities), kuralların ve enum’ların bulunduğu katmandır. İşin kalbidir. |
| ⚙️ **Application** | Servisler, DTO’lar, iş akışları, validasyonlar ve hata yönetimi burada yer alır. |
| 🗄️ **Infrastructure** | Veritabanı erişimi, Repository, Unit of Work, Migration ve Seed işlemleri buradadır. |
| 💻 **UI (Console)** | Konsol arayüzü üzerinden kullanıcı ile etkileşimin sağlandığı kısımdır. |

---

## 🧩 Öne Çıkan Özellikler
- ✅ **Clean Architecture** prensipleriyle katman bağımsızlığı  
- 🧱 **Repository + Unit of Work Pattern**  
- 🐶 **Pet, Owner, TrackerDevice, HealthRecord, ActivityLog, VetAppointment** domain yapıları  
- ⚙️ **ValidationHelper** ile merkezi doğrulama yapısı  
- 💾 **Soft Delete (IsDeleted / Status / DeletedDate)**  
- 🔥 **Seed Data** ile örnek veri yükleme  
- 🧮 **LINQ tabanlı raporlama** (en aktif pet, sıcaklık ortalaması vb.)  
- 🧰 **Custom Exception** altyapısı  
- 🧑‍💻 **Console Menü** üzerinden CRUD işlemleri  
- 🧬 **EF Core Migration** desteği  
- 🔗 **SQL Server bağlantısı**

---

🐾 PETTRACK SİSTEMİ
--------------------------
1. Evcil Hayvanları Listele
2. Yeni Hayvan Ekle
3. Aktivite Raporlarını Gör
4. Sağlık Kayıtlarını Yönet
5. Veteriner Randevuları
0. Çıkış

## 📚 Kullanılan Teknolojiler

| Teknoloji | Amaç |
|------------|------|
| .NET 8 | Uygulama çatısı |
| C# 12 | Programlama dili |
| Entity Framework Core | ORM yapısı |
| SQL Server | Veritabanı |
| LINQ | Veri sorgulama |
| Dependency Injection | Bağımlılık çözümü |
| Git & GitHub | Versiyon kontrolü |
| JSON | Veri saklama (Seed dosyaları) |

---

## 🗂️ Proje Klasör Yapısı
```
```bash
📦 PetTrack
├── 📁 Domain
│   ├── 🧩 Entities
│   │   ├── Pet.cs
│   │   ├── PetOwner.cs
│   │   ├── TrackerDevice.cs
│   │   ├── ActivityLog.cs
│   │   ├── HealthRecord.cs
│   │   ├── VetAppointment.cs
│   │   └── Alert.cs
│   ├── ⚙️ Enums
│   │   ├── EntityStatus.cs
│   │   └── PetFamily.cs
│   └── 🧠 BaseEntity.cs
│
├── 📁 Application
│   ├── 💾 DTOs
│   │   ├── PetDto.cs
│   │   └── TrackerDeviceDto.cs
│   ├── 🔌 Interfaces
│   │   ├── IRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── 🧮 Services
│   │   ├── PetService.cs
│   │   ├── AlertService.cs
│   │   └── HealthService.cs
│   └── ⚠️ Exceptions
│       ├── NotFoundException.cs
│       ├── ValidationException.cs
│       └── DuplicateException.cs
│
├── 📁 Infrastructure
│   ├── 🗄️ Context
│   │   ├── AppDbContext.cs
│   │   └── DbInitializer.cs
│   ├── 📚 Repository
│   │   ├── GenericRepository.cs
│   │   ├── PetRepository.cs
│   │   └── TrackerRepository.cs
│   ├── 🔁 UnitOfWork.cs
│   └── 🌱 Seed
│       ├── PetSeedData.cs
│       ├── OwnerSeedData.cs
│       └── TrackerSeedData.cs
│
└── 📁 ConsoleUI
    ├── 🖥️ Program.cs
    ├── 📜 MenuScreen.cs
    ├── 📊 ReportsScreen.cs
    └── 🧩 Helpers
        └── ValidationHelper.cs
