#Project Ecommerce Demo

- Task 1: Authentication & Authorization

  - Task 1.1: Register API
    
    Endpoint: POST /api/v1/auth/register
    Yêu cầu: Nhận thông tin user, kiểm tra email trùng lặp, mã hóa mật khẩu bằng BCrypt trước khi lưu vào DB.
  - Task 1.2: Login API
  
    Endpoint: POST /api/v1/auth/login 
    Yêu cầu: Xác thực thông tin, tạo và trả về JWT Token (chứa userId, username, roles).
  - Task 1.3: Security Middleware/Filter
  
    Yêu cầu: Cấu hình phân quyền (Role-based). Ví dụ: Admin mới được thêm sản phẩm, User mới được đặt hàng.
- Task 2: Product Catalog (Quản lý sản phẩm)
  Mục tiêu: Làm quen với các thao tác CRUD cơ bản và kỹ năng xử lý dữ liệu lớn (phân trang).
  
  - Task 2.1: List Products API
    
    Endpoint: GET /api/v1/products
    Yêu cầu: Trả về danh sách sản phẩm có Pagination (page, size) và Filtering (theo category, khoảng giá).
  - Task 2.2: Product Details API
    
    Endpoint: GET /api/v1/products/{id}
    Yêu cầu: Trả về thông tin chi tiết của một sản phẩm.
  - Task 2.3: Admin Product Management (CRUD)
    
    Endpoints: POST, PUT, DELETE cho /api/v1/products.
    Yêu cầu: Chỉ Admin mới có quyền gọi. Thực hiện Soft Delete (không xóa hẳn record khỏi DB).
    
- Task 3: Shopping Cart (Giỏ hàng)
  
  - Task 3.1: Add to Cart API
    
    Endpoint: POST /api/v1/cart/add
    Yêu cầu: Kiểm tra sản phẩm có tồn tại và còn đủ hàng trong kho (Inventory) không trước khi thêm vào giỏ.
  
  - Task 3.2: View & Update Cart
  
    Endpoints: GET /api/v1/cart, PUT /api/v1/cart/items/{id}.
    Yêu cầu: Lấy ID người dùng từ Token để truy xuất đúng giỏ hàng của họ.
      
- Task 4: Order & Checkout (Đặt hàng)
      
  - Task 4.1: Checkout API
      
    Endpoint: POST /api/v1/orders/checkout
  
    Logic yêu cầu (phải nằm trong 1 Transaction):
  
    Kiểm tra tồn kho lần cuối cho tất cả item trong giỏ.
  
    Trừ số lượng tồn kho (Locking).
  
    Tạo bản ghi Order và các OrderItem.
  
    Xóa giỏ hàng sau khi đặt hàng thành công.
 
  - Task 4.2: Order History API

    Endpoint: GET /api/v1/orders
    
    Yêu cầu: Trả về danh sách đơn hàng của User đang đăng nhập. 
    
    NOTE: cài swagger để xem danh sách api