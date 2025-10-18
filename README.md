# 🐾 PetTrack - Akıllı Evcil Hayvan Takip Sistemi

## 📘 Proje Özeti
**PetTrack**, evcil hayvanların **konum, sağlık ve aktivite takibini** sağlayan bir .NET 8 projesidir.  
Katmanlı mimari (Clean Architecture) prensiplerine göre geliştirilmiştir.  

Uygulama; sahip, cihaz, sağlık kaydı ve yürüyüş aktiviteleri gibi bilgileri yönetir.  
Modern bir yapıda Entity Framework Core kullanılarak veri erişimi, repository pattern ile soyutlanmıştır.

---

## 🏗️ Mimarinin Genel Yapısı
Proje **4 ana katmandan** oluşur:

| Katman | Görev |
|--------|-------|
| 🧠 **Domain** | Temel iş kuralları, varlıklar (Entities), enumlar ve değer nesneleri |
| ⚙️ **Application** | Servisler, DTO’lar, validasyonlar ve iş akışı yönetimi |
| 🗄️ **Infrastructure** | EF Core Context, Repository, Migration, Seed ve UnitOfWork yapısı |
| 💻 **UI (Console)** | Kullanıcı arayüzü: menü tabanlı etkileşim (CLI) |

---

## 🧩 Öne Çıkan Özellikler
- 🧱 **Clean Architecture** yapısı (katmanlar arası bağımsızlık)
- 🔄 **Repository & Unit of Work Pattern**
- 🐾 **Pet, Owner, Tracker, Activity, HealthRecord** domain model yapısı
- 🧮 **LINQ tabanlı raporlar** (ör. En çok yürüyen pet, sıcaklık ortalaması)
- ⏳ **Soft Delete** (Status + DeletedDate)
- 🧰 **Seed Data** (örnek başlangıç verileri)
- 💾 **EF Core Migration & SQL Server** desteği
- ⚠️ **Custom Exception & Validation Helper** altyapısı
- 🧑‍💻 **Console UI Menü** ile CRUD işlemleri

---

## 🧱 Kullanılan Teknolojiler
| Teknoloji | Amaç |
|------------|------|
| .NET 8 | Uygulama platformu |
| C# 12 | Kodlama dili |
| Entity Framework Core | ORM yapısı |
| SQL Server | Veritabanı yönetimi |
| LINQ | Veri sorgulama işlemleri |
| Dependency Injection | Bağımlılık yönetimi |
| Git + GitHub | Versiyon kontrolü |

---

## 📂 Klasör Yapısı
PetTrack/
│
├── PetTrack.Domain/
│ ├── Entities/
│ ├── Enums/
│ └── BaseEntity.cs
│
├── PetTrack.Application/
│ ├── DTOs/
│ ├── Services/
│ ├── Exceptions/
│ └── Validation/
│
├── PetTrack.Infrastructure/
│ ├── Context/
│ ├── Repository/
│ ├── Seed/
│ └── UoW/
│
└── PetTrack.ConsoleUI/
└── Program.cs
