using System;
using System.Linq;
using PetTrack.Domain.Entities;
using PetTrack.Domain.Enums;
using PetTrack.Infrastructure.Contexts;

namespace PetTrack.Infrastructure.Seed
{
    public static class DbSeeder
    {
        /// <summary>
        /// Mevcut veriyi korur, her tabloyu 10 kayda tamamlar.
        /// </summary>
        public static void Seed(AppDbContext ctx)
        {
            // =================== PET OWNERS ===================
            var ownerCnt = ctx.PetOwners.Count();
            for (int i = ownerCnt + 1; i <= 10; i++)
            {
                var email = $"owner{i}@example.com";
                if (!ctx.PetOwners.Any(x => x.Email == email))
                {
                    ctx.PetOwners.Add(new PetOwner
                    {
                        Name = $"Owner {i}",
                        Email = email,
                        Phone = $"+90 555 000 0{i:00}",
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }
            ctx.SaveChanges();

            // =================== PETS ===================
            var petCnt = ctx.Pets.Count();
            var ownerIds = ctx.PetOwners.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            for (int i = petCnt + 1; i <= 10; i++)
            {
                var ownerId = ownerIds[(i - 1) % ownerIds.Count];
                ctx.Pets.Add(new Pet
                {
                    Name = $"Pet {i}",
                    SpeciesName = (i % 2 == 0 ? "Dog" : "Cat"),
                    Family = (i % 3 == 0 ? PetFamily.Bird : PetFamily.Mammal),
                    OwnerId = ownerId,
                    CreatedDate = DateTime.UtcNow
                });
            }
            ctx.SaveChanges();

            // =================== DEVICES ===================
            var devCnt = ctx.TrackerDevices.Count();
            var petIds = ctx.Pets.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            var usedPets = ctx.TrackerDevices.Select(d => d.PetId).ToHashSet();

            int target = Math.Min(10, petIds.Count);
            int need = Math.Max(0, target - devCnt);

            if (need > 0)
            {
                var availablePetIds = petIds.Where(pid => !usedPets.Contains(pid))
                                            .Take(need)
                                            .ToList();

                int serialNum = 1;
                string NextSerial()
                {
                    while (true)
                    {
                        var s = $"TRK-{serialNum:0000}";
                        serialNum++;
                        if (!ctx.TrackerDevices.Any(d => d.Serial == s)) return s;
                    }
                }

                foreach (var pid in availablePetIds)
                {
                    ctx.TrackerDevices.Add(new TrackerDevice
                    {
                        Serial = NextSerial(),
                        PetId = pid,
                        CreatedDate = DateTime.UtcNow
                    });
                }
                ctx.SaveChanges();
            }

            // =================== ACTIVITY LOGS ===================
            var logCnt = ctx.ActivityLogs.Count();
            var deviceIds = ctx.TrackerDevices.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            for (int i = logCnt + 1; i <= 10 && deviceIds.Any(); i++)
            {
                var deviceId = deviceIds[(i - 1) % deviceIds.Count];
                ctx.ActivityLogs.Add(new ActivityLog
                {
                    DeviceId = deviceId,
                    Date = DateTime.UtcNow.Date.AddHours(8 + (i % 10)),
                    Steps = 500 + i * 250,
                    Temperature = 37.0 + (i * 0.1),
                    CreatedDate = DateTime.UtcNow
                });
            }
            ctx.SaveChanges();

            // =================== ALERTS ===================
            var alertCnt = ctx.Alerts.Count();
            petIds = ctx.Pets.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            for (int i = alertCnt + 1; i <= 10 && petIds.Any(); i++)
            {
                var petId = petIds[(i - 1) % petIds.Count];
                ctx.Alerts.Add(new Alert
                {
                    PetId = petId,
                    Type = (i % 2 == 0 ? AlertType.LowActivity : AlertType.HighTemperature),
                    Message = $"Auto alert {i}",
                    CreatedDate = DateTime.UtcNow
                });
            }
            ctx.SaveChanges();

            // =================== VET APPOINTMENTS ===================
            // Çakışma yok: A.Start < B.End && B.Start < A.End koşulunu sağlayan var ise yeni saat arıyoruz.
            var apptCnt = ctx.VetAppointments.Count();
            petIds = ctx.Pets.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            DateTime baseDay = DateTime.UtcNow.Date.AddDays(1).AddHours(9); // yarın 09:00'dan başla

            for (int i = apptCnt + 1; i <= 10 && petIds.Any(); i++)
            {
                var petId = petIds[(i - 1) % petIds.Count];
                var start = baseDay.AddDays(i);       // her i için farklı gün
                var end = start.AddMinutes(45);     // 45 dk slot
                int shifts = 0;

                // basit çakışma çözümü: gerekirse saat dilimini 1'er saat kaydır
                while (ctx.VetAppointments.Any(a =>
                           a.PetId == petId &&
                           a.StartUtc < end &&
                           start < a.EndUtc))
                {
                    start = start.AddHours(1);
                    end = start.AddMinutes(45);
                    shifts++;
                    if (shifts > 8) break; // güvenlik: 8 kez dene
                }

                // son kez kontrol: çakışma kalmadıysa ekle
                if (!ctx.VetAppointments.Any(a =>
                        a.PetId == petId &&
                        a.StartUtc < end &&
                        start < a.EndUtc))
                {
                    ctx.VetAppointments.Add(new VetAppointment
                    {
                        PetId = petId,
                        StartUtc = start,
                        EndUtc = end,
                        Title = $"Checkup #{i}",
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }
            ctx.SaveChanges();

            // =================== HEALTH RECORDS ===================
            var hrCnt = ctx.HealthRecords.Count();
            var petIdsForHr = ctx.Pets.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            for (int i = hrCnt + 1; i <= 10 && petIdsForHr.Any(); i++)
            {
                var pid = petIdsForHr[(i - 1) % petIdsForHr.Count];
                ctx.HealthRecords.Add(new HealthRecord
                {
                    PetId = pid,
                    Title = (i % 2 == 0) ? "Kuduz Aşısı" : "Genel Kontrol",
                    Notes = (i % 2 == 0) ? "Yıllık doz uygulandı." : "Her şey normal.",
                    RecordDateUtc = DateTime.UtcNow.AddDays(-i),
                    CreatedDate = DateTime.UtcNow
                });
            }
            ctx.SaveChanges();
        }
    }
}