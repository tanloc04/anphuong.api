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
using Azure.Core;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly IEmailService _emailService;
        public AuthController(IJwtService jwtService, IUserService userService,
            IGoogleAuthService googleAuthService, IEmailService emailService)
        {
            _jwtService = jwtService;
            _userService = userService;
            _googleAuthService = googleAuthService;
            _emailService = emailService;
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
            if (user.Status.Equals("DEACTIVE", StringComparison.OrdinalIgnoreCase) || user.Status.Equals("0"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "User is not permitted to log in. Account might be deactivated or restricted."
                });
            }

            var accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(Consts.REFRESHTOKEN_EXPIRED_TIME);
            await _userService.Update(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Secure = false,
                Path = "/"
            };
            Response.Cookies.Append("accessToken", accessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

            return Ok(new ApiResponseDTO<LoginDTO>()
            {
                Success = true,
                Message = "Login Success"
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

                var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);
                refreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.Now.AddDays(Consts.REFRESHTOKEN_EXPIRED_TIME);

                await _userService.Update(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    Path = "/"
                };
                Response.Cookies.Append("accessToken", token, cookieOptions);
                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

                return Ok(new ApiResponseDTO<LoginDTO>
                {
                    Success = true,
                    Message = "Login Success"
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
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "No Token provided in Cookies"
                });
            }

            var user = await _userService.CheckRefreshToken(refreshToken);

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

            var accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);
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

        #region Change Password
        [Authorize]
        [HttpPost("password")]
        [ProducesResponseType(typeof(ApiResponseDTO<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ApiResponseDTO<object>), StatusCodes.Status403Forbidden)]
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

        #region Forgot Password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            var user = await _userService.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponseDTO<object> { Success = false, Message = "Email không tồn tại trong hệ thống!" });
            }

            var otp = new Random().Next(100000, 999999).ToString();

            user.ResetPasswordToken = otp;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            await _userService.Update(user);

            string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <h2 style='color: #333;'>Khôi phục mật khẩu</h2>
                    <p>Bạn vừa yêu cầu đặt lại mật khẩu tại Nội Thất An Phương.</p>
                    <p>Mã xác nhận (OTP) của bạn là: <strong style='color: #c4a484; font-size: 24px; padding: 5px 10px; background: #f9f9f9; border-radius: 5px;'>{otp}</strong></p>
                    <p style='color: #777; font-size: 14px;'>Mã này sẽ hết hạn sau 15 phút. Vui lòng không chia sẻ mã này cho bất kỳ ai!</p>
                </div>";

            await _emailService.SendEmailAsync(request.Email, "Mã xác nhận khôi phục mật khẩu - Nội Thất An Phương", htmlBody);

            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                Message = "Mã OTP đã được gửi đến email của bạn!"
            });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            var user = await _userService.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponseDTO<object> { Success = false, Message = "Email không hợp lệ!" });
            }

            if (user.ResetPasswordToken != request.Otp || user.ResetPasswordExpiry < DateTime.Now)
            {
                return BadRequest(new ApiResponseDTO<object> { Success = false, Message = "Mã OTP không hợp lệ hoặc đã hết hạn!" });
            }

            user.PasswordHash = request.NewPassword;
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;
            await _userService.Update(user);

            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập lại."
            });
        }
        #endregion

        #region Block User
        [HttpPut("block/{id}")]
        [Authorize(Policy = "AllowSpecificEmail")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var success = await _userService.BlockUserAsync(id);

            if (!success)
            {
                return NotFound(new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng trong hệ thống!"
                });
            }

            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                Message = "Đã khóa tài khoản người dùng thành công!"
            });
        }
        #endregion
    }
}
