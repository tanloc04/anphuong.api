using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(int id, string? includeProperties = null, CancellationToken cancellationToken = default);

        Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetWithPaginationAsync(PageInfoRequestDTO pageInfo, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);

        bool Update(T entity);

        bool UpdateRange(IEnumerable<T> entities);

        bool Delete(params T[] entities);
    }
}