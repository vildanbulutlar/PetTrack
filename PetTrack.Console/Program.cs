using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PetTrack.Application.Services;
using PetTrack.ConsoleUI.UI;
using PetTrack.Domain.Abstractions;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repositories;
using PetTrack.Infrastructure.Seed;
using PetTrack.Infrastructure.UoW;

namespace PetTrack.ConsoleUI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "PetTrack Console";

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => logging.ClearProviders())
            .ConfigureServices((ctx, services) =>
            {
                var conn =
                    "Server=(localdb)\\MSSQLLocalDB;Database=PetTrackDb;Trusted_Connection=True;MultipleActiveResultSets=true;";

                services.AddDbContext<AppDbContext>(opt =>
                {
                    opt.UseSqlServer(conn);
                    opt.LogTo(_ => { }, LogLevel.None);
                });

                // Repositories
                services.AddScoped<IPetRepository, PetRepository>();
                services.AddScoped<IPetOwnerRepository, PetOwnerRepository>();
                services.AddScoped<ITrackerDeviceRepository, TrackerDeviceRepository>();
                services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
                services.AddScoped<IAlertRepository, AlertRepository>();

                // UoW
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                // Application services
                services.AddScoped<AppOwnerService>();
                services.AddScoped<AppPetService>();
                services.AddScoped<AppointmentService>();  // randevu/çakışma

                // UI screens
                services.AddSingleton<Menu>();
                services.AddScoped<OwnerScreens>();
                services.AddScoped<PetScreens>();
                services.AddScoped<DeviceScreens>();
                services.AddScoped<ActivityScreens>();
                services.AddScoped<ReportsScreens>();
                services.AddScoped<AppointmentScreens>();   // randevu akışı
                services.AddScoped<HealthRecordScreens>();  // sağlık kaydı akışı
            })
            .Build();

        try
        {
            using var scope = host.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await ctx.Database.MigrateAsync();  // tablo & şema garanti

            DbSeeder.Seed(ctx);                 // idempotent seed

            // ---- görünür log ----
            var owners = await ctx.PetOwners.AsNoTracking().CountAsync();
            var pets = await ctx.Pets.AsNoTracking().CountAsync();
            var devs = await ctx.TrackerDevices.AsNoTracking().CountAsync();
            var logs = await ctx.ActivityLogs.AsNoTracking().CountAsync();
            var alerts = await ctx.Alerts.AsNoTracking().CountAsync();
            var appts = await ctx.VetAppointments.AsNoTracking().CountAsync();
            var health = await ctx.HealthRecords.AsNoTracking().CountAsync();

            Console.WriteLine();
            Console.WriteLine($"[SEED RESULT] Owners={owners}, Pets={pets}, Devices={devs}, Logs={logs}, Alerts={alerts}, VetAppointments={appts}, HealthRecords={health}");

            Console.WriteLine("\n--- İlk 3 Owner ---");
            foreach (var o in await ctx.PetOwners.AsNoTracking().OrderBy(x => x.Id).Take(3).ToListAsync())
                Console.WriteLine($"{o.Id} - {o.Name} - {o.Email}");

            Console.WriteLine("\n--- İlk 3 Pet ---");
            foreach (var p in await ctx.Pets.AsNoTracking().OrderBy(x => x.Id).Take(3).ToListAsync())
                Console.WriteLine($"{p.Id} - {p.Name} - OwnerId={p.OwnerId}");

            Console.WriteLine("\nDevam etmek için bir tuşa basın (menü açılacak)...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n❌ Başlatma sırasında hata:");
            Console.WriteLine(ex.Message);
            Console.WriteLine("Detay (inner): " + ex.InnerException?.Message);
            Console.WriteLine("\nDevam etmek için Enter...");
            Console.ReadLine();
        }

        var menu = host.Services.GetRequiredService<Menu>();
        menu.Run();
    }
}