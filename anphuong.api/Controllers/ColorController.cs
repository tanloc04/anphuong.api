using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Color;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;

        private readonly IPaginationService<ColorDTO> _paginationService;

        public ColorController(IColorService colorService, IPaginationService<ColorDTO> paginationService)
        {
            _colorService = colorService;
            _paginationService = paginationService;
        }

        #region GetAll
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<ColorDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] SearchColorsRequestDTO request)
        {
            var (data, totalItems) = await _colorService.GetAll(request);
            var paginatedItems = _paginationService.GetPagedData(totalItems, data, request.PageInfo);
            return Ok(new ApiResponseDTO<PagingResponseDTO<ColorDTO>>
            {
                Success = true,
                Data = paginatedItems
            });
        }
        #endregion

        #region Get
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<ColorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var items = await _colorService.Get(id);
            return Ok(new ApiResponseDTO<ColorDTO>
            {
                Success = true,
                Data = items
            });
        }
        #endregion

        #region Create
        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDTO<ColorDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateColorRequestDTO request)
        {
            var createdColor = await _colorService.Create(request);
            var colorDTO = new ColorDTO
            {
                Id = createdColor.Id,
                Name = createdColor.Name,
                HexCode = createdColor.HexCode
            };
            return CreatedAtAction(nameof(Get), new { id = colorDTO.Id }, new ApiResponseDTO<ColorDTO>
            {
                Success = true,
                Data = colorDTO
            });
        }
        #endregion

        #region Update
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<ColorDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateColorRequestDTO request)
        {
            var updatedColor = await _colorService.Update(id, request);
            return Ok(new ApiResponseDTO<ColorDTO>
            {
                Success = true,
                Data = updatedColor
            });
        }
        #endregion
    }
}
