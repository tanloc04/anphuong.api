using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Orders;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IPaginationService<OrderDTO> _paginationService;

        public OrderController(IOrderService service,
            IPaginationService<OrderDTO> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }
        #region GetAll
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<OrderDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] SearchOrderRequestDTO request)
        {
            var (data, totalItems) = await _service.GetAll(request);

            var paginatedItems = _paginationService.GetPagedData(totalItems, data, request.PageInfo);

            return Ok(new ApiResponseDTO<PagingResponseDTO<OrderDTO>>
            {
                Success = true,
                Data = paginatedItems
            });
        }
        #endregion

        #region Get
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<OrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _service.Get(id);

                return Ok(new ApiResponseDTO<OrderDTO>
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
        //[Authorize(Policy = "AllowSpecificEmail")]
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequestDTO request)
        {
            var item = await _service.PlaceOrderAsync(request);
            return Ok(new ApiResponseDTO<OrderDTO>
            {
                Success = true,
                Data = item
            });
        }
        #endregion

        //#region Update
        ////[Authorize(Policy = "AllowSpecificEmail")]
        //[HttpPut("{id}")]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOrderRequestDTO request)
        //{
        //    var item = await _service.Update(id, request);

        //    return Ok(new ApiResponseDTO<OrderDTO>
        //    {
        //        Success = true,
        //        Data = item
        //    });
        //}
        //#endregion

        #region Delete
        //[Authorize(Policy = "AllowSpecificEmail")]
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
