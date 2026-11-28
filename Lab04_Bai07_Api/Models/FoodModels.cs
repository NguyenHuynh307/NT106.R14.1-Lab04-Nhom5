/*
Tầng Models của API đóng vai trò là bản hợp đồng quy định cấu trúc dữ liệu xuyên suốt hệ thống—từ việc mô hình hóa thực thể lưu trữ nội 
bộ ở server, ràng buộc dữ liệu đầu vào từ client, đóng gói kết quả đầu ra an toàn đến chuẩn hóa cấu trúc phản hồi lỗi qua JSON.
/*

/*
File được đặt tên là FoodModels.cs vì nó gom chung các lớp mô hình dữ liệu thuộc nhóm thực thể món ăn
(FoodItem, CreateFoodRequest, FoodResponse, PagedFoodsResponse, ApiErrorResponse) trong tầng Models của API.
Trong quá trình vận hành app, file này định nghĩa cấu trúc dữ liệu để lưu trữ món ăn ở server, 
nhận yêu cầu tạo món từ client, trả về danh sách món ăn có phân trang/đóng góp và chuẩn hóa thông báo lỗi.
*/

// Khai báo namespace thuộc tầng Models của project Web API "Lab04_Bai07_Api".
namespace Lab04_Bai07_Api.Models;

// Lớp đại diện cho một bản ghi món ăn được lưu trữ và quản lý nội bộ trên server (trong InMemoryDataStore / file JSON).
public class FoodItem
{
    // Khóa định danh duy nhất của món ăn, tự sinh chuỗi GUID 32 ký tự hex (không dấu gạch ngang) khi khởi tạo.
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // Tên món ăn (mặc định chuỗi rỗng để tránh null).
    public string Name { get; set; } = string.Empty;

    // Giá món ăn dùng kiểu decimal để đảm bảo độ chính xác thập phân cho tiền tệ.
    public decimal Price { get; set; }

    // Địa chỉ quán hoặc khu vực bán món ăn.
    public string Address { get; set; } = string.Empty;

    // Mô tả thêm về món ăn (thành phần, hương vị, ghi chú...).
    public string Description { get; set; } = string.Empty;

    // Đường dẫn file cục bộ hoặc URL hình ảnh của món ăn.
    public string Image { get; set; } = string.Empty;

    // ID của người dùng đã tạo/đóng góp món ăn này (lấy từ phiên đăng nhập hợp lệ).
    public string CreatedByUserId { get; set; } = string.Empty;

    // Tên đăng nhập (username) của người tạo món để hiển thị thông tin đóng góp.
    public string CreatedByUsername { get; set; } = string.Empty;

    // Thời điểm tạo bản ghi món ăn, lưu theo chuẩn UTC để đồng bộ múi giờ.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Lớp dữ liệu đầu vào (request body) khi client gửi yêu cầu tạo món ăn mới (POST /api/v1/foods).
public class CreateFoodRequest
{
    // Tên món ăn do người dùng nhập trên form thêm món.
    public string Name { get; set; } = string.Empty;

    // Giá món ăn do người dùng nhập.
    public decimal Price { get; set; }

    // Địa chỉ quán do người dùng nhập.
    public string Address { get; set; } = string.Empty;

    // Mô tả món ăn do người dùng nhập.
    public string Description { get; set; } = string.Empty;

    // Đường dẫn hoặc URL ảnh món ăn do người dùng nhập/chọn.
    public string Image { get; set; } = string.Empty;
}

// Lớp dữ liệu đầu ra (response DTO) trả về cho client cho từng món ăn sau khi xử lý/gom nhóm.
// Ghi chú: (DTO = Data Transfer Object) là đối tượng truyền tải dữ liệu.
public class FoodResponse
{
    // Mã định danh món ăn đại diện trả về cho client.
    public string Id { get; set; } = string.Empty;

    // Tên món ăn.
    public string Name { get; set; } = string.Empty;

    // Giá món ăn.
    public decimal Price { get; set; }

    // Địa chỉ quán.
    public string Address { get; set; } = string.Empty;

    // Mô tả món ăn.
    public string Description { get; set; } = string.Empty;

    // Đường dẫn hoặc URL ảnh món ăn.
    public string Image { get; set; } = string.Empty;

    // Tên tài khoản người đóng góp đại diện/đầu tiên của món.
    public string CreatedByUsername { get; set; } = string.Empty;

    // Thời điểm món được tạo (UTC).
    public DateTime CreatedAt { get; set; }

    // Tổng số người đóng góp cho món ăn này (mặc định là 1 khi chưa gom hoặc có 1 người đóng góp).
    public int ContributorCount { get; set; } = 1;

    // Danh sách tên người đóng góp (dùng collection expression `[]` để khởi tạo List<string> rỗng).
    public List<string> Contributors { get; set; } = [];
}

// Lớp bao bọc kết quả danh sách món ăn có phân trang trả về cho WinForms.
public class PagedFoodsResponse
{
    // Số thứ tự trang hiện tại (bắt đầu từ 1).
    public int Page { get; set; }

    // Số lượng món tối đa trên mỗi trang.
    public int PageSize { get; set; }

    // Tổng số món ăn thỏa mãn điều kiện lọc/gom nhóm.
    public int TotalItems { get; set; }

    // Tổng số trang tính được từ TotalItems và PageSize.
    public int TotalPages { get; set; }

    // Danh sách các món ăn thuộc trang hiện tại.
    public List<FoodResponse> Items { get; set; } = [];
}

// Lớp chuẩn hóa thông báo lỗi trả về từ API khi xảy ra lỗi nghiệp vụ hoặc xác thực.
public class ApiErrorResponse
{
    // Nội dung mô tả lỗi để client đọc và hiển thị cho người dùng.
    public string Message { get; set; } = string.Empty;
}