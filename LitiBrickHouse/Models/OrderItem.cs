using System.ComponentModel.DataAnnotations.Schema;

namespace LitiBrickHouse.Models
{
    // Lưu các món đồ đơn lẻ (Background, Cầu thủ)
    public class OrderItem
    {
        public int Id { get; set; }

        // Khóa ngoại: Món này thuộc Đơn hàng nào?
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        // Khóa ngoại: Món này là CustomOption nào?
        // (Đây sẽ là ID của Background hoặc ID của Cầu thủ)
        public int CustomOptionId { get; set; }
        [ForeignKey("CustomOptionId")]
        public CustomOption CustomOption { get; set; }
    }
}