using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên khách hàng phải có từ 2 đến 100 ký tự")]
        [Display(Name = "Tên khách hàng")]
        public string Name { get; set; } = null!;

        [StringLength(20, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có từ 10 đến 20 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        // Thêm field password cho đăng nhập khách hàng
        [StringLength(255)]
        public string? Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
