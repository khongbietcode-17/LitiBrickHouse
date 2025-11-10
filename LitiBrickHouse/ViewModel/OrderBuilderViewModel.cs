using LitiBrickHouse.Models;
using System.ComponentModel.DataAnnotations;

namespace LitiBrickHouse.ViewModels
{
    // Đây là ViewModel chính, lưu toàn bộ 5 giai đoạn vào Session
    public class OrderBuilderViewModel
    {
        // GIAI ĐOẠN 1: Chọn loại sản phẩm
        public int? ProductTypeId { get; set; }

        // GIAI ĐOẠN 2: Background và Ghi chú
        public int? BackgroundId { get; set; }
        public string CustomerNote { get; set; }

        // GIAI ĐOẠN 3: Tùy chỉnh
        public LegoCustomizationSet Lego1 { get; set; } = new();
        public LegoCustomizationSet Lego2 { get; set; } = new(); // Chỉ dùng cho loại "2 Lego"
        public int? SportPlayerId { get; set; } // Chỉ dùng cho "Lego Bóng đá"

        // GIAI ĐOẠN 4: Thông tin giao hàng
        public string? SocialMediaName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập SĐT")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        // GIAI ĐOẠN 5: Tổng kết
        public decimal TotalPrice { get; set; }

        // (Phần này dùng để hiển thị ở Giai đoạn 5)
        public ProductType SelectedProductType { get; set; }
        public CustomOption SelectedBackground { get; set; }
        public CustomOption SelectedSportPlayer { get; set; }
        // (Chúng ta sẽ thêm cách hiển thị Lego1, Lego2 sau)
    }
}