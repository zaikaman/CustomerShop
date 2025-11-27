using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int? CustomerId { get; set; }

        public int? UserId { get; set; }

        public int? PromoId { get; set; }

        [Required(ErrorMessage = "Ngày đặt hàng là bắt buộc")]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Trạng thái đơn hàng là bắt buộc")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "pending"; // pending, paid, canceled

        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Range(0, 999999999.99, ErrorMessage = "Tổng tiền phải từ 0")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Range(0, 999999999.99)]
        [Display(Name = "Giảm giá")]
        public decimal DiscountAmount { get; set; } = 0;

        // Navigation properties
        public Customer? Customer { get; set; }
        public User? User { get; set; }
        public Promotion? Promotion { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // Tổng tiền sau khi giảm giá
        public decimal FinalAmount => TotalAmount - DiscountAmount;
    }
}
