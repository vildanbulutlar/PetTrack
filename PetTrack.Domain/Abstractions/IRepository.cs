using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetTrack.Domain.Abstractions;
public interface IRepository<TEntity> where TEntity : class
{
    TEntity? GetById(int id);
    IQueryable<TEntity> GetAll(bool includeDeleted = false);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(int id);
}