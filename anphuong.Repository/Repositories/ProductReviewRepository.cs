using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Repository.Repositories
{
    public class ProductReviewRepository: GenericRepository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(anphuongDbContext context) : base(context)
        {
            
        }
    }
}
