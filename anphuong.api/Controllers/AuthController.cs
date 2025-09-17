using System.Security.Claims;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AuthController(IJwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        #region Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponseDTO<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var user = await _userService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }
            if (user.Status.Equals("0"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "User is not permitted to log in. Account might be deactivated or restricted."
                });
            }

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);
            return Ok(new ApiResponseDTO<LoginDTO>()
            {
                Success = true,
                Data = new LoginDTO() { AccessToken = token }
            });
        }
        #endregion

        #region Get Current User
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDTO<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = true,
                    Message = "User is not authenticated"
                });
            }

            // Extract user information from claims
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            if (id == null)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = true,
                    Message = "User ID is not found in the claims."
                });
            }
            int intId = int.TryParse(id, out var parsed) ? parsed : 0;
            if (intId == 0)
            {
                return NotFound(new ApiResponseDTO<object>
                {
                    Success = true,
                    Message = "User not found."
                });
            }

            var userDTO = await _userService.FindByIdAsync(intId);
            userDTO.Email = email ?? string.Empty;
            return Ok(new ApiResponseDTO<UserDTO>
            {
                Success = true,
                Data = userDTO
            });
            #endregion
        }
    }
}
