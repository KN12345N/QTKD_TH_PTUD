# PHÂN HỆ QUẢN LÝ KINH DOANH (QTKD) – PHASE 2 (TUẦN 05)

## 1. Giới thiệu

Dự án xây dựng hệ thống Web API quản lý thông tin khách hàng, hợp đồng và bảng giá dịch vụ thuộc phân hệ Quản trị kinh doanh (QTKD).

Trong phạm vi Tuần 05, dự án tập trung xây dựng nền tảng ban đầu, kết nối cơ sở dữ liệu PostgreSQL, sử dụng Entity Framework Core theo phương pháp Code-First, tạo các bảng dữ liệu và xây dựng API đọc thông tin khách hàng, hợp đồng.

## 2. Công nghệ sử dụng

- **Ngôn ngữ:** C#
- **Framework:** .NET 8 / ASP.NET Core Web API
- **ORM:** Entity Framework Core 8
- **Cơ sở dữ liệu:** PostgreSQL
- **PostgreSQL Provider:** Npgsql.EntityFrameworkCore.PostgreSQL
- **Tài liệu và kiểm thử API:** Swagger UI / Swashbuckle
- **Quản lý mã nguồn:** Git

## 3. Cấu trúc thư mục dự án

```text
QTKD/
├── src/
│   └── QTKD.Api/
│       ├── Controllers/
│       │   ├── CustomersController.cs
│       │   └── ContractsController.cs
│       ├── Data/
│       │   └── QtkdDbContext.cs
│       ├── Entities/
│       │   ├── Customer.cs
│       │   ├── Contract.cs
│       │   ├── PriceList.cs
│       │   └── PriceItem.cs
│       ├── Migrations/
│       ├── appsettings.json
│       ├── Program.cs
│       └── QTKD.Api.csproj
├── tests/
├── docs/
└── README.md
```

Các thành phần chính:

- `Controllers/`: Xử lý các yêu cầu HTTP của API khách hàng và hợp đồng.
- `Data/`: Chứa `QtkdDbContext`, cấu hình truy cập và quan hệ dữ liệu.
- `Entities/`: Khai báo các thực thể Customer, Contract, PriceList và PriceItem.
- `Migrations/`: Lưu các migration được tạo bởi Entity Framework Core.
- `appsettings.json`: Chứa cấu hình ứng dụng và chuỗi kết nối mặc định.
- `Program.cs`: Cấu hình dịch vụ, DbContext và Swagger UI.

## 4. Yêu cầu môi trường

Trước khi chạy dự án, cần cài đặt:

- .NET 8 SDK.
- PostgreSQL.
- Git.
- Visual Studio Code hoặc Visual Studio.
- Công cụ dòng lệnh `dotnet-ef`.

Kiểm tra phiên bản .NET:

```bash
dotnet --version
```

Nếu chưa cài đặt `dotnet-ef`, chạy lệnh:

```bash
dotnet tool install --global dotnet-ef
```

Nếu công cụ đã được cài đặt, có thể cập nhật bằng:

```bash
dotnet tool update --global dotnet-ef
```

## 5. Hướng dẫn cấu hình và khởi chạy

### Bước 1. Lấy mã nguồn

Clone repository về máy:

```bash
git clone <URL_REPOSITORY>
cd QTKD
```

Thay `<URL_REPOSITORY>` bằng địa chỉ repository Git thực tế.

### Bước 2. Cấu hình PostgreSQL

Khởi động PostgreSQL và kiểm tra dịch vụ đang chạy trên cổng mặc định `5432`.

Thông tin cơ sở dữ liệu sử dụng trong môi trường phát triển:

- Host: `localhost`
- Port: `5432`
- Database: `qtkd_db`
- Username: `postgres`

Mở terminal tại thư mục project:

```bash
cd src/QTKD.Api
```

Khởi tạo User Secrets nếu project chưa được cấu hình:

```bash
dotnet user-secrets init
```

Thiết lập chuỗi kết nối bằng mật khẩu PostgreSQL trên máy cá nhân:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=qtkd_db;Username=postgres;Password=MAT_KHAU_POSTGRES"
```

Thay `MAT_KHAU_POSTGRES` bằng mật khẩu thực tế của tài khoản PostgreSQL.

**Lưu ý:** Không lưu mật khẩu trực tiếp trong mã nguồn hoặc commit thông tin xác thực lên Git. Ứng dụng cần đọc chuỗi kết nối bằng khóa `ConnectionStrings:DefaultConnection`.

### Bước 3. Khôi phục package

Tại thư mục `src/QTKD.Api`, chạy:

```bash
dotnet restore
```

Project cần có các package phù hợp, bao gồm:

- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.EntityFrameworkCore.Design`
- Các package Swagger/Swashbuckle theo cấu hình hiện tại của project.

### Bước 4. Tạo hoặc cập nhật cơ sở dữ liệu

Nếu project đã có migration, chạy:

```bash
dotnet ef database update
```

Entity Framework Core sẽ áp dụng các migration hiện có để tạo hoặc cập nhật cơ sở dữ liệu và các bảng tương ứng.

Nếu project chưa có migration nào, tạo migration đầu tiên:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Chỉ tạo `InitialCreate` khi chưa có migration ban đầu trong project.

### Bước 5. Khởi động ứng dụng

Tại thư mục `src/QTKD.Api`, chạy:

```bash
dotnet run
```

Theo dõi Terminal để xác định địa chỉ ứng dụng, ví dụ:

```text
http://localhost:5284
```

Cổng thực tế phụ thuộc vào cấu hình của project.

## 6. Các chức năng trong phạm vi Tuần 05

