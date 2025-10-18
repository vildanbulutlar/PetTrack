using PetTrack.Application.DTOs.Pets;
using PetTrack.Application.Exceptions;
using PetTrack.Application.Services;
using PetTrack.Domain.Enums;

namespace PetTrack.ConsoleUI.UI;

public class PetScreens
{
    private readonly AppPetService _svc;
    private readonly OwnerScreens _ownerScreens;

    public PetScreens(AppPetService svc, OwnerScreens ownerScreens)
    {
        _svc = svc;
        _ownerScreens = ownerScreens;
    }

    public void AddPet()
    {
        // 1) Önce owner seç/oluştur
        var ownerId = _ownerScreens.PickOrCreateOwner();

        // 2) Pet bilgilerini al
        System.Console.Write("Ad: ");
        var name = System.Console.ReadLine() ?? "";

        System.Console.Write("Tür (SpeciesName): ");
        var species = System.Console.ReadLine() ?? "";

        System.Console.Write("Family (1=Mammal, 2=Bird, 3=Reptile): ");
        var famStr = System.Console.ReadLine();
        if (!Enum.TryParse<PetFamily>(famStr, out var family)) family = PetFamily.Mammal;

        // 3) Kaydet
        try
        {
            var dto = new CreatePetDto(name, species, family, ownerId);
            var created = _svc.Create(dto);
            Menu.Info($"✅ Pet eklendi: #{created.Id} {created.Name} - OwnerId={created.OwnerId}");
        }
        catch (ValidationException ex) { Menu.Info("⚠️ Doğrulama: " + ex.Message); }
        catch (NotFoundException ex) { Menu.Info("⚠️ Bulunamadı: " + ex.Message); }
        catch (Exception ex) { Menu.Info("❌ Hata: " + ex.Message); }
    }

    public void ListPets()
    {
        var list = _svc.Query().OrderBy(p => p.Id).ToList();
        if (!list.Any()) { Menu.Info("Kayıt yok."); return; }

        foreach (var p in list)
            System.Console.WriteLine($"#{p.Id} | {p.Name} | {p.SpeciesName} | {p.Family} | OwnerId={p.OwnerId}");
        Menu.Info("");
    }

    public void UpdatePet()
    {
        ListPets();
        System.Console.Write("Güncellenecek Pet Id: ");
        if (!int.TryParse(System.Console.ReadLine(), out var id))
        { Menu.Info("Geçersiz Id."); return; }

        System.Console.Write("Yeni ad: ");
        var name = System.Console.ReadLine() ?? "";

        System.Console.Write("Yeni tür (SpeciesName): ");
        var species = System.Console.ReadLine() ?? "";

        System.Console.Write("Family (1=Mammal, 2=Bird, 3=Reptile): ");
        var famStr = System.Console.ReadLine();
        if (!Enum.TryParse<PetFamily>(famStr, out var family)) family = PetFamily.Mammal;

        try
        {
            // Not: OwnerId burada değişmiyor (UpdatePetDto'da yok). Gerekirse servis ve DTO'yu genişletiriz.
            var updated = _svc.Update(new UpdatePetDto(id, name, species, family));
            Menu.Info($"✅ Pet güncellendi: #{updated.Id} {updated.Name}");
        }
        catch (NotFoundException ex) { Menu.Info("⚠️ Bulunamadı: " + ex.Message); }
        catch (ValidationException ex) { Menu.Info("⚠️ Doğrulama: " + ex.Message); }
        catch (Exception ex) { Menu.Info("❌ Hata: " + ex.Message); }
    }

    public void DeletePet()
    {
        ListPets();
        System.Console.Write("Silinecek Pet Id: ");
        if (!int.TryParse(System.Console.ReadLine(), out var id))
        { Menu.Info("Geçersiz Id."); return; }

        try
        {
            _svc.Delete(id);
            Menu.Info("✅ Pet silindi.");
        }
        catch (NotFoundException ex) { Menu.Info("⚠️ Bulunamadı: " + ex.Message); }
        catch (Exception ex) { Menu.Info("❌ Hata: " + ex.Message); }
    }
}