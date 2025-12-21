using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;

namespace anphuong.Repository.Repositories
{
    public class ColorRepository : GenericRepository<Color>, IColorRepository
    {
        public ColorRepository(anphuongDbContext context) : base(context)
        {
        }
    }
}
