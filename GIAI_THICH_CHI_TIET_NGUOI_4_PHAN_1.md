# 📚 GIẢI THÍCH CHI TIẾT PHẦN NGƯỜI 4 - CART & CHECKOUT
## Phần 1: Lý Thuyết + Code Analysis (Dành cho 2 ngày học)

---

## 📌 MỤC LỤC
1. [Kiến thức nền tảng C#](#1-kiến-thức-nền-tảng-c)
2. [Kiến thức nền tảng Blazor](#2-kiến-thức-nền-tảng-blazor)
3. [Phân tích Models/Cart.cs](#3-phân-tích-modelscartcs)
4. [Phân tích Services/CartService.cs](#4-phân-tích-servicescartservicecs)
5. [Event-Driven Architecture](#5-event-driven-architecture)
6. [LocalStorage Integration](#6-localstorage-integration)

---

## 1. KIẾN THỨC NỀN TẢNG C#

### 1.1 Syntax Cơ Bản Cần Biết

```csharp
// ===== KHAI BÁO BIẾN =====
int quantity = 5;                      // Số nguyên
decimal price = 99.99m;                // Số thập phân (m = đặt chỉ định decimal)
string productName = "iPhone 15";      // Chuỗi
bool isAvailable = true;               // Boolean (true/false)
int? nullableInt = null;               // ? cho phép giá trị null
string? nullableString = null;         // String có thể null

// ===== LIST - DANH SÁCH ĐỘNG =====
List<int> numbers = new List<int>();
numbers.Add(1);                        // Thêm item
numbers.Add(2);
numbers.Add(3);
int count = numbers.Count;             // Đếm = 3
int firstNum = numbers[0];             // Lấy phần tử đầu tiên = 1
numbers.Remove(2);                     // Xóa giá trị 2

// ===== NULLABLE REFERENCE TYPES =====
// "= null!" là khai báo: cái này có thể null nhưng tôi chắc chắn nó không null lúc dùng
public string ProductName { get; set; } = null!;

// ===== PROPERTIES - THUỘC TÍNH (GET/SET) =====
public int ProductId { get; set; }     // Cả get và set đều public
public string Name { get; private set; } // Chỉ get public, set private
public decimal Price { get; }          // Chỉ get (read-only)
```

### 1.2 LINQ - Language Integrated Query

LINQ là cách truy vấn dữ liệu trong .NET. Rất quan trọng cho Cart!

```csharp
List<CartItem> cartItems = new List<CartItem>
{
    new CartItem { ProductId = 1, ProductName = "iPhone", Price = 999 },
    new CartItem { ProductId = 2, ProductName = "iPad", Price = 599 },
    new CartItem { ProductId = 3, ProductName = "AirPods", Price = 199 }
};

// ===== WHERE - LỌC DỮ LIỆU =====
// Tìm item có ProductId = 1
var item = cartItems.Where(i => i.ProductId == 1).FirstOrDefault();
// Hoặc ngắn gọn hơn:
var item = cartItems.FirstOrDefault(i => i.ProductId == 1);
// Giải thích: i => i.ProductId == 1 
// - i là từng phần tử trong list
// - i => ... là lambda expression (hàm ẩn danh)
// - FirstOrDefault() lấy phần tử đầu tiên, hoặc null nếu không có

// ===== SELECT - BIẾN ĐỔI DỮ LIỆU =====
// Lấy tất cả ProductName
var names = cartItems.Select(i => i.ProductName).ToList();
// Kết quả: ["iPhone", "iPad", "AirPods"]

// ===== SUM - CỘNG DỮ LIỆU =====
// Tính tổng tiền
decimal total = cartItems.Sum(i => i.Price * i.Quantity);
// Hoặc: cartItems.Sum(i => i.Subtotal);

// ===== ANY & All - KIỂM TRA =====
bool hasItems = cartItems.Any();                    // Có phần tử nào không?
bool allExpensive = cartItems.All(i => i.Price > 500); // Tất cả > 500?
bool hasMobile = cartItems.Any(i => i.ProductName.Contains("iPhone")); // Có iPhone?

// ===== ORDER BY - SẮP XẾP =====
var sorted = cartItems.OrderBy(i => i.Price).ToList();      // Giá tăng dần
var descending = cartItems.OrderByDescending(i => i.Price).ToList(); // Giá giảm dần

// ===== COUNT - ĐẾM =====
int totalItems = cartItems.Sum(i => i.Quantity);   // Đếm tổng quantity
int productCount = cartItems.Count();               // Đếm số sản phẩm
```

### 1.3 Null Coalescing & Null Conditional

```csharp
// ===== NULLABLE COALESCING (??) =====
// Nếu giá trị null, dùng giá trị thay thế
string address = customer.Address ?? "Chưa cập nhật";
// Nếu customer.Address == null, thì address = "Chưa cập nhật"

// ===== NULL CONDITIONAL (?.) =====
// Nếu object null, không gọi method/property, trả về null
string? categoryName = product.Category?.CategoryName;
// Nếu product.Category == null, categoryName = null (không crash)
// Nếu product.Category != null, categoryName = product.Category.CategoryName

// ===== RESULT PATTERN (?.Count) =====
int itemCount = cartItems?.Count ?? 0;
// Nếu cartItems null, itemCount = 0
// Nếu cartItems không null, itemCount = cartItems.Count
```

### 1.4 Async/Await - Xử Lý Bất Đồng Bộ

Async/await dùng để không block UI khi làm việc chậm (IO, network, etc.)

```csharp
// ===== TASK & TASK<T> =====
// Task = không trả về gì (void async)
// Task<T> = trả về kiểu T

// Hàm async trả về Task (không giá trị)
public async Task SaveDataAsync()
{
    // Làm việc chậm (lưu file, call API, etc.)
    await File.WriteAllTextAsync("data.txt", "content");
    // UI không bị block
}

// Hàm async trả về Task<T> (có giá trị)
public async Task<List<CartItem>> GetCartItemsAsync()
{
    // Lấy dữ liệu từ storage
    var items = await localStorage.GetItemAsync<List<CartItem>>("cart");
    return items;
}

// ===== GỌI HÀM ASYNC =====
// Cách 1: Dùng await (chờ kết quả)
List<CartItem> items = await GetCartItemsAsync();
Console.WriteLine(items.Count); // In kết quả sau khi lấy được

// Cách 2: Fire and forget (không chờ)
_ = SaveDataAsync(); // _ = bỏ qua kết quả

// ===== ASYNC VOID (KHÔNG DÙNG) =====
// public async void BadExample() { } // ❌ TRÁNH - khó debug
// Chỉ dùng async void cho event handlers
public async void OnButtonClick()  // ✓ OK cho events
{
    await SomeAsyncMethod();
}

// ===== TRY-CATCH VỚI ASYNC =====
try
{
    var items = await GetCartItemsAsync();
}
catch (Exception ex)
{
    // Xử lý lỗi
    Console.WriteLine(ex.Message);
}
```

---

## 2. KIẾN THỨC NỀN TẢNG BLAZOR

### 2.1 Razor Syntax - Cách Viết Giao Diện

Razor là cách viết HTML + C# ở cùng 1 file

```razor
@* ===== COMMENT =====  *@
@* Đây là comment trong Razor *@

@* ===== HIỂN THỊ BIẾN ===== *@
<h1>@pageTitle</h1>        @* Hiển thị biến pageTitle *@
<p>Giá: @price.ToString("C")</p>  @* Format tiền tệ *@
<p>Số lượng: @quantity</p>

@* ===== ĐIỀU KIỆN ===== *@
@if (isLoggedIn)
{
    <p>Xin chào, @userName</p>
}
else if (isPending)
{
    <p>Đang tải...</p>
}
else
{
    <p>Vui lòng đăng nhập</p>
}

@* ===== VÒNG LẶP ===== *@
<ul>
    @foreach (var item in cartItems)
    {
        <li>
            @item.ProductName - @item.Price.ToString("N0")₫
        </li>
    }
</ul>

@* ===== SỰ KIỆN ===== *@
<button @onclick="HandleClick">Click tôi</button>
<button @onclick="() => RemoveItem(item.Id)">Xóa</button>
<input @bind="searchText" />  @* Binding 2 chiều *@
<input @bind="quantity" @bind:event="oninput" />  @* Update khi input *@

@* ===== CONDITIONAL CLASS =====  *@
<div class="item @(isSelected ? "selected" : "")">...</div>
@* Hoặc: *@
<div class="@(cartItems.Count > 0 ? "has-items" : "empty")">...</div>

@* ===== ESCAPE HTML ===== *@
@Html.Raw(content)  @* Render raw HTML (cẩn thận XSS!) *@
@content              @* Escape HTML - an toàn *@
```

### 2.2 Component Lifecycle - Các Giai Đoạn Sống Của Component

Component Blazor có các giai đoạn:

```csharp
@code {
    // ===== 1. FIELD INITIALIZATION =====
    // Chạy đầu tiên khi component được khởi tạo
    private List<CartItem> cartItems = new();

    // ===== 2. OnInitialized / OnInitializedAsync =====
    // Chạy 1 lần khi component được load
    // Dùng để: Load dữ liệu ban đầu, subscribe events
    protected override void OnInitialized()
    {
        // Synchronous - không async
        CartService.OnChange += RefreshCart;
    }

    protected override async Task OnInitializedAsync()
    {
        // Asynchronous - có await
        cartItems = await CartService.GetCartItemsAsync();
    }

    // ===== 3. OnAfterRender / OnAfterRenderAsync =====
    // Chạy SAU khi component được render lên UI
    // firstRender = true khi render lần đầu tiên
    // Dùng để: JS interop, focus element, load dữ liệu sau render
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Chạy 1 lần sau render đầu tiên
            await CartService.LoadCartFromStorageAsync();
            StateHasChanged(); // Render lại
        }
    }

    // ===== 4. StateHasChanged =====
    // Gọi để re-render component khi dữ liệu thay đổi
    private void UpdateUI()
    {
        cartItems = CartService.GetCart().Items;
        StateHasChanged(); // Render lại
    }

    // ===== 5. Dispose / IDisposable =====
    // Chạy khi component bị destroy (user rời khỏi page)
    // Dùng để: Cleanup - unsubscribe events, close connections
    public void Dispose()
    {
        CartService.OnChange -= RefreshCart; // Unsubscribe
    }
}
```

### 2.3 Data Binding - Ràng Buộc Dữ Liệu

```razor
@* ===== ONE-WAY BINDING (Property -> UI) ===== *@
<p>Tên: @productName</p>
<input value="@productName" />
@* Nếu productName thay đổi trong code, UI cập nhật
   Nếu user thay đổi input, code KHÔNG cập nhật *@

@* ===== TWO-WAY BINDING (Property <-> UI) ===== *@
<input @bind="productName" />
@* Nếu productName thay đổi, input cập nhật
   Nếu user nhập, productName cập nhật *@

@* ===== BINDING VỚI EVENT ===== *@
<input @bind="quantity" @bind:event="oninput" />
@* Mặc định: update khi blur (rời khỏi input)
   oninput: update khi gõ từng ký tự *@

@* ===== BINDING CHECKED ===== *@
<input type="checkbox" @bind="agreeTerms" />
@* agreeTerms = true/false *@

@* ===== BINDING SELECT ===== *@
<select @bind="selectedPayment">
    <option value="cash">Tiền mặt</option>
    <option value="bank">Chuyển khoản</option>
</select>
@* selectedPayment = "cash" hoặc "bank" *@
```

### 2.4 Event Handling

```csharp
@code {
    // ===== EVENT KHÔNG CÓ PARAMETER =====
    private void HandleClick()
    {
        Console.WriteLine("Button clicked!");
        StateHasChanged();
    }

    // ===== EVENT CÓ PARAMETER =====
    private void RemoveItem(int productId)
    {
        CartService.RemoveFromCart(productId);
    }

    // ===== EVENT ASYNC =====
    private async Task LoadDataAsync()
    {
        var items = await CartService.GetCartItemsAsync();
        StateHasChanged();
    }

    // ===== EVENT VỚI LAMBDA =====
    private void IncreaseQuantity(int productId, int amount)
    {
        var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity += amount;
            StateHasChanged();
        }
    }
}
```

```razor
@* HTML *@
<button @onclick="HandleClick">Click</button>
<button @onclick="() => RemoveItem(item.Id)">Xóa Item</button>
<button @onclick="LoadDataAsync">Load Data</button>
<button @onclick="() => IncreaseQuantity(item.Id, 1)">Tăng</button>
```

---

## 3. PHÂN TÍCH MODELS/CART.CS

File `Models/Cart.cs` định nghĩa cấu trúc dữ liệu giỏ hàng.

### 3.1 CartItem Class

```csharp
public class CartItem
{
    // ===== PROPERTIES =====
    public int ProductId { get; set; }              // ID sản phẩm
    public string ProductName { get; set; } = null!;   // Tên sản phẩm
    public decimal Price { get; set; }              // Giá đơn vị
    public int Quantity { get; set; }               // Số lượng mua
    public string? Unit { get; set; }               // Đơn vị (kg, hộp, etc.)
    public string? CategoryName { get; set; }       // Tên danh mục
    public string? ImageUrl { get; set; }           // URL ảnh sản phẩm

    // ===== CALCULATED PROPERTY =====
    public decimal Subtotal => Price * Quantity;    // Tính toán tự động
    // => là shorthand cho { get { return ...; } }
    // Mỗi lần truy cập Subtotal, nó tính lại Price * Quantity
}
```

**Giải thích chi tiết:**

| Thuộc tính | Kiểu | Mục đích | Ví dụ |
|-----------|------|---------|-------|
| ProductId | int | ID của sản phẩm | 5 |
| ProductName | string | Tên sản phẩm | "iPhone 15" |
| Price | decimal | Giá bán (tiền tệ) | 29990000m |
| Quantity | int | Số lượng mua | 2 |
| Unit | string? | Đơn vị tính | "cái", "hộp" |
| CategoryName | string? | Tên danh mục | "Điện thoại" |
| ImageUrl | string? | URL hình ảnh | "/images/iphone.jpg" |
| Subtotal | decimal (readonly) | Thành tiền = Price * Quantity | 59980000m |

**Tại sao dùng `decimal` cho giá tiền?**
- `decimal` có độ chính xác cao, không có lỗi làm tròn
- `double` có thể sai: 0.1 + 0.2 ≠ 0.3

### 3.2 Cart Class

```csharp
public class Cart
{
    // ===== PROPERTIES =====
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    // Danh sách các item trong giỏ

    // ===== CALCULATED PROPERTIES =====
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    // Tổng tiền = cộng tất cả subtotal của items

    public int TotalItems => Items.Sum(i => i.Quantity);
    // Tổng số lượng = cộng tất cả quantity

    // ===== METHODS =====

    /// <summary>
    /// Thêm sản phẩm vào giỏ
    /// Nếu sản phẩm đã có, tăng quantity
    /// Nếu chưa có, thêm mới
    /// </summary>
    public void AddItem(Product product, int quantity = 1)
    {
        // 1. Tìm xem sản phẩm này đã có trong giỏ chưa
        var existingItem = Items.FirstOrDefault(i => i.ProductId == product.ProductId);
        //    FirstOrDefault() tìm phần tử đầu tiên hoặc trả về null

        if (existingItem != null)
        {
            // 2. Nếu đã có → tăng quantity
            existingItem.Quantity += quantity;
        }
        else
        {
            // 3. Nếu chưa có → tạo CartItem mới
            Items.Add(new CartItem
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = quantity,
                Unit = product.Unit,
                CategoryName = product.Category?.CategoryName,
                ImageUrl = product.ImageUrl
            });
        }
    }

    /// <summary>
    /// Cập nhật số lượng của 1 sản phẩm
    /// Nếu quantity <= 0, xóa sản phẩm
    /// </summary>
    public void UpdateQuantity(int productId, int quantity)
    {
        // 1. Tìm item cần cập nhật
        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                // 2a. Nếu quantity <= 0, xóa item
                Items.Remove(item);
            }
            else
            {
                // 2b. Nếu quantity > 0, cập nhật
                item.Quantity = quantity;
            }
        }
    }

    /// <summary>
    /// Xóa 1 sản phẩm khỏi giỏ
    /// </summary>
    public void RemoveItem(int productId)
    {
        // 1. Tìm item cần xóa
        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            // 2. Xóa nó khỏi list
            Items.Remove(item);
        }
    }

    /// <summary>
    /// Xóa tất cả sản phẩm, làm rỗng giỏ
    /// </summary>
    public void Clear()
    {
        Items.Clear();
    }
}
```

**Flow Logic AddItem:**

```
USER CLICK "THÊM VÀO GIỎ" (product ID = 5)
            ↓
AddItem(product, quantity=1) được gọi
            ↓
Tìm item có ProductId == 5 trong Items list
            ↓
        ┌─────────────────────────┐
        │ Sản phẩm có trong giỏ?  │
        └─────────────────────────┘
                  ↙          ↖
              CÓ              KHÔNG
              ↙               ↖
    item.Quantity += 1     Tạo CartItem mới
        ↓                  và thêm vào Items
    Quantity = 2           Quantity = 1
        ↓                  ↓
        └──────────────────┘
                ↓
        Giỏ hàng cập nhật
```

---

## 4. PHÂN TÍCH SERVICES/CARTSERVICE.CS

`CartService` quản lý logic giỏ hàng và tương tác với localStorage.

### 4.1 Interface ICartService

```csharp
public interface ICartService
{
    // Lấy giỏ hàng hiện tại
    Cart GetCart();
    
    // Các thao tác trên giỏ hàng
    void AddToCart(Product product, int quantity = 1);
    void UpdateQuantity(int productId, int quantity);
    void RemoveFromCart(int productId);
    void ClearCart();
    
    // Lấy thông tin giỏ hàng
    int GetCartItemCount();      // Tổng số lượng sản phẩm
    decimal GetCartTotal();       // Tổng tiền
    
    // Lưu/tải giỏ hàng
    Task LoadCartFromStorageAsync();    // Load từ localStorage
    Task SaveCartToStorageAsync();      // Lưu vào localStorage
    
    // Event thông báo UI cập nhật
    event Action? OnChange;
}
```

**Interface là cái gì?**
- Interface = hợp đồng (contract)
- Nó định nghĩa những phương thức (methods) và thuộc tính (properties) mà một service phải có.
- Lớp (class) triển khai interface phải cung cấp (implement) tất cả những phương thức và thuộc tính đó; nếu không, mã sẽ không biên dịch.
- Interface là một "hợp đồng": chỉ khai báo chữ ký method/property, không chứa phần triển khai (implementation).

### 4.2 CartService Implementation

```csharp
public class CartService : ICartService
{
    // ===== FIELDS - BIẾN PRIVATE =====
    private readonly ILocalStorageService _localStorage;
    // readonly = sau khi set, không thay đổi được
    // Dùng dependency injection (sẽ giải thích sau)

    private Cart _cart = new Cart();
    // Giỏ hàng hiện tại lưu trong RAM

    private const string CART_STORAGE_KEY = "customer_cart";
    // Khóa dùng để lưu/lấy giỏ hàng từ localStorage
    // const = hằng số, không thay đổi

    private bool _isLoaded = false;
    // Cờ để kiểm tra đã load từ localStorage chưa

    // ===== EVENT =====
    public event Action? OnChange;
    // Action = hàm không có parameter, không có return
    // ? = có thể null
    // Event này được gọi khi giỏ hàng thay đổi
    // Để notify UI cập nhật

    // ===== CONSTRUCTOR =====
    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        // Nhận ILocalStorageService từ dependency injection
    }
}
```

### 4.3 Các Method Chính

#### Method 1: LoadCartFromStorageAsync()

```csharp
public async Task LoadCartFromStorageAsync()
{
    if (_isLoaded) return;
    // Nếu đã load, không load lại
    
    try
    {
        // 1. Lấy dữ liệu từ localStorage
        var savedItems = await _localStorage.GetItemAsync<List<CartItem>>(CART_STORAGE_KEY);
        // GetItemAsync<List<CartItem>> = lấy giá trị kiểu List<CartItem>
        
        if (savedItems != null && savedItems.Count > 0)
        {
            // 2. Nếu có dữ liệu, gán vào _cart
            _cart.Items = savedItems;
            
            // 3. Notify UI cập nhật
            NotifyStateChanged();
        }
        
        _isLoaded = true;
        // Đánh dấu đã load xong
    }
    catch
    {
        // Nếu lỗi (localStorage null, parse error, etc.)
        // Giỏ hàng vẫn rỗng, không crash
        _isLoaded = true;
    }
}
```

**Flow khi load page:**

```
User mở trang website
        ↓
Blazor load CartIcon component
        ↓
OnAfterRenderAsync(firstRender: true) chạy
        ↓
LoadCartFromStorageAsync() được gọi
        ↓
        ┌────────────────────────────┐
        │ localStorage có cart data? │
        └────────────────────────────┘
              ↙           ↖
            CÓ           KHÔNG
            ↙           ↖
    Restore giỏ hàng  Giỏ trống
         ↓             ↓
    NotifyStateChanged()
         ↓
    UI cập nhật badge
```

#### Method 2: SaveCartToStorageAsync()

```csharp
public async Task SaveCartToStorageAsync()
{
    try
    {
        // Lưu list items vào localStorage với key "customer_cart"
        await _localStorage.SetItemAsync(CART_STORAGE_KEY, _cart.Items);
        // SetItemAsync(key, value) = lưu
        // value sẽ được serialize thành JSON tự động
    }
    catch
    {
        // Nếu lỗi (storage đầy, permission, etc.), bỏ qua
        // Nhưng giỏ hàng vẫn ở trong RAM, user vẫn dùng được
    }
}
```

**Serialization là gì?**
- Object C# → JSON text (để lưu/gửi)
- JSON text → Object C# (để dùng)

Ví dụ:
```csharp
// C# Object
var items = new List<CartItem>
{
    new CartItem { ProductId = 1, ProductName = "iPhone", Quantity = 2 }
};

// Serialize thành JSON
[{"ProductId":1,"ProductName":"iPhone","Quantity":2}]

// Serialize xong, lưu vào localStorage
// localStorage["customer_cart"] = "[{...}]"
```

#### Method 3: AddToCart()

```csharp
public void AddToCart(Product product, int quantity = 1)
{
    // 1. Dùng Cart.AddItem() để thêm vào danh sách
    _cart.AddItem(product, quantity);

    // 2. Thông báo UI cần cập nhật
    NotifyStateChanged();

    // 3. Lưu vào localStorage (Fire and forget)
    _ = SaveCartToStorageAsync();
    // _ = bỏ qua kết quả, không chờ lưu xong
    // Nên nhanh, không block UI
}
```

**Why Fire and Forget?**
- Saving to localStorage không quan trọng ngay lúc đó
- Nếu await SaveCartAsync(), khi network chậm, UI sẽ lag
- Bằng fire and forget, UI update ngay, save diễn ra ở background

#### Method 4: UpdateQuantity()

```csharp
public void UpdateQuantity(int productId, int quantity)
{
    // 1. Cập nhật trong _cart
    _cart.UpdateQuantity(productId, quantity);
    // Nếu quantity <= 0, sản phẩm bị xóa

    // 2. Notify UI
    NotifyStateChanged();

    // 3. Lưu vào localStorage
    _ = SaveCartToStorageAsync();
}
```

#### Method 5: RemoveFromCart()

```csharp
public void RemoveFromCart(int productId)
{
    // 1. Xóa item khỏi _cart
    _cart.RemoveItem(productId);

    // 2. Notify UI
    NotifyStateChanged();

    // 3. Lưu vào localStorage
    _ = SaveCartToStorageAsync();
}
```

#### Method 6: ClearCart()

```csharp
public async void ClearCart()
{
    // 1. Xóa tất cả items
    _cart.Clear();

    // 2. Notify UI
    NotifyStateChanged();

    // 3. Xóa từ localStorage
    await _localStorage.RemoveItemAsync(CART_STORAGE_KEY);
    // RemoveItemAsync = xóa key khỏi storage
}
```

### 4.4 NotifyStateChanged() - Thông Báo UI Cập Nhật

```csharp
private void NotifyStateChanged() => OnChange?.Invoke();

// Giải thích:
// => là shorthand cho { OnChange?.Invoke(); }
// OnChange?. = null-safe: chỉ gọi nếu OnChange != null
// .Invoke() = gọi event (tương đương gọi hàm)

// Lúc nào gọi?
// - Khi AddToCart()
// - Khi UpdateQuantity()
// - Khi RemoveFromCart()
// - Khi ClearCart()
// - Khi LoadCartFromStorageAsync() xong
```

---

## 5. EVENT-DRIVEN ARCHITECTURE

Event-driven là pattern: khi có sự thay đổi, thông báo những cái đang lắng nghe.

### 5.1 Cách Hoạt Động

```csharp
// ===== BỀN (PUBLISHER) =====
public class CartService
{
    // 1. Khai báo event
    public event Action? OnChange;

    public void AddToCart(...)
    {
        // ... logic ...
        
        // 2. Trigger event (gọi tất cả subscriber)
        OnChange?.Invoke();
        // Hoặc: OnChange?.Invoke();
    }
}

// ===== NGƯỜI LẮNG NGHE (SUBSCRIBER) =====
public class CartIcon // Component
{
    [Inject] ICartService CartService { get; set; }

    protected override void OnInitialized()
    {
        // 3. Subscribe event
        CartService.OnChange += UpdateCartCount;
        // UpdateCartCount sẽ được gọi mỗi khi CartService trigger OnChange
    }

    private void UpdateCartCount()
    {
        // 4. Khi CartService trigger OnChange
        // Hàm này được gọi
        cartItemCount = CartService.GetCartItemCount();
        StateHasChanged(); // Re-render
    }

    public void Dispose()
    {
        // 5. Cleanup - Unsubscribe
        CartService.OnChange -= UpdateCartCount;
        // Ngừng lắng nghe
    }
}
```

### 5.2 Flow Thực Tế

```
TIMELINE:
─────────────────────────────────────────────────────────────

User ở trang ProductDetail
Thêm sản phẩm vào giỏ
        ↓
ProductDetail gọi:
    CartService.AddToCart(product, quantity)
        ↓
CartService:
    1. _cart.AddItem(product, quantity)
    2. NotifyStateChanged()
    3. _ = SaveCartToStorageAsync()
        ↓
NotifyStateChanged():
    OnChange?.Invoke()
    ↓
Tất cả subscriber nhận thông báo:
    ├─ CartIcon:
    │  └─ UpdateCartCount()
    │     └─ StateHasChanged() → Badge update 2 → 3
    │
    ├─ CartPage (nếu đang mở):
    │  └─ RefreshCart()
    │     └─ StateHasChanged() → List update
    │
    └─ Checkout (nếu đang mở):
       └─ RefreshCheckout()
          └─ StateHasChanged() → Total update
        ↓
localStorage:
    Async lưu giỏ hàng (background, không block UI)
        ↓
User thấy:
    - Badge tăng ngay
    - Nếu ở CartPage, list cập nhật
    - Nếu ở Checkout, tổng tiền cập nhật
```

### 5.3 Ưu Điểm Event Pattern

| Ưu điểm | Giải thích |
|---------|-----------|
| **Loose Coupling** | CartService không biết ai đang lắng nghe |
| **Multiple Subscribers** | Nhiều component có thể lắng nghe cùng 1 event |
| **Real-time Updates** | UI cập nhật ngay lập tức |
| **Clean Code** | Không cần pass data từ component này sang kia |
| **Scalable** | Thêm subscriber mới không ảnh hưởng code cũ |

---

## 6. LOCALSTORAGE INTEGRATION

LocalStorage = kho lưu trữ trên browser của user

### 6.1 LocalStorage là gì?

```
Browser Memory:
├─ RAM (Session Storage) - Mất khi đóng tab
└─ Disk (LocalStorage) - Giữ lại đến khi user clear

LocalStorage:
- Domain riêng: example.com chỉ nhìn thấy data của example.com
- Key-Value: lưu giá trị dưới dạng cặp khóa-giá trị
- String: chỉ lưu được string, object phải JSON
- Capacity: ~5MB per domain
- Persistent: tồn tại lâu dài
```

### 6.2 LocalStorageService (Blazor)

```csharp
public interface ILocalStorageService
{
    // Lưu dữ liệu
    Task SetItemAsync<T>(string key, T value);

    // Lấy dữ liệu
    Task<T?> GetItemAsync<T>(string key);

    // Xóa dữ liệu
    Task RemoveItemAsync(string key);

    // Kiểm tra khóa tồn tại
    Task<bool> ContainKeyAsync(string key);

    // Lấy toàn bộ dữ liệu
    Task<IEnumerable<string>> KeysAsync();
}
```

### 6.3 Cách Sử Dụng trong CartService

```csharp
// ===== LƯỚI VỊ TRÍ =====
// Key: "customer_cart"
// Value: JSON của List<CartItem>

// ===== LƯU =====
List<CartItem> items = new List<CartItem>
{
    new CartItem { ProductId = 1, ProductName = "iPhone", Quantity = 2 }
};

await localStorage.SetItemAsync("customer_cart", items);

// Thực tế lưu vào localStorage:
// localStorage["customer_cart"] = "[{\"ProductId\":1,\"ProductName\":\"iPhone\",\"Quantity\":2}]"

// ===== LẤY =====
var savedItems = await localStorage.GetItemAsync<List<CartItem>>("customer_cart");
// LocalStorageService tự động deserialize JSON → Object

// ===== XÓA =====
await localStorage.RemoveItemAsync("customer_cart");
// localStorage["customer_cart"] = undefined
```

### 6.4 Persistence Flow

```
                TIMELINE: USER SESSION
────────────────────────────────────────────────────────

Day 1:
  User mở trang → OnAfterRenderAsync
  LoadCartFromStorageAsync()
  localStorage trống → Giỏ hàng trống
  
  User click "Thêm iPhone"
  AddToCart() → SaveCartToStorageAsync()
  localStorage["customer_cart"] = "[iPhone item]"
  
  User tắt trình duyệt
  
Day 2:
  User mở lại trang
  OnAfterRenderAsync → LoadCartFromStorageAsync()
  Lấy dữ liệu từ localStorage
  _cart.Items = [iPhone item]
  ✓ Giỏ hàng vẫn có iPhone!
  
  User click "Thanh toán"
  CreateOrder()
  ClearCart() → RemoveItemAsync("customer_cart")
  localStorage["customer_cart"] = undefined
  
  Giỏ hàng rỗng
```

---

## TÓM TẮT PHẦN 1

**Kiến thức C#:**
- ✓ LINQ để truy vấn dữ liệu
- ✓ Async/await cho xử lý bất đồng bộ
- ✓ Null coalescing & null conditional
- ✓ List<T> và CRUD operations

**Kiến thức Blazor:**
- ✓ Razor syntax (HTML + C#)
- ✓ Component lifecycle
- ✓ Data binding (one-way, two-way)
- ✓ Event handling

**Models:**
- ✓ CartItem: đại diện 1 sản phẩm trong giỏ
- ✓ Cart: chứa list CartItem, tính toán total

**Services:**
- ✓ CartService: quản lý logic giỏ hàng
- ✓ 6 methods chính: Add, Update, Remove, Clear, Load, Save
- ✓ Event OnChange để notify UI

**Architecture:**
- ✓ Event-Driven: subscriber pattern
- ✓ LocalStorage Persistence: giữ lại giỏ hàng sau khi reload

---

## 7. PHÂN TÍCH CARTICON.RAZOR

`CartIcon.razor` là component nhỏ hiển thị icon giỏ hàng + badge số lượng trên header.

### 7.1 Code Đầy Đủ

```razor
@rendermode InteractiveServer
@inject ICartService CartService
@implements IDisposable

<a href="/cart" class="action-btn cart-btn">
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <circle cx="8" cy="21" r="1"></circle>
        <circle cx="19" cy="21" r="1"></circle>
        <path d="M2.05 2.05h2l2.66 12.42a2 2 0 0 0 2 1.58h9.78a2 2 0 0 0 1.95-1.57l1.65-7.43H5.12"></path>
    </svg>
    @if (cartItemCount > 0)
    {
        <span class="cart-badge">@cartItemCount</span>
    }
</a>

@code {
    private int cartItemCount = 0;

    protected override void OnInitialized()
    {
        CartService.OnChange += UpdateCartCount;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CartService.LoadCartFromStorageAsync();
            cartItemCount = CartService.GetCartItemCount();
            StateHasChanged();
        }
    }

    private void UpdateCartCount()
    {
        cartItemCount = CartService.GetCartItemCount();
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        CartService.OnChange -= UpdateCartCount;
    }
}
```

### 7.2 Giải Thích Từng Phần

**1. Directives & Inject**
```razor
@rendermode InteractiveServer    // Component chạy ở server, có tương tác
@inject ICartService CartService  // Inject CartService để dùng
@implements IDisposable           // Implement IDisposable để cleanup
```

**2. HTML - Icon + Badge**
```razor
<a href="/cart" class="action-btn cart-btn">
    <svg>...</svg>                    <!-- Icon giỏ hàng -->
    @if (cartItemCount > 0)
    {
        <span class="cart-badge">@cartItemCount</span>  <!-- Badge số lượng -->
    }
</a>
```
- Chỉ hiển thị badge nếu có sản phẩm
- Badge hiển thị tổng số lượng items

**3. OnInitialized - Subscribe Event**
```csharp
protected override void OnInitialized()
{
    CartService.OnChange += UpdateCartCount;
    // Subscribe: khi CartService thay đổi, gọi UpdateCartCount()
}
```

**4. OnAfterRenderAsync - Load Data**
```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)  // Chỉ chạy lần đầu tiên
    {
        // 1. Load giỏ hàng từ localStorage
        await CartService.LoadCartFromStorageAsync();
        
        // 2. Lấy số lượng items
        cartItemCount = CartService.GetCartItemCount();
        
        // 3. Re-render để hiển thị badge
        StateHasChanged();
    }
}
```

**Tại sao load trong OnAfterRenderAsync?**
- `OnInitialized()` chạy trước khi render, chưa có JS interop
- `localStorage` cần JS interop
- `OnAfterRenderAsync()` chạy sau render, JS đã sẵn sàng

**5. UpdateCartCount - Event Handler**
```csharp
private void UpdateCartCount()
{
    cartItemCount = CartService.GetCartItemCount();
    InvokeAsync(StateHasChanged);
    // InvokeAsync vì event có thể gọi từ thread khác
}
```

**6. Dispose - Cleanup**
```csharp
public void Dispose()
{
    CartService.OnChange -= UpdateCartCount;
    // Unsubscribe để tránh memory leak
}
```

### 7.3 Flow Real-time Update

```
USER ở trang ProductDetail, click "Thêm vào giỏ"
        ↓
CartService.AddToCart(product, 1)
        ↓
    _cart.AddItem(product, 1)
    Items.Count: 2 → 3
        ↓
    NotifyStateChanged()
    OnChange?.Invoke()
        ↓
CartIcon.UpdateCartCount() được gọi
        ↓
    cartItemCount = CartService.GetCartItemCount()
    cartItemCount: 2 → 3
        ↓
    StateHasChanged()
        ↓
Badge re-render: "2" → "3"
        ↓
User thấy badge update ngay lập tức
```

---

## 8. PHÂN TÍCH CARTPAGE.RAZOR

`CartPage.razor` là trang hiển thị danh sách sản phẩm trong giỏ hàng.

### 8.1 Structure Tổng Quan

```
CartPage
├── Empty State (nếu giỏ trống)
│   └── Link "Tiếp tục mua sắm"
│
└── Cart Content (nếu có items)
    ├── Cart Items List
    │   ├── Item 1: Image, Name, Price, Quantity Controls, Subtotal, Remove
    │   ├── Item 2
    │   └── ...
    │
    ├── Cart Actions
    │   ├── "Xóa tất cả"
    │   └── "Tiếp tục mua sắm"
    │
    └── Cart Summary
        ├── Tổng sản phẩm
        ├── Tạm tính
        ├── Phí vận chuyển
        ├── Tổng cộng
        └── Button "Thanh toán"
```

### 8.2 Code @code Block

```csharp
@code {
    private Models.Cart cart = new Models.Cart();

    // ===== LIFECYCLE =====
    protected override void OnInitialized()
    {
        // Subscribe event để update khi cart thay đổi
        CartService.OnChange += RefreshCart;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load giỏ hàng từ localStorage
            await CartService.LoadCartFromStorageAsync();
            cart = CartService.GetCart();
            StateHasChanged();
        }
    }

    // ===== EVENT HANDLERS =====
    private void RefreshCart()
    {
        cart = CartService.GetCart();
        InvokeAsync(StateHasChanged);
    }

    private void IncreaseQuantity(int productId)
    {
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            CartService.UpdateQuantity(productId, item.Quantity + 1);
        }
    }

    private void DecreaseQuantity(int productId)
    {
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null && item.Quantity > 1)
        {
            CartService.UpdateQuantity(productId, item.Quantity - 1);
        }
    }

    private void RemoveItem(int productId)
    {
        CartService.RemoveFromCart(productId);
    }

    private void ClearCart()
    {
        CartService.ClearCart();
    }

    private void ProceedToCheckout()
    {
        if (!AuthService.IsAuthenticated)
        {
            Navigation.NavigateTo("/login?returnUrl=/checkout", forceLoad: false);
        }
        else
        {
            Navigation.NavigateTo("/checkout", forceLoad: false);
        }
    }

    public void Dispose()
    {
        CartService.OnChange -= RefreshCart;
    }
}
```

### 8.3 Giải Thích Chi Tiết Các Method

**1. RefreshCart()**
```csharp
private void RefreshCart()
{
    cart = CartService.GetCart();  // Lấy cart mới nhất
    InvokeAsync(StateHasChanged);   // Re-render
}
```
- Được gọi khi CartService.OnChange trigger
- `InvokeAsync()` vì event có thể từ thread khác

**2. IncreaseQuantity()**
```csharp
private void IncreaseQuantity(int productId)
{
    // 1. Tìm item trong cart local
    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
    
    if (item != null)
    {
        // 2. Gọi CartService để update (quantity + 1)
        CartService.UpdateQuantity(productId, item.Quantity + 1);
        // CartService sẽ trigger OnChange
        // → RefreshCart() được gọi
        // → UI update
    }
}
```

**Flow khi user click "+":**
```
User click nút "+" trên item (productId = 5)
        ↓
IncreaseQuantity(5) được gọi
        ↓
Tìm item có ProductId = 5 trong cart.Items
item.Quantity hiện tại = 2
        ↓
CartService.UpdateQuantity(5, 3)
        ↓
CartService:
    _cart.UpdateQuantity(5, 3)
    item.Quantity = 3
    NotifyStateChanged()
    SaveCartToStorageAsync()
        ↓
OnChange event trigger
        ↓
RefreshCart() được gọi
        ↓
cart = CartService.GetCart()  // Lấy cart mới (quantity = 3)
StateHasChanged()
        ↓
UI re-render: hiển thị quantity = 3
```

**3. DecreaseQuantity()**
```csharp
private void DecreaseQuantity(int productId)
{
    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
    
    if (item != null && item.Quantity > 1)  // Chỉ giảm nếu > 1
    {
        CartService.UpdateQuantity(productId, item.Quantity - 1);
    }
}
```
- Nếu `item.Quantity == 1`, nút "-" không làm gì
- User phải dùng nút "Xóa" để remove item

**4. RemoveItem()**
```csharp
private void RemoveItem(int productId)
{
    CartService.RemoveFromCart(productId);
    // CartService sẽ:
    // 1. Remove item khỏi _cart.Items
    // 2. Trigger OnChange
    // 3. RefreshCart() → UI update
}
```

**5. ProceedToCheckout()**
```csharp
private void ProceedToCheckout()
{
    if (!AuthService.IsAuthenticated)
    {
        // Chưa đăng nhập → redirect to login với returnUrl
        Navigation.NavigateTo("/login?returnUrl=/checkout", forceLoad: false);
    }
    else
    {
        // Đã đăng nhập → đi thẳng checkout
        Navigation.NavigateTo("/checkout", forceLoad: false);
    }
}
```

### 8.4 HTML Markup Quan Trọng

**Empty State:**
```razor
@if (cart.Items.Count == 0)
{
    <div class="empty-cart">
        <div class="empty-icon"><i class="hgi-stroke hgi-shopping-cart-01"></i></div>
        <h2>Giỏ hàng trống</h2>
        <p>Bạn chưa có sản phẩm nào trong giỏ hàng</p>
        <a href="/shop" class="btn btn-primary btn-lg">Tiếp tục mua sắm</a>
    </div>
}
```

**Cart Item với Quantity Controls:**
```razor
<div class="item-quantity">
    <div class="quantity-controls">
        <button class="qty-btn" @onclick="() => DecreaseQuantity(item.ProductId)">−</button>
        <span class="qty-value">@item.Quantity</span>
        <button class="qty-btn" @onclick="() => IncreaseQuantity(item.ProductId)">+</button>
    </div>
</div>
```

**Cart Summary:**
```razor
<div class="summary-card">
    <h2 class="summary-title">Tóm tắt đơn hàng</h2>
    
    <div class="summary-row">
        <span>Tổng sản phẩm</span>
        <span>@cart.TotalItems sản phẩm</span>
    </div>
    
    <div class="summary-row">
        <span>Tạm tính</span>
        <span>@cart.TotalAmount.ToString("N0")₫</span>
    </div>
    
    <div class="summary-total">
        <span>Tổng cộng</span>
        <span class="total-amount">@cart.TotalAmount.ToString("N0")₫</span>
    </div>
    
    <button class="btn btn-primary btn-lg checkout-btn" @onclick="ProceedToCheckout">
        Tiến hành thanh toán
    </button>
</div>
```

---

## 9. PHÂN TÍCH CHECKOUT.RAZOR

`Checkout.razor` là trang thanh toán phức tạp nhất.

### 9.1 Structure Tổng Quan

```
Checkout Page
├── Authentication Check
│   └── Redirect to login nếu chưa đăng nhập
│
├── Empty Cart Check
│   └── Hiển thị "Giỏ trống" nếu không có items
│
└── Checkout Form (nếu authenticated & có items)
    ├── Section 1: Thông tin giao hàng
    │   ├── Tên, Email, Điện thoại, Địa chỉ
    │   └── Warning nếu chưa có địa chỉ
    │
    ├── Section 2: Mã giảm giá
    │   ├── Input nhập mã
    │   ├── Button "Áp dụng"
    │   └── Hiển thị promo applied hoặc error
    │
    ├── Section 3: Phương thức thanh toán
    │   ├── Radio: Tiền mặt (COD)
    │   ├── Radio: Chuyển khoản ngân hàng
    │   │   └── Show QR + thông tin TK
    │   └── Radio: Ví điện tử MoMo
    │       └── Show QR + số điện thoại
    │
    └── Order Summary (Sidebar)
        ├── Danh sách items
        ├── Tạm tính
        ├── Giảm giá (nếu có)
        ├── Phí ship
        ├── Tổng cộng
        └── Button "Đặt hàng"
```

### 9.2 Code @code Block

```csharp
@code {
    // ===== STATE VARIABLES =====
    private Models.Cart cart = new Models.Cart();
    private string paymentMethod = "cash";           // Mặc định: tiền mặt
    private string promoCode = "";                   // Input mã giảm giá
    private Models.Promotion? appliedPromo;          // Promo đã apply
    private decimal discountAmount = 0;              // Số tiền giảm
    private string? promoError;                      // Lỗi khi apply promo
    private string? errorMessage;                    // Lỗi chung
    private bool isApplyingPromo = false;            // Loading apply promo
    private bool isPlacingOrder = false;             // Loading đặt hàng
    private bool isRedirecting = false;              // Đang redirect
    private string transferContent = "";             // Nội dung chuyển khoản

    // ===== HELPER METHOD =====
    private void GenerateTransferContent()
    {
        // Tạo nội dung chuyển khoản unique
        var timestamp = DateTime.Now.ToString("ddMMHHmm");
        var customerId = AuthService.CurrentCustomer?.CustomerId ?? 0;
        transferContent = $"STUFFSUS {customerId} {timestamp}";
        // Ví dụ: "STUFFSUS 123 13121530"
    }

    // ===== LIFECYCLE =====
    protected override void OnInitialized()
    {
        CartService.OnChange += RefreshCart;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CartService.LoadCartFromStorageAsync();
            cart = CartService.GetCart();
            GenerateTransferContent();
            StateHasChanged();
        }
    }

    private void RefreshCart()
    {
        cart = CartService.GetCart();
        InvokeAsync(StateHasChanged);
    }

    // ===== PROMO CODE LOGIC =====
    private async Task ApplyPromoCode()
    {
        if (string.IsNullOrWhiteSpace(promoCode))
        {
            promoError = "Vui lòng nhập mã giảm giá";
            return;
        }

        isApplyingPromo = true;
        promoError = null;
        StateHasChanged();

        try
        {
            // 1. Validate promo code với OrderService
            var promo = await OrderService.ValidatePromoCodeAsync(
                promoCode.Trim().ToUpper(), 
                cart.TotalAmount
            );
            
            if (promo != null)
            {
                // 2. Promo hợp lệ → apply
                appliedPromo = promo;
                discountAmount = await OrderService.CalculateDiscountAsync(
                    promo, 
                    cart.TotalAmount
                );
                promoCode = "";  // Clear input
            }
            else
            {
                // 3. Promo không hợp lệ
                promoError = "Mã giảm giá không hợp lệ hoặc không áp dụng được cho đơn hàng này";
            }
        }
        catch
        {
            promoError = "Đã xảy ra lỗi khi kiểm tra mã giảm giá";
        }
        finally
        {
            isApplyingPromo = false;
            StateHasChanged();
        }
    }

    private void RemovePromo()
    {
        appliedPromo = null;
        discountAmount = 0;
    }

    // ===== PLACE ORDER =====
    private async Task PlaceOrder()
    {
        // 1. Check authentication
        if (AuthService.CurrentCustomer == null)
        {
            Navigation.NavigateTo("/login?returnUrl=/checkout", forceLoad: false);
            return;
        }

        // 2. Check địa chỉ giao hàng
        if (string.IsNullOrEmpty(AuthService.CurrentCustomer.Address))
        {
            errorMessage = "Vui lòng cập nhật địa chỉ giao hàng trong Thông tin cá nhân để tiếp tục đặt hàng.";
            return;
        }

        isPlacingOrder = true;
        errorMessage = null;
        StateHasChanged();

        try
        {
            // 3. Tạo order trong database
            var order = await OrderService.CreateOrderAsync(
                AuthService.CurrentCustomer.CustomerId,
                cart,
                appliedPromo?.PromoId,
                paymentMethod,
                transferContent
            );

            // 4. Mark redirecting
            isRedirecting = true;
            isPlacingOrder = false;
            StateHasChanged();
            
            // 5. Clear cart
            CartService.ClearCart();
            
            // 6. Redirect to success page
            Navigation.NavigateTo($"/order-success/{order.OrderId}", forceLoad: false);
        }
        catch (Exception ex)
        {
            errorMessage = $"Đã xảy ra lỗi khi đặt hàng: {ex.Message}";
            Console.WriteLine($"PlaceOrder Error: {ex}");
        }
        finally
        {
            isPlacingOrder = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        CartService.OnChange -= RefreshCart;
    }
}
```

### 9.3 Giải Thích Chi Tiết

#### 1. GenerateTransferContent()

```csharp
private void GenerateTransferContent()
{
    var timestamp = DateTime.Now.ToString("ddMMHHmm");
    // dd = ngày (13)
    // MM = tháng (12)
    // HH = giờ (15)
    // mm = phút (30)
    // → "13121530"
    
    var customerId = AuthService.CurrentCustomer?.CustomerId ?? 0;
    // Lấy ID khách hàng, nếu null thì = 0
    
    transferContent = $"STUFFSUS {customerId} {timestamp}";
    // Ví dụ: "STUFFSUS 123 13121530"
}
```

**Tại sao cần nội dung chuyển khoản?**
- Để nhân viên kiểm tra chuyển khoản nào thuộc đơn hàng nào
- Format: `TÊN_SHOP CUSTOMER_ID TIMESTAMP`
- Unique cho mỗi giao dịch

#### 2. ApplyPromoCode() - Logic Áp Mã Giảm Giá

```csharp
private async Task ApplyPromoCode()
{
    // ===== VALIDATION =====
    if (string.IsNullOrWhiteSpace(promoCode))
    {
        promoError = "Vui lòng nhập mã giảm giá";
        return;
    }

    // ===== SET LOADING STATE =====
    isApplyingPromo = true;
    promoError = null;
    StateHasChanged();

    try
    {
        // ===== CALL API VALIDATE =====
        var promo = await OrderService.ValidatePromoCodeAsync(
            promoCode.Trim().ToUpper(),  // Normalize: trim space, uppercase
            cart.TotalAmount              // Pass total để check min order
        );
        
        // ===== CHECK RESULT =====
        if (promo != null)
        {
            // Valid promo
            appliedPromo = promo;
            
            // Calculate discount
            discountAmount = await OrderService.CalculateDiscountAsync(
                promo, 
                cart.TotalAmount
            );
            // Ví dụ: 
            // cart.TotalAmount = 1,000,000₫
            // promo.DiscountPercentage = 10%
            // discountAmount = 100,000₫
            
            promoCode = "";  // Clear input
        }
        else
        {
            // Invalid promo
            promoError = "Mã giảm giá không hợp lệ hoặc không áp dụng được cho đơn hàng này";
        }
    }
    catch
    {
        promoError = "Đã xảy ra lỗi khi kiểm tra mã giảm giá";
    }
    finally
    {
        // ===== RESET LOADING STATE =====
        isApplyingPromo = false;
        StateHasChanged();
    }
}
```

**Flow Apply Promo:**
```
User nhập "SUMMER50" → Click "Áp dụng"
        ↓
ApplyPromoCode() gọi
        ↓
isApplyingPromo = true
Button text: "Áp dụng" → "Đang kiểm tra..."
        ↓
OrderService.ValidatePromoCodeAsync("SUMMER50", 1000000)
        ↓
    Database query:
    SELECT * FROM promotions
    WHERE PromoCode = 'SUMMER50'
      AND StartDate <= NOW()
      AND EndDate >= NOW()
      AND (MinOrderAmount IS NULL OR MinOrderAmount <= 1000000)
        ↓
    Result: Promotion { DiscountPercentage = 50 }
        ↓
CalculateDiscountAsync(promo, 1000000)
    = 1000000 * 50 / 100
    = 500,000₫
        ↓
appliedPromo = promo
discountAmount = 500,000
        ↓
StateHasChanged()
        ↓
UI update:
✓ SUMMER50 - Giảm 50%
Giảm giá: -500,000₫
Tổng cộng: 500,000₫
```

#### 3. PlaceOrder() - Đặt Hàng

```csharp
private async Task PlaceOrder()
{
    // ===== STEP 1: AUTHENTICATION CHECK =====
    if (AuthService.CurrentCustomer == null)
    {
        Navigation.NavigateTo("/login?returnUrl=/checkout");
        return;
    }

    // ===== STEP 2: ADDRESS CHECK =====
    if (string.IsNullOrEmpty(AuthService.CurrentCustomer.Address))
    {
        errorMessage = "Vui lòng cập nhật địa chỉ giao hàng...";
        return;  // Không cho đặt hàng
    }

    // ===== STEP 3: SET LOADING =====
    isPlacingOrder = true;
    errorMessage = null;
    StateHasChanged();  // Button: "Đặt hàng" → "Đang xử lý..."

    try
    {
        // ===== STEP 4: CREATE ORDER IN DATABASE =====
        var order = await OrderService.CreateOrderAsync(
            AuthService.CurrentCustomer.CustomerId,  // Người đặt
            cart,                                     // Giỏ hàng
            appliedPromo?.PromoId,                   // Mã giảm giá (nullable)
            paymentMethod,                           // cash/bank/momo
            transferContent                          // Nội dung CK
        );
        
        // OrderService.CreateOrderAsync() sẽ:
        // 1. Insert vào bảng orders
        // 2. Insert vào bảng order_items (từng sản phẩm)
        // 3. Insert vào bảng payments
        // 4. Return Order object với OrderId

        // ===== STEP 5: PREPARE REDIRECT =====
        isRedirecting = true;
        isPlacingOrder = false;
        StateHasChanged();
        
        // ===== STEP 6: CLEAR CART =====
        CartService.ClearCart();
        // Xóa giỏ hàng khỏi RAM và localStorage
        
        // ===== STEP 7: REDIRECT TO SUCCESS PAGE =====
        Navigation.NavigateTo($"/order-success/{order.OrderId}");
    }
    catch (Exception ex)
    {
        // ===== ERROR HANDLING =====
        errorMessage = $"Đã xảy ra lỗi khi đặt hàng: {ex.Message}";
        Console.WriteLine($"PlaceOrder Error: {ex}");
    }
    finally
    {
        isPlacingOrder = false;
        StateHasChanged();
    }
}
```

**Flow Đặt Hàng Thành Công:**
```
User click "Đặt hàng"
        ↓
PlaceOrder() gọi
        ↓
Check authenticated? ✓
Check address? ✓
        ↓
isPlacingOrder = true
Button disabled, text: "Đang xử lý..."
        ↓
OrderService.CreateOrderAsync()
        ↓
    Database Transaction:
    BEGIN TRANSACTION
        INSERT INTO orders (...) VALUES (...)
        → OrderId = 1001
        
        INSERT INTO order_items VALUES
        (1001, ProductId=5, Quantity=2, Price=999000),
        (1001, ProductId=7, Quantity=1, Price=500000)
        
        INSERT INTO payments VALUES
        (1001, Method='bank', Status='Pending', Amount=1399000)
    COMMIT TRANSACTION
        ↓
    Return Order { OrderId = 1001, ... }
        ↓
CartService.ClearCart()
    _cart.Items.Clear()
    localStorage.RemoveItem("customer_cart")
        ↓
Navigation.NavigateTo("/order-success/1001")
        ↓
User thấy trang "Đặt hàng thành công"
```

### 9.4 Payment Methods - 3 Phương Thức Thanh Toán

#### 1. Cash (Tiền mặt - COD)
```razor
<label class="payment-option @(paymentMethod == "cash" ? "selected" : "")">
    <input type="radio" name="payment" value="cash" 
           @onchange="@(() => paymentMethod = "cash")" 
           checked="@(paymentMethod == "cash")" />
    <div class="payment-content">
        <span class="payment-icon"><i class="hgi-stroke hgi-money-01"></i></span>
        <div class="payment-info">
            <span class="payment-name">Tiền mặt</span>
            <span class="payment-desc">Thanh toán khi nhận hàng (COD)</span>
        </div>
    </div>
</label>
```
- User chọn → `paymentMethod = "cash"`
- Không cần thông tin thêm
- Thanh toán khi ship giao hàng

#### 2. Bank Transfer (Chuyển khoản)
```razor
<label class="payment-option @(paymentMethod == "bank_transfer" ? "selected" : "")">
    <input type="radio" name="payment" value="bank_transfer" 
           @onchange="@(() => paymentMethod = "bank_transfer")" />
    ...
</label>

@if (paymentMethod == "bank_transfer")
{
    <div class="payment-detail-section">
        <div class="qr-container">
            <img src="/images/mbbank.jpg" alt="MB Bank QR Code" class="qr-image" />
        </div>
        <div class="transfer-info">
            <h3>Thông tin chuyển khoản</h3>
            <div class="transfer-row">
                <span class="transfer-label">Ngân hàng:</span>
                <span class="transfer-value">MB Bank</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Số tài khoản:</span>
                <span class="transfer-value account-number">0931816175</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Tên người nhận:</span>
                <span class="transfer-value">DINH PHUC THINH</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Số tiền:</span>
                <span class="transfer-value amount">@((cart.TotalAmount - discountAmount).ToString("N0"))₫</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Nội dung:</span>
                <span class="transfer-value content">@transferContent</span>
            </div>
        </div>
        <div class="payment-warning">
            <i class="hgi-stroke hgi-alert-02"></i>
            <span>Vui lòng chuyển khoản <strong>ĐÚNG SỐ TIỀN</strong> và <strong>NỘI DUNG</strong> như trên...</span>
        </div>
    </div>
}
```

**Conditional Rendering:**
- Chỉ hiển thị khi `paymentMethod == "bank_transfer"`
- Hiển thị QR code + thông tin tài khoản
- Số tiền = `TotalAmount - discountAmount`
- Nội dung = `transferContent` (unique)

#### 3. E-Wallet (Ví điện tử MoMo)
```razor
@if (paymentMethod == "e-wallet")
{
    <div class="payment-detail-section momo">
        <div class="qr-container">
            <img src="/images/momo.jpg" alt="MoMo QR Code" />
        </div>
        <div class="transfer-info">
            <h3>Thông tin chuyển khoản MoMo</h3>
            <div class="transfer-row">
                <span class="transfer-label">Số điện thoại:</span>
                <span class="transfer-value account-number">0931816175</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Tên người nhận:</span>
                <span class="transfer-value">DINH PHUC THINH</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Số tiền:</span>
                <span class="transfer-value amount">@((cart.TotalAmount - discountAmount).ToString("N0"))₫</span>
            </div>
            <div class="transfer-row">
                <span class="transfer-label">Nội dung:</span>
                <span class="transfer-value content">@transferContent</span>
            </div>
        </div>
    </div>
}
```

### 9.5 Order Summary Sidebar

```razor
<div class="order-summary">
    <div class="summary-card">
        <h2 class="summary-title">Đơn hàng của bạn</h2>
        
        <!-- Danh sách items -->
        <div class="order-items">
            @foreach (var item in cart.Items)
            {
                <div class="order-item">
                    <div class="item-details">
                        <span class="item-name">@item.ProductName</span>
                        <span class="item-qty">x @item.Quantity</span>
                    </div>
                    <span class="item-total">@item.Subtotal.ToString("N0")₫</span>
                </div>
            }
        </div>

        <div class="summary-divider"></div>

        <!-- Tạm tính -->
        <div class="summary-row">
            <span>Tạm tính</span>
            <span>@cart.TotalAmount.ToString("N0")₫</span>
        </div>
        
        <!-- Giảm giá (nếu có) -->
        @if (discountAmount > 0)
        {
            <div class="summary-row discount">
                <span>Giảm giá</span>
                <span>-@discountAmount.ToString("N0")₫</span>
            </div>
        }
        
        <!-- Phí ship -->
        <div class="summary-row">
            <span>Phí vận chuyển</span>
            <span class="free">Miễn phí</span>
        </div>
        
        <div class="summary-divider"></div>
        
        <!-- Tổng cộng -->
        <div class="summary-total">
            <span>Tổng cộng</span>
            <span class="total-amount">@((cart.TotalAmount - discountAmount).ToString("N0"))₫</span>
        </div>

        <!-- Button đặt hàng -->
        <button class="btn btn-primary btn-lg place-order-btn" 
                @onclick="PlaceOrder" 
                disabled="@(isPlacingOrder || string.IsNullOrEmpty(AuthService.CurrentCustomer?.Address))">
            @if (isPlacingOrder)
            {
                <span class="spinner"></span>
                <span>Đang xử lý...</span>
            }
            else
            {
                <span>Đặt hàng</span>
            }
        </button>
    </div>
</div>
```

**Button Disabled Conditions:**
```csharp
disabled="@(isPlacingOrder || string.IsNullOrEmpty(AuthService.CurrentCustomer?.Address))"
```
- Disabled khi:
  - `isPlacingOrder = true` (đang xử lý)
  - HOẶC chưa có địa chỉ giao hàng

---

## 10. TÓM TẮT TOÀN BỘ FLOW HỆ THỐNG

### Flow 1: Thêm Sản Phẩm Vào Giỏ
```
ProductDetail.razor
    User click "Thêm vào giỏ"
        ↓
    @onclick="() => AddToCart(product)"
        ↓
    CartService.AddToCart(product, quantity)
        ↓
CartService
    _cart.AddItem(product, quantity)
        ├─ Tìm item existing
        ├─ Nếu có: Quantity += quantity
        └─ Nếu không: Items.Add(new CartItem)
    NotifyStateChanged()
        └─ OnChange?.Invoke()
    _ = SaveCartToStorageAsync()
        └─ localStorage["customer_cart"] = JSON
        ↓
Event Subscribers
    CartIcon.UpdateCartCount()
        └─ cartCount = 2 → 3
        └─ StateHasChanged() → Badge update
    
    CartPage.RefreshCart() (nếu đang mở)
        └─ cart = GetCart()
        └─ StateHasChanged() → List update
```

### Flow 2: Cập Nhật Quantity Trong CartPage
```
CartPage.razor
    User click nút "+"
        ↓
    @onclick="() => IncreaseQuantity(item.ProductId)"
        ↓
    IncreaseQuantity(productId)
        ├─ Tìm item trong cart.Items
        └─ CartService.UpdateQuantity(productId, item.Quantity + 1)
            ↓
CartService
    _cart.UpdateQuantity(productId, quantity)
        ├─ Tìm item
        ├─ Nếu quantity <= 0: Remove
        └─ Nếu quantity > 0: item.Quantity = quantity
    NotifyStateChanged()
    _ = SaveCartToStorageAsync()
        ↓
CartPage.RefreshCart()
    cart = CartService.GetCart()
    StateHasChanged()
        ↓
UI: Quantity "2" → "3"
```

### Flow 3: Áp Dụng Mã Giảm Giá
```
Checkout.razor
    User nhập "SUMMER50" → click "Áp dụng"
        ↓
    ApplyPromoCode()
        ├─ Validation: empty check
        ├─ isApplyingPromo = true
        └─ OrderService.ValidatePromoCodeAsync("SUMMER50", 1000000)
            ↓
OrderService
    Database query promotions table
        ├─ WHERE PromoCode = 'SUMMER50'
        ├─ AND StartDate <= NOW()
        ├─ AND EndDate >= NOW()
        └─ AND MinOrderAmount <= 1000000
            ↓
    Return Promotion { DiscountPercentage = 50 }
        ↓
    CalculateDiscountAsync(promo, 1000000)
        └─ Return 500,000
            ↓
Checkout.razor
    appliedPromo = promo
    discountAmount = 500,000
    StateHasChanged()
        ↓
UI Update:
    ✓ SUMMER50 - Giảm 50%
    Giảm giá: -500,000₫
    Tổng: 500,000₫
```

### Flow 4: Đặt Hàng Thành Công
```
Checkout.razor
    User click "Đặt hàng"
        ↓
    PlaceOrder()
        ├─ Check authenticated ✓
        ├─ Check address ✓
        ├─ isPlacingOrder = true
        └─ OrderService.CreateOrderAsync(customerId, cart, promoId, payment, content)
            ↓
OrderService
    using var transaction = dbContext.Database.BeginTransaction()
        ├─ Insert orders table
        │   └─ OrderId = 1001
        ├─ Insert order_items table
        │   ├─ (1001, ProductId=5, Qty=2, Price=999000)
        │   └─ (1001, ProductId=7, Qty=1, Price=500000)
        ├─ Insert payments table
        │   └─ (1001, Method='bank', Status='Pending', Amount=1399000)
        └─ transaction.Commit()
            ↓
    Return Order { OrderId = 1001 }
        ↓
Checkout.razor
    CartService.ClearCart()
        ├─ _cart.Items.Clear()
        ├─ localStorage.RemoveItem("customer_cart")
        └─ NotifyStateChanged()
            ↓
    Navigation.NavigateTo("/order-success/1001")
        ↓
OrderSuccess.razor
    Hiển thị "Đặt hàng thành công!"
    OrderId: #1001
    Trạng thái: Đang xử lý
```

### Flow 5: Load Page Sau Khi Refresh
```
Browser Load
    ↓
MainLayout.razor
    CartIcon component render
        ↓
CartIcon
    OnInitialized()
        └─ CartService.OnChange += UpdateCartCount
    
    OnAfterRenderAsync(firstRender: true)
        ↓
        CartService.LoadCartFromStorageAsync()
            ├─ localStorage.GetItemAsync("customer_cart")
            ├─ Deserialize JSON → List<CartItem>
            ├─ _cart.Items = savedItems
            └─ NotifyStateChanged()
                ↓
        cartItemCount = CartService.GetCartItemCount()
        StateHasChanged()
            ↓
Badge hiển thị số lượng đúng
```

---

## TÓM TẮT CUỐI CÙNG

### ✅ Đã Học Được

**1. Models:**
- `CartItem`: đại diện 1 sản phẩm (ProductId, Name, Price, Quantity, Subtotal)
- `Cart`: chứa List<CartItem>, có AddItem(), UpdateQuantity(), RemoveItem(), Clear()

**2. CartService:**
- 6 methods chính: AddToCart, UpdateQuantity, RemoveFromCart, ClearCart, Load, Save
- Event `OnChange` để notify UI
- LocalStorage integration để persist data

**3. Components:**
- `CartIcon`: badge real-time update
- `CartPage`: list items, quantity controls, checkout button
- `Checkout`: 3 payment methods, promo code, place order

**4. Patterns:**
- Event-Driven: Publisher-Subscriber
- Async/Await: non-blocking operations
- Component Lifecycle: OnInitialized, OnAfterRender, Dispose

**5. Blazor Concepts:**
- Razor syntax: @if, @foreach, @bind, @onclick
- Data binding: one-way, two-way
- StateHasChanged(): manual re-render
- Dependency Injection: @inject

---

## 📝 CHECKLIST TRƯỚC BÁO CÁO

- [ ] Hiểu CartItem: Price, Quantity, Subtotal
- [ ] Hiểu Cart: AddItem, UpdateQuantity, RemoveItem
- [ ] Hiểu CartService: 6 methods, Event OnChange, LocalStorage
- [ ] Hiểu Event pattern: Subscribe, Invoke, Unsubscribe, Dispose
- [ ] Hiểu CartIcon: real-time badge update flow
- [ ] Hiểu CartPage: IncreaseQty, DecreaseQty, RemoveItem
- [ ] Hiểu Checkout: ApplyPromoCode logic
- [ ] Hiểu Checkout: PlaceOrder flow (authentication → address → create order → clear cart → redirect)
- [ ] Hiểu 3 payment methods: Cash, Bank Transfer, MoMo
- [ ] Hiểu component lifecycle: OnInitialized, OnAfterRenderAsync, Dispose
- [ ] Luyện giải thích từng flow bằng lời
- [ ] Test demo ít nhất 1 lần: add product → see badge → go cart → checkout → place order
