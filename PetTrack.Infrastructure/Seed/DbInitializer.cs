using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetTrack.Domain.Entities;
using PetTrack.Domain.Enums;
using PetTrack.Infrastructure.Contexts;

namespace PetTrack.Infrastructure.Seed;

public static class DbInitializer
{
    public static void Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // DB yoksa oluşturur, varsa şema günceller (migrations uygulayarak)
        ctx.Database.Migrate();

        // OWNER yoksa ekle
        if (!ctx.PetOwners.Any())
        {
            ctx.PetOwners.AddRange(
                new PetOwner { Id = 1, Name = "Default Owner", CreatedDate = DateTime.UtcNow, Status = EntityStatus.Active },
                new PetOwner { Id = 2, Name = "Alice", CreatedDate = DateTime.UtcNow, Status = EntityStatus.Active }
            );
            ctx.SaveChanges();
        }

        // İsteğe bağlı: en az bir PET ekle (OwnerId=1'e bağlı)
        if (!ctx.Pets.Any())
        {
            ctx.Pets.Add(new Pet
            {
                Name = "Kaju",
                SpeciesName = "Cat",
                Family = PetFamily.Mammal,
                OwnerId = 1,
                CreatedDate = DateTime.UtcNow,
                Status = EntityStatus.Active
            });
            ctx.SaveChanges();
        }
    }
}