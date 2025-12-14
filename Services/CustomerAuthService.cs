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
        Task LoadAuthStateFromStorageAsync();
        Customer? CurrentCustomer { get; }
        bool IsAuthenticated { get; }
        bool IsLoading { get; }
        Task LogoutAsync();
        event Action? OnAuthStateChanged;
    }

    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILocalStorageService _localStorage;
        private Customer? _currentCustomer;
        private const string AUTH_STORAGE_KEY = "customer_auth";
        private bool _isLoaded = false;
        private bool _isLoading = false;

        public event Action? OnAuthStateChanged;

        public CustomerAuthService(IDbContextFactory<ApplicationDbContext> contextFactory, ILocalStorageService localStorage)
        {
            _contextFactory = contextFactory;
            _localStorage = localStorage;
        }

        public Customer? CurrentCustomer => _currentCustomer;
        public bool IsAuthenticated => _currentCustomer != null;
        public bool IsLoading => _isLoading;

        /// <summary>
        /// Tải trạng thái đăng nhập từ LocalStorage
        /// </summary>
        public async Task LoadAuthStateFromStorageAsync()
        {
            if (_isLoaded) return;
            
            _isLoading = true;
            NotifyAuthStateChanged();
            
            try
            {
                var customerId = await _localStorage.GetItemAsync<int?>(AUTH_STORAGE_KEY);
                if (customerId.HasValue && customerId.Value > 0)
                {
                    await using var context = await _contextFactory.CreateDbContextAsync();
                    var customer = await context.Customers.FindAsync(customerId.Value);
                    if (customer != null)
                    {
                        _currentCustomer = customer;
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi
            }
            finally
            {
                _isLoaded = true;
                _isLoading = false;
                NotifyAuthStateChanged();
            }
        }

        /// <summary>
        /// Lưu trạng thái đăng nhập vào LocalStorage
        /// </summary>
        private async Task SaveAuthStateAsync()
        {
            try
            {
                if (_currentCustomer != null)
                {
                    await _localStorage.SetItemAsync(AUTH_STORAGE_KEY, _currentCustomer.CustomerId);
                }
                else
                {
                    await _localStorage.RemoveItemAsync(AUTH_STORAGE_KEY);
                }
            }
            catch
            {
                // Bỏ qua lỗi
            }
        }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
            
            if (customer != null)
            {
                _currentCustomer = customer;
                await SaveAuthStateAsync();
                NotifyAuthStateChanged();
            }
            
            return customer;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string name, string email, string phone, string password, string? address)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            // Kiểm tra email đã tồn tại
            var existingCustomer = await context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
            
            if (existingCustomer != null)
            {
                return (false, "Email đã được sử dụng. Vui lòng chọn email khác.");
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (!string.IsNullOrEmpty(phone))
            {
                var existingPhone = await context.Customers
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

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Tự động đăng nhập sau khi đăng ký
            _currentCustomer = customer;
            await SaveAuthStateAsync();
            NotifyAuthStateChanged();

            return (true, "Đăng ký thành công!");
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                context.Customers.Update(customer);
                await context.SaveChangesAsync();
                
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

        public async Task LogoutAsync()
        {
            _currentCustomer = null;
            await _localStorage.RemoveItemAsync(AUTH_STORAGE_KEY);
            NotifyAuthStateChanged();
        }

        public async Task<bool> UpdateProfileAsync(string name, string? phone, string? address)
        {
            if (_currentCustomer == null) return false;

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                
                // Lấy customer từ database
                var customer = await context.Customers.FindAsync(_currentCustomer.CustomerId);
                if (customer == null) return false;
                
                customer.Name = name;
                customer.Phone = phone;
                customer.Address = address;

                context.Customers.Update(customer);
                await context.SaveChangesAsync();
                
                // Cập nhật local state
                _currentCustomer.Name = name;
                _currentCustomer.Phone = phone;
                _currentCustomer.Address = address;
                
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

                await using var context = await _contextFactory.CreateDbContextAsync();
                
                var customer = await context.Customers.FindAsync(_currentCustomer.CustomerId);
                if (customer == null) return false;
                
                customer.Password = newPassword;
                context.Customers.Update(customer);
                await context.SaveChangesAsync();
                
                _currentCustomer.Password = newPassword;
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
