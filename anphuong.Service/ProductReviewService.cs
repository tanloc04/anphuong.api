using anphuong.Core.Domains.DTOs.RequestDTOs.ProductReviews;
using anphuong.Core.Domains.DTOs.ResponseDTOs.ProductReview;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Service
{
    public class ProductReviewService: IProductReviewService
    {
        private readonly IProductReviewRepository _repository;

        public ProductReviewService(IProductReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddReviewAsync(ProductReviewCreateDTO dto)
        {
            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                ReviewerName = dto.ReviewerName,
                RatingValue = dto.RatingValue,
                Comment = dto.Comment,
                UserId = dto.UserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _repository.AddAsync(review);
            return true;
        }

        public async Task<List<ProductReviewResponseDTO>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _repository.GetAllAsync(r => r.ProductId == productId && !r.IsDeleted);

            return reviews.OrderByDescending(r => r.CreatedAt).Select(r => new ProductReviewResponseDTO
            {
                Id = r.Id,
                ReviewerName = r.ReviewerName,
                RatingValue = r.RatingValue,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}