### 6.1. Quản lý dữ liệu khách hàng

Xây dựng entity `Customer` và API lấy danh sách khách hàng từ PostgreSQL.

**Endpoint:**

```http
GET /api/Customers
```

Kết quả mong đợi:

- HTTP `200 OK` khi truy vấn thành công.
- Trả về danh sách khách hàng được lưu trong cơ sở dữ liệu.
- Có ít nhất hai bản ghi khách hàng mẫu sau khi dữ liệu Seed Data được nạp thành công.

### 6.2. Tra cứu thông tin hợp đồng

Xây dựng entity `Contract` và API lấy chi tiết hợp đồng theo ID, có liên kết với khách hàng.

**Endpoint:**

```http
GET /api/Contracts/{id}
```

**Trường hợp hợp đồng tồn tại:**

Gửi yêu cầu với `id = 1`.

Kết quả mong đợi:

- HTTP `200 OK`.
- Trả về thông tin hợp đồng và thông tin khách hàng liên quan.
- Nếu dữ liệu mẫu đã được cấu hình đúng, hợp đồng có mã `HD-2026-001` và trạng thái `Draft`.

**Trường hợp hợp đồng không tồn tại:**

Gửi yêu cầu với `id = 999`.

Kết quả mong đợi:

- HTTP `404 Not Found`.
- API trả về thông báo phù hợp nếu không tìm thấy hợp đồng.

Các kết quả trên là tiêu chí kiểm tra; cần chạy ứng dụng để xác nhận kết quả thực tế.

### 6.3. Khởi tạo dữ liệu bảng giá

Xây dựng các entity `PriceList` và `PriceItem`, cấu hình quan hệ một-nhiều giữa bảng giá và các dịch vụ thuộc bảng giá.

Yêu cầu dữ liệu:

- Có ít nhất một bản ghi trong `PriceLists`.
- Có hai dịch vụ mẫu trong `PriceItems`.
- Các bản ghi `PriceItems` liên kết đúng với bảng giá thông qua `PriceListId`.

Trong phạm vi Tuần 05, nội dung này tập trung vào mô hình dữ liệu, quan hệ và dữ liệu mẫu; chưa yêu cầu xây dựng đầy đủ các API quản lý bảng giá.

## 7. Hướng dẫn kiểm thử API bằng Swagger UI

Sau khi chạy ứng dụng, mở trình duyệt và truy cập địa chỉ được hiển thị trong Terminal, ví dụ:

```text
http://localhost:5284/
```

Thực hiện kiểm tra hai endpoint:

**API 1: `GET /api/Customers`**

1. Mở endpoint trong Swagger UI.
2. Chọn **Try it out**.
3. Chọn **Execute**.
4. Kiểm tra HTTP status và danh sách khách hàng trả về.

**API 2: `GET /api/Contracts/{id}`**

1. Mở endpoint trong Swagger UI.
2. Chọn **Try it out**.
3. Nhập `1` vào trường `id`, sau đó chọn **Execute**.
4. Kiểm tra thông tin hợp đồng và khách hàng liên quan.
5. Nhập `999` để kiểm tra trường hợp không tìm thấy hợp đồng.

## 8. Đối chiếu dữ liệu trong PostgreSQL

Sử dụng pgAdmin hoặc DBeaver để kết nối vào cơ sở dữ liệu `qtkd_db`.

Kiểm tra các bảng:

- **Customers:** Có ít nhất hai khách hàng mẫu.
- **Contracts:** Có ít nhất một hợp đồng ở trạng thái `Draft`, liên kết với khách hàng thông qua `CustomerId`.
- **PriceLists:** Có ít nhất một bảng giá mẫu.
- **PriceItems:** Có hai dịch vụ mẫu liên kết với bảng giá thông qua `PriceListId`.

Đối chiếu dữ liệu trả về từ API với dữ liệu thực tế trong PostgreSQL để xác nhận ứng dụng đang đọc dữ liệu từ cơ sở dữ liệu.

## 9. Kết quả cần đạt cuối Tuần 05

- [ ] Ứng dụng ASP.NET Core Web API chạy được trên .NET 8.
- [ ] Kết nối thành công với PostgreSQL.
- [ ] Có các entity Customer, Contract, PriceList và PriceItem.
- [ ] Có QtkdDbContext và các quan hệ dữ liệu cần thiết.
- [ ] Có migration tạo các bảng trong PostgreSQL.
- [ ] Có ít nhất hai khách hàng mẫu.
- [ ] Có ít nhất một hợp đồng mẫu ở trạng thái Draft.
- [ ] Có một bảng giá chứa hai dịch vụ mẫu.
- [ ] API `GET /api/Customers` hoạt động.
- [ ] API `GET /api/Contracts/{id}` hoạt động và xử lý trường hợp không tìm thấy hợp đồng.
- [ ] Đã thử nghiệm API bằng Swagger UI.
- [ ] Đã đối chiếu dữ liệu API với PostgreSQL.
- [ ] Đã cập nhật README hướng dẫn cấu hình và khởi chạy dự án.

## 10. Phạm vi thực hiện

README này chỉ mô tả các chức năng thuộc **Phase 2 – Tuần 05**: xây dựng nền tảng Web API, kết nối PostgreSQL, tạo migration, nạp dữ liệu mẫu và đọc dữ liệu khách hàng, hợp đồng.

Các chức năng tạo, cập nhật dữ liệu, xử lý nghiệp vụ gửi hợp đồng trình duyệt và kiểm thử tự động sẽ được cập nhật vào tài liệu khi triển khai các tuần tiếp theo.