using PetTrack.Domain.Abstractions;
using PetTrack.Domain.Entities;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repository;

namespace PetTrack.Infrastructure.Repositories;
public class AlertRepository : GenericRepository<Alert>, IAlertRepository
{
    public AlertRepository(AppDbContext ctx) : base(ctx) { }
}