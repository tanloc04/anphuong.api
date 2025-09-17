using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;

namespace anphuong.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        public Task<Customer> Create(CustomerDTO customerDTO);

        public Task<List<CustomerDTO>> GetCustomerDTOs();

        public Task<(List<CustomerUserDTO>, int totalItems)> GetCustomerUserDTOsAsync(
            SearchCondition searchCondition,
            PageInfoRequestDTO pageInfo);

        public Task<CustomerDTO> GetCustomerDTO(int id);

        public Task<CustomerUserDTO?> GetCustomerUserDTO(int id);
    }
}
