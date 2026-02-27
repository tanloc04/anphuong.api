using System.Linq.Expressions;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.AuthController;
using anphuong.Core.Domains.DTOs.RequestDTOs.Customer;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Ultilities;
using Mapster;

namespace anphuong.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IUserRepository _userRepository;
        public CustomerService(ICustomerRepository repository,
            IUserRepository userRepository)
        {
            _repository = repository;
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
            await _repository.AddAsync(customerEntity);

            return customerEntity;
        }

        public async Task<List<CustomerDTO>> GetCustomerDTOs()
        {
            return (await _repository.GetAllAsync()).Select(c => c.Adapt<CustomerDTO>()).ToList();
        }

        public async Task<(List<CustomerUserDTO>, int totalItems)> GetCustomerUserDTOsAsync(SearchUsersRequestDTO request)
        {
            Expression<Func<Customer, bool>> filter = c => true;

            if (!string.IsNullOrEmpty(request.SearchCondition.Keyword))
            {
                string keyword = request.SearchCondition.Keyword.ToLower();
                filter = AddFilter(filter, c =>
                    (c.FullName != null && c.FullName.ToLower().Contains(keyword)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(keyword)) ||
                    (c.Address != null && c.Address.ToLower().Contains(keyword)) ||
                    (c.User != null && c.User.Username != null && c.User.Username.ToLower().Contains(keyword))
                );
            }

            if (!string.IsNullOrEmpty(request.SearchCondition.Status))
            {
                filter = AddFilter(filter, c => c.User != null && c.User.Status == request.SearchCondition.Status);
            }

            filter = AddFilter(filter, c => c.IsDeleted == request.SearchCondition.IsDeleted);

            var customers = await _repository.GetWithPaginationAsync(request.PageInfo, filter, includeProperties: "User");

            int totalItems = await _repository.CountAsync(filter);

            var customerDTOs = customers.Select(c => new CustomerUserDTO
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsDeleted = c.IsDeleted,
                Fullname = c.FullName,
                Phone = c.Phone,
                CustomerAddress = c.Address,
                Username = c.User?.Username,
                Email = c.User?.Email,
                Status = c.User?.Status
            }).ToList();

            return (customerDTOs, totalItems);
        }


        public async Task<CustomerDTO> GetCustomerDTO(int id)
        {
            var customer = await _repository.GetAsync(id);

            if (customer == null)
                return null!;

            return customer.Adapt<CustomerDTO>();
        }

        public async Task<CustomerUserDTO?> GetCustomerUserDTO(int id)
        {
            var customer = await _repository.GetAsync(id, includeProperties: "User");

            if (customer == null) return null;

            var dto = new CustomerUserDTO
            {
                Id = customer.Id,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                IsDeleted = customer.IsDeleted,
                Fullname = customer.FullName,
                Phone = customer.Phone,
                CustomerAddress = customer.Address,
                Username = customer.User?.Username,
                Email = customer.User?.Email,
                Status = customer.User?.Status
            };

            return dto;
        }
        public async Task<CustomerDTO> Update(int id, UpdateCustomerRequestDTO request)
        {
            var item = await _repository.GetAsync(id) 
                ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            bool isChanged = false;
        
            // Apply updates
            isChanged |= GenericHelperUtils.SetIfChanged(request.Fullname, () => item.FullName, i => item.FullName = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.Phone, () => item.Phone, i => item.Phone = i);
            isChanged |= GenericHelperUtils.SetIfChanged(request.CustomerAddress, () => item.Address, i => item.Address = i);
            if (isChanged)
            {
                item.UpdatedAt = DateTime.UtcNow;
                if (!_repository.Update(item))
                    throw new BusinessException(ErrorDetails.DEFAULT);
            }
            var itemDTO = item.Adapt<CustomerDTO>();
            return itemDTO;
        }
        public async Task Delete(int id)
        {
            var customer = await _repository.GetAsync(id) ?? throw new BusinessException(ErrorDetails.ID_NOT_FOUND);
            var user = await _userRepository.GetAsync(customer.User.Id);
            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.Now;
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.Now;  

            if (!_repository.Update(customer) && !_userRepository.Update(user))
            {
                throw new BusinessException(ErrorDetails.DEFAULT);
            }
        }
    }  
}
