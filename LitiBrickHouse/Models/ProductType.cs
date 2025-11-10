
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitiBrickHouse.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Ví dụ: "1 Lego Tùy chỉnh", "Lego Bóng đá"

        public string Description { get; set; }

        // Giá gốc cho loại này (dùng ở Giai đoạn 5)
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePrice { get; set; } // Ví dụ: 239000
    }
}
