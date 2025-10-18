using System;
using System.Linq;
using PetTrack.Domain.Abstractions;
using PetTrack.Domain.Entities;

namespace PetTrack.ConsoleUI.UI;

public class ActivityScreens
{
    private readonly IUnitOfWork _uow;
    public ActivityScreens(IUnitOfWork uow) => _uow = uow;

    public void GenerateToday()
    {
        var devices = _uow.Devices.GetAll().ToList();
        if (devices.Count == 0) { Menu.Info("Kayıtlı cihaz yok."); return; }

        var rnd = new Random();

        foreach (var d in devices)
        {
            var steps = rnd.Next(100, 1200);                 // 100–1200 adım
            var temp = 36.5 + rnd.NextDouble() * 4.0;       // 36.5–40.5 °C

            var log = new ActivityLog
            {
                DeviceId = d.Id,
                Steps = steps,
                Temperature = temp,
                CreatedDate = DateTime.UtcNow
            };
            _uow.ActivityLogs.Add(log);

            // Basit eşikler → uyarı
            if (steps < 500)
                _uow.Alerts.Add(new Alert
                {
                    PetId = d.PetId,
                    Message = $"Düşük aktivite: {steps} adım",
                    CreatedDate = DateTime.UtcNow
                });

            if (temp >= 39.5)
                _uow.Alerts.Add(new Alert
                {
                    PetId = d.PetId,
                    Message = $"Yüksek sıcaklık: {temp:F1} °C",
                    CreatedDate = DateTime.UtcNow
                });
        }

        _uow.SaveChanges();
        Menu.Info("✅ Bugünün aktivite logları üretildi.");
    }

    public void ListAlerts()
    {
        var alerts = _uow.Alerts.GetAll()
            .OrderByDescending(a => a.CreatedDate)
            .Take(50)
            .ToList();

        if (alerts.Count == 0) { Menu.Info("Uyarı yok."); return; }

        foreach (var a in alerts)
            Console.WriteLine($"[{a.CreatedDate:u}] PetId={a.PetId} | {a.Message}");

        Menu.Info("");
    }
}