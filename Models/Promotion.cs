using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class Promotion
    {
        public int PromoId { get; set; }

        [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Mã khuyến mãi phải có từ 3 đến 50 ký tự")]
        [Display(Name = "Mã khuyến mãi")]
        public string PromoCode { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [Display(Name = "Loại giảm giá")]
        public string DiscountType { get; set; } = null!; // percent or fixed

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0.01, 100000000, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        [Display(Name = "Giá trị giảm giá")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Range(0, 999999999, ErrorMessage = "Giá trị đơn hàng tối thiểu phải từ 0")]
        [Display(Name = "Giá trị đơn hàng tối thiểu")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Range(0, int.MaxValue)]
        [Display(Name = "Giới hạn sử dụng")]
        public int UsageLimit { get; set; } = 0;

        [Range(0, int.MaxValue)]
        [Display(Name = "Đã sử dụng")]
        public int UsedCount { get; set; } = 0;

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "active";

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        // Helper method để kiểm tra khuyến mãi còn hiệu lực
        public bool IsValid()
        {
            var now = DateTime.Now;
            return Status == "active" 
                && now >= StartDate 
                && now <= EndDate 
                && (UsageLimit == 0 || UsedCount < UsageLimit);
        }
    }
}
