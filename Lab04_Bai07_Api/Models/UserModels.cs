/*
Tầng Models của API đóng vai trò là bản hợp đồng quy định cấu trúc dữ liệu xuyên suốt hệ thống—từ việc mô hình hóa thực thể lưu trữ nội bộ ở server, 
ràng buộc dữ liệu đầu vào từ client, đóng gói kết quả đầu ra an toàn đến chuẩn hóa cấu trúc phản hồi lỗi qua JSON.
/*

/*
File được đặt tên là UserModels.cs vì nó gom chung các lớp mô hình dữ liệu liên quan trực tiếp đến 
người dùng và xác thực tài khoản (UserAccount, RegisterRequest, LoginRequest, LoginResponse, UserResponse) trong tầng Models của API. 
Trong quá trình vận hành app, file này định nghĩa cấu trúc dữ liệu để lưu trữ tài khoản (kèm hash và salt) ở phía server, 
tiếp nhận yêu cầu đăng ký/đăng nhập từ client, cấp token phiên làm việc và trả về thông tin hồ sơ người dùng an toàn.
*/

// Khai báo namespace thuộc tầng Models của project Web API "Lab04_Bai07_Api".
namespace Lab04_Bai07_Api.Models;

// Lớp đại diện cho tài khoản người dùng lưu trữ nội bộ trên server (trong data store / file JSON).
public class UserAccount
{
    // Khóa định danh duy nhất của tài khoản, tự sinh chuỗi GUID 32 ký tự hex khi khởi tạo.
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // Tên đăng nhập duy nhất của người dùng.
    public string Username { get; set; } = string.Empty;

    // Địa chỉ email của người dùng.
    public string Email { get; set; } = string.Empty;

    // Tên của người dùng.
    public string FirstName { get; set; } = string.Empty;

    // Họ (và tên đệm) của người dùng.
    public string LastName { get; set; } = string.Empty;

    // Số điện thoại liên hệ của người dùng.
    public string Phone { get; set; } = string.Empty;

    // Ngày sinh của người dùng.
    public DateTime Birthday { get; set; }

    // Ngôn ngữ hiển thị ưa thích của người dùng, mặc định là "Tiếng Việt".
    public string Language { get; set; } = "Tiếng Việt";

    // Giới tính của người dùng, mặc định là "Nam".
    public string Sex { get; set; } = "Nam";

    // Chuỗi mật khẩu đã được băm (hash), tuyệt đối không lưu mật khẩu thô.
    public string PasswordHash { get; set; } = string.Empty;

    // Chuỗi salt ngẫu nhiên dùng kèm khi băm mật khẩu để chống tấn công rainbow table / brute-force.
    public string PasswordSalt { get; set; } = string.Empty;

    // Thời điểm tạo tài khoản theo chuẩn UTC.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// DTO nhận dữ liệu đầu vào khi client gửi yêu cầu đăng ký tài khoản mới.
public class RegisterRequest
{
    // Tên đăng nhập mong muốn.
    public string Username { get; set; } = string.Empty;

    // Mật khẩu dạng plain text do client gửi lên (server sẽ băm cùng salt trước khi lưu vào UserAccount).
    public string Password { get; set; } = string.Empty;

    // Email đăng ký.
    public string Email { get; set; } = string.Empty;

    // Tên người dùng.
    public string FirstName { get; set; } = string.Empty;

    // Họ và tên đệm người dùng.
    public string LastName { get; set; } = string.Empty;

    // Số điện thoại người dùng.
    public string Phone { get; set; } = string.Empty;

    // Ngày sinh người dùng.
    public DateTime Birthday { get; set; }

    // Ngôn ngữ mặc định nếu client không truyền giá trị khác.
    public string Language { get; set; } = "Tiếng Việt";

    // Giới tính mặc định nếu client không truyền giá trị khác.
    public string Sex { get; set; } = "Nam";
}

// DTO nhận thông tin xác thực khi client gửi yêu cầu đăng nhập.
public class LoginRequest
{
    // Tên đăng nhập dùng để xác thực tài khoản.
    public string Username { get; set; } = string.Empty;

    // Mật khẩu người dùng nhập khi đăng nhập.
    public string Password { get; set; } = string.Empty;
}

// DTO phản hồi kết quả sau khi đăng nhập thành công.
public class LoginResponse
{
    // Chuỗi token xác thực phiên làm việc cấp cho client (dùng cho các request yêu cầu đăng nhập).
    public string AccessToken { get; set; } = string.Empty;

    // Loại token chuẩn HTTP Authorization header, mặc định là "Bearer".
    public string TokenType { get; set; } = "Bearer";

    // Thông tin hồ sơ người dùng an toàn trả về cho client (khởi tạo sẵn đối tượng UserResponse mặc định).
    public UserResponse User { get; set; } = new();
}

// DTO đại diện thông tin người dùng trả ra cho client, loại bỏ các trường nhạy cảm như PasswordHash và PasswordSalt.
public class UserResponse
{
    // Mã định danh người dùng.
    public string Id { get; set; } = string.Empty;

    // Tên đăng nhập.
    public string Username { get; set; } = string.Empty;

    // Email người dùng.
    public string Email { get; set; } = string.Empty;

    // Tên người dùng.
    public string FirstName { get; set; } = string.Empty;

    // Họ và tên đệm người dùng.
    public string LastName { get; set; } = string.Empty;

    // Số điện thoại người dùng.
    public string Phone { get; set; } = string.Empty;

    // Ngày sinh người dùng.
    public DateTime Birthday { get; set; }

    // Mã/giá trị ngôn ngữ của người dùng (mặc định Tiếng Việt).
    public string Language { get; set; } = "Tiếng Việt";

    // Giới tính người dùng.
    public string Sex { get; set; } = "Nam";
}