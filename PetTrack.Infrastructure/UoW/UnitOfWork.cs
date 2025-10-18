using PetTrack.Domain.Abstractions;
using PetTrack.Infrastructure.Contexts;

namespace PetTrack.Infrastructure.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;

    public UnitOfWork(
        AppDbContext ctx,
        IPetRepository pets,
        IPetOwnerRepository owners,
        ITrackerDeviceRepository devices,
        IActivityLogRepository activityLogs,
        IAlertRepository alerts)
    {
        _ctx = ctx;
        Pets = pets;
        PetOwners = owners;
        Devices = devices;
        ActivityLogs = activityLogs;
        Alerts = alerts;
    }

    public IPetRepository Pets { get; }
    public IPetOwnerRepository PetOwners { get; }
    public ITrackerDeviceRepository Devices { get; }
    public IActivityLogRepository ActivityLogs { get; }
    public IAlertRepository Alerts { get; }

    public int SaveChanges() => _ctx.SaveChanges();

    public void Dispose()
    {
        _ctx.Dispose();
        GC.SuppressFinalize(this);
    }
}