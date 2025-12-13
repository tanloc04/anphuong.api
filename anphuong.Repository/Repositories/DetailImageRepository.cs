using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;

namespace anphuong.Repository.Repositories
{
    public class DetailImageRepository : GenericRepository<DetailImage>, IDetailImageRepository
    {
        public DetailImageRepository(anphuongDbContext context) : base(context)
        {
        }
    }
}
