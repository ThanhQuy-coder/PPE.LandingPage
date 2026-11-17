# Gia Hoàng – Website Giới Thiệu Sản Phẩm Bảo Hộ Lao Động

Website giới thiệu doanh nghiệp và danh mục sản phẩm cho **Công ty TNHH SX TM Bảo hộ lao động Gia Hoàng**, xây dựng bằng **Blazor WebAssembly Standalone (.NET 10)** theo kiến trúc tĩnh, tối ưu hiệu năng và sẵn sàng triển khai lên **Cloudflare Pages**.

---

## Tổng quan tính năng

- **Landing Page (`/`)**:
  - **Hero Section**: Giới thiệu thương hiệu Gia Hoàng, CTA điều hướng danh mục và gọi nhanh hotline `0901290023`, hiển thị trực quan sản phẩm thực tế và số liệu catalog từ JSON.
  - **Giới thiệu doanh nghiệp**: Trình bày lĩnh vực sản xuất và phân phối quần áo bảo hộ lao động, đồng phục doanh nghiệp và thiết bị bảo vệ cá nhân (PPE).
  - **Danh mục sản phẩm**: Tự động tổng hợp **18 nhóm sản phẩm** từ dữ liệu JSON, hỗ trợ mở rộng xem toàn bộ danh mục và điều hướng lọc nhanh.
  - **Sản phẩm tiêu biểu**: Hiển thị 8 sản phẩm đại diện đa dạng danh mục ngay trên trang chủ.
  - **Liên hệ**: Thông tin doanh nghiệp, hotline `0901290023` và biểu mẫu gửi nhu cầu tư vấn.
- **Trang danh sách sản phẩm (`/san-pham`)**:
  - Tải toàn bộ **219 sản phẩm** từ `wwwroot/data/normalized_products.json` và cache trong phiên làm việc.
  - Tìm kiếm theo tên sản phẩm hoặc tên danh mục.
  - Lọc theo danh mục qua dropdown hoặc thanh chip, đồng bộ query string (`?category=...&q=...`).
  - Phân trang (16 sản phẩm/trang) giúp DOM gọn nhẹ và phản hồi nhanh.
  - Đầy đủ trạng thái tải dữ liệu (loading), lỗi kèm nút thử lại, và không tìm thấy kết quả.
- **Trang chi tiết sản phẩm (`/san-pham/{Id}`)**:
  - Hiển thị hình ảnh sản phẩm (kèm ảnh dự phòng `placeholder-product.svg` khi có lỗi tải).
  - Tự động lọc và chỉ hiển thị các thông số kỹ thuật đã xác nhận (ẩn các trường trống hoặc giá trị `"Đang cập nhật"`).
  - Hiển thị tối đa 4 sản phẩm liên quan cùng danh mục và nút gọi tư vấn trực tiếp.

---

## Công nghệ sử dụng

| Thành phần | Chi tiết |
| --- | --- |
| Framework | .NET 10 – Blazor WebAssembly Standalone (`Microsoft.AspNetCore.Components.WebAssembly` 10.0.12) |
| Giao diện | Razor Components, HTML5, CSS3 (CSS Design Tokens), Bootstrap 5 |
| Ngôn ngữ | C# |
| Dữ liệu | JSON tĩnh (`wwwroot/data/normalized_products.json`) |
| Kiểu chữ | `Be Vietnam Pro` (Google Fonts) |
| Mục tiêu triển khai | Cloudflare Pages (Static Hosting) |

---

## Cấu trúc thư mục

```text
PPE.LandingPage/
├── Components/
│   ├── Layout/
│   │   ├── SiteHeader.razor          # Header dính đầu trang, topbar hotline, menu desktop/mobile
│   │   └── SiteFooter.razor          # Footer thông tin Gia Hoàng & điều hướng
│   ├── Sections/
│   │   ├── HeroSection.razor         # Hero giới thiệu & showcase sản phẩm
│   │   ├── AboutSection.razor        # Giới thiệu năng lực & lĩnh vực hoạt động
│   │   ├── CategorySection.razor     # Lưới danh mục sản phẩm từ JSON
│   │   ├── FeaturedProductsSection.razor # Sản phẩm tiêu biểu trên trang chủ
│   │   └── ContactSection.razor      # Thông tin hotline & biểu mẫu liên hệ
│   └── Shared/
│       └── ProductCard.razor         # Card sản phẩm dùng chung (lazy loading ảnh)
├── Layout/
│   └── MainLayout.razor              # Layout chính dùng SiteHeader và SiteFooter
├── Models/
│   └── ProductModels.cs              # Model dữ liệu JSON, chuẩn hóa đường dẫn ảnh, BrandInfo
├── Pages/
│   ├── Home.razor                    # Trang chủ (/)
│   ├── Products.razor                # Danh sách sản phẩm (/san-pham)
│   ├── ProductDetail.razor           # Chi tiết sản phẩm (/san-pham/{Id})
│   └── NotFound.razor                # Trang 404 (/not-found)
├── Services/
│   └── ProductCatalogService.cs      # Đọc, cache, lọc và phân nhóm dữ liệu JSON
├── wwwroot/
│   ├── css/app.css                   # Hệ thống CSS variables & style responsive
│   ├── data/normalized_products.json # Dữ liệu chính thức 219 sản phẩm
│   ├── images/                       # Hình ảnh sản phẩm và placeholder-product.svg
│   └── index.html                    # Shell HTML, SEO meta tags & boot loader
├── Program.cs                        # Đăng ký DI & khởi tạo WebAssemblyHost
└── PPE.LandingPage.csproj            # Cấu hình dự án .NET 10
```

---

## Yêu cầu hệ thống

- [.NET 10 SDK](https://dotnet.microsoft.com/download) trở lên.

---

## Hướng dẫn chạy cục bộ (Local Development)

1. Di chuyển vào thư mục dự án:

```bash
cd PPE.LandingPage
```

2. Khôi phục gói phụ thuộc và biên dịch:

```bash
dotnet restore
dotnet build
```

3. Chạy máy chủ phát triển:

```bash
dotnet run --urls http://localhost:5013
```

4. Mở trình duyệt tại:
   - Trang chủ: `http://localhost:5013/`
   - Danh sách sản phẩm: `http://localhost:5013/san-pham`
   - Chi tiết sản phẩm: `http://localhost:5013/san-pham/product-0001`

---

## Build & Triển khai lên Cloudflare Pages

Dự án không sử dụng backend hay cơ sở dữ liệu, toàn bộ đầu ra là các tệp tĩnh sẵn sàng cho Cloudflare Pages.

### Build bản phát hành (Release)

```bash
dotnet publish PPE.LandingPage.csproj -c Release -o dist
```

Thư mục tài nguyên tĩnh sau khi publish nằm tại `dist/wwwroot`.

### Cấu hình trên Cloudflare Pages

- **Build command**: `dotnet publish PPE.LandingPage.csproj -c Release -o dist`
- **Build output directory**: `dist/wwwroot`

> **Lưu ý về định tuyến SPA**: Với các đường dẫn con như `/san-pham` hoặc `/san-pham/product-0001`, Cloudflare Pages tự động phục vụ fallback về `index.html` nếu không có tệp `404.html` riêng (hoặc có thể sao chép `index.html` thành `404.html` trong bước build nếu cần hỗ trợ điều hướng trực tiếp từ URL).

---

## Quy ước dữ liệu & bảo trì

- Toàn bộ dữ liệu sản phẩm và thông số kỹ thuật được đọc từ `wwwroot/data/normalized_products.json`. Không hardcode sản phẩm, giá bán hoặc chứng nhận chưa có trong dữ liệu gốc.
- Đường dẫn ảnh dạng `../images/...` trong JSON được tự động chuẩn hóa về `images/...` qua `ProductItem.NormalizeImagePath` để hoạt động ổn định từ root khi triển khai.
- Thông tin thương hiệu và hotline tập trung tại `BrandInfo` trong `Models/ProductModels.cs` (`0901290023`).
- Màu sắc, khoảng cách, bo góc và bóng đổ được quản lý tập trung bằng CSS variables (`--gh-*`) trong `wwwroot/css/app.css`.
