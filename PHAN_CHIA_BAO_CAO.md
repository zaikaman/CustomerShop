# PHÂN CHIA CÔNG VIỆC HỌC VÀ BÁO CÁO - ĐỀ TÀI QUẢN LÝ CỬA HÀNG BÁN LẺ

## 🎯 TỔNG QUAN DỰ ÁN
**Tên dự án:** CustomerShop - Hệ thống quản lý cửa hàng bán lẻ  
**Công nghệ:** ASP.NET Core Blazor Server, Entity Framework Core, MySQL  
**Tổng số thành viên:** 5 người  
**Phân chia:** Mỗi người 20%

---

## 👤 NGƯỜI 1 (20%) - PHẦN CƠ SỞ DỮ LIỆU & MODELS

### 📚 Nội dung cần học và trình bày:

#### 1. Database Schema & Entity Framework Core
**File cần nắm:** `schema.sql`, `Data/ApplicationDbContext.cs`

**Kiến thức cần trình bày:**
- Cấu trúc cơ sở dữ liệu MySQL (11 bảng)
- Entity Framework Core và Code First Approach
- Cách cấu hình DbContext với MySQL
- Connection pooling trong Blazor Server

**Các bảng cần giải thích:**
- `users` - Quản lý người dùng hệ thống
- `customers` - Quản lý khách hàng
- `categories` - Danh mục sản phẩm
- `suppliers` - Nhà cung cấp

#### 2. Models (4 models chính)
**File cần học:**
- `Models/User.cs` - Model người dùng
- `Models/Customer.cs` - Model khách hàng  
- `Models/Category.cs` - Model danh mục
- `Models/Supplier.cs` - Model nhà cung cấp

**Kiến thức cần trình bày:**
- Data Annotations và validation
- Navigation properties trong EF Core
- Relationship mapping (One-to-Many)
- DateTime handling và default values

#### 3. ApplicationDbContext Configuration
**Nội dung trình bày:**
```csharp
// Cấu hình kết nối database
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```
- Giải thích `PooledDbContextFactory` 
- Tại sao dùng cho Blazor Server
- Cách load biến môi trường từ `.env`

### 📝 Câu hỏi demo có thể gặp:
1. Giải thích cấu trúc bảng `customers` và các trường quan trọng?
2. Relationship giữa `Category` và `Product` là gì?
3. Tại sao dùng `PooledDbContextFactory` thay vì `AddDbContext` thường?
4. Giải thích cách EF Core map C# properties sang MySQL columns?

---

## 👤 NGƯỜI 2 (20%) - PHẦN QUẢN LÝ SẢN PHẨM & INVENTORY

### 📚 Nội dung cần học và trình bày:

#### 1. Product Management Models
**File cần học:**
- `Models/Product.cs` - Model sản phẩm
- `Models/Inventory.cs` - Model tồn kho
- `Models/Promotion.cs` - Model khuyến mãi

**Kiến thức cần trình bày:**
- Cấu trúc Product với Category và Supplier
- Quản lý tồn kho (Inventory management)
- Hệ thống khuyến mãi (Promotion system)
- ImageUrl và xử lý hình ảnh

#### 2. ProductService
**File cần học:** `Services/ProductService.cs`

**Các method quan trọng cần giải thích:**
```csharp
- GetAllProductsAsync() - Lấy tất cả sản phẩm
- GetProductsByCategoryAsync(categoryId) - Lọc theo danh mục
- SearchProductsAsync(searchTerm) - Tìm kiếm sản phẩm
- GetProductsWithFiltersAsync() - Lọc và phân trang
- GetShopDataAsync() - Load dữ liệu cho trang Shop
```

**Kiến thức cần trình bày:**
- Async/Await pattern trong C#
- LINQ queries với Include() cho navigation properties
- Pagination và sorting
- Search functionality với Contains()

#### 3. Shop Page
**File cần học:** `Components/Pages/Shop.razor`

**Nội dung trình bày:**
- Blazor component lifecycle (`OnInitializedAsync`)
- Data binding với `@bind`
- Event handling (`@onclick`)
- Conditional rendering (`@if`)
- Loop rendering (`@foreach`)

### 📝 Câu hỏi demo có thể gặp:
1. Giải thích flow từ user click filter đến hiển thị sản phẩm?
2. Làm thế nào để implement tìm kiếm sản phẩm theo tên?
3. Promotion system hoạt động như thế nào?
4. Giải thích pagination logic trong `GetProductsWithFiltersAsync`?

---

## 👤 NGƯỜI 3 (20%) - PHẦN AUTHENTICATION & AUTHORIZATION

### 📚 Nội dung cần học và trình bày:

#### 1. Customer Authentication Service
**File cần học:** `Services/CustomerAuthService.cs`

**Các method quan trọng:**
```csharp
- LoginAsync(email, password) - Đăng nhập
- RegisterAsync() - Đăng ký tài khoản
- LogoutAsync() - Đăng xuất
- UpdateProfileAsync() - Cập nhật thông tin
- ChangePasswordAsync() - Đổi mật khẩu
- LoadAuthStateFromStorageAsync() - Load trạng thái từ localStorage
```

**Kiến thức cần trình bày:**
- Password hashing với BCrypt
- Session management trong Blazor
- LocalStorage cho persistent login
- Event-driven architecture (`OnAuthStateChanged`)

#### 2. LocalStorage Service
**File cần học:** `Services/LocalStorageService.cs`

**Nội dung trình bày:**
- JSInterop trong Blazor
- Serialize/Deserialize JSON với System.Text.Json
- Browser storage management
- Try-catch error handling

#### 3. Authentication Pages
**File cần học:**
- `Components/Pages/Login.razor` - Trang đăng nhập
- `Components/Pages/Register.razor` - Trang đăng ký
- `Components/Pages/Profile.razor` - Trang thông tin cá nhân

**Kiến thức cần trình bày:**
- EditForm và validation trong Blazor
- DataAnnotations validation
- Form submission handling
- Navigation và ReturnUrl parameter
- Conditional rendering based on auth state

#### 4. Header Authentication Component
**File cần học:** `Components/Layout/HeaderAuth.razor`

**Nội dung trình bày:**
- Component composition
- User dropdown menu
- Reactive UI updates với `StateHasChanged()`
- Event subscription và cleanup

### 📝 Câu hỏi demo có thể gặp:
1. Giải thích flow đăng nhập từ UI đến Database?
2. Tại sao cần hash password? BCrypt hoạt động như thế nào?
3. LocalStorage khác SessionStorage ở điểm nào?
4. Làm thế nào để maintain login state sau khi refresh page?

---

## 👤 NGƯỜI 4 (20%) - PHẦN GIỎ HÀNG & THANH TOÁN

### 📚 Nội dung cần học và trình bày:

#### 1. Cart Management
**File cần học:**
- `Models/Cart.cs` - Model giỏ hàng
- `Services/CartService.cs` - Service quản lý giỏ hàng

**Các method quan trọng:**
```csharp
- AddToCart(product, quantity) - Thêm sản phẩm
- UpdateQuantity(productId, quantity) - Cập nhật số lượng
- RemoveFromCart(productId) - Xóa sản phẩm
- ClearCart() - Xóa toàn bộ giỏ hàng
- GetCartItemCount() - Đếm số sản phẩm
- LoadCartFromStorageAsync() - Load từ localStorage
- SaveCartToStorageAsync() - Lưu vào localStorage
```

**Kiến thức cần trình bày:**
- Cart state management
- Real-time cart updates với Events
- Cart persistence với LocalStorage
- Cart calculations (subtotal, total)

#### 2. Cart Page
**File cần học:** `Components/Pages/CartPage.razor`

**Nội dung trình bày:**
- Display cart items với formatting
- Quantity controls (tăng/giảm)
- Remove item functionality
- Cart summary calculations
- Empty cart state handling

#### 3. Checkout Process
**File cần học:** `Components/Pages/Checkout.razor`

**Nội dung trình bày:**
- Multi-step checkout flow
- Payment method selection (Cash, Bank Transfer, E-wallet)
- Promotion code application
- Order validation
- QR code payment display
- Transfer content generation

#### 4. Cart Icon Component
**File cần học:** `Components/Layout/CartIcon.razor`

**Nội dung trình bày:**
- Real-time cart badge update
- Event subscription (`CartService.OnChange`)
- Component lifecycle và Dispose pattern

### 📝 Câu hỏi demo có thể gặp:
1. Giải thích flow thêm sản phẩm vào giỏ hàng?
2. Làm thế nào cart icon update real-time khi thêm sản phẩm?
3. Promotion code được validate như thế nào?
4. Giải thích 3 phương thức thanh toán trong hệ thống?

---

## 👤 NGƯỜI 5 (20%) - PHẦN ĐƠN HÀNG & UI/UX

### 📚 Nội dung cần học và trình bày:

#### 1. Order Management
**File cần học:**
- `Models/Order.cs` - Model đơn hàng
- `Models/OrderItem.cs` - Model chi tiết đơn hàng
- `Models/Payment.cs` - Model thanh toán
- `Services/OrderService.cs` - Service quản lý đơn hàng

**Các method quan trọng:**
```csharp
- CreateOrderAsync() - Tạo đơn hàng mới
- GetOrderByIdAsync() - Lấy chi tiết đơn hàng
- GetCustomerOrdersAsync() - Lấy danh sách đơn hàng
- ValidatePromoCodeAsync() - Validate mã giảm giá
- CalculateDiscountAsync() - Tính toán giảm giá
- CancelOrderAsync() - Hủy đơn hàng
```

**Kiến thức cần trình bày:**
- Order lifecycle (pending → paid → canceled)
- Transaction handling với EF Core
- Order items và quantity validation
- Discount calculation logic
- Payment record creation

#### 2. Orders Page
**File cần học:** `Components/Pages/Orders.razor`

**Nội dung trình bày:**
- Display order history
- Order status badges
- Order details expansion
- Order item listing
- Payment information display

#### 3. Order Success & Toast Notifications
**File cần học:**
- `Components/Pages/OrderSuccess.razor`
- `Services/ToastService.cs`
- `Components/Layout/ToastContainer.razor`

**Nội dung trình bày:**
- Success page design
- Toast notification system
- Event-driven notifications
- Auto-dismiss functionality
- Different toast types (Success, Error, Warning, Info)

#### 4. UI/UX Components & Styling
**File cần học:**
- `Components/Layout/MainLayout.razor`
- `Components/Layout/NavMenu.razor`
- `Components/Pages/Home.razor`
- CSS files: `wwwroot/css/*.css`

**Nội dung trình bày:**
- Layout structure trong Blazor
- Navigation menu và routing
- Hero section design
- Responsive design principles
- CSS organization
- Icons system (HGI Icons)

### 📝 Câu hỏi demo có thể gặp:
1. Giải thích flow tạo đơn hàng từ checkout đến database?
2. Order status có những trạng thái nào? Chuyển đổi như thế nào?
3. Toast notification system hoạt động như thế nào?
4. Giải thích routing và navigation trong Blazor?

---

## 📊 BẢNG PHÂN CÔNG CHI TIẾT

| Người | Phần trách nhiệm | Files chính | Kiến thức core | % công việc |
|-------|------------------|-------------|----------------|-------------|
| **1** | Database & Models cơ bản | schema.sql, ApplicationDbContext.cs, User.cs, Customer.cs, Category.cs, Supplier.cs | EF Core, Database Design, Models Mapping | 20% |
| **2** | Product & Inventory | Product.cs, Inventory.cs, Promotion.cs, ProductService.cs, Shop.razor | LINQ, Async/Await, Product Management | 20% |
| **3** | Authentication | CustomerAuthService.cs, LocalStorageService.cs, Login.razor, Register.razor, Profile.razor | Security, BCrypt, JSInterop, Session | 20% |
| **4** | Cart & Checkout | Cart.cs, CartService.cs, CartPage.razor, Checkout.razor, CartIcon.razor | State Management, Events, Cart Logic | 20% |
| **5** | Orders & UI/UX | Order.cs, OrderItem.cs, Payment.cs, OrderService.cs, Orders.razor, ToastService.cs, MainLayout.razor, Home.razor | Transaction, UI Components, Styling | 20% |

---

## 🎓 HƯỚNG DẪN HỌC VÀ CHUẨN BỊ BÁO CÁO

### Bước 1: Đọc và hiểu code của phần mình (1-2 ngày)
- Đọc kỹ các file được phân công
- Chạy thử nghiệm các chức năng liên quan
- Debug để hiểu flow xử lý
- Note lại các điểm quan trọng

### Bước 2: Tìm hiểu kiến thức nền tảng (1-2 ngày)
- Học các concept liên quan (EF Core, Blazor, Authentication, etc.)
- Xem documentation chính thức
- Tìm hiểu best practices

### Bước 3: Chuẩn bị slide/tài liệu (1 ngày)
- Tạo slide PowerPoint hoặc tài liệu Word
- Include code snippets quan trọng
- Vẽ diagram/flowchart nếu cần
- Chuẩn bị demo trực tiếp

### Bước 4: Luyện tập trình bày (0.5 ngày)
- Luyện nói trong 8-10 phút (vì mỗi người 20%)
- Chuẩn bị trả lời câu hỏi
- Test demo để đảm bảo không lỗi

---

## 💡 MẸO TRÌNH BÀY HIỆU QUẢ

### 1. Cấu trúc trình bày (8-10 phút/người)
```
1. Giới thiệu phần của mình (30s)
2. Giải thích kiến thức nền tảng (2 phút)
3. Trình bày code quan trọng (3 phút)
4. Demo thực tế (2-3 phút)
5. Tổng kết và Q&A (1-2 phút)
```

### 2. Khi trình bày code
- Không đọc code từng dòng
- Giải thích ý tưởng chính và flow
- Highlight những dòng code quan trọng
- Giải thích tại sao code như vậy

### 3. Demo trực tiếp
- Mở Visual Studio hoặc VS Code
- Chạy ứng dụng và demo chức năng
- Set breakpoint và debug để show flow
- Show database để thấy thay đổi

---

## 🔗 CÁC FILE QUAN TRỌNG CHUNG

**Mọi người nên đọc qua:**
1. `Program.cs` - Entry point và dependency injection
2. `Components/App.razor` - Root component
3. `Components/Routes.razor` - Routing configuration
4. `Components/_Imports.razor` - Global using statements
5. `appsettings.json` - Configuration

---

## 📞 LIÊN HỆ VÀ HỖ TRỢ

**Nếu có thắc mắc về phần của nhau:**
- Trao đổi trong nhóm để hiểu toàn bộ hệ thống
- Mỗi người nên biết cơ bản về phần của người khác
- Chuẩn bị câu hỏi có thể hỏi chéo

**Trước ngày báo cáo 2-3 ngày:**
- Họp nhóm online/offline
- Review chéo phần của nhau
- Tổng duyệt 1 lần hoàn chỉnh

---

## ✅ CHECKLIST TRƯỚC KHI BÁO CÁO

**Mỗi người cần:**
- [ ] Đã đọc và hiểu hết code của phần mình
- [ ] Chuẩn bị slide/tài liệu trình bày
- [ ] Test demo chạy OK không lỗi
- [ ] Luyện tập trình bày ít nhất 2 lần
- [ ] Chuẩn bị câu trả lời cho câu hỏi thường gặp
- [ ] Hiểu cơ bản về phần của người khác trong nhóm
- [ ] Backup code và database test

**Cả nhóm cần:**
- [ ] Tổng duyệt 1 lần hoàn chỉnh
- [ ] Đảm bảo app chạy OK trên máy sẽ demo
- [ ] Chuẩn bị backup plan nếu demo lỗi
- [ ] Phân chia thời gian trình bày rõ ràng

---

## 🎯 KẾT LUẬN

Phân chia này đảm bảo:
- ✅ Mỗi người 20% công việc tương đương
- ✅ Các phần có liên hệ logic với nhau
- ✅ Đủ kiến thức để trả lời câu hỏi
- ✅ Có thể demo trực tiếp phần của mình
- ✅ Cover toàn bộ source code của project

**Chúc các bạn báo cáo thành công! 🚀**
