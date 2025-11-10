using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LitiBrickHouse.Models
{
    public class OptionCategory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // "Tóc", "Mặt", "Áo", "Background", "Cầu thủ"

        // Navigation property: Một danh mục có nhiều tùy chọn (phụ kiện)
        public ICollection<CustomOption> CustomOptions { get; set; }
    }
}