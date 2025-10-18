using PetTrack.Application.Exceptions;
using PetTrack.Domain.Abstractions;

namespace PetTrack.Application.Services;

public abstract class BaseCrudService<TEntity, TCreate, TUpdate> where TEntity : class
{
    protected readonly IUnitOfWork _uow;
    protected readonly IRepository<TEntity> _repo;

    // TEK kurucu: her şey burada atanıyor
    protected BaseCrudService(IUnitOfWork uow, IRepository<TEntity> repo)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    // !!! BUNU SİL: DI yanlış kurucuyu seçip _uow/_repo'yu null bırakıyor
    // protected BaseCrudService(IUnitOfWork uow, IPetRepository pets) { }

    protected abstract void ValidateCreate(TCreate dto);
    protected abstract void ValidateUpdate(TUpdate dto);
    protected abstract TEntity MapCreate(TCreate dto);
    protected abstract void MapUpdate(TUpdate dto, TEntity e);

    public virtual TEntity Create(TCreate dto)
    {
        ValidateCreate(dto);
        var e = MapCreate(dto);
        _repo.Add(e);
        _uow.SaveChanges();
        return e;
    }

    public virtual TEntity Update(TUpdate dto)
    {
        ValidateUpdate(dto);
        var id = typeof(TUpdate).GetProperty("Id") is { } p ? (int)(p.GetValue(dto) ?? 0) : 0;
        var e = _repo.GetById(id) ?? throw new NotFoundException($"{typeof(TEntity).Name} bulunamadı (Id={id})");
        MapUpdate(dto, e);
        _repo.Update(e);
        _uow.SaveChanges();
        return e;
    }

    public virtual void Delete(int id)
    {
        _repo.Delete(id);
        _uow.SaveChanges();
    }

    public virtual TEntity GetById(int id) =>
        _repo.GetById(id) ?? throw new NotFoundException($"{typeof(TEntity).Name} bulunamadı (Id={id})");

    public virtual IQueryable<TEntity> Query(bool includeDeleted = false) =>
        _repo.GetAll(includeDeleted);
}