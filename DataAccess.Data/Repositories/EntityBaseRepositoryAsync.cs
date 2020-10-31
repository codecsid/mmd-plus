using DataAccess.Data.Abstract;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Repositories
{
    public class EntityBaseRepositoryAsync<T> : IAsyncEntityBaseRepository<T>
            where T : class, IEntityBase, new()
    {
        private DataContext _context;

        public EntityBaseRepositoryAsync(DataContext context)
        {
            _context = context;
        }
        public virtual IAsyncEnumerable<T> GetAllAsync()
        {
            return _context.Set<T>().AsAsyncEnumerable();
        }

        public virtual Task<int> CountAsync()
        {
            return _context.Set<T>().CountAsync();
        }
        public virtual IAsyncEnumerable<T> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsAsyncEnumerable();
        }

        public Task<T> GetSingleAsync(long id)
        {
            return _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual IAsyncEnumerable<T> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).AsAsyncEnumerable();
        }

        public virtual void AddAsync(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public virtual void UpdateAsync(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void DeleteAsync(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public virtual void CommitAsync()
        {
            _context.SaveChangesAsync();
        }
    }
}
