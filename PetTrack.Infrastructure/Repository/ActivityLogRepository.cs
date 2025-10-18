using PetTrack.Domain.Abstractions;
using PetTrack.Domain.Entities;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repository;

namespace PetTrack.Infrastructure.Repositories;
public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(AppDbContext ctx) : base(ctx) { }
}