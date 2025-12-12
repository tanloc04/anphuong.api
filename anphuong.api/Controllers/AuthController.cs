using System.Security.Claims;
using System.Security.Cryptography;
using anphuong.Core.Constants;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Auth;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Core.Ultilities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static anphuong.Core.Exceptions.GoogleException;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IGoogleAuthService _googleAuthService;
        public AuthController(IJwtService jwtService, IUserService userService,
            IGoogleAuthService googleAuthService)
        {
            _jwtService = jwtService;
            _userService = userService;
            _googleAuthService = googleAuthService;
        }

        #region Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponseDTO<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
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

            var accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(Consts.REFRESHTOKEN_EXPIRED_TIME);
            await _userService.Update(user);

            return Ok(new ApiResponseDTO<LoginDTO>()
            {
                Success = true,
                Data = new LoginDTO() {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            });
        }
        #endregion

        #region Get Current User
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDTO<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
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
                return BadRequest(new ApiResponseDTO<object>
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
        }
        #endregion

        #region Google Login
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
        {
            if (string.IsNullOrEmpty(request?.IdToken))
                return BadRequest(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "No Token provided"
                });

            try
            {
                // verify token
                var payload = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);

                // validate expiration
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (payload.ExpirationTimeSeconds < now)
                {
                    return Unauthorized(new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "Google token has expired."
                    });
                }

                var email = payload.Email;
                var name = payload.Name;

                // check user exist
                var user = await _userService.FindByEmailAsync(email);

                // if not exist -> register
                if (user == null)
                {
                    var registerDTO = new GoogleRegisterRequestDTO
                    {
                        Email = email,
                        FullName = name,
                        Username = StringGeneratorUtils.GenerateRandomUsername(),
                    };

                    user = await _userService.GoogleRegisterAsync(registerDTO);
                }

                // check status
                if (!user.Status.Equals("ACTIVE"))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "User is not permitted to log in."
                    });
                }

                // create JWT
                var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

                return Ok(new ApiResponseDTO<LoginDTO>
                {
                    Success = true,
                    Data = new LoginDTO { AccessToken = token }
                });
            }
            catch (TokenExpiredException)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Google token has expired."
                });
            }
            catch (InvalidTokenException)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Invalid Google Token."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("GOOGLE LOGIN EXCEPTION: " + ex.ToString());
                return StatusCode(500, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Server error during Google authentication."
                });
            }
        }
        #endregion

        #region Refresh Token
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponseDTO<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> checkRefreshToken([FromBody] LoginDTO loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.RefreshToken))
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "No Token provided"
                });
            }

            var user = await _userService.CheckRefreshToken(loginRequest.RefreshToken);

            if (user == null)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Invalid Refresh Token (User not found)"
                });
            }

            if (user.RefreshTokenExpiry <= DateTime.Now)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Token expired! Please login again"
                });
            }

            if (user.Status.Equals("0"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "User is not permitted to log in. Account might be deactivated."
                });
            }

            var accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email);
            return Ok(new ApiResponseDTO<LoginDTO>()
            {
                Success = true,
                Data = new LoginDTO()
                {
                    AccessToken = accessToken
                }
            });
        }
        #endregion
    }
}
