using Microsoft.EntityFrameworkCore;
using PetTrack.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetTrack.Infrastructure.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _ctx; protected readonly DbSet<TEntity> _set;
        public GenericRepository(DbContext ctx) { _ctx = ctx; _set = _ctx.Set<TEntity>(); }
        public TEntity? GetById(int id) => _set.Find(id);
        public IQueryable<TEntity> GetAll(bool includeDeleted = false) => _set.AsQueryable();
        public void Add(TEntity e) => _set.Add(e);
        public void Update(TEntity e) => _set.Update(e);
        public void Delete(int id) { var e = _set.Find(id); if (e != null) _set.Remove(e); }
    }
}
