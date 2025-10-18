namespace PetTrack.Domain.Abstractions;
public interface IUnitOfWork : IDisposable
{
    IPetRepository Pets { get; }
    IPetOwnerRepository PetOwners { get; }

    ITrackerDeviceRepository Devices { get; }   // ← eklendi
    IActivityLogRepository ActivityLogs { get; } // ← eklendi
    IAlertRepository Alerts { get; }             // ← eklendi

    int SaveChanges();
}