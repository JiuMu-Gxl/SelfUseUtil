using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorkService.MockApi.Models;

namespace WorkService.MockApi.Repositorys
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> Query();              // 可跟踪（用于更新）
        IQueryable<T> QueryNoTracking();    // 查询专用（推荐用这个）

        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<int> SaveChangesAsync();
    }

    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly MqttDbContext _db;
        protected readonly DbSet<T> _set;

        public RepositoryBase(MqttDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _set.AsQueryable();
        }

        public IQueryable<T> QueryNoTracking()
        {
            return _set.AsNoTracking();
        }


        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _set.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _set.ToListAsync()
                : await _set.Where(predicate).ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _set.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _set.Update(entity);
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
