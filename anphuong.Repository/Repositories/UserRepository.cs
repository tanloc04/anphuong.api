using System.Linq.Expressions;
using System.Reflection;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace anphuong.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(anphuongDbContext context) : base(context)
        {

        }
        public async Task<bool> ActivateUserAsync(int id)
        {
            return await UpdateFieldAsync(id, u => u.Status, "ACTIVE");
        }

        public async Task<bool> UpdateFieldAsync<TField>(int id, Expression<Func<User, TField>> selector, TField value)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                return false;

            if (selector.Body is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
                return false;

            property.SetValue(entity, value);

            return await UpdateEntityAsync(entity);
        }
        private async Task<bool> UpdateEntityAsync(User entity)
        {
            try
            {
                await UpdateAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
