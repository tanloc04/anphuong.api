using anphuong.Core.Domains.DTOs.RequestDTOs.VNPay;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Interfaces.Services.External
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPayRequestDTO request, HttpContext context);
    }
}
