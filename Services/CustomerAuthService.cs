using Microsoft.EntityFrameworkCore;
using CustomerShop.Data;
using CustomerShop.Models;

namespace CustomerShop.Services
{
    public interface ICustomerAuthService
    {
        Task<Customer?> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> RegisterAsync(string name, string email, string phone, string password, string? address);
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> UpdateProfileAsync(string name, string? phone, string? address);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Customer? CurrentCustomer { get; }
        bool IsAuthenticated { get; }
        void Logout();
        event Action? OnAuthStateChanged;
    }

    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly ApplicationDbContext _context;
        private Customer? _currentCustomer;

        public event Action? OnAuthStateChanged;

        public CustomerAuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Customer? CurrentCustomer => _currentCustomer;
        public bool IsAuthenticated => _currentCustomer != null;

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
            
            if (customer != null)
            {
                _currentCustomer = customer;
                NotifyAuthStateChanged();
            }
            
            return customer;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string name, string email, string phone, string password, string? address)
        {
            // Kiểm tra email đã tồn tại
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
            
            if (existingCustomer != null)
            {
                return (false, "Email đã được sử dụng. Vui lòng chọn email khác.");
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (!string.IsNullOrEmpty(phone))
            {
                var existingPhone = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Phone == phone);
                if (existingPhone != null)
                {
                    return (false, "Số điện thoại đã được sử dụng. Vui lòng chọn số khác.");
                }
            }

            var customer = new Customer
            {
                Name = name,
                Email = email,
                Phone = phone,
                Password = password, // Trong thực tế nên hash password
                Address = address,
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập sau khi đăng ký
            _currentCustomer = customer;
            NotifyAuthStateChanged();

            return (true, "Đăng ký thành công!");
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                
                if (_currentCustomer?.CustomerId == customer.CustomerId)
                {
                    _currentCustomer = customer;
                    NotifyAuthStateChanged();
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            _currentCustomer = null;
            NotifyAuthStateChanged();
        }

        public async Task<bool> UpdateProfileAsync(string name, string? phone, string? address)
        {
            if (_currentCustomer == null) return false;

            try
            {
                _currentCustomer.Name = name;
                _currentCustomer.Phone = phone;
                _currentCustomer.Address = address;

                _context.Customers.Update(_currentCustomer);
                await _context.SaveChangesAsync();
                NotifyAuthStateChanged();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (_currentCustomer == null) return false;

            try
            {
                // Kiểm tra mật khẩu hiện tại
                if (_currentCustomer.Password != currentPassword)
                {
                    return false;
                }

                _currentCustomer.Password = newPassword;
                _context.Customers.Update(_currentCustomer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void NotifyAuthStateChanged() => OnAuthStateChanged?.Invoke();
    }
}
