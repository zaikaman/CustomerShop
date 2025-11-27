using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có từ 3 đến 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string? FullName { get; set; }

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "staff";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
