using PetTrack.Domain.Abstractions;                 // DİKKAT: Domain.Abstractions
using PetTrack.Application.DTOs.Owners;
using PetTrack.Application.Exceptions;
using PetTrack.Application.Validation;
using PetTrack.Domain.Entities;

namespace PetTrack.Application.Services;

public class AppOwnerService : BaseCrudService<PetOwner, CreateOwnerDto, UpdateOwnerDto>
{
    public AppOwnerService(IUnitOfWork uow) : base(uow, uow.PetOwners) { }

    protected override void ValidateCreate(CreateOwnerDto dto)
    {
        ValidationService.NotEmpty(dto.Name, "Name");
        // aynı isim varsa engellemek istersen:
        // if (_repo.GetAll().Any(o => o.Name == dto.Name.Trim()))
        //     throw new ValidationException("Owner adı zaten var.");
    }

    protected override void ValidateUpdate(UpdateOwnerDto dto)
    {
        ValidationService.Positive(dto.Id, "Id");
        ValidationService.NotEmpty(dto.Name, "Name");

        var entity = _repo.GetById(dto.Id) ?? throw new NotFoundException("Owner bulunamadı!");
        entity.Name = dto.Name.Trim();
        _repo.Update(entity);
    }

    protected override PetOwner MapCreate(CreateOwnerDto dto) => new()
    {
        Name = dto.Name.Trim()
    };

    protected override void MapUpdate(UpdateOwnerDto dto, PetOwner entity) { /* yukarıda yaptık */ }
}