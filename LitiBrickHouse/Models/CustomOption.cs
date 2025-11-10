using LitiBrickHouse.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitiBrickHouse.Models
{
    public class CustomOption
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string Name { get; set; }

        // ImageUrl 1024x1024 là bắt buộc
        [Required(ErrorMessage = "ImageUrl là bắt buộc")]
        public string ImageUrl { get; set; }

        // --- SỬA LỖI ---
        // Bỏ [Required] và thêm '?' để cho phép để trống (null)
        public string? ThumbnailUrl { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AdditionalPrice { get; set; } = 0;

        // --- SỬA LỖI ---
        // Bỏ [Required] và thêm '?' để cho phép để trống (null)
        public int? Quantity { get; set; } = null;

        // Gender sẽ có mặc định là Unisex (số 0)
        public GenderType Gender { get; set; } = GenderType.Unisex;

        // Bắt buộc phải chọn Danh mục
        [Required(ErrorMessage = "Vui lòng chọn Danh mục")]
        public int OptionCategoryId { get; set; }

        [ForeignKey("OptionCategoryId")]
        public OptionCategory OptionCategory { get; set; }
    }
}