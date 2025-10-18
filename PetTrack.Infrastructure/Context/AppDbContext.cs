using Microsoft.EntityFrameworkCore;
using PetTrack.Domain.Entities;

namespace PetTrack.Infrastructure.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PetOwner> PetOwners => Set<PetOwner>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<TrackerDevice> TrackerDevices => Set<TrackerDevice>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<VetAppointment> VetAppointments => Set<VetAppointment>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();   // ✅ eklendi

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------- TrackerDevice (1 ↔ 1) Pet --------
        modelBuilder.Entity<TrackerDevice>()
            .HasIndex(d => d.Serial)
            .IsUnique();

        modelBuilder.Entity<TrackerDevice>()
            .HasIndex(d => d.PetId)
            .IsUnique();

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.Device)
            .WithOne(d => d.Pet)
            .HasForeignKey<TrackerDevice>(d => d.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        // -------- ActivityLog (N ↔ 1) TrackerDevice --------
        modelBuilder.Entity<ActivityLog>()
            .HasOne(l => l.Device)
            .WithMany()
            .HasForeignKey(l => l.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // -------- Alert (N ↔ 1) Pet --------
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Pet)
            .WithMany()
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        // -------- VetAppointment (N ↔ 1) Pet --------
        modelBuilder.Entity<VetAppointment>()
            .HasOne(a => a.Pet)
            .WithMany()
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VetAppointment>()
            .HasIndex(a => new { a.PetId, a.StartUtc });

        // -------- HealthRecord (N ↔ 1) Pet --------
        modelBuilder.Entity<HealthRecord>()
            .HasOne(h => h.Pet)
            .WithMany()
            .HasForeignKey(h => h.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HealthRecord>()
            .HasIndex(h => new { h.PetId, h.RecordDateUtc });  // tarih + pet için hızlı sorgu
    }
}