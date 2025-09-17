using anphuong.Core.Domains.DTOs.StandardizedDTOs;

namespace anphuong.Core.Interfaces.Services
{
    public interface IPaginationService<T>
    {
        PagingResponseDTO<T> GetPagedData(int totalItems, IEnumerable<T> data, PageInfoRequestDTO pageInfo);
    }
}
