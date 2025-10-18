using PetTrack.Domain.Abstractions;           // ← DİKKAT
using PetTrack.Application.DTOs.Pets;
using PetTrack.Application.Exceptions;
using PetTrack.Application.Validation;
using PetTrack.Domain.Entities;
using PetTrack.Domain.Enums;

namespace PetTrack.Application.Services;

public class AppPetService : BaseCrudService<Pet, CreatePetDto, UpdatePetDto>
{
    public AppPetService(IUnitOfWork uow) : base(uow, uow.Pets) { }

    protected override void ValidateCreate(CreatePetDto dto)
    {
        ValidationService.NotEmpty(dto.Name, "Name");
        ValidationService.NotEmpty(dto.SpeciesName, "SpeciesName");
        ValidationService.Positive(dto.OwnerId, "OwnerId");
        if (!Enum.IsDefined(typeof(PetFamily), dto.Family))
            throw new ValidationException("Family geçersiz!");

        var owner = _uow.PetOwners?.GetById(dto.OwnerId);
        if (owner is null)
            throw new NotFoundException("Owner bulunamadı!");
    }

    protected override void ValidateUpdate(UpdatePetDto dto)
    {
        ValidationService.Positive(dto.Id, "Id");
        ValidationService.NotEmpty(dto.Name, "Name");
        ValidationService.NotEmpty(dto.SpeciesName, "SpeciesName");
        if (!Enum.IsDefined(typeof(PetFamily), dto.Family))
            throw new ValidationException("Family geçersiz!");
    }

    protected override Pet MapCreate(CreatePetDto dto) => new()
    {
        Name = dto.Name.Trim(),
        SpeciesName = dto.SpeciesName.Trim(),
        Family = dto.Family,
        OwnerId = dto.OwnerId
    };

    protected override void MapUpdate(UpdatePetDto dto, Pet e)
    {
        e.Name = dto.Name.Trim();
        e.SpeciesName = dto.SpeciesName.Trim();
        e.Family = dto.Family;
    }

    public IQueryable<Pet> GetByOwner(int ownerId) => _repo.GetAll().Where(p => p.OwnerId == ownerId);
}