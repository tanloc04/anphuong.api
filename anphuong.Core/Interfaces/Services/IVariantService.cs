using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Variant;

namespace anphuong.Core.Interfaces.Services
{
    public interface IVariantService
    {
        public Task<VariantDTO> Create(CreateVariantRequestDTO request);
        public Task Delete(int id);

        public Task<(IEnumerable<VariantDTO>, int totalItems)> GetAll(SearchVariantRequestDTO request);

        public Task<VariantDTO> Get(int id);

        public Task<VariantDTO> Update(int id, UpdateVariantRequestDTO request);
    }
}
