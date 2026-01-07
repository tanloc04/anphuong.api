using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.RequestDTOs.Customer;
using anphuong.Core.Domains.DTOs.RequestDTOs.Products;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IEmailService _emailService;
        private readonly IPaginationService<CustomerUserDTO> _paginationService;

        public CustomerController(IUserService userService, ICustomerService customerService,
            IEmailService emailService,
            IPaginationService<CustomerUserDTO> paginationService)
        {
            _userService = userService;
            _customerService = customerService;
            _emailService = emailService;
            _paginationService = paginationService;
        }

        #region Test Register
        [HttpPost("test-register")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DumpRegister([FromBody] RegisterRequestDTO registerRequest)
        {
            if (_userService.IsUserExists(registerRequest.Email).GetAwaiter().GetResult())
            {
                return BadRequest(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Email already exists"
                });
            }

            await _userService.TestRegisterAsync(registerRequest);

            return StatusCode(StatusCodes.Status201Created, new ApiResponseDTO<object>
            {
                Success = true,
                Message = "User registered successfully"
            });
        }
        #endregion

        #region Register
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
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
            try
            {
                var id = await _userService.RegisterAsync(registerRequest);
                var baseConfirmAccountEndpoint = Environment.GetEnvironmentVariable("CONFIRM_ACCOUNT_ENDPOINT")
                ?? throw new InvalidOperationException("CONFIRM_ACCOUNT_ENDPOINT environment variable is not set.");

                string confirmAccountEndpoint = baseConfirmAccountEndpoint + id;

                await _emailService.SendEmailAsync(
                    registerRequest.Email,
                    "Confirm Your An Phuong Account",
                    "Click here to confirm your account: " + confirmAccountEndpoint
                );

                return StatusCode(StatusCodes.Status201Created, new ApiResponseDTO<object>
                {
                    Success = true,
                    Message = "Email have sent to your"
                });
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDTO<object>
                {
                    Success = true,
                    Message = ex.ToString()
                });
            }

        }
        #endregion

        #region Confirm Account
        [HttpPut("account-confirmation/{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmAccount(int id)
        {
            await _userService.ActivateUserAsync(id);
            return StatusCode(StatusCodes.Status200OK, new ApiResponseDTO<object>
            {
                Success = true,
                Message = "Account Activated Successfully."
            });
        }
        #endregion

        #region Get Customers     
        [Authorize(Policy = "AllowSpecificEmail")]
        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponseDTO<PagingResponseDTO<CustomerUserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers([FromBody] SearchUsersRequestDTO request)
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
            var searchCondition = request.SearchCondition ?? new SearchUsersCondition();
            var pageInfo = request.PageInfo ?? new PageInfoRequestDTO();

            var (data, totalItems) = await _customerService.GetCustomerUserDTOsAsync(request);

            var paginatedUsers = _paginationService.GetPagedData(totalItems, data, pageInfo);
            return Ok(new ApiResponseDTO<PagingResponseDTO<CustomerUserDTO>>
            {
                Success = true,
                Data = paginatedUsers
            });
        }
        #endregion

        #region Send Email
        [HttpPost("send-email")]
        public async Task<IActionResult> SendForm([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");
            var anphuongIcon = Environment.GetEnvironmentVariable("AN_PHUONG_ICON")
            ?? throw new InvalidOperationException("AN_PHUONG_ICON environment variable is not set.");
            string url = "https://www.youtube.com/watch?v=UwuAPyOImoI&list=RDUwuAPyOImoI&start_radio=1";
            string htmlBody = $@"
            <html>
            <body style='font-family: Roboto, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; text-align: center;'>
                    <img src='{anphuongIcon}' 
                         style='margin-bottom: 20px; max-width: 150px; height: auto;' />
                    <h2 style='color: #202124;'>Xác nhận tài khoản của bạn</h2>
                    <p style='color: #5f6368;'>Vui lòng nhấn nút bên dưới để xác nhận tài khoản và hoàn tất quá trình đăng ký.</p>
                    <a href='{url}' 
                       style='display: inline-block; margin: 20px 0; padding: 12px 25px; background-color: #1a73e8; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: bold;'>Xác Nhận</a>
                    <p style='color: #5f6368; font-size: 12px;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                </div>
            </body>
            </html>
            ";

            await _emailService.SendEmailAsync(email, "Xác Nhận Tài Khoản An Phương", htmlBody);

            return Ok(new { Success = true, Message = "Email has been sent." });
        }
        #endregion

        #region Update
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCustomerRequestDTO request)
        {
            var item = await _customerService.Update(id, request);

            return Ok(new ApiResponseDTO<CustomerDTO>
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
            await _customerService.Delete(id);
            return Ok(new ApiResponseDTO<object> { Success = true });
        }
        #endregion
    }
}
