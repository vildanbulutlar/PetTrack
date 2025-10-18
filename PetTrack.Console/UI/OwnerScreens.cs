using PetTrack.Application.DTOs.Owners;
using PetTrack.Application.Exceptions;
using PetTrack.Application.Services;

namespace PetTrack.ConsoleUI.UI;

public class OwnerScreens
{
    private readonly AppOwnerService _svc;
    public OwnerScreens(AppOwnerService svc) => _svc = svc;

    public void AddOwner()
    {
        System.Console.Write("Owner adı: ");
        var name = System.Console.ReadLine() ?? "";
        try
        {
            var created = _svc.Create(new CreateOwnerDto(name));
            Menu.Info($"✅ Owner eklendi: #{created.Id} {created.Name}");
        }
        catch (ValidationException ex) { Menu.Info("⚠️ Doğrulama: " + ex.Message); }
        catch (Exception ex) { Menu.Info("❌ Hata: " + ex.Message); }
    }

    public void ListOwners()
    {
        var list = _svc.Query().OrderBy(o => o.Id).ToList();
        if (!list.Any()) { Menu.Info("Kayıtlı owner yok."); return; }
        foreach (var o in list)
            System.Console.WriteLine($"#{o.Id} | {o.Name}");
        Menu.Info("");
    }

    public void UpdateOwner()
    {
        ListOwners();
        System.Console.Write("Güncellenecek Owner Id: ");
        if (!int.TryParse(System.Console.ReadLine(), out var id))
        { Menu.Info("Geçersiz Id."); return; }

        System.Console.Write("Yeni ad: ");
        var name = System.Console.ReadLine() ?? "";

        try
        {
            var updated = _svc.Update(new UpdateOwnerDto(id, name));
            Menu.Info($"✅ Owner güncellendi: #{updated.Id} {updated.Name}");
        }
        catch (NotFoundException ex) { Menu.Info("⚠️ Bulunamadı: " + ex.Message); }
        catch (ValidationException ex) { Menu.Info("⚠️ Doğrulama: " + ex.Message); }
        catch (Exception ex) { Menu.Info("❌ Hata: " + ex.Message); }
    }

    public void DeleteOwner()
    {
        ListOwners();
        System.Console.Write("Silinecek Owner Id: ");
        if (!int.TryParse(System.Console.ReadLine(), out var id))
        { Menu.Info("Geçersiz Id."); return; }

        try
        {
            _svc.Delete(id);
            Menu.Info("✅ Owner silindi.");
        }
        catch (NotFoundException ex) { Menu.Info("⚠️ Bulunamadı: " + ex.Message); }
        catch (Exception ex) { Menu.Info("❌ Hata: " + ex.Message); }
    }

    /// Owner seçtirir; yoksa yeni oluşturma imkanı verir.
    public int PickOrCreateOwner()
    {
        while (true)
        {
            var owners = _svc.Query().OrderBy(o => o.Id).ToList();

            if (!owners.Any())
            {
                Menu.Info("Hiç owner yok. Önce bir owner oluşturalım.");
                AddOwner();
                continue; // tekrar listeye dön
            }

            System.Console.WriteLine("Mevcut Owner'lar:");
            foreach (var o in owners)
                System.Console.WriteLine($"#{o.Id} | {o.Name}");
            System.Console.WriteLine("0) Yeni owner oluştur");
            System.Console.Write("Owner Id seçin: ");
            var input = System.Console.ReadLine();

            if (int.TryParse(input, out var id))
            {
                if (id == 0) { AddOwner(); continue; }
                if (owners.Any(o => o.Id == id)) return id;
            }
            Menu.Info("Geçersiz seçim.");
        }
    }
}