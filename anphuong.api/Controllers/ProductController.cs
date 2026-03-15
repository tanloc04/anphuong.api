using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.DTOs.ResponseDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IPaginationService<ProductDTO> _paginationService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductController(IProductService productService,
            IPaginationService<ProductDTO> paginationService,
            ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _paginationService = paginationService;
            _cloudinaryService = cloudinaryService;
        }
        #region GetAll
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<ProductListResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] SearchProductsRequestDTO request)
        {
            var (data, totalItems) = await _productService.GetAll(request);

            var paginatedItems = _paginationService.GetPagedData(totalItems, data, request.PageInfo);

            return Ok(new ApiResponseDTO<PagingResponseDTO<ProductDTO>>
            {
                Success = true,
                Data = paginatedItems
            });
        }
        #endregion

        #region Get
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _productService.Get(id);

                return Ok(new ApiResponseDTO<ProductDTO>
                {
                    Success = true,
                    Data = item
                });
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        #endregion

        #region Create        
        [Authorize(Policy = "AllowSpecificEmail")]
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDTO request)
        {
            try
            {
                var item = await _productService.Create(request);
                return Ok(new ApiResponseDTO<ProductDTO>
                {
                    Success = true,
                    Data = item
                });
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        #endregion

        #region Update
        [Authorize(Policy = "AllowSpecificEmail")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequestDTO request)
        {
            var item = await _productService.Update(id, request);

            return Ok(new ApiResponseDTO<ProductDTO>
            {
                Success = true,
                Data = item
            });
        }
        #endregion

        #region Delete
        [Authorize(Policy = "AllowSpecificEmail")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.Delete(id);
            return Ok(new ApiResponseDTO<object> { Success = true });
        }
        #endregion

        //#region Upload Image
        //[HttpPost("image")]
        ////[Authorize(Policy = "AllowSpecificEmail")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadImage([FromForm] UploadImageRequestDTO request)
        //{
        //    if (request.File == null || request.File.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var url = await _cloudinaryService.UploadImageAsync(request.File, "anphuong/images");
        //    return Ok(new { imageUrl = url });
        //}
        //#endregion

        #region LowStockCount
        [HttpGet("low-stock-count")]
        public async Task<IActionResult> GetLowStockCount([FromQuery] int threshold = 5)
        {
            try
            {
                var count = await _productService.GetLowStockCountAsync(threshold);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion
    }
}
