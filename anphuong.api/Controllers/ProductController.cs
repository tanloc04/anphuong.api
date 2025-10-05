using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IPaginationService<ProductDTO> _paginationService;

        public ProductController(IProductService productService, IPaginationService<ProductDTO> paginationService)
        {
            _productService = productService;
            _paginationService = paginationService;
        }
        #region Get Criterias

        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<ProductDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] SearchProductRequestDTO request)
        {
            var (data, totalItems) = await _productService.GetAll(request);

            var paginatedCriterias = _paginationService.GetPagedData(totalItems, data, request.PageInfo);

            return Ok(new ApiResponseDTO<PagingResponseDTO<ProductDTO>>
            {
                Success = true,
                Data = paginatedCriterias
            });
        }

        #endregion

        #region Get Criteria

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.Get(id);

            return Ok(new ApiResponseDTO<ProductDTO>
            {
                Success = true,
                Data = product
            });
        }

        #endregion

        #region Create Criteria

        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDTO request)
        {
            await _productService.Create(request);
            return Ok(new ApiResponseDTO<object> { Success = true });
        }

        #endregion

        #region Update Criteria

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequestDTO request)
        {
            await _productService.Update(id, request);
            return Ok(new ApiResponseDTO<object> { Success = true });
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.Delete(id);
            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                Message = "Product deleted successfully"
            });
        }

        #endregion
    }
}
