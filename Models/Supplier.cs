using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên nhà cung cấp phải có từ 2 đến 100 ký tự")]
        [Display(Name = "Tên nhà cung cấp")]
        public string Name { get; set; } = null!;

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
