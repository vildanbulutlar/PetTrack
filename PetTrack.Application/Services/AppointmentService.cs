using Microsoft.EntityFrameworkCore;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Domain.Entities;

namespace PetTrack.Application.Services;

public class AppointmentService
{
    private readonly AppDbContext _ctx;
    public AppointmentService(AppDbContext ctx) => _ctx = ctx;

    public async Task<string> CreateAsync(int petId, DateTime startLocal, DateTime endLocal, string title)
    {
        var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Local).ToUniversalTime();
        var endUtc = DateTime.SpecifyKind(endLocal, DateTimeKind.Local).ToUniversalTime();

        if (endUtc <= startUtc)
            return "❌ Bitiş zamanı başlangıçtan sonra olmalı.";

        using var tx = await _ctx.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        // Aynı pet’in randevularını kısa süreli kilitle
        await _ctx.Database.ExecuteSqlRawAsync(
            "SELECT 1 FROM dbo.VetAppointments WITH (UPDLOCK, HOLDLOCK) WHERE PetId = {0}", petId);

        // Kesişim: A.Start < B.End && B.Start < A.End
        bool conflict = await _ctx.VetAppointments
            .AnyAsync(a => a.PetId == petId && a.StartUtc < endUtc && startUtc < a.EndUtc);

        if (conflict)
        {
            await tx.RollbackAsync();
            return "⚠️ Çakışan bir randevu var. Lütfen başka bir saat seçin.";
        }

        _ctx.VetAppointments.Add(new VetAppointment
        {
            PetId = petId,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Title = title,
            CreatedDate = DateTime.UtcNow
        });

        await _ctx.SaveChangesAsync();
        await tx.CommitAsync();
        return "✅ Randevu oluşturuldu.";
    }
}