using PetTrack.Domain.Abstractions;           // ← DİKKAT
using PetTrack.Domain.Entities;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repository;

namespace PetTrack.Infrastructure.Repositories;

public class PetRepository : GenericRepository<Pet>, IPetRepository
{
    public PetRepository(AppDbContext ctx) : base(ctx) { }
}