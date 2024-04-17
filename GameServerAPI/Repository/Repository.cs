using Microsoft.EntityFrameworkCore;
using GameServerAPI.Data;
using GameServerAPI.Repository.IRepository;
using System;
using System.Linq.Expressions;

namespace GameServerAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDBContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public async Task<List<T>?> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<T> query = dbSet;
            if(predicate != null)
            {
                query = query.Where(predicate);
            }
            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }
            if(includeProperties != null)
            {
                foreach(string property in GetProperties(includeProperties)) { 
                    query.Include(property);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>>? predicate = null, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {
                foreach (string property in GetProperties(includeProperties))
                {
                    query.Include(property);
                }
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<T>CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
            return entity;
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        static IEnumerable<string> GetProperties(string properties)
        {
            foreach (var property in properties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (property != null)
                {
                    yield return property;
                    //query.Include(property);
                }
            }
        }
    }
}
