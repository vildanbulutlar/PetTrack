using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetTrack.Application.Services;
using PetTrack.Infrastructure.Contexts;

namespace PetTrack.ConsoleUI.UI;

public class AppointmentScreens
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AppointmentScreens(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async void AddAppointment()
    {
        try
        {
            Console.Write("PetId: ");
            if (!int.TryParse(Console.ReadLine(), out var petId))
            {
                Menu.Info("Geçersiz PetId.");
                return;
            }

            Console.Write("Başlangıç (yyyy-MM-dd HH:mm): ");
            var startText = Console.ReadLine();
            var fmt = "yyyy-MM-dd HH:mm";
            if (!DateTime.TryParseExact(startText, fmt, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var startLocal))
            {
                Menu.Info("Geçersiz tarih/saat. Örn: 2025-10-05 10:00");
                return;
            }

            Console.Write("Süre (dakika, varsayılan 45): ");
            var durText = Console.ReadLine();
            var minutes = 45;
            if (!string.IsNullOrWhiteSpace(durText)) int.TryParse(durText, out minutes);
            if (minutes <= 0) minutes = 45;

            var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Local).ToUniversalTime();
            var endUtc = startUtc.AddMinutes(minutes);

            Console.Write("Başlık: ");
            var title = Console.ReadLine() ?? "Muayene";

            // <<< KENDİ SCOPE'UNU AÇ >>>
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<AppointmentService>();

            var msg = await service.CreateAsync(petId, startUtc, endUtc, title);
            Menu.Info(msg);
        }
        catch (Exception ex)
        {
            Menu.Info("Hata: " + ex.Message);
        }
    }

    public async void ListAppointments()
    {
        // <<< KENDİ SCOPE'UNU AÇ >>>
        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var appts = await ctx.VetAppointments
            .AsNoTracking()
            .OrderBy(a => a.StartUtc)
            .ToListAsync();

        if (appts.Count == 0)
        {
            Menu.Info("Kayıt yok.");
            return;
        }

        foreach (var a in appts)
            Console.WriteLine($"#{a.Id}  Pet={a.PetId}  {a.StartUtc:dd.MM.yyyy HH:mm} - {a.EndUtc:HH:mm}  {a.Title}");

        Menu.Info("");
    }
}