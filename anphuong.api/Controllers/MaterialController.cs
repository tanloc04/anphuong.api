using anphuong.Core.Domains.DTOs.RequestDTOs.Materials;
using anphuong.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _materialService.GetAllAsync();
            return Ok(new { Success = true, Data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _materialService.GetByIdAsync(id);

            if (result == null) return NotFound(new {Success = false, Message = "Không tìm thấy chất liệu!"});
            return Ok(new { Success = true, Data = result });
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateMaterialRequestDto request)
        {
            var result = await _materialService.CreateAsync(request);
            return Ok(new { Success = true, Message = "Thêm chất liệu thành công!" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody]CreateMaterialRequestDto request)
        {
            var success = await _materialService.UpdateAsync(id, request);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chất liệu không tồn tại hoặc lỗi cập nhật!" });
            }
            return Ok(new { Success = true, Message = "Cập nhật chất liệu thành công!" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _materialService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { Success = false, Message = "Chất liệu không tồn tại!" });
            }
            return Ok(new { Success = true, Message = "Xóa chất liệu thành công!" });
        }
    }
}
