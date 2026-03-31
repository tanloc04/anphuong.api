using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.ProductReviews
{
    public class ProductReviewCreateDTO
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên người đánh giá là bắt buộc")]
        public string ReviewerName { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        public int RatingValue { get; set; }

        public string? Comment { get; set; }
        public string? UserId { get; set; } // Nếu khách đã login
    }
}
