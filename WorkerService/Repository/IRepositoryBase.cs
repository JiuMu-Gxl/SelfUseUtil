using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Repository
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query();

        Task<TEntity?> GetByIdAsync(object id);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        Task<int> SaveChangesAsync();
    }

    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<TEntity> _set;

        public RepositoryBase(AppDbContext db)
        {
            _db = db;
            _set = db.Set<TEntity>();
        }

        public IQueryable<TEntity> Query()
        {
            return _set.AsQueryable();
        }

        public async Task<TEntity?> GetByIdAsync(object id)
        {
            return await _set.FindAsync(id);
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _set.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _set.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _set.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            _set.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _set.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
