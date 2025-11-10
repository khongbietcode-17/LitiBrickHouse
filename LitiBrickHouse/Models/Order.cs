using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitiBrickHouse.Models
{
    public class Order
    {
        public int Id { get; set; }

        // --- Thông tin Khách hàng (Từ Giai đoạn 4) ---

        public string? SocialMediaName { get; set; } // Tên Mạng Xã hội (Facebook, Zalo...)
        
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }


        // --- Ghi chú (Từ Giai đoạn 2) ---
        public string Note { get; set; }

        // --- Thông tin Đơn hàng (Từ Giai đoạn 5) ---
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }
        public Enums.OrderStatus Status { get; set; } = Enums.OrderStatus.Moi; // Mặc định là "Mới"
        // --- Navigation Properties (Các món hàng trong đơn) ---

        // 1. Danh sách các bộ Lego TÙY CHỈNH (cho loại 1 Lego, 2 Lego)
        public ICollection<OrderCustomLego> CustomLegos { get; set; }

        // 2. Danh sách các món đồ ĐƠN LẺ (Background, Cầu thủ)
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}