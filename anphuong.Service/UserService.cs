using System.Linq.Expressions;
using System.Security.Claims;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace anphuong.Service
{

    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterAsync(RegisterRequestDTO requestDTO)
        {
            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDTO.Password);

            // Create the user and save to the database
            var newUser = new User
            {
                Email = requestDTO.Email,
                PasswordHash = hashedPassword,
                Username = requestDTO.Username,
                Status = "1"
            };
            await _userRepository.AddAsync(newUser);
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetAsync(user => user.Email == email);
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
            var user = await _userRepository.GetAsync(user => user.Email == email);
            return user != null;
        }

        public async Task<UserDTO?> FindByEmailAsync(string email)
        {
            var user = await _userRepository.GetAsync(user => user.Email == email);
            if (user == null)
            {
                return null;
            }
            return user.Adapt<UserDTO>();
        }

        public async Task<bool> UpdatePassword(UserDTO userDTO, string password)
        {
            var user = await _userRepository.GetAsync(userDTO.Id);
            if (user == null)
            {
                return false;
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password); // Update with a hashed password
            user.UpdatedAt = DateTime.Now;
            return _userRepository.Update(user);
        }

        public async Task<UserDTO?> FindByIdAsync(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return null;
            }
            return user.Adapt<UserDTO>();
        }

        public async Task<bool> VerifyPassword(UserDTO userDTO, string oldPassword)
        {
            var user = await _userRepository.GetAsync(userDTO.Id);
            if (user == null)
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
        }

        public async Task<(List<UserDTO>, int totalItems)> GetUsersAsync(SearchCondition searchCondition, PageInfoRequestDTO pageInfo)
        {
            // Start with a base filter that is always true
            Expression<Func<User, bool>> filter = u => true;

            // Apply filters dynamically
            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                string keyword = searchCondition.Keyword.ToLower();
                filter = AddFilter(filter, u =>
                    (u.Username != null && u.Username.ToLower().Contains(keyword)) ||
                    u.Email.ToLower().Contains(keyword));
            }

            filter = AddFilter(filter, u => u.Status == searchCondition.Status && u.IsDeleted == searchCondition.IsDeleted);

            var users = await _userRepository.GetWithPaginationAsync(pageInfo, filter);
            int totalItems = await _userRepository.CountAsync(filter);

            List<UserDTO> userDTOs = users.Select(user => user.Adapt<UserDTO>()).ToList();

            return (userDTOs, totalItems);
        }

        private Expression<Func<User, bool>> AddFilter(
            Expression<Func<User, bool>> existingFilter,
            Expression<Func<User, bool>> newFilter)
        {
            var parameter = Expression.Parameter(typeof(User), "u");

            var combined = Expression.Lambda<Func<User, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(existingFilter, parameter),
                    Expression.Invoke(newFilter, parameter)
                ),
                parameter
            );

            return combined;
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

            var user = await _userRepository.GetAsync(id);
            if (user == null) return false; // User not found
            user.IsDeleted = true;
            return _userRepository.Update(user);
        }
    }
}
