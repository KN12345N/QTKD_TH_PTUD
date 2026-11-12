# PHÂN HỆ QUẢN LÝ KINH DOANH (QTKD) - PHASE 2

## 1. Giới thiệu

Dự án xây dựng hệ thống **Quản lý kinh doanh (QTKD)** dưới dạng ASP.NET Core Web API, phục vụ quản lý thông tin khách hàng, hợp đồng và bảng giá dịch vụ.
Dự án được phát triển theo từng giai đoạn thực hành, sử dụng Entity Framework Core theo phương pháp Code-First và PostgreSQL làm hệ quản trị cơ sở dữ liệu.

Tính đến hiện tại, dự án đã triển khai các nội dung chính:
- Xây dựng cấu trúc Web API và kết nối PostgreSQL.
- Xây dựng các thực thể và quan hệ dữ liệu.
- Xây dựng API quản lý khách hàng và hợp đồng.
- Triển khai các quy tắc nghiệp vụ liên quan đến hợp đồng.
- Bổ sung thông tin mô tả và tệp đính kèm cho hợp đồng.
- Xây dựng Unit Tests và Integration Tests bằng xUnit.
- Sử dụng database InMemory riêng trong quá trình kiểm thử để tránh ảnh hưởng đến dữ liệu PostgreSQL.

## 2. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ lập trình | C# |
| Framework | .NET 10 |
| Web API | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 |
| Cơ sở dữ liệu | PostgreSQL 18 |
| PostgreSQL Provider | Npgsql.EntityFrameworkCore.PostgreSQL |
| Kiểm thử tự động | xUnit |
| Integration Testing | Microsoft.AspNetCore.Mvc.Testing |
| Database kiểm thử | EF Core InMemory |
| API Documentation | Swagger / Swashbuckle |
| Quản lý mã nguồn | Git |
| IDE | Visual Studio Code |

## 3. Cấu trúc thư mục dự án

```text
QTKD/
├── src/
│   ├── QTKD.Api/
│   │   ├── Controllers/
│   │   │   ├── CustomersController.cs
│   │   │   ├── ContractsController.cs
│   │   │   ├── PriceListsController.cs
│   │   │   └── PriceItemsController.cs
│   │   │
│   │   ├── Data/
│   │   │   └── QtkdDbContext.cs
│   │   │
│   │   ├── Entities/
│   │   │   ├── Customer.cs
│   │   │   ├── Contract.cs
│   │   │   ├── PriceList.cs
│   │   │   └── PriceItem.cs
│   │   │
│   │   ├── Migrations/
│   │   ├── Properties/
│   │   ├── appsettings.json
│   │   ├── Program.cs
│   │   └── QTKD.Api.csproj
│   │
│   └── tests/
│       └── QTKD.Tests/
│           ├── UnitTests/
│           │   ├── CustomerTests.cs
│           │   └── ContractTests.cs
│           │
│           ├── IntegrationTests/
│           │   ├── CustomWebApplicationFactory.cs
│           │   ├── CustomerIntegrationTests.cs
│           │   └── ContractIntegrationTests.cs
│           │
│           └── QTKD.Tests.csproj
│
├── QTKD.slnx
├── .gitignore
└── README.md
```

### Chức năng các thành phần

