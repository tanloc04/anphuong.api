using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using anphuong.Core.Domains.DTOs.RequestDTOs.Product;
using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.DetailImage;

namespace anphuong.Core.Interfaces.Services
{
    public interface IDetailImageService
    {
        public Task Delete(int id);

        public Task<(IEnumerable<DetailImageDTO>, int totalItems)> GetAll(SearchDetailImageRequestDTO request);

        public Task<DetailImageDTO> Get(int id);

        public Task<DetailImageDTO> Update(int id, UpdateDetailImageRequestDTO request);
    }
}
