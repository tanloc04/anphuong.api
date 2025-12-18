using System.Security.Cryptography;
using anphuong.Core.Domains.DTOs;

namespace anphuong.Core.Ultilities
{
    public class StringGeneratorUtils
    {
        public static string GenerateRandomUsername()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var bytes = new byte[6];
            RandomNumberGenerator.Fill(bytes);

            char[] result = new char[6];
            for (int i = 0; i < 6; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }

            return "anphuong_" + new string(result);
        }

        public string GenerateOrderEmailHtml(OrderDTO order)
        {
            string paymentMethod = order.PaymentMethod switch
            {
                0 => "COD",
                1 => "Thanh toán qua Ngân Hàng",
                2 => "Thanh toán tại quầy",
                _ => "Không rõ"
            };

            string status = order.Status switch
            {
                0 => "Bị hủy",
                1 => "Đang xử lý",
                2 => "Hoàn thành",
                _ => "Đang xử lý"
            };

            // Header
            string header = $@"
            <h2>Đơn hàng của bạn đã được đặt!</h2>
            <p>Phương thức thanh toán: {paymentMethod}<br/>
               Trạng thái: {status}<br/>
               Ngày giao hàng dự kiến: {order.ShippingDate:yyyy-MM-dd}</p>";

                // Table header
                string tableHeader = @"
            <table style='width:100%;border-collapse:collapse;'>
                <thead>
                    <tr>
                        <th style='border:1px solid #ddd;padding:8px;'>Hình Sản Phẩm</th>
                        <th style='border:1px solid #ddd;padding:8px;'>Tên</th>
                        <th style='border:1px solid #ddd;padding:8px;'>Giá</th>
                        <th style='border:1px solid #ddd;padding:8px;'>Số lượng</th>
                        <th style='border:1px solid #ddd;padding:8px;'>Tổng</th>
                    </tr>
                </thead>
                <tbody>";

                // Table rows
                string tableRows = "";
                foreach (var item in order.OrderDetails)
                {
                    string imageUrl = !string.IsNullOrEmpty(item.Thumbnail)
                        ? item.Thumbnail
                        : "https://via.placeholder.com/80";
                    double price = item.Quantity > 0 ? item.SubTotalPrice / item.Quantity : 0;

                    tableRows += $@"
                <tr>
                    <td style='border:1px solid #ddd;padding:8px;text-align:center;'>
                        <img src='{imageUrl}' width='80' height='80'/>
                    </td>
                    <td style='border:1px solid #ddd;padding:8px;'>{item.ProductName}</td>
                    <td style='border:1px solid #ddd;padding:8px;text-align:right;'>{price:N0}</td>
                    <td style='border:1px solid #ddd;padding:8px;text-align:center;'>{item.Quantity}</td>
                    <td style='border:1px solid #ddd;padding:8px;text-align:right;'>{item.SubTotalPrice:N0}</td>
                </tr>";
                }

                // Total row
                tableRows += $@"
            <tr>
                <td colspan='4' style='border:1px solid #ddd;padding:8px;text-align:right;font-weight:bold;'>Giá Trị đơn hàng</td>
                <td style='border:1px solid #ddd;padding:8px;text-align:right;font-weight:bold;'>{order.TotalPrice:N0}</td>
            </tr>";

            string tableFooter = "</tbody></table>";

            return header + tableHeader + tableRows + tableFooter;
        }

        public string GenerateRevenueReportHtml(IEnumerable<OrderDTO> orders, 
            DateTime? fromDate, DateTime? toDate, double? totalRevenue)
        {
            string header = $@"
            <h2>Báo Cáo Doanh Thu</h2>
            <p>Từ Ngày: {fromDate:dd-MM-yyyy} &nbsp;&nbsp; Đến Ngày: {toDate:dd-MM-yyyy}</p>";

                string tableHeader = @"
            <table style='width:100%; border-collapse: collapse;'>
                <thead>
                    <tr style='background-color:#f2f2f2;'>
                        <th style='border:1px solid #ddd; padding:8px;'>Trạng Thái Đơn Hàng</th>
                        <th style='border:1px solid #ddd; padding:8px;'>Tổng Tiền</th>
                        <th style='border:1px solid #ddd; padding:8px;'>Ngày Tạo</th>
                        <th style='border:1px solid #ddd; padding:8px;'>Ngày Hoàn Thành</th>
                    </tr>
                </thead>
                <tbody>";

                string tableRows = "";

                foreach (var order in orders)
                {
                    string statusText = order.Status switch
                    {
                        0 => "Bị hủy",
                        1 => "Đang xử lý",
                        2 => "Hoàn thành",
                        _ => "Đang xử lý"
                    };

                    tableRows += $@"
                <tr>
                    <td style='border:1px solid #ddd; padding:8px; text-align:center'>{statusText}</td>
                    <td style='border:1px solid #ddd; padding:8px; text-align:right;'>{order.TotalPrice:N0}</td>
                    <td style='border:1px solid #ddd; padding:8px; text-align:center'>{order.CreatedAt:dd-MM-yyyy HH:mm}</td>
                    <td style='border:1px solid #ddd; padding:8px; text-align:center'>{order.UpdatedAt:dd-MM-yyyy HH:mm}</td>
                </tr>";
                }

                tableRows += $@"
            <tr style='font-weight:bold; background-color:#f9f9f9;'>
                <td colspan='1' style='border:1px solid #ddd; padding:8px; text-align:center;'>Tổng Doanh Thu</td>
                <td colspan='3' style='border:1px solid #ddd; padding:8px; text-align:center;'>{totalRevenue:N0}</td>
            </tr>";

            string tableFooter = "</tbody></table>";

            return header + tableHeader + tableRows + tableFooter;
        }
    }
}
