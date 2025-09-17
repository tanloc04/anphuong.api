using anphuong.Core.Constants;
using anphuong.Core.Domains.DTOs.StandardizedDTOs;
using anphuong.Core.Interfaces.Services;

namespace anphuong.Service
{
    public class PaginationService<T> : IPaginationService<T>
    {
        public PagingResponseDTO<T> GetPagedData(int totalItems, IEnumerable<T> data, PageInfoRequestDTO pageInfo)
        {

            if (totalItems == 0 && data == null)
            {
                return new PagingResponseDTO<T>
                {
                    PageInfo = new PageInfoResponse
                    {
                        PageNum = 0,
                        PageSize = 0,
                        TotalItems = 0,
                        TotalPages = 0
                    },
                    PageData = null
                };
            }

            int pageNum;
            int pageSize;
            int totalPages;

            if (pageInfo.PageNum == 0)
            {
                pageNum = Consts.PAGE_NUM_DEFAULT;
            }
            else pageNum = pageInfo.PageNum;

            if (pageInfo.PageSize == 0)
            {
                pageSize = Consts.PAGE_SIZE_DEFAULT;
            }
            else pageSize = pageInfo.PageSize;

            if (pageSize == 0)
            {
                totalPages = 0;
            }
            else
            {
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            }

            return new PagingResponseDTO<T>
            {
                PageInfo = new PageInfoResponse
                {
                    PageNum = pageNum,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                },
                PageData = data
            };
        }
    }
}
