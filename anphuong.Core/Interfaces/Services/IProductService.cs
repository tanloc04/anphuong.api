using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Ultilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Interfaces.Services
{
    public interface IProductService
    {
        public Task Create(CreateProductRequestDTO request);

        public Task Delete(int id);

        public Task<(IEnumerable<ProductDTO>, int totalItems)> GetAll(SearchProductRequestDTO request);

        public Task<ProductDTO> Get(int id);

        public Task Update(int id, UpdateProductRequestDTO request);
    }
}
