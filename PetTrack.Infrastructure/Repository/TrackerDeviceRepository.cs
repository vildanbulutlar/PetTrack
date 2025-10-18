using PetTrack.Domain.Abstractions;
using PetTrack.Domain.Entities;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repository;

namespace PetTrack.Infrastructure.Repositories;
public class TrackerDeviceRepository : GenericRepository<TrackerDevice>, ITrackerDeviceRepository
{
    public TrackerDeviceRepository(AppDbContext ctx) : base(ctx) { }
}