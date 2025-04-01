using System.Linq.Expressions;

namespace Exnaton.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ReadAllAsync(int? limit);
    Task<T?> ReadByIdAsync(Guid id);
    Task<T?> ReadFromPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> ReadAllFromPredicateAsync(Expression<Func<T, bool>> predicate);
    Task CreateAsync(T entity);
    Task CreateAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity, Expression<Func<T, bool>>? predicate = null, T? oldEntity = null);
    Task DeletePredicateAsync(Expression<Func<T, bool>> predicate);
}