using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Data.Abstract
{
    public interface IAsyncEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        IAsyncEnumerable<T> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
        IAsyncEnumerable<T> GetAllAsync();
        Task<int> CountAsync();
        Task<T> GetSingleAsync(long id);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IAsyncEnumerable<T> FindByAsync(Expression<Func<T, bool>> predicate);
        void AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        void DeleteWhereAsync(Expression<Func<T, bool>> predicate);
        void CommitAsync();
    }
}
