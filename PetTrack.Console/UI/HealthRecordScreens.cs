using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetTrack.Domain.Entities;
using PetTrack.Infrastructure.Contexts;

namespace PetTrack.ConsoleUI.UI;

public class HealthRecordScreens
{
    private readonly IServiceScopeFactory _scopeFactory;
    public HealthRecordScreens(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async void AddHealthRecord()
    {
        Console.Write("PetId: ");
        if (!int.TryParse(Console.ReadLine(), out var petId))
        {
            Menu.Info("Geçersiz PetId.");
            return;
        }

        Console.Write("Başlık: ");
        var title = Console.ReadLine() ?? "Kayıt";

        Console.Write("Notlar: ");
        var notes = Console.ReadLine();

        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ctx.HealthRecords.Add(new HealthRecord
        {
            PetId = petId,
            Title = title,
            Notes = notes,
            RecordDateUtc = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        Menu.Info("✅ Sağlık kaydı eklendi.");
    }

    public async void ListHealthRecords()
    {
        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var list = await ctx.HealthRecords
            .AsNoTracking()
            .OrderByDescending(h => h.RecordDateUtc)
            .ToListAsync();

        if (list.Count == 0)
        {
            Menu.Info("Kayıt yok.");
            return;
        }

        foreach (var h in list)
            Console.WriteLine($"#{h.Id} Pet={h.PetId}  {h.RecordDateUtc:dd.MM.yyyy}  {h.Title}  {h.Notes}");

        Menu.Info("");
    }
}