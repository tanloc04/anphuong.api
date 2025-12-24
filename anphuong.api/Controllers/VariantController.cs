using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Variant;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantController : ControllerBase
    {
        private readonly IVariantService _service;
        private readonly IPaginationService<VariantDTO> _paginationService;

        public VariantController(IVariantService service,
            IPaginationService<VariantDTO> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }

        #region Create        
        [Authorize(Policy = "AllowSpecificEmail")]
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateVariantRequestDTO request)
        {
            try
            {
                var item = await _service.Create(request);
                return Ok(new ApiResponseDTO<VariantDTO>
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

        #region GetAll
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<VariantDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] SearchVariantRequestDTO request)
        {
            var (data, totalItems) = await _service.GetAll(request);

            var paginatedItems = _paginationService.GetPagedData(totalItems, data, request.PageInfo);

            return Ok(new ApiResponseDTO<PagingResponseDTO<VariantDTO>>
            {
                Success = true,
                Data = paginatedItems
            });
        }
        #endregion

        #region Get
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<VariantDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _service.Get(id);

                return Ok(new ApiResponseDTO<VariantDTO>
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

        #region Update
        [Authorize(Policy = "AllowSpecificEmail")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateVariantRequestDTO request)
        {
            var item = await _service.Update(id, request);

            return Ok(new ApiResponseDTO<VariantDTO>
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
            await _service.Delete(id);
            return Ok(new ApiResponseDTO<object> { Success = true });
        }
        #endregion
    }
}
