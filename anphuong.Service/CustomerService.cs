using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using Mapster;

namespace anphuong.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        public CustomerService(ICustomerRepository customerRepository,
            IUserRepository userRepository)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
        }

        private Expression<Func<Customer, bool>> AddFilter(
            Expression<Func<Customer, bool>> existingFilter,
            Expression<Func<Customer, bool>> newFilter)
        {
            var parameter = Expression.Parameter(typeof(Customer), "u");

            var combined = Expression.Lambda<Func<Customer, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(existingFilter, parameter),
                    Expression.Invoke(newFilter, parameter)
                ),
                parameter
            );
            return combined;
        }

        public async Task<Customer> Create(CustomerDTO customerDTO)
        {
            var customerEntity = customerDTO.Adapt<Customer>();

            await _customerRepository.AddAsync(customerEntity);

            return customerEntity;
        }

        public async Task<List<CustomerDTO>> GetCustomerDTOs()
        {
            return (await _customerRepository.GetAllAsync()).Select(c => c.Adapt<CustomerDTO>()).ToList();
        }

        public async Task<(List<CustomerUserDTO>, int totalItems)> GetCustomerUserDTOsAsync(
            SearchCondition searchCondition,
            PageInfoRequestDTO pageInfo)
        {
            Expression<Func<Customer, bool>> filter = c => true;

            if (!string.IsNullOrEmpty(searchCondition.Keyword))
            {
                string keyword = searchCondition.Keyword.ToLower();
                filter = AddFilter(filter, c =>
                    (c.Fullname != null && c.Fullname.ToLower().Contains(keyword)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(keyword)) ||
                    (c.CustomerAddress != null && c.CustomerAddress.ToLower().Contains(keyword)) ||
                    (c.User != null && c.User.Username != null && c.User.Username.ToLower().Contains(keyword))
                );
            }

            if (!string.IsNullOrEmpty(searchCondition.Status))
            {
                filter = AddFilter(filter, c => c.User != null && c.User.Status == searchCondition.Status);
            }

            filter = AddFilter(filter, c => c.IsDeleted == searchCondition.IsDeleted);

            var customers = await _customerRepository.GetWithPaginationAsync(pageInfo, filter, includeProperties: "User");

            int totalItems = await _customerRepository.CountAsync(filter);

            var customerDTOs = customers.Select(c => new CustomerUserDTO
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsDeleted = c.IsDeleted,
                Fullname = c.Fullname,
                Phone = c.Phone,
                CustomerAddress = c.CustomerAddress,
                Username = c.User?.Username,
                Email = c.User?.Email,
                Status = c.User?.Status
            }).ToList();

            return (customerDTOs, totalItems);
        }


        public async Task<CustomerDTO> GetCustomerDTO(int id)
        {
            var customer = await _customerRepository.GetAsync(id);

            if (customer == null)
                return null!;

            return customer.Adapt<CustomerDTO>();
        }

        public async Task<CustomerUserDTO?> GetCustomerUserDTO(int id)
        {
            var customer = await _customerRepository.GetAsync(id, includeProperties: "User");

            if (customer == null) return null;

            var dto = new CustomerUserDTO
            {
                Id = customer.Id,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                IsDeleted = customer.IsDeleted,
                Fullname = customer.Fullname,
                Phone = customer.Phone,
                CustomerAddress = customer.CustomerAddress,
                Username = customer.User?.Username,
                Email = customer.User?.Email,
                Status = customer.User?.Status
            };

            return dto;
        }
    }
}
