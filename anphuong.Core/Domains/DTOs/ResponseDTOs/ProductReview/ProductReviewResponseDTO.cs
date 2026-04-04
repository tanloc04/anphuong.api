using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.ResponseDTOs.ProductReview
{
    public class ProductReviewResponseDTO
    {
        public int Id { get; set; }
        public string ReviewerName { get; set; }
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
