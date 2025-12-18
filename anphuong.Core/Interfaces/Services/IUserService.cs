using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Auth;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> IsUserExists(string email);
        Task<int> RegisterAsync(RegisterRequestDTO requestDTO);
        Task TestRegisterAsync(RegisterRequestDTO requestDTO);
        Task<UserDTO> GoogleRegisterAsync(GoogleRegisterRequestDTO requestDTO);
        Task<User?> AuthenticateUserAsync(string email, string password);
        Task<UserDTO?> FindByEmailAsync(string email);
        Task<UserDTO?> FindByIdAsync(int id);
        Task<bool> UpdatePassword(UserDTO user, string password);
        Task<bool> VerifyPassword(UserDTO user, string oldPassword);
        Task<(IEnumerable<UserDTO>, int totalItems)> GetUsersAsync(SearchUsersRequestDTO request);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ActivateUserAsync(int userId);
        Task<User?> CheckRefreshToken(string refreshToken);
        Task<bool> Update(User user);
    }
}
