using anphuong.Core.Domains.DTOs.RequestDTOs.ProductReviews;
using anphuong.Core.Domains.DTOs.ResponseDTOs.ProductReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Interfaces.Services
{
    public interface IProductReviewService
    {
        Task<bool> AddReviewAsync(ProductReviewCreateDTO dto);
        Task<List<ProductReviewResponseDTO>> GetReviewsByProductIdAsync(int productId);
    }
}
