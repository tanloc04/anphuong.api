
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;

namespace anphuong.Repository.Repositories
{
    public class VariantRepository : GenericRepository<Variant>, IVariantRepository
    {
        public VariantRepository(anphuongDbContext context) : base(context)
        {
        }
    }
}
