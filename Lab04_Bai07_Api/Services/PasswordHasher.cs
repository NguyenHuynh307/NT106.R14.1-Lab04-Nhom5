/*
Đóng gói theo chức năng nghiệp vụ/hạ tầng: Thư mục Services gom các lớp xử lý nghiệp vụ, 
lưu trữ và bảo mật tách khỏi tầng Controllers (nhận/trả HTTP) và Models (định nghĩa dữ liệu/DTO).

Phân tách rõ hai vai trò trong cùng một thư mục:
InMemoryDataStore.cs: lớp quản lý dữ liệu và trạng thái (giữ danh sách người dùng, token phiên, món ăn trên RAM và đồng bộ xuống file JSON).
PasswordHasher.cs: lớp tiện ích mật mã (băm mật khẩu có salt bằng PBKDF2-SHA256 và xác thực an toàn theo thời gian cố định).

Cầu nối giữa Controller và lưu trữ: Khi Controller tiếp nhận request (đăng ký, đăng nhập, lấy/thêm/xóa/random món ăn), 
nó gọi sang InMemoryDataStore, còn InMemoryDataStore gọi PasswordHasher khi cần băm hoặc kiểm tra mật khẩu.

Tương tác với môi trường và cấu hình (appsettings.json): InMemoryDataStore nhận IWebHostEnvironment để xác định thư mục gốc của ứng dụng và 
tạo/đọc/ghi file dữ liệu (App_Data/store-data.json), trong khi appsettings.json ở ngay bên dưới giữ cấu hình chung 
của host/logging/app cho toàn bộ Web API.

Sự khác biệt về vòng đời và trạng thái:
InMemoryDataStore có trạng thái sống xuyên suốt ứng dụng nên trong ASP.NET Core thường được đăng ký dưới dạng Singleton để mọi request dùng chung một nguồn dữ liệu và một ổ khóa _lock.
PasswordHasher là lớp tiện ích static (không lưu trạng thái riêng), gọi trực tiếp qua tên lớp mà không phụ thuộc vòng đời từng request.

An toàn đa luồng và bền vững dữ liệu: Vì nhiều request HTTP có thể chạy đồng thời trên các thread khác nhau, 
InMemoryDataStore dùng lock để bảo vệ bộ nhớ và ghi file tạm .tmp rồi move sang file chính nhằm tránh hỏng dữ liệu khi ghi.
*/


/*
PasswordHasher.cs là lớp tiện ích tĩnh chuyên xử lý bảo mật mật khẩu cho API: không lưu mật khẩu dạng văn bản thuần (plain text) 
mà chuyển sang dạng băm có muối (salt) để lưu trữ và xác thực an toàn.

1/ Chức năng chính đang đáp ứng
- Băm mật khẩu mới (HashPassword): Khi người dùng đăng ký hoặc hệ thống tạo tài khoản mẫu, 
hàm nhận mật khẩu thô, sinh muối ngẫu nhiên và tạo ra cặp (Hash, Salt) dạng chuỗi Base64 để lưu vào UserAccount.

- Xác thực mật khẩu khi đăng nhập (VerifyPassword): Khi người dùng nhập mật khẩu đăng nhập, 
hàm lấy storedSalt và storedHash đã lưu, tính lại giá trị băm từ mật khẩu vừa nhập rồi đối chiếu xem có khớp hay không.

2/ Những công việc cụ thể mà nó phải làm
- Sinh salt ngẫu nhiên riêng cho từng tài khoản: Dùng RandomNumberGenerator.GetBytes(16) tạo 16 byte ngẫu nhiên để hai người dùng có 
cùng mật khẩu vẫn ra hai chuỗi hash khác nhau, chống tấn công tra bảng băm có sẵn (rainbow table).

- Dẫn xuất khóa băm bằng PBKDF2-SHA256 với 100.000 vòng lặp: Gọi Rfc2898DeriveBytes.Pbkdf2(..., 100_000, HashAlgorithmName.SHA256, 32) 
để tạo hash 32 byte, làm tăng chi phí tính toán nhằm làm chậm các đợt dò mật khẩu tự động (brute-force).

- Chuyển đổi dữ liệu giữa mảng byte và chuỗi Base64: Mã hóa mảng byte sang chuỗi Base64 khi trả về để lưu dễ dàng vào file JSON, 
và giải mã ngược lại từ chuỗi Base64 thành mảng byte khi kiểm tra mật khẩu.

- So sánh băm theo thời gian cố định: Dùng CryptographicOperations.FixedTimeEquals thay vì so sánh chuỗi/mảng thông thường 
để ngăn tấn công suy đoán dựa trên độ lệch thời gian xử lý (timing attack).

*/


// Nạp các lớp mật mã học chuẩn của .NET (RandomNumberGenerator, Rfc2898DeriveBytes, CryptographicOperations...).
using System.Security.Cryptography;

// Khai báo namespace thuộc tầng Services của project Web API "Lab04_Bai07_Api".
namespace Lab04_Bai07_Api.Services;

// Lớp tiện ích tĩnh (static) chuyên băm và xác thực mật khẩu, không cần khởi tạo đối tượng.
public static class PasswordHasher
{
    // Độ dài salt ngẫu nhiên tính theo byte (16 bytes = 128 bit) để mỗi mật khẩu có chuỗi muối riêng.
    private const int SaltSize = 16;

    // Độ dài chuỗi hash đầu ra tính theo byte (32 bytes = 256 bit).
    private const int HashSize = 32;

    // Số vòng lặp PBKDF2 (100.000 lần) nhằm làm chậm tấn công dò mật khẩu (brute-force).
    private const int Iterations = 100_000;

    // Tạo salt ngẫu nhiên và băm mật khẩu thuần thành cặp (Hash, Salt) dạng chuỗi Base64.
    public static (string Hash, string Salt) HashPassword(string password)
    {
        // Sinh ngẫu nhiên 16 byte salt bằng bộ sinh số ngẫu nhiên an toàn mật mã.
        byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);

        // Dẫn xuất khóa băm 32 byte từ mật khẩu và salt bằng thuật toán PBKDF2 với hàm băm SHA-256.
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // Chuyển cả mảng byte hash và salt sang chuỗi Base64 để lưu an toàn trong JSON/database.
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    // Kiểm tra mật khẩu người dùng nhập có khớp với hash và salt đã lưu hay không.
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        // Giải mã chuỗi salt Base64 đã lưu trở lại thành mảng byte.
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        // Giải mã chuỗi hash Base64 đã lưu trở lại thành mảng byte kỳ vọng.
        byte[] expectedHash = Convert.FromBase64String(storedHash);

        // Tính lại hash từ mật khẩu vừa nhập với đúng salt, số vòng lặp, SHA-256 và độ dài cũ.
        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // So sánh hai mảng byte theo thời gian cố định để chống tấn công rò rỉ thời gian (timing attack).
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}