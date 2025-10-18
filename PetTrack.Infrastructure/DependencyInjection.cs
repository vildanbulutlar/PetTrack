using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetTrack.Domain.Abstractions;
using PetTrack.Infrastructure.Contexts;
using PetTrack.Infrastructure.Repositories;
using PetTrack.Infrastructure.UoW;

namespace PetTrack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IPetOwnerRepository, PetOwnerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}