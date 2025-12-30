using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace anphuong.Repository.Repositories
{

    public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        protected readonly anphuongDbContext _context;
        private readonly DbSet<T> _set;

        public GenericRepository(anphuongDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public async Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity != null)
            {
                await _context.AddAsync(entity, cancellationToken);
                return _context.SaveChangesAsync(cancellationToken).GetAwaiter().GetResult() > 0;
            }
            return false;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities.Any())
            {
                await _context.AddRangeAsync(entities, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _set;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Where(x => x.IsDeleted == false).CountAsync(cancellationToken);
        }

        public bool Delete(params T[] entities)
        {
            if (entities != null)
            {
                _context.RemoveRange(entities);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _set;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var incluProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluProp);
                }
            }
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(int id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _set;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var incluProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluProp);
                }
            }
            return await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _set.SingleOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetWithPaginationAsync(PageInfoRequestDTO pageInfo, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            int pageNum = pageInfo.PageNum;
            int pageSize = pageInfo.PageSize;
            IQueryable<T> query = _set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pageNum > 0 && pageSize > 0)
            {
                query = query.Skip((pageNum - 1) * pageSize).Take(pageSize);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var incluProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluProp);
                }
            }
            return await query.ToListAsync(cancellationToken);
        }

        public bool Update(T entity)
        {
            if (entity != null)
            {
                _context.Update(entity);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateRange(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _context.UpdateRange(entities);
                return _context.SaveChanges() == entities.Count();
            }
            return false;
        }
        public async Task DeleteAsync(int id)
        {
            var contest = await GetAsync(id);
            if (contest != null)
            {
                _context.Remove(contest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _set.Where(x => !x.IsDeleted)
                .AnyAsync(predicate, cancellationToken);
        }
    }
}
