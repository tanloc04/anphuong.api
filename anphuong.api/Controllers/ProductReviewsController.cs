using anphuong.Core.Domains.DTOs.RequestDTOs.ProductReviews;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _service;

        public ProductReviewsController(IProductReviewService service)
        {
            _service = service;
        }

        [HttpGet("product/{producId}")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var reviews = await _service.GetReviewsByProductIdAsync(productId);
            return Ok(new { data = reviews, success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ProductReviewCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.AddReviewAsync(dto);
            if (result)
            {
                return Ok(new { message = "Cảm ơn bạn đã đánh giá sản phẩm!", success = true });
            }

            return BadRequest(new { message = "Đánh giá thất bại", success = false });
        }
    }
}
