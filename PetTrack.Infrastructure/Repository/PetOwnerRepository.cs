using PetTrack.Domain.Abstractions;          
using PetTrack.Domain.Entities;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repository;

namespace PetTrack.Infrastructure.Repositories;

public class PetOwnerRepository : GenericRepository<PetOwner>, IPetOwnerRepository
{
    public PetOwnerRepository(AppDbContext ctx) : base(ctx) { }
}