using System;
using System.Linq;
using PetTrack.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace PetTrack.ConsoleUI.UI;

public class ReportsScreens
{
    private readonly IUnitOfWork _uow;
    public ReportsScreens(IUnitOfWork uow) => _uow = uow;

    // 1) Günlük rapor: En çok yürüyen hayvanlar (bugün)
    public void TopWalkersDaily()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Bugün atılan adımlar -> { PetId, Steps }
        var stepsToday =
            from l in _uow.ActivityLogs.GetAll()
            where l.CreatedDate >= today && l.CreatedDate < tomorrow
            join d in _uow.Devices.GetAll() on l.DeviceId equals d.Id
            group l by d.PetId into g
            select new { PetId = g.Key, Steps = g.Sum(x => x.Steps) };

        // Pet + Owner ile LEFT JOIN
        var top =
            (from s in stepsToday
             join p in _uow.Pets.GetAll() on s.PetId equals p.Id
             join o in _uow.PetOwners.GetAll() on p.OwnerId equals o.Id into oj
             from o in oj.DefaultIfEmpty()
             select new
             {
                 Pet = p.Name,
                 Owner = (o == null ? "-" : o.Name),
                 s.Steps
             })
            .OrderByDescending(x => x.Steps)
            .Take(10)
            .ToList();

        if (top.Count == 0) { Menu.Info("Bugün için aktivite kaydı yok."); return; }

        Console.WriteLine("=== En Çok Yürüyenler (Bugün) ===");
        int rank = 1;
        foreach (var r in top)
        {
            Console.WriteLine($"{rank,2}. {r.Pet,-15} | Owner: {r.Owner,-12} | Adım: {r.Steps}");
            rank++;
        }
        Menu.Info("");
    }

    // 2) "En sağlıklı hayvanlar" (son 7 gün toplam adım - uyarı cezası)
    public void HealthiestPets()
    {
        var since = DateTime.UtcNow.Date.AddDays(-7);

        // 1) Küçük projeksiyonları belleğe al
        var pets = _uow.Pets.GetAll()
            .Select(p => new { p.Id, p.Name, p.OwnerId })
            .ToList();

        var owners = _uow.PetOwners.GetAll()
            .Select(o => new { o.Id, o.Name })
            .ToList()
            .ToDictionary(o => o.Id, o => o.Name);

        var deviceMap = _uow.Devices.GetAll()
            .Select(d => new { d.Id, d.PetId })
            .ToList()
            .ToDictionary(d => d.Id, d => d.PetId);   // DeviceId -> PetId

        var logs = _uow.ActivityLogs.GetAll()
            .Where(l => l.CreatedDate >= since)
            .Select(l => new { l.DeviceId, l.Steps })
            .ToList();

        var alerts = _uow.Alerts.GetAll()
            .Where(a => a.CreatedDate >= since)
            .Select(a => new { a.PetId })
            .ToList();

        // 2) Bellekte gruplayıp sözlüklere çevir
        var stepsByPet = logs
            .Where(l => deviceMap.ContainsKey(l.DeviceId))
            .GroupBy(l => deviceMap[l.DeviceId])
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Steps));

        var alertsByPet = alerts
            .GroupBy(a => a.PetId)
            .ToDictionary(g => g.Key, g => g.Count());

        // 3) Skor hesapla ve sırala (tamamı LINQ to Objects)
        var score = pets
            .Select(p => new
            {
                PetId = p.Id,
                Pet = p.Name,
                Owner = owners.TryGetValue(p.OwnerId, out var n) ? n : "-",
                Steps = stepsByPet.TryGetValue(p.Id, out var s) ? s : 0,
                Alerts = alertsByPet.TryGetValue(p.Id, out var c) ? c : 0
            })
            .Select(x => new
            {
                x.PetId,
                x.Pet,
                x.Owner,
                x.Steps,
                x.Alerts,
                Score = x.Steps - x.Alerts * 800 // ceza katsayısı
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Steps)
            .Take(10)
            .ToList();

        if (score.Count == 0) { Menu.Info("Veri bulunamadı."); return; }

        Console.WriteLine("=== En Sağlıklı Hayvanlar (7 Gün) ===");
        int rank = 1;
        foreach (var r in score)
        {
            Console.WriteLine($"{rank,2}. {r.Pet,-15} | Owner: {r.Owner,-12} | Skor: {r.Score,6} | Adım: {r.Steps,6} | Uyarı: {r.Alerts}");
            rank++;
        }
        Menu.Info("");
    }
    

    // 3) Kritik uyarıların e-posta simülasyonu (son 24 saat)
    public void SendCriticalAlertsEmailSimulation()
    {
        var since = DateTime.UtcNow.AddHours(-24);

        // Uyarı + Pet + Owner tek sorgu
        var emails =
            (from a in _uow.Alerts.GetAll()
             where a.CreatedDate >= since
             join p in _uow.Pets.GetAll() on a.PetId equals p.Id
             join o in _uow.PetOwners.GetAll() on p.OwnerId equals o.Id into oj
             from o in oj.DefaultIfEmpty()
             orderby a.CreatedDate descending
             select new
             {
                 Pet = p.Name,
                 OwnerName = (o == null ? "Owner" : o.Name),
                 OwnerEmail = (o == null ? null : o.Email),
                 a.Message,
                 a.CreatedDate
             })
            .Take(100)
            .ToList();

        if (emails.Count == 0) { Menu.Info("Son 24 saatte kritik uyarı yok."); return; }

        Console.WriteLine("=== E-posta Simülasyonu ===");
        foreach (var m in emails)
        {
            var to = string.IsNullOrWhiteSpace(m.OwnerEmail)
                ? $"{m.OwnerName} <no-email>"
                : $"{m.OwnerName} <{m.OwnerEmail}>";

            Console.WriteLine($"To     : {to}");
            Console.WriteLine($"Subject: PetTrack Kritik Uyarı – {m.Pet}");
            Console.WriteLine($"Body   : [{m.CreatedDate:u}] {m.Message}");
            Console.WriteLine(new string('-', 60));
        }
        Menu.Info("Toplam gönderim (simülasyon): " + emails.Count);
    }

    // (Opsiyonel) Randevu çakışma kontrol demo'su – in-memory örnek
    public void CheckAppointmentConflictsDemo()
    {
        Console.Write("Pet Id: ");
        if (!int.TryParse(Console.ReadLine(), out var petId)) { Menu.Info("Geçersiz Id."); return; }

        Console.Write("Başlangıç (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var start)) { Menu.Info("Tarih formatı hatalı."); return; }

        Console.Write("Süre (dakika): ");
        if (!int.TryParse(Console.ReadLine(), out var minutes)) { Menu.Info("Geçersiz süre."); return; }
        var end = start.AddMinutes(minutes);

        var existing = Array.Empty<(DateTime Start, DateTime End)>();
        var conflict = existing.Any(x => Overlaps(start, end, x.Start, x.End));

        Menu.Info(conflict ? "⚠ ÇAKIŞMA VAR" : "✅ Çakışma yok");
    }

    private static bool Overlaps(DateTime s1, DateTime e1, DateTime s2, DateTime e2)
        => s1 < e2 && s2 < e1; // yarı-açık aralık
}