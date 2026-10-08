# ShoeStore

ShoeStore là ứng dụng bán giày được xây dựng trong một đồ án sinh viên làm theo nhóm. Dự án gồm giao diện mua hàng cho khách hàng và giao diện quản trị, kết nối với backend ASP.NET Core.

## Chức năng

- Quản lý sản phẩm và các biến thể theo màu sắc, kích cỡ, chất liệu, thương hiệu.
- Tìm kiếm sản phẩm, quản lý giỏ hàng và địa chỉ giao hàng.
- Quản lý đơn hàng, hóa đơn và yêu cầu trả hàng.
- Quản lý khách hàng, nhân viên, voucher và chương trình giảm giá.
- Thống kê hoạt động bán hàng.
- Tích hợp thanh toán payOS, vận chuyển GHN và gửi email.

## Công nghệ

| Thành phần | Công nghệ |
| --- | --- |
| Backend | C#, ASP.NET Core Web API (.NET 6) |
| Dữ liệu | Entity Framework Core, SQL Server |
| Frontend | HTML, CSS, JavaScript, AngularJS, Bootstrap |
| Tài liệu API | Swagger / OpenAPI |
| Dịch vụ tích hợp | payOS, GHN, SMTP |

## Cấu trúc dự án

```text
backend/
  ShoeStore.sln
  AppAPI/              # API và tích hợp dịch vụ
  AppData/             # Model, truy cập dữ liệu và nghiệp vụ
frontend/
  TemplateCustomer/    # Giao diện khách hàng
  TemplateAdmin/       # Giao diện quản trị
tests/
  GhnConfigurationChecks/
```

Các trường cấu hình được cung cấp trong `backend/AppAPI/appsettings.Local.example.json`. Giá trị thực được cấu hình riêng cho từng môi trường và không đưa vào repository.

## Trạng thái

Đây là dự án học tập đang được bảo trì và hoàn thiện. Danh sách chức năng mô tả phạm vi mã nguồn hiện có; repository không kèm database hoặc thông tin truy cập các dịch vụ tích hợp.