- **Controllers/**: Tiếp nhận và xử lý các yêu cầu HTTP liên quan đến khách hàng, hợp đồng và bảng giá.
- **Data/**: Chứa `QtkdDbContext`, quản lý kết nối và cấu hình quan hệ giữa các thực thể.
- **Entities/**: Khai báo các thực thể Customer, Contract, PriceList và PriceItem.
- **Migrations/**: Lưu lịch sử thay đổi cấu trúc cơ sở dữ liệu thông qua Entity Framework Core.
- **UnitTests/**: Chứa các kiểm thử đơn vị cho những quy tắc nghiệp vụ.
- **IntegrationTests/**: Chứa các kiểm thử tích hợp thông qua HTTP API.
- **CustomWebApplicationFactory.cs**: Cấu hình môi trường chạy API dành cho Integration Tests.
- **README.md**: Tài liệu hướng dẫn cấu hình, khởi chạy và kiểm thử dự án.

## 4. Yêu cầu môi trường

Trước khi chạy dự án, cần chuẩn bị:
- .NET 10 SDK.
- PostgreSQL 18.
- Visual Studio Code hoặc Visual Studio.
- Git.
- Công cụ dòng lệnh `dotnet-ef`.

Kiểm tra phiên bản .NET:

```bash
dotnet --version
```

Kiểm tra PostgreSQL:

```bash
psql --version
```

Nếu chưa cài đặt công cụ Entity Framework Core:

```bash
dotnet tool install --global dotnet-ef
```

Nếu đã cài đặt:

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

Thay `<URL_REPOSITORY>` bằng địa chỉ repository thực tế.

### Bước 2. Cấu hình PostgreSQL

Khởi động PostgreSQL và đảm bảo dịch vụ đang hoạt động.
Thông tin kết nối môi trường phát triển:

| Thông tin | Giá trị |
|---|---|
| Host | localhost |
| Port | 5432 |
| Database | qtkd_db |
| Username | secret-user |

Mở terminal tại thư mục API:

```bash
cd src/QTKD.Api
```

Khởi tạo User Secrets nếu project chưa được cấu hình:

```bash
dotnet user-secrets init
```

Thiết lập chuỗi kết nối:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=qtkd_db;Username=secret-user;Password=MAT_KHAU_POSTGRES"
```

Thay `MAT_KHAU_POSTGRES` bằng mật khẩu thực tế của tài khoản PostgreSQL.

**Lưu ý:** Không lưu mật khẩu trực tiếp trong mã nguồn hoặc commit thông tin xác thực lên Git. Ứng dụng sử dụng `ConnectionStrings:DefaultConnection` để lấy chuỗi kết nối.

### Bước 3. Khôi phục package

Tại thư mục `src/QTKD.Api`, chạy:

```bash
dotnet restore
```

Sau đó build project:

```bash
dotnet build
```

Đối với project kiểm thử:

```bash
cd ../tests/QTKD.Tests
dotnet restore
dotnet build
```

### Bước 4. Cập nhật cơ sở dữ liệu

Quay lại thư mục API:

```bash
cd ../../QTKD.Api
```

Áp dụng các migration hiện có:

```bash
dotnet ef database update
```

Không tạo lại migration ban đầu nếu project đã có migration.

### Bước 5. Khởi chạy API

Tại thư mục `src/QTKD.Api`, chạy:

```bash
dotnet run
```

Mở địa chỉ ứng dụng được hiển thị trong Terminal để truy cập Swagger UI.
Project hiện cấu hình Swagger UI tại đường dẫn gốc của ứng dụng.

## 6. Các chức năng đã triển khai

### 6.1. Quản lý khách hàng (Customer)

Thực thể `Customer` lưu trữ thông tin khách hàng, bao gồm:

- Id: Mã khách hàng.
- Name: Tên khách hàng.
- Status: Trạng thái khách hàng.

Các API đã triển khai:

| Phương thức | Endpoint | Chức năng |
|---|---|---|
| GET | `/api/customers` | Lấy danh sách khách hàng |
| GET | `/api/customers/{id}` | Lấy thông tin khách hàng theo ID |
| POST | `/api/customers` | Tạo khách hàng mới |
| PUT | `/api/customers/{id}` | Cập nhật thông tin khách hàng |

Một số quy tắc kiểm tra:

- Tên khách hàng không được để trống.
- Trạng thái khách hàng không được để trống.
- Dữ liệu tên và trạng thái được loại bỏ khoảng trắng thừa trước khi lưu.
- Trả về `404 Not Found` nếu không tìm thấy khách hàng.

### 6.2. Quản lý hợp đồng (Contract)

Thực thể `Contract` lưu trữ thông tin hợp đồng và liên kết với khách hàng thông qua `CustomerId`.

Các thuộc tính chính:

- Id: Mã hợp đồng.
- ContractNo: Số hợp đồng.
- CustomerId: Mã khách hàng.
- StartDate: Ngày bắt đầu.
- EndDate: Ngày kết thúc.
- Status: Trạng thái hợp đồng.
- Description: Nội dung mô tả hợp đồng.
- AttachmentName: Tên tệp đính kèm.

Các API đã triển khai:

| Phương thức | Endpoint | Chức năng |
|---|---|---|
| GET | `/api/contracts` | Lấy danh sách hợp đồng |
| GET | `/api/contracts/{id}` | Lấy chi tiết hợp đồng |
| POST | `/api/contracts` | Tạo hợp đồng mới |
| PUT | `/api/contracts/{id}` | Cập nhật hợp đồng |
| POST | `/api/contracts/{id}/submit` | Gửi hợp đồng trình duyệt |

Các quy tắc nghiệp vụ chính:

**CTR-01 – Điều kiện cập nhật hợp đồng**

- Chỉ cho phép cập nhật hợp đồng ở trạng thái `Draft` hoặc `Revision Requested`.
- Không cho phép cập nhật hợp đồng ở các trạng thái khác.
- Không cho phép client tự ý thay đổi trạng thái hợp đồng thông qua API cập nhật.

**CTR-02 – Điều kiện thông tin hợp đồng**

- Số hợp đồng không được để trống.
- Ngày kết thúc phải sau ngày bắt đầu.
- Khách hàng liên kết phải tồn tại và đang hoạt động.
- Nội dung mô tả hợp đồng không được để trống khi Submit.
- Tên tệp đính kèm không được để trống khi Submit.

**CTR-03 – Điều kiện gửi hợp đồng trình duyệt**

- Chỉ cho phép Submit hợp đồng đang ở trạng thái `Draft`.
- Hợp đồng phải đáp ứng các điều kiện nghiệp vụ.
- Khi Submit thành công, trạng thái được chuyển từ `Draft` sang `PendingApproval`.
- Không cho phép Submit lại hợp đồng đã được gửi.

### 6.3. Quản lý bảng giá (PriceList và PriceItem)

Dự án xây dựng hai thực thể:

- `PriceList`: Lưu thông tin bảng giá dịch vụ.
- `PriceItem`: Lưu thông tin từng dịch vụ và đơn giá tương ứng.

Quan hệ dữ liệu:

- Một PriceList có thể chứa nhiều PriceItem.
- Mỗi PriceItem thuộc về một PriceList thông qua `PriceListId`.

Các thành phần này đã được xây dựng trong mô hình dữ liệu và có các Controller tương ứng.

## 7. Kiểm thử tự động

Dự án sử dụng xUnit để xây dựng bộ kiểm thử tự động, chia thành hai nhóm riêng biệt.

### 7.1. Unit Tests

Các Unit Tests gọi trực tiếp Controller và sử dụng EF Core InMemory để kiểm tra các điều kiện nghiệp vụ.

File `CustomerTests.cs`:
- Tạo Customer với dữ liệu hợp lệ.
- Từ chối tạo Customer khi thiếu tên.
- Từ chối tạo Customer khi thiếu trạng thái.

File `ContractTests.cs`:
- Cập nhật hợp đồng Draft thành công.
- Từ chối cập nhật hợp đồng PendingApproval.
- Từ chối Submit khi thiếu nội dung mô tả.
- Submit hợp đồng hợp lệ.
- Từ chối Submit hợp đồng lần thứ hai.
- Từ chối yêu cầu chuyển Draft trực tiếp sang Approved.

Kết quả: **9 Unit Tests thành công.**

### 7.2. Integration Tests

Integration Tests sử dụng `Microsoft.AspNetCore.Mvc.Testing` và `WebApplicationFactory` để khởi chạy API trong môi trường kiểm thử.

File `CustomWebApplicationFactory.cs` cấu hình thay thế PostgreSQL bằng EF Core InMemory, giúp các kiểm thử không tác động đến cơ sở dữ liệu PostgreSQL đang sử dụng.

File `CustomerIntegrationTests.cs`:
- Gửi HTTP POST tạo Customer hợp lệ.
- Kiểm tra HTTP `201 Created`.
- Kiểm tra nội dung phản hồi.
- Truy vấn lại dữ liệu từ database kiểm thử.
- Kiểm tra trường hợp tạo Customer thiếu tên và nhận HTTP `400 Bad Request`.

File `ContractIntegrationTests.cs`:
- Tạo Customer thông qua HTTP API.
- Tạo Contract thông qua HTTP API.
- Submit Contract.
- Đọc lại hợp đồng và xác nhận trạng thái `PendingApproval`.
- Thử Submit hợp đồng hai lần và xác nhận lần thứ hai bị từ chối.

Kết quả: **4 Integration Tests thành công.**

### 7.3. Tổng hợp kết quả kiểm thử

| Loại kiểm thử | Số lượng | Kết quả |
|---|---:|---|
| Unit Tests | 9 | Passed |
| Integration Tests | 4 | Passed |
| Tổng cộng | 13 | Passed |

Tình trạng kiểm thử các tình huống TC01–TC08:

| Test Case | Nội dung | Kết quả |
|---|---|---|
| TC01 | Tạo Customer hợp lệ | Pass |
| TC02 | Tạo Customer thiếu thông tin bắt buộc | Pass |
| TC03 | Cập nhật Contract Draft | Pass |
| TC04 | Từ chối cập nhật Contract PendingApproval | Pass |
| TC05 | Submit Contract thiếu điều kiện CTR-02 | Pass |
| TC06 | Submit Contract hợp lệ | Pass |
| TC07 | Từ chối chuyển Draft trực tiếp sang Approved | Pass |
| TC08 | Từ chối Submit hợp đồng hai lần | Pass |

Các test case được kiểm tra thông qua các Unit Tests và Integration Tests tương ứng. Không phải mỗi test case đều có một Integration Test riêng.

### 7.4. Hướng dẫn chạy kiểm thử

Mở Terminal tại thư mục `src/tests/QTKD.Tests`:

Chạy toàn bộ kiểm thử:

```bash
dotnet test
```

Xuất báo cáo kiểm thử định dạng TRX:

```bash
dotnet test --logger "trx;LogFileName=Week07_TestResults.trx"
```

Sau khi chạy, báo cáo được lưu trong thư mục:

```text
QTKD.Tests/
└── TestResults/
    └── Week07_TestResults.trx
```

**Lưu ý về môi trường kiểm thử:**

- Unit Tests sử dụng InMemory để kiểm tra logic xử lý của Controller.
- Integration Tests chạy qua HTTP API thực tế trong môi trường kiểm thử nhưng sử dụng database InMemory.
- Các kiểm thử hiện tại không trực tiếp xác nhận khả năng tương thích của toàn bộ truy vấn và hành vi giao dịch trên PostgreSQL.
- Khi chạy ứng dụng bình thường, API vẫn sử dụng PostgreSQL theo cấu hình `DefaultConnection`.

## 8. Kiểm thử API bằng Swagger UI

Sau khi khởi chạy ứng dụng bằng:

```bash
dotnet run
```

Truy cập địa chỉ Swagger UI được cấu hình trong `Program.cs`.

Một số API có thể kiểm tra:

**Customer:**

```http
GET /api/customers
GET /api/customers/1
POST /api/customers
PUT /api/customers/1
```

**Contract:**

```http
GET /api/contracts
GET /api/contracts/1
POST /api/contracts
PUT /api/contracts/1
POST /api/contracts/1/submit
```

Khi kiểm thử, cần kiểm tra:
- HTTP status code.
- Nội dung JSON trả về.
- Thông báo lỗi khi dữ liệu không hợp lệ.
- Trạng thái hợp đồng trước và sau khi Submit.
- Dữ liệu thực tế được lưu trong PostgreSQL khi chạy ứng dụng bình thường.

## 9. Đối chiếu dữ liệu PostgreSQL

Có thể sử dụng pgAdmin hoặc DBeaver để kết nối với PostgreSQL.

Kiểm tra các bảng chính:
- `Customers`: Thông tin khách hàng.
- `Contracts`: Thông tin hợp đồng và trạng thái xử lý.
- `PriceLists`: Thông tin bảng giá.
- `PriceItems`: Các dịch vụ và đơn giá thuộc bảng giá.

Đối chiếu dữ liệu giữa PostgreSQL và kết quả API để xác nhận dữ liệu được lưu trữ và truy xuất chính xác.

Lưu ý: Database InMemory chỉ phục vụ kiểm thử tự động, không phải database PostgreSQL đang sử dụng cho ứng dụng.

## 10. Tình trạng hiện tại và hướng phát triển

### 10.1. Các nội dung đã hoàn thành

- [x] Xây dựng ASP.NET Core Web API.
- [x] Kết nối PostgreSQL.
- [x] Xây dựng các Entity Customer, Contract, PriceList và PriceItem.
- [x] Cấu hình QtkdDbContext và các quan hệ dữ liệu.
- [x] Quản lý migration bằng Entity Framework Core.
- [x] Xây dựng API đọc và quản lý khách hàng.
- [x] Xây dựng API đọc và quản lý hợp đồng.
- [x] Triển khai các quy tắc CTR-01, CTR-02 và CTR-03.
- [x] Bổ sung Description và AttachmentName cho hợp đồng.
- [x] Xây dựng Unit Tests bằng xUnit.
- [x] Xây dựng Integration Tests bằng WebApplicationFactory.
- [x] Tách riêng thư mục UnitTests và IntegrationTests.
- [x] Cấu hình database InMemory dành cho kiểm thử.
- [x] Kiểm thử các tình huống TC01–TC08.
- [x] Chạy thành công 13 kiểm thử tự động.

### 10.2. Hướng phát triển tiếp theo

- Bổ sung thêm các trường hợp kiểm thử cho những tình huống nghiệp vụ chưa được bao phủ.
- Bổ sung kiểm thử tích hợp sử dụng PostgreSQL trong môi trường kiểm thử riêng.
- Hoàn thiện quy trình trình duyệt và phê duyệt hợp đồng.
- Bổ sung xử lý lịch sử trạng thái và kết quả phê duyệt.
- Tiếp tục hoàn thiện chức năng quản lý bảng giá và dịch vụ.
- Bổ sung logging, xử lý ngoại lệ và cải thiện khả năng bảo trì mã nguồn.

## 11. Kết luận

Dự án QTKD Phase 2 hiện đã có nền tảng Web API kết nối PostgreSQL, mô hình dữ liệu quản lý khách hàng, hợp đồng và bảng giá dịch vụ.
Các chức năng quản lý khách hàng, hợp đồng và những quy tắc nghiệp vụ chính đã được triển khai. Bên cạnh đó, dự án đã có bộ kiểm thử tự động gồm 9 Unit Tests và 4 Integration Tests, với tổng cộng 13 kiểm thử thành công.
Đây là cơ sở để tiếp tục phát triển các chức năng nghiệp vụ nâng cao và hoàn thiện hệ thống trong những giai đoạn tiếp theo.