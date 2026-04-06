using anphuong.Core.Domains.DTOs.RequestDTOs.VNPay;
using anphuong.Core.Interfaces.Services.External;
using anphuong.Core.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Service
{
    public class VnPayService: IVnPayService
    {
        public VnPayService()
        {
            
        }

        public string CreatePaymentUrl(VnPayRequestDTO request, HttpContext context)
        {
            var vnp_TmnCode = Environment.GetEnvironmentVariable("VNPAY_TMN_CODE");
            var vnp_HashSecret = Environment.GetEnvironmentVariable("VNPAY_HASH_SECRET");
            var vnp_Url = Environment.GetEnvironmentVariable("VNPAY_BASE_URL");
            var vnp_Returnurl = Environment.GetEnvironmentVariable("VNPAY_RETURN_URL");

            // Kiểm tra xem đã đọc được .env chưa (tránh lỗi null)
            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                throw new Exception("Chưa cấu hình VNPay trong file .env sếp ơi!");
            }

            // VNPay yêu cầu số tiền phải nhân lên 100 lần (VD: 10,000 VND -> 1000000)
            var amount = (long)(request.Amount * 100);
      
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var vnpayData = new SortedList<string, string>(new VnPayCompare())
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", amount.ToString() },
        { "vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss") },
        { "vnp_CurrCode", "VND" },
        { "vnp_IpAddr", GetIpAddress(context) },
        { "vnp_Locale", "vn" },
        { "vnp_OrderInfo", request.OrderDescription },
        { "vnp_OrderType", "other" },
        { "vnp_ReturnUrl", vnp_Returnurl },
        { "vnp_TxnRef", request.OrderId.ToString() + "_" + timeNow.Ticks }, 
        { "vnp_ExpireDate", timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss") }
    };

            var queryString = new StringBuilder();
            foreach (var kvp in vnpayData)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    queryString.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
                }
            }
            var signData = queryString.ToString().TrimEnd('&');

            var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
            var paymentUrl = $"{vnp_Url}?{signData}&vnp_SecureHash={vnp_SecureHash}";

            return paymentUrl;
        }

        private string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            return string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress;
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }


}

