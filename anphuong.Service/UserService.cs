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

namespace anphuong.Service
{

    public class UserService : IUserService
    {

        private readonly IUserRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, ICustomerRepository customerRepository,
            IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = userRepository;
            _customerRepository = customerRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> RegisterAsync(RegisterRequestDTO requestDTO)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDTO.Password);

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
                    Status = "DEACTIVE"
                }
            };

            await _customerRepository.AddAsync(newCustomer);
            return newCustomer.Id;
        }

        public async Task TestRegisterAsync(RegisterRequestDTO requestDTO)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDTO.Password);

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
                    Status = "ACTIVE"
                }
            };

            await _customerRepository.AddAsync(newCustomer);
        }
        public async Task<User> GoogleRegisterAsync(GoogleRegisterRequestDTO requestDTO)
        {
            var newCustomer = new Customer
            {
                Phone = "",
                FullName = requestDTO.FullName,
                Address = "",
                User = new User
                {
                    Email = requestDTO.Email,
                    Username = requestDTO.Username,
                    Status = "ACTIVE"
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
                // User with the given email doesn't exist
                return null;
            }
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                // Password is incorrect
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
            var user = await _repository.GetAsync(id, includeProperties:"Customer");
            if (user == null)
            {
                return null;
            }
            if (user == null) return null;

            var dto = new CustomerUserDTO
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted,
                Fullname = user.Customer.FullName,
                Phone = user.Customer.Phone,
                CustomerAddress = user.Customer.Address,
                Username = user.Username,
                Email = user.Email,
                Status = user.Status
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

        //public async Task<(List<UserDTO>, int totalItems)> GetUsersAsync(SearchUsersRequestDTO request)
        //{
        //    // Start with a base filter that is always true
        //    Expression<Func<User, bool>> filter = u => true;

        //    // Apply filters dynamically
        //    if (!string.IsNullOrEmpty(SearchUsersCondition.Keyword))
        //    {
        //        string keyword = SearchUsersCondition.Keyword.ToLower();
        //        filter = AddFilter(filter, u =>
        //            (u.Username != null && u.Username.ToLower().Contains(keyword)) ||
        //            u.Email.ToLower().Contains(keyword));
        //    }

        //    filter = AddFilter(filter, u => u.Status == searchCondition.Status && u.IsDeleted == searchCondition.IsDeleted);

        //    var users = await _repository.GetWithPaginationAsync(pageInfo, filter);
        //    int totalItems = await _repository.CountAsync(filter);

        //    List<UserDTO> userDTOs = users.Select(user => user.Adapt<UserDTO>()).ToList();

        //    return (userDTOs, totalItems);
        //}

        public async Task<(IEnumerable<UserDTO>, int totalItems)> GetUsersAsync(SearchUsersRequestDTO request)
        {
            // If request or its components are null, create safe defaults
            var searchCondition = request?.SearchCondition ?? new SearchUsersCondition();
            var pageInfo = request?.PageInfo ?? new PageInfoRequestDTO();

            // Start with a base filter that is always true
            Expression<Func<User, bool>> filter = u => true;

            // Only apply keyword filter if keyword exists
            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                var key = searchCondition.Keyword.ToLower();
                filter = ExpressionUtils.AddFilter(filter, x =>
                    x.Username.ToLower().Contains(key) ||
                    x.Email.ToLower().Contains(key)
                );
            }

            // Only apply deletion filter if specified (default: return non-deleted)
            filter = ExpressionUtils.AddFilter(filter, u => u.IsDeleted == searchCondition.IsDeleted);

            // Query paginated products
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
            if (user == null) return false; // User not found
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
    }
}
