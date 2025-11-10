using System.ComponentModel.DataAnnotations.Schema;

namespace LitiBrickHouse.Models
{
    // Lưu 1 bộ Lego tùy chỉnh (gồm 6 món)
    public class OrderCustomLego
    {
        public int Id { get; set; }

        // Khóa ngoại: Bộ Lego này thuộc Đơn hàng nào?
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        // --- Khóa ngoại cho 6 bộ phận ---
        // Chúng ta dùng nullable int (int?) vì có thể có món khách không chọn

        public int? HairId { get; set; }
        [ForeignKey("HairId")]
        public CustomOption Hair { get; set; }

        public int? FaceId { get; set; }
        [ForeignKey("FaceId")]
        public CustomOption Face { get; set; }

        public int? ClothesId { get; set; }
        [ForeignKey("ClothesId")]
        public CustomOption Clothes { get; set; }

        public int? PantsId { get; set; }
        [ForeignKey("PantsId")]
        public CustomOption Pants { get; set; }

        public int? Accessory1Id { get; set; }
        [ForeignKey("Accessory1Id")]
        public CustomOption Accessory1 { get; set; }

        public int? Accessory2Id { get; set; }
        [ForeignKey("Accessory2Id")]
        public CustomOption Accessory2 { get; set; }
    }
}