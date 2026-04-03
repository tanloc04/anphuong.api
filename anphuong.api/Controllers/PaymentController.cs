using anphuong.Core.Domains.DTOs.RequestDTOs.VnPay;
using anphuong.Core.Domains.DTOs.RequestDTOs.VNPay;
using anphuong.Core.Interfaces.Services;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Core.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace anphuong.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        public PaymentController(IVnPayService vnPayService, IOrderService orderService, IEmailService emailService)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
            _emailService = emailService;
        }

        [HttpPost("create-payment-url")]
        public IActionResult CreatePaymentUrl([FromBody] VnPayRequestDTO request)
        {
            try
            {
                var paymentUrl = _vnPayService.CreatePaymentUrl(request, HttpContext);
                return Ok(new { success = true, paymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("vnpay-return")]
        public async Task<IActionResult> VnPayReturn([FromBody] VnPayReturnRequestDTO request)
        {
            try
            {
                // 1. Nếu giao dịch thất bại hoặc hủy (mã khác 00)
                if (request.ResponseCode != "00")
                {
                    // Chuyển Status về 0 (Cancel)
                    await _orderService.UpdateStatus(request.OrderId, 0);
                    return Ok(new { success = true, message = "Thanh toán thất bại, đã hủy đơn!" });
                }

                // 2. Giao dịch thành công (mã 00)
                // Không cần UpdateStatus vì mặc định lúc tạo nó đã là 1 (Processing) rồi.

                // Chỉ việc bế Email ra gửi cho khách thôi
                var order = await _orderService.Get(request.OrderId);
                var generator = new StringGeneratorUtils();
                string htmlBody = generator.GenerateOrderEmailHtml(order);

                await _emailService.SendEmailAsync(
                    order.Customer?.Email ?? "customer@example.com",
                    "Thanh toán VNPay Thành Công - Nội Thất An Phương",
                    htmlBody);

                return Ok(new { success = true, message = "Ghi nhận thanh toán và gửi mail thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
