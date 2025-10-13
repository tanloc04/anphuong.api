using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Category;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Services;
using anphuong.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IPaginationService<CategoryDTO> _paginationService;

        public CategoryController(ICategoryService categoryService, IPaginationService<CategoryDTO> paginationService)
        {
            _categoryService = categoryService;
            _paginationService = paginationService;
        }
        #region GetAll
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<CategoryDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] SearchCategoryRequestDTO request)
        {
            var (data, totalItems) = await _categoryService.GetAll(request);

            var paginatedItems = _paginationService.GetPagedData(totalItems, data, request.PageInfo);

            return Ok(new ApiResponseDTO<PagingResponseDTO<CategoryDTO>>
            {
                Success = true,
                Data = paginatedItems
            });
        }
        #endregion

        #region Get
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var items = await _categoryService.Get(id);

            return Ok(new ApiResponseDTO<CategoryDTO>
            {
                Success = true,
                Data = items
            });
        }

        #endregion

        #region Create
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDTO request)
        {
            var item = await _categoryService.Create(request);
            return Ok(new ApiResponseDTO<Category>
            {
                Success = true,
                Data = item
            });
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequestDTO request)
        {
            var item = await _categoryService.Update(id, request);

            return Ok(new ApiResponseDTO<CategoryDTO>
            {
                Success = true,
                Data = item
            });
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
            await _categoryService.Delete(id);
            return Ok(new ApiResponseDTO<object> { Success = true });
        }
        #endregion
    }
}
