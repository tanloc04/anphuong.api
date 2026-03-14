using System.Linq.Expressions;
using System.Security.Claims;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Domains.DTOs.RequestDTOs.Auth;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Core.Ultilities;
using anphuong.Repository.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace anphuong.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IConfiguration _config;

        public UserService(IUserRepository userRepository, ICustomerRepository customerRepository,
            IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _repository = userRepository;
            _customerRepository = customerRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public async Task<int> RegisterAsync(RegisterRequestDTO requestDTO)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDTO.Password);

            string allowedEmails = _config["ALLOWED_EMAILS"] ?? "";

            bool isAdmin = allowedEmails.Split(',').Any(e => e.Trim().Equals(requestDTO.Email, StringComparison.OrdinalIgnoreCase));

            var newCustomer = new Customer
            {
                Phone = requestDTO.Phone,
                FullName = requestDTO.FullName,
                Address = requestDTO.CustomerAddress,
                User = new User
                {
                    Email = requestDTO.Email,
                    Username = requestDTO.Username,
                    PasswordHash = hashedPassword,
                    Status = "DEACTIVE",
                    Role = isAdmin
                }
            };

            await _customerRepository.AddAsync(newCustomer);
            return newCustomer.Id;
        }

        public async Task TestRegisterAsync(RegisterRequestDTO requestDTO)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDTO.Password);

            string allowedEmails = _config["ALLOWED_EMAILS"] ?? "";
            bool isAdmin = allowedEmails.Split(',').Any(e => e.Trim().Equals(requestDTO.Email, StringComparison.OrdinalIgnoreCase));

            var newCustomer = new Customer
            {
                Phone = requestDTO.Phone,
                FullName = requestDTO.FullName,
                Address = requestDTO.CustomerAddress,
                User = new User
                {
                    Email = requestDTO.Email,
                    Username = requestDTO.Username,
                    PasswordHash = hashedPassword,
                    Status = "ACTIVE",
                    Role = isAdmin
                }
            };

            await _customerRepository.AddAsync(newCustomer);
        }

        public async Task<User> GoogleRegisterAsync(GoogleRegisterRequestDTO requestDTO)
        {
            string allowedEmails = _config["ALLOWED_EMAILS"] ?? "";
            bool isAdmin = allowedEmails.Split(',').Any(e => e.Trim().Equals(requestDTO.Email, StringComparison.OrdinalIgnoreCase));
            var newCustomer = new Customer
            {
                Phone = "",
                FullName = requestDTO.FullName,
                Address = "",
                User = new User
                {
                    Email = requestDTO.Email,
                    Username = requestDTO.Username,
                    Status = "ACTIVE",
                    Role = isAdmin
                }
            };

            await _customerRepository.AddAsync(newCustomer);
            return newCustomer.User;
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            var user = await _repository.GetAsync(user => user.Email == email);
            if (user == null)
            {
                return null;
            }
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }
            return user;
        }

        public async Task<bool> IsUserExists(string email)
        {
            var user = await _repository.GetAsync(user => user.Email == email);
            return user != null;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var user = await _repository.GetAsync(user => user.Email == email);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<(bool Success, string Message)> UpdatePassword(ChangePasswordRequestDTO request)
        {
            if (!await UserExist(request.Id))
                return (false, "User does not exist.");

            if (request.Password == request.OldPassword)
                return (false, "New password must be different from old password.");

            var user = await _repository.GetAsync(request.Id);
            if (user == null)
                return (false, "User not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                return (false, "Invalid old password.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.UpdatedAt = DateTime.UtcNow;

            return _repository.Update(user)
                ? (true, "Password updated successfully.")
                : (false, "Failed to update password.");
        }

        public async Task<CustomerUserDTO?> FindByIdAsync(int id)
        {
            var user = await _repository.GetAsync(id, includeProperties: "Customer");
            if (user == null) return null;

            var dto = new CustomerUserDTO
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted,
                Fullname = user.Customer?.FullName ?? user.Username,
                Phone = user.Customer?.Phone ?? string.Empty,
                CustomerAddress = user.Customer?.Address ?? string.Empty,
                Username = user.Username,
                Email = user.Email,
                Status = user.Status,
                Role = user.Role
            };

            return dto;
        }

        public async Task<bool> VerifyPassword(UserDTO userDTO, string oldPassword)
        {
            var user = await _repository.GetAsync(userDTO.Id);
            if (user == null)
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
        }

        public async Task<(IEnumerable<UserDTO>, int totalItems)> GetUsersAsync(SearchUsersRequestDTO request)
        {
            var searchCondition = request?.SearchCondition ?? new SearchUsersCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            Expression<Func<User, bool>> filter = u => true;

            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                var key = searchCondition.Keyword.ToLower();
                filter = ExpressionUtils.AddFilter(filter, x =>
                    x.Username.ToLower().Contains(key) ||
                    x.Email.ToLower().Contains(key)
                );
            }

            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            var items = await _repository.GetWithPaginationAsync(pageInfo, filter);
            var totalItems = await _repository.CountAsync(filter);

            return (items.Adapt<IEnumerable<UserDTO>>(), totalItems);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? currentUserId = null;
            if (int.TryParse(userIdClaim, out var parsedId))
            {
                currentUserId = parsedId;
            }
            if (parsedId == id) throw new BusinessException(ErrorDetails.CAN_NOT_DELETE_YOURSELF);

            var user = await _repository.GetAsync(id);
            if (user == null) return false;
            user.IsDeleted = true;
            return _repository.Update(user);
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            return await _repository.ActivateUserAsync(id);
        }

        public async Task<User?> CheckRefreshToken(string refreshToken)
        {
            return await _repository.CheckRefreshToken(refreshToken);
        }

        public async Task<bool> Update(User user)
        {
            return _repository.Update(user);
        }

        public async Task<bool> UserExist(int id)
        {
            return await _repository.ExistsAsync(u => u.Id == id);
        }

        //public async Task<bool> BlockUserAsync(int userId)
        //{
        //    var user = await _repository.GetAsync(userId);

        //    if (user == null)
        //        return false;

        //    user.Status = user.Status == "ACTIVE" ? "DEACTIVE" : "ACTIVE";
        //    user.UpdatedAt = DateTime.Now;

        //    await Update(user);

        //    return true;
        //}

        public async Task<bool> BlockUserAsync(int userId)
        {
            var user = await _repository.GetAsync(userId);
            if (user == null)
                return false;

            string currentStatus = user.Status?.Trim().ToUpper();

            user.Status = (currentStatus == "ACTIVE") ? "DEACTIVE" : "ACTIVE";
            user.UpdatedAt = DateTime.Now;

            bool isSaved = await Update(user);
            Console.WriteLine($"\n[DEBUG] USER ID: {userId}");
            Console.WriteLine($"[DEBUG] CŨ: '{currentStatus}' ---> MỚI: '{user.Status}'");
            Console.WriteLine($"[DEBUG] TRẠNG THÁI LƯU (isSaved): {isSaved}\n");

            return isSaved;
        }
    }
}