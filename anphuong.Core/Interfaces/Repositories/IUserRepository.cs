using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<bool> ActivateUserAsync(int id);

        public Task<User?> CheckRefreshToken(string refreshToken);
    }
}
