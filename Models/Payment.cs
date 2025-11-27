using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0, 999999999.99)]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "cash"; // cash, card, bank_transfer, e-wallet

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } = "pending"; // pending, completed, failed, refunded

        [Display(Name = "Ngày thanh toán")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Navigation property
        public Order? Order { get; set; }
    }
}
