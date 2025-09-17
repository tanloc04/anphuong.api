using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IPaginationService<CustomerUserDTO> _paginationService;

        public CustomerController(IUserService userService, ICustomerService customerService,
            IPaginationService<CustomerUserDTO> paginationService)
        {
            _userService = userService;
            _customerService = customerService;
            _paginationService = paginationService;
        }

        #region Register
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            if (_userService.IsUserExists(registerRequest.Email).GetAwaiter().GetResult())
            {
                return BadRequest(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Email already exists"
                });
            }

            await _userService.RegisterAsync(registerRequest);

            return StatusCode(StatusCodes.Status201Created, new ApiResponseDTO<object>
            {
                Success = true,
                Message = "User registered successfully"
            });
        }
        #endregion

        #region Get Customers
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<CustomerUserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers([FromBody] GetUsersRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Keys.Select(key => new ValidationErrorDTO
                    {
                        Field = key,
                        Message = ModelState[key]?.Errors.Select(e => e.ErrorMessage).ToList()
                    }).ToList()
                });
            }

            // Ensure non-null values for searchCondition and pageInfo
            var searchCondition = request.SearchCondition ?? new SearchCondition();
            var pageInfo = request.PageInfo ?? new PageInfoRequestDTO();

            var (data, totalItems) = await _customerService.GetCustomerUserDTOsAsync(searchCondition, pageInfo);

            var paginatedUsers = _paginationService.GetPagedData(totalItems, data, pageInfo);
            return Ok(new ApiResponseDTO<PagingResponseDTO<CustomerUserDTO>>
            {
                Success = true,
                Data = paginatedUsers
            });
        }
        #endregion
    }
}
