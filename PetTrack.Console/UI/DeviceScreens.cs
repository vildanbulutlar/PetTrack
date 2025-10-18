using System;
using System.Linq;
using PetTrack.Domain.Abstractions;
using PetTrack.Domain.Entities;

namespace PetTrack.ConsoleUI.UI;

public class DeviceScreens
{
    private readonly IUnitOfWork _uow;
    public DeviceScreens(IUnitOfWork uow) => _uow = uow;

    public void AttachDevice()
    {
        var pets = _uow.Pets.GetAll().OrderBy(p => p.Id).ToList();
        if (pets.Count == 0) { Menu.Info("Önce pet ekleyin."); return; }

        foreach (var p in pets)
            Console.WriteLine($"#{p.Id} | {p.Name} | OwnerId={p.OwnerId}");

        Console.Write("Cihaz bağlanacak Pet Id: ");
        if (!int.TryParse(Console.ReadLine(), out var petId)) { Menu.Info("Geçersiz Id."); return; }

        var pet = _uow.Pets.GetById(petId);
        if (pet is null) { Menu.Info("Pet bulunamadı."); return; }

        Console.Write("Cihaz seri no: ");
        var serial = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(serial)) { Menu.Info("Seri no boş olamaz."); return; }

        // aynı pette/seride cihaz var mı?
        var clash = _uow.Devices.GetAll().FirstOrDefault(d => d.PetId == petId || d.Serial == serial);
        if (clash != null) { Menu.Info("Bu pette veya bu seri numarasıyla zaten cihaz var."); return; }

        var dev = new TrackerDevice { PetId = petId, Serial = serial };
        _uow.Devices.Add(dev);
        _uow.SaveChanges();

        Menu.Info($"✅ Cihaz bağlandı: DeviceId={dev.Id}, Serial={dev.Serial}");
    }
}