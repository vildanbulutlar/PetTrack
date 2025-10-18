using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetTrack.Domain.Entities;
using PetTrack.Domain.Enums;
using PetTrack.Infrastructure.Contexts;

namespace PetTrack.Infrastructure.Contexts
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services, bool includeConflicts = false)
        {
            using var scope = services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await ctx.Database.MigrateAsync();

            // 1) PetOwners (10)
            if (!await ctx.PetOwners.AnyAsync())
            {
                var owners = Enumerable.Range(1, 10).Select(i => new PetOwner
                {
                    Id = i,
                    Name = $"Owner {i}",
                    Email = $"owner{i}@example.com",
                    Phone = $"+90 555 00{i:00} 00 {i:00}",
                    CreatedDate = DateTime.UtcNow
                });
                await ctx.PetOwners.AddRangeAsync(owners);
                await ctx.SaveChangesAsync();
            }

            // 2) Pets (10) – bind to owners 1..10
            if (!await ctx.Pets.AnyAsync())
            {
                var pets = Enumerable.Range(1, 10).Select(i => new Pet
                {
                    Id = i,
                    Name = $"Pet {i}",
                    SpeciesName = (i % 2 == 0 ? "Dog" : "Cat"),
                    Family = i % 3 == 0 ? PetFamily.Bird : PetFamily.Mammal,
                    OwnerId = i,
                    CreatedDate = DateTime.UtcNow
                });
                await ctx.Pets.AddRangeAsync(pets);
                await ctx.SaveChangesAsync();
            }

            // 3) TrackerDevices (10) – Serial unique, assign first 6 to pets 1..6
            if (!await ctx.TrackerDevices.AnyAsync())
            {
                var devices = Enumerable.Range(1, 10).Select(i => new TrackerDevice
                {
                    Id = i,
                    Serial = $"TRK-{i:0000}",
                    PetId = i <= 6 ? i : 6, // assign last 4 also to pet 6 to create conflicts optionally
                    CreatedDate = DateTime.UtcNow
                }).ToList();

                // By default, avoid conflicts: ensure unique PetId mapping (one device per pet) for first 6 only.
                for (int i = 7; i <= 10; i++)
                {
                    devices[i - 1].PetId = i; // assign devices 7..10 to pets 7..10 too
                }

                await ctx.TrackerDevices.AddRangeAsync(devices);
                await ctx.SaveChangesAsync();
            }

            // 4) ActivityLogs (10) – uses DeviceId, Date, Steps, Temperature
            if (!await ctx.ActivityLogs.AnyAsync())
            {
                var date = DateTime.UtcNow.Date;
                var logs = Enumerable.Range(1, 10).Select(i => new ActivityLog
                {
                    Id = i,
                    DeviceId = ((i - 1) % 10) + 1, // cycle over 1..10 devices
                    Date = date.AddHours(8 + i),
                    Steps = 500 + i * 250,
                    Temperature = 37.0 + (i * 0.1),
                    CreatedDate = DateTime.UtcNow
                });
                await ctx.ActivityLogs.AddRangeAsync(logs);
                await ctx.SaveChangesAsync();
            }

            // 5) Alerts (10) – uses PetId, Type, Message
            if (!await ctx.Alerts.AnyAsync())
            {
                var alerts = Enumerable.Range(1, 10).Select(i => new Alert
                {
                    Id = i,
                    PetId = ((i - 1) % 10) + 1,
                    Type = (i % 2 == 0 ? AlertType.LowActivity : AlertType.HighTemperature),
                    Message = $"Auto alert {i}",
                    CreatedDate = DateTime.UtcNow
                });
                await ctx.Alerts.AddRangeAsync(alerts);
                await ctx.SaveChangesAsync();
            }

            if (includeConflicts)
            {
                await SeedConflictsAsync(ctx);
            }
        }

        // Optional conflicts adapted to actual schema
        private static async Task SeedConflictsAsync(AppDbContext ctx)
        {
            // Duplicate Owner Email (requires unique index if you add it)
            try
            {
                await ctx.PetOwners.AddAsync(new PetOwner
                {
                    Id = 999,
                    Name = "Conflict Owner",
                    Email = "owner1@example.com", // duplicate
                    Phone = "+90 555 999 99 99",
                    CreatedDate = DateTime.UtcNow
                });
                await ctx.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ctx.ChangeTracker.Clear();
            }

            // Duplicate Device Serial (requires unique index on Serial)
            try
            {
                await ctx.TrackerDevices.AddAsync(new TrackerDevice
                {
                    Id = 1000,
                    Serial = "TRK-0001", // duplicate
                    PetId = 1,
                    CreatedDate = DateTime.UtcNow
                });
                await ctx.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ctx.ChangeTracker.Clear();
            }
        }
    }
}