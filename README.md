# 🛍️ CustomerShop - Hệ Thống Quản Lý Cửa Hàng Bán Lẻ

[![.NET Version](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Blazor](https://img.shields.io/badge/Blazor-Server-blue.svg)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-orange.svg)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Hệ thống quản lý cửa hàng bán lẻ toàn diện được xây dựng bằng **Blazor Server** và **Entity Framework Core**, cung cấp trải nghiệm mua sắm trực tuyến mượt mà và hiệu quả cho khách hàng.

## 📋 Mục Lục

- [Tính Năng Chính](#-tính-năng-chính)
- [Công Nghệ Sử Dụng](#-công-nghệ-sử-dụng)
- [Kiến Trúc Hệ Thống](#-kiến-trúc-hệ-thống)
- [Cài Đặt](#-cài-đặt)
- [Cấu Hình](#-cấu-hình)
- [Cơ Sở Dữ Liệu](#-cơ-sở-dữ-liệu)
- [Sử Dụng](#-sử-dụng)
- [Cấu Trúc Dự Án](#-cấu-trúc-dự-án)
- [API và Services](#-api-và-services)
- [Đóng Góp](#-đóng-góp)
- [Giấy Phép](#-giấy-phép)

## ✨ Tính Năng Chính

### 🛒 Dành Cho Khách Hàng

- **Xác Thực & Tài Khoản**
  - Đăng ký tài khoản mới với validation đầy đủ
  - Đăng nhập/Đăng xuất an toàn
  - Quản lý thông tin cá nhân (tên, email, điện thoại, địa chỉ)
  - Lưu trữ phiên đăng nhập với LocalStorage

- **Trải Nghiệm Mua Sắm**
  - Trang chủ hiện đại với hero section và danh mục sản phẩm
  - Duyệt sản phẩm theo danh mục
  - Tìm kiếm sản phẩm thông minh (theo tên, danh mục)
  - Xem chi tiết sản phẩm với thông tin đầy đủ
  - Lọc và sắp xếp sản phẩm (giá, tên, mới nhất)
  - Phân trang sản phẩm

- **Giỏ Hàng & Thanh Toán**
  - Thêm/Xóa/Cập nhật sản phẩm trong giỏ hàng
  - Lưu giỏ hàng vào LocalStorage (persistent cart)
  - Hiển thị số lượng sản phẩm trong giỏ hàng realtime
  - Áp dụng mã khuyến mãi
  - Nhiều phương thức thanh toán:
    - Tiền mặt (COD)
    - Chuyển khoản ngân hàng
    - Ví điện tử
  - Tính toán tổng tiền tự động với giảm giá

- **Quản Lý Đơn Hàng**
  - Xem lịch sử đơn hàng
  - Theo dõi trạng thái đơn hàng (pending, paid, canceled)
  - Xem chi tiết từng đơn hàng
  - Hủy đơn hàng đang chờ
  - Trang xác nhận đơn hàng thành công

- **Thông Báo & UX**
  - Hệ thống toast notifications (success, error, info, warning)
  - Responsive design cho mọi thiết bị
  - Giao diện đẹp mắt, hiện đại với CSS tùy chỉnh
  - Tìm kiếm header với gợi ý
  - Icons giỏ hàng động với badge số lượng

### 🏪 Dành Cho Quản Trị

- **Quản Lý Sản Phẩm**
  - CRUD đầy đủ cho sản phẩm
  - Quản lý tồn kho (inventory)
  - Quản lý danh mục và nhà cung cấp
  - Upload và quản lý hình ảnh sản phẩm

- **Quản Lý Đơn Hàng**
  - Xem tất cả đơn hàng
  - Cập nhật trạng thái đơn hàng
  - Xác nhận thanh toán
  - Quản lý chi tiết đơn hàng

- **Khuyến Mãi**
  - Tạo và quản lý mã giảm giá
  - Giảm giá theo phần trăm hoặc số tiền cố định
  - Đặt điều kiện đơn hàng tối thiểu
  - Giới hạn số lần sử dụng
  - Đặt thời gian hiệu lực

## 🚀 Công Nghệ Sử Dụng

### Backend

- **.NET 9.0** - Framework chính
- **Blazor Server** - UI framework với SignalR
- **Entity Framework Core 9.0** - ORM
- **Pomelo.EntityFrameworkCore.MySql 9.0** - MySQL provider
- **ASP.NET Core Authentication** - Xác thực người dùng

### Frontend

- **Blazor Components** - Component-based architecture
- **Razor Syntax** - Template engine
- **CSS3** - Styling với custom styles
- **JavaScript Interop** - LocalStorage integration
- **Bootstrap Icons** - Icon library

### Database

- **MySQL 8.0** - Relational database
- **AWS RDS** - Cloud database hosting (production)

### DevOps & Tools

- **DotNetEnv** - Environment variables management
- **.env** - Configuration file
- **Git** - Version control

## 🏗️ Kiến Trúc Hệ Thống

```
CustomerShop/
├── Components/              # Blazor components
│   ├── Layout/             # Layout components
│   │   ├── MainLayout.razor
│   │   ├── NavMenu.razor
│   │   ├── HeaderSearch.razor
│   │   ├── HeaderAuth.razor
│   │   ├── CartIcon.razor
│   │   └── ToastContainer.razor
│   ├── Pages/              # Page components
│   │   ├── Home.razor
│   │   ├── Shop.razor
│   │   ├── ProductDetail.razor
│   │   ├── CartPage.razor
│   │   ├── Checkout.razor
│   │   ├── Orders.razor
│   │   ├── Login.razor
│   │   ├── Register.razor
│   │   └── Profile.razor
│   ├── App.razor
│   └── Routes.razor
├── Data/                   # Database context
│   └── ApplicationDbContext.cs
├── Models/                 # Entity models
│   ├── Product.cs
│   ├── Category.cs
│   ├── Customer.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Cart.cs
│   ├── Payment.cs
│   ├── Promotion.cs
│   ├── Inventory.cs
│   └── Supplier.cs
├── Services/               # Business logic
│   ├── ProductService.cs
│   ├── CartService.cs
│   ├── OrderService.cs
│   ├── CustomerAuthService.cs
│   ├── LocalStorageService.cs
│   └── ToastService.cs
├── wwwroot/               # Static files
│   ├── css/              # Stylesheets
│   ├── js/               # JavaScript files
│   └── images/           # Images
└── Program.cs            # Application entry point
```

### Kiến Trúc Layered

1. **Presentation Layer** (Components/Pages)
   - Blazor components với @rendermode InteractiveServer
   - Tương tác người dùng và hiển thị dữ liệu

2. **Business Logic Layer** (Services)
   - ProductService: Quản lý sản phẩm, tìm kiếm, lọc
   - CartService: Quản lý giỏ hàng với persistent storage
   - OrderService: Xử lý đơn hàng và thanh toán
   - CustomerAuthService: Xác thực và quản lý phiên
   - ToastService: Hệ thống thông báo

3. **Data Access Layer** (Data/Models)
   - ApplicationDbContext: EF Core DbContext
   - Entity Models với Data Annotations
   - IDbContextFactory pattern cho Blazor Server

4. **Database Layer**
   - MySQL với schema được định nghĩa rõ ràng
   - Foreign keys và relationships
   - Indexing cho performance

## 📦 Cài Đặt

### Yêu Cầu Hệ Thống

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL 8.0+](https://dev.mysql.com/downloads/mysql/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc [VS Code](https://code.visualstudio.com/)
- Git (tùy chọn)

### Các Bước Cài Đặt

1. **Clone repository**

```bash
git clone https://github.com/your-username/CustomerShop.git
cd CustomerShop
```

2. **Cài đặt dependencies**

```bash
dotnet restore
```

3. **Tạo file .env**

Tạo file `.env` trong thư mục gốc của dự án:

```env
DATABASE_HOST=localhost
DATABASE_PORT=3306
DATABASE_NAME=customershop_db
DATABASE_USER=your_username
DATABASE_PASSWORD=your_password
```

4. **Tạo database từ schema**

```bash
# Import schema.sql vào MySQL
mysql -u your_username -p < schema.sql
```

Hoặc sử dụng MySQL Workbench/phpMyAdmin để import file `schema.sql`.

5. **Chạy ứng dụng**

```bash
dotnet run
```

Ứng dụng sẽ chạy tại `https://localhost:5001` hoặc `http://localhost:5000`.

## ⚙️ Cấu Hình

### Biến Môi Trường (.env)

```env
# Database Configuration
DATABASE_HOST=your_mysql_host
DATABASE_PORT=3306
DATABASE_NAME=your_database_name
DATABASE_USER=your_mysql_user
DATABASE_PASSWORD=your_mysql_password
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Connection String

Connection string được tự động tạo từ biến môi trường:

```csharp
var connectionString = $"Server={host};Port={port};Database={database};User={user};Password={password};";
```

## 🗄️ Cơ Sở Dữ Liệu

### Database Schema

Hệ thống sử dụng 10 bảng chính:

#### 1. **categories** - Danh mục sản phẩm
```sql
- category_id (PK, AUTO_INCREMENT)
- category_name (VARCHAR(100), NOT NULL)
```

#### 2. **suppliers** - Nhà cung cấp
```sql
- supplier_id (PK, AUTO_INCREMENT)
- name (VARCHAR(100), NOT NULL)
- phone (VARCHAR(20))
- email (VARCHAR(100))
- address (TEXT)
```

#### 3. **products** - Sản phẩm
```sql
- product_id (PK, AUTO_INCREMENT)
- category_id (FK -> categories)
- supplier_id (FK -> suppliers)
- product_name (VARCHAR(100), NOT NULL)
- barcode (VARCHAR(50))
- price (DECIMAL(10,2))
- unit (VARCHAR(20))
- image_url (VARCHAR(500))
- created_at (TIMESTAMP)
```

#### 4. **inventory** - Tồn kho
```sql
- inventory_id (PK, AUTO_INCREMENT)
- product_id (FK -> products)
- quantity (INT, DEFAULT 0)
- updated_at (TIMESTAMP)
```

#### 5. **customers** - Khách hàng
```sql
- customer_id (PK, AUTO_INCREMENT)
- name (VARCHAR(100), NOT NULL)
- phone (VARCHAR(20))
- email (VARCHAR(100))
- address (TEXT)
- password (VARCHAR(255))
- created_at (TIMESTAMP)
```

#### 6. **promotions** - Khuyến mãi
```sql
- promo_id (PK, AUTO_INCREMENT)
- promo_code (VARCHAR(50), UNIQUE)
- description (TEXT)
- discount_type (ENUM: 'percentage', 'fixed_amount')
- discount_value (DECIMAL(10,2))
- min_order_amount (DECIMAL(10,2))
- max_uses (INT)
- used_count (INT)
- start_date (DATE)
- end_date (DATE)
- is_active (BOOLEAN)
```

#### 7. **orders** - Đơn hàng
```sql
- order_id (PK, AUTO_INCREMENT)
- customer_id (FK -> customers)
- user_id (FK -> users)
- promo_id (FK -> promotions)
- order_date (TIMESTAMP)
- status (ENUM: 'pending', 'paid', 'canceled')
- total_amount (DECIMAL(10,2))
- discount_amount (DECIMAL(10,2))
- transfer_content (VARCHAR(100))
```

#### 8. **order_items** - Chi tiết đơn hàng
```sql
- order_item_id (PK, AUTO_INCREMENT)
- order_id (FK -> orders)
- product_id (FK -> products)
- quantity (INT)
- price (DECIMAL(10,2))
- subtotal (DECIMAL(10,2))
```

#### 9. **payments** - Thanh toán
```sql
- payment_id (PK, AUTO_INCREMENT)
- order_id (FK -> orders)
- amount (DECIMAL(10,2))
- payment_method (ENUM: 'cash', 'bank_transfer', 'e-wallet')
- payment_status (ENUM: 'pending', 'completed', 'failed')
- payment_date (TIMESTAMP)
```

#### 10. **users** - Người dùng (Admin/Staff)
```sql
- user_id (PK, AUTO_INCREMENT)
- username (VARCHAR(50), UNIQUE)
- password (VARCHAR(255))
- full_name (VARCHAR(100))
- role (VARCHAR(20))
- created_at (TIMESTAMP)
```

### Entity Relationships

```
categories 1---* products
suppliers  1---* products
products   1---1 inventory
customers  1---* orders
users      1---* orders (optional)
promotions 1---* orders (optional)
orders     1---* order_items
orders     1---* payments
products   1---* order_items
```

## 🎯 Sử Dụng

### Dành Cho Khách Hàng

1. **Đăng ký tài khoản mới**
   - Truy cập `/register`
   - Điền thông tin: Tên, Email, Số điện thoại, Mật khẩu, Địa chỉ
   - Hệ thống tự động đăng nhập sau khi đăng ký thành công

2. **Đăng nhập**
   - Truy cập `/login`
   - Nhập Email và Mật khẩu
   - Phiên đăng nhập được lưu trong LocalStorage

3. **Mua sắm**
   - Duyệt sản phẩm tại trang `/shop`
   - Lọc theo danh mục, tìm kiếm theo tên
   - Click vào sản phẩm để xem chi tiết
   - Thêm vào giỏ hàng

4. **Thanh toán**
   - Xem giỏ hàng tại `/cart`
   - Cập nhật số lượng hoặc xóa sản phẩm
   - Click "Thanh toán" để đến trang checkout
   - Nhập thông tin giao hàng
   - Áp dụng mã khuyến mãi (nếu có)
   - Chọn phương thức thanh toán
   - Xác nhận đơn hàng

5. **Quản lý đơn hàng**
   - Xem lịch sử đơn hàng tại `/orders`
   - Xem chi tiết từng đơn hàng
   - Hủy đơn hàng đang chờ

### Dành Cho Admin (Cần phát triển thêm)

Hiện tại admin có thể:
- Quản lý sản phẩm, danh mục, nhà cung cấp qua database
- Xem và cập nhật đơn hàng
- Quản lý khuyến mãi
- Xác nhận thanh toán

## 📂 Cấu Trúc Dự Án Chi Tiết

### Components/Layout

- **MainLayout.razor**: Layout chính của ứng dụng
- **NavMenu.razor**: Menu điều hướng
- **HeaderSearch.razor**: Thanh tìm kiếm header
- **HeaderAuth.razor**: Hiển thị trạng thái đăng nhập/đăng xuất
- **CartIcon.razor**: Icon giỏ hàng với badge số lượng
- **ToastContainer.razor**: Container cho toast notifications

### Components/Pages

- **Home.razor**: Trang chủ với hero section và danh mục
- **Shop.razor**: Trang cửa hàng với lọc, tìm kiếm, phân trang
- **ProductDetail.razor**: Chi tiết sản phẩm
- **CartPage.razor**: Trang giỏ hàng
- **Checkout.razor**: Trang thanh toán
- **Orders.razor**: Lịch sử đơn hàng
- **OrderSuccess.razor**: Xác nhận đơn hàng thành công
- **Login.razor**: Trang đăng nhập
- **Register.razor**: Trang đăng ký
- **Profile.razor**: Trang thông tin cá nhân

### Services

#### IProductService / ProductService
```csharp
Task<List<Product>> GetAllProductsAsync()
Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
Task<List<Product>> SearchProductsAsync(string searchTerm)
Task<Product?> GetProductByIdAsync(int id)
Task<List<Category>> GetAllCategoriesAsync()
Task<List<Product>> GetProductsWithFiltersAsync(...)
Task<int> GetTotalProductsCountAsync(...)
Task<(List<Product>, int, List<Category>)> GetShopDataAsync(...)
```

#### ICartService / CartService
```csharp
Cart GetCart()
void AddToCart(Product product, int quantity = 1)
void UpdateQuantity(int productId, int quantity)
void RemoveFromCart(int productId)
void ClearCart()
int GetCartItemCount()
decimal GetCartTotal()
Task LoadCartFromStorageAsync()
Task SaveCartToStorageAsync()
event Action? OnChange
```

#### IOrderService / OrderService
```csharp
Task<Order> CreateOrderAsync(...)
Task<Order?> GetOrderByIdAsync(int orderId)
Task<List<Order>> GetCustomerOrdersAsync(int customerId)
Task<Promotion?> ValidatePromoCodeAsync(string promoCode, decimal orderAmount)
Task<decimal> CalculateDiscountAsync(Promotion promotion, decimal orderAmount)
Task<bool> CancelOrderAsync(int orderId)
```

#### ICustomerAuthService / CustomerAuthService
```csharp
Task<Customer?> GetCurrentCustomerAsync()
bool IsAuthenticated()
Task<Customer?> LoginAsync(string email, string password)
Task<(bool Success, string Message)> RegisterAsync(...)
Task LogoutAsync()
Task<Customer?> UpdateProfileAsync(...)
Task LoadAuthStateFromStorageAsync()
event Action? OnAuthStateChanged
```

#### ILocalStorageService / LocalStorageService
```csharp
Task<T?> GetItemAsync<T>(string key)
Task SetItemAsync<T>(string key, T value)
Task RemoveItemAsync(string key)
```

#### IToastService / ToastService
```csharp
void ShowSuccess(string message)
void ShowError(string message)
void ShowInfo(string message)
void ShowWarning(string message)
event Action<ToastMessage>? OnShow
```

## 🎨 Styling

Dự án sử dụng custom CSS cho từng trang:

- `app.css`: Styles global
- `home.css`: Trang chủ với hero section, categories grid
- `shop.css`: Trang cửa hàng với filters, product grid
- `product-detail.css`: Chi tiết sản phẩm
- `cart.css`: Giỏ hàng
- `checkout.css`: Trang thanh toán
- `orders.css`: Lịch sử đơn hàng
- `profile.css`: Trang profile
- `auth.css`: Trang đăng nhập/đăng ký
- `toast.css`: Toast notifications

### Design Principles

- **Responsive Design**: Mobile-first approach
- **Color Scheme**: Modern với màu chủ đạo
- **Typography**: Rõ ràng, dễ đọc
- **Spacing**: Consistent spacing system
- **Animations**: Subtle transitions và hover effects

## 🔐 Bảo Mật

### Authentication

- Password hashing (cần implement BCrypt hoặc similar)
- Session management với LocalStorage
- Auto logout khi token expires

### Data Validation

- Client-side validation với Data Annotations
- Server-side validation trong services
- SQL Injection protection với EF Core parameterized queries
- XSS protection với Blazor auto-encoding

### Best Practices

- Environment variables cho sensitive data
- HTTPS enforced in production
- CORS configuration
- Input sanitization

## 🧪 Testing

### Unit Tests (Recommended)

```bash
dotnet test
```

Nên test:
- Services logic
- Model validation
- Cart calculations
- Promotion discounts

### Integration Tests

- Database operations
- API endpoints
- Authentication flow

## 🚢 Deployment

### Production Checklist

- [ ] Update connection string cho production database
- [ ] Enable HTTPS
- [ ] Set environment to Production
- [ ] Configure logging
- [ ] Setup backup strategy
- [ ] Monitor performance
- [ ] Security audit

### Deploy to IIS

1. Publish project:
```bash
dotnet publish -c Release
```

2. Copy published files to IIS wwwroot
3. Configure application pool (.NET CLR Version: No Managed Code)
4. Set environment variables
5. Start application

### Deploy to Azure/AWS

- Azure App Service với Azure Database for MySQL
- AWS Elastic Beanstalk với RDS MySQL
- Docker containerization

## 🤝 Đóng Góp

Contributions, issues và feature requests được hoan nghênh!

### Quy Trình Đóng Góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

### Coding Standards

- Tuân thủ C# coding conventions
- Comment code rõ ràng bằng tiếng Việt
- Write unit tests cho features mới
- Update documentation

## 📝 TODO & Roadmap

### Version 2.0 (Planned)

- [ ] Admin dashboard
- [ ] Product reviews và ratings
- [ ] Wishlist functionality
- [ ] Advanced search với filters
- [ ] Email notifications
- [ ] SMS OTP verification
- [ ] Payment gateway integration (VNPay, MoMo)
- [ ] Order tracking
- [ ] Inventory alerts
- [ ] Sales reports và analytics
- [ ] Multi-language support
- [ ] Dark mode

### Version 1.5 (In Progress)

- [x] Customer authentication
- [x] Shopping cart với persistent storage
- [x] Order management
- [x] Promotion system
- [ ] Password hashing
- [ ] Email verification
- [ ] Forgot password
- [ ] Better error handling

## 📞 Liên Hệ

**Developer**: Thịnh Hi

**Email**: zaikaman123@gmail.com

**Repository**: [CustomerShop](https://github.com/zaikaman/CustomerShop)

## 📜 Giấy Phép

Dự án này được phân phối dưới giấy phép MIT. Xem file `LICENSE` để biết thêm chi tiết.

---

## 🙏 Acknowledgments

- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [Bootstrap Icons](https://icons.getbootstrap.com/)

---

<div align="center">

**⭐ Nếu dự án này hữu ích, hãy cho một ngôi sao! ⭐**

Made with ❤️ by Thịnh Hi

</div>
