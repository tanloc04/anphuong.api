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
using anphuong.Repository.Repositories;
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

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {

                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, 
                Expires = DateTime.UtcNow.AddDays(Consts.REFRESHTOKEN_EXPIRED_TIME)
            };

            Response.Cookies.Append("accessToken", accessToken, cookieOptions);

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        #region Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponseDTO<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
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

            SetTokenCookies(accessToken, refreshToken);

            return Ok(new ApiResponseDTO<LoginDTO>()
            {
                Success = true,
                Data = new LoginDTO()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            });
        }
        #endregion

        #region Get Current User
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDTO<CustomerUserDTO>), StatusCodes.Status200OK)]
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
            return Ok(new ApiResponseDTO<CustomerUserDTO>
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
                var payload = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);
                string refreshToken;
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

                var user = await _userService.FindByEmailAsync(email);

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

                if (!user.Status.Equals("ACTIVE"))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "User is not permitted to log in."
                    });
                }

                var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);
                refreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.Now.AddDays(Consts.REFRESHTOKEN_EXPIRED_TIME);

                await _userService.Update(user);

                // --- GỌI HÀM SET COOKIE TẠI ĐÂY ---
                SetTokenCookies(token, refreshToken);

                return Ok(new ApiResponseDTO<LoginDTO>
                {
                    Success = true,
                    Data = new LoginDTO
                    {
                        AccessToken = token,
                        RefreshToken = user.RefreshToken
                    }
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
            // Logic mới: Ưu tiên lấy Refresh Token từ Cookie nếu Body không có (Optional)
            var refreshTokenToCheck = loginRequest.RefreshToken;
            if (string.IsNullOrEmpty(refreshTokenToCheck))
            {
                Request.Cookies.TryGetValue("refreshToken", out refreshTokenToCheck);
            }

            if (string.IsNullOrEmpty(refreshTokenToCheck))
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "No Token provided"
                });
            }

            var user = await _userService.CheckRefreshToken(refreshTokenToCheck);

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

            // --- GỌI HÀM SET COOKIE TẠI ĐÂY ---
            // Lưu ý: Có thể bạn muốn tạo Refresh Token mới mỗi lần refresh (Rotation)
            // Nếu giữ nguyên refresh token cũ thì chỉ cần update access token cookie
            SetTokenCookies(accessToken, user.RefreshToken);

            return Ok(new ApiResponseDTO<LoginDTO>()
            {
                Success = true,
                Data = new LoginDTO()
                {
                    AccessToken = accessToken,
                    // RefreshToken = user.RefreshToken // Có thể trả về hoặc không tuỳ client
                }
            });
        }

        // Thêm API Logout để xóa Cookie
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            };

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok(new ApiResponseDTO<object> { Success = true, Message = "Logged out successfully" });
        }
        #endregion

        #region Change Password
        [Authorize]
        [HttpPost("password")]
        [ProducesResponseType(typeof(ApiResponseDTO<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var (success, message) = await _userService.UpdatePassword(request);

            if (!success)
            {
                return BadRequest(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = message
                });
            }

            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                Message = message
            });
        }
        #endregion
    }
}