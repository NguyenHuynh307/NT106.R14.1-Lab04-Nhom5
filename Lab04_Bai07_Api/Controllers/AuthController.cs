/*
Controllers (thường nằm ở thư mục Controllers) là tầng tiếp nhận request HTTP và trả response HTTP cho client.
Thay vì tự giữ dữ liệu hay tự băm mật khẩu, Controller đóng vai trò điều phối: nhận dữ liệu đầu vào, 
kiểm tra hợp lệ/quyền truy cập, gọi tầng Services để xử lý nghiệp vụ, và trả về mã trạng thái HTTP (200, 201, 204, 400, 401, 403, 404, 409) 
kèm dữ liệu phản hồi.

Các chức năng chính của tầng Controllers trong API
- Định tuyến và tiếp nhận request HTTP ([ApiController], [Route], [HttpGet], [HttpPost], [HttpDelete]): 
Ánh xạ từng URL và phương thức HTTP tới đúng phương thức xử lý (action), đồng thời đọc dữ liệu từ body ([FromBody]), 
query string ([FromQuery]), route ({id}) hoặc header (Authorization).

- Quản lý xác thực và phiên người dùng (AuthController): Xử lý đăng ký tài khoản (POST /api/v1/auth/register), 
đăng nhập và cấp token (POST /api/v1/auth/login), cũng như tra cứu thông tin người dùng hiện tại (GET /api/v1/user/me).

- Điều phối thao tác dữ liệu món ăn (FoodController): Xử lý truy vấn danh sách có phân trang/lọc (GET /api/v1/foods), 
chọn ngẫu nhiên (GET /api/v1/foods/random), tạo mới (POST /api/v1/foods) và xóa món ăn (DELETE /api/v1/foods/{id}).

- Kiểm tra đầu vào và phân quyền cơ bản: Chặn dữ liệu thiếu/sai định dạng (tên đăng nhập, mật khẩu, email, họ tên, số điện thoại, 
tuổi >= 18, tên/giá/địa chỉ món ăn), kiểm tra token hợp lệ và kiểm tra quyền sở hữu trước khi xóa món.

- Quy đổi kết quả nghiệp vụ sang mã HTTP & DTO: Chuyển dữ liệu nội bộ sang DTO an toàn 
(UserResponse, FoodResponse, LoginResponse, PagedFoodsResponse) và trả mã HTTP phù hợp kèm ApiErrorResponse khi gặp lỗi.
*/

/*
AuthController.cs là controller xử lý xác thực, đăng ký tài khoản và truy vấn thông tin người dùng hiện tại 
cho Web API (api/v1/auth/... và /api/v1/user/me). File này đóng vai trò tiếp nhận request HTTP, kiểm tra hợp lệ dữ liệu đầu vào, 
ủy quyền xử lý nghiệp vụ/lưu trữ cho InMemoryDataStore, và trả về mã trạng thái HTTP cùng DTO an toàn cho client.

1/ Chức năng chính đang thực hiện
- Đăng ký tài khoản mới (POST /api/v1/auth/register): Tiếp nhận RegisterRequest, kiểm tra định dạng/quy tắc dữ liệu, 
chặn tên đăng nhập trùng lặp (409 Conflict), tạo tài khoản mới trong InMemoryDataStore (kèm băm mật khẩu + salt) và 
trả về UserResponse (201 Created).

- Đăng nhập và cấp token (POST /api/v1/auth/login): Tiếp nhận LoginRequest, kiểm tra đầu vào cơ bản, xác thực tài khoản/mật khẩu 
qua InMemoryDataStore.Authenticate, cấp AccessToken dạng "Bearer" và trả về LoginResponse kèm thông tin người dùng (200 OK).

- Lấy thông tin người dùng đang đăng nhập (GET /api/v1/user/me): Đọc token từ header Authorization, xác định người dùng hiện tại và 
trả về UserResponse (200 OK), hoặc trả 401 Unauthorized nếu token thiếu/sai/không hợp lệ.

2/ Những công việc cụ thể phải làm
- Kiểm tra dữ liệu đầu vào và quy tắc đăng ký (ValidateRegisterRequest, IsValidEmail):

- Kiểm tra req không null và các trường bắt buộc khi đăng nhập.

- Kiểm tra Username: độ dài 4..30 ký tự sau Trim(), chỉ gồm [A-Za-z0-9._], và không nằm trong danh sách tên cấm (ForbiddenUsernames như admin, root, system, null, undefined...).

- Kiểm tra Password: độ dài 8..12 ký tự (giữ nguyên chuỗi gốc, không Trim()).

- Kiểm tra Email: không chứa khoảng trắng, vượt qua kiểm tra MailAddress, có đúng một @, tên miền có dấu . và không bắt đầu/kết thúc bằng ..

- Kiểm tra FirstName và LastName: không rỗng/toàn khoảng trắng và chỉ chứa chữ cái (\p{L}) cùng khoảng trắng.

- Kiểm tra Phone: khớp định dạng số điện thoại Việt Nam (^0\d{9}$ — 10 số, bắt đầu bằng 0).

- Kiểm tra Birthday: không lớn hơn ngày hiện tại và người dùng phải đủ từ 18 tuổi trở lên.

- Trích xuất và đối chiếu token phiên (ResolveCurrentUser): Đọc header Authorization, tự động bóc tiền tố "Bearer " (không phân biệt hoa/thường) 
nếu có, cắt khoảng trắng và tra cứu người dùng qua _store.GetUserByToken(token).

Ánh xạ sang DTO và quy đổi mã HTTP: Gọi InMemoryDataStore.ToUserResponse(...) để che giấu thông tin nội bộ 
(PasswordHash, PasswordSalt, CreatedAt) và ánh xạ kết quả xử lý sang đúng mã HTTP (200, 201, 400, 401, 409) kèm ApiErrorResponse khi có lỗi.
*/

using System.Net.Mail; // Cung cấp MailAddress để kiểm tra cú pháp địa chỉ email chuẩn.
using System.Text.RegularExpressions; // Cung cấp Regex để kiểm tra định dạng username, số điện thoại, họ tên.

using Lab04_Bai07_Api.Models; // Nạp các model/DTO như RegisterRequest, LoginRequest, UserResponse, UserAccount...
using Lab04_Bai07_Api.Services; // Nạp InMemoryDataStore để gọi logic lưu trữ và ánh xạ DTO.

using Microsoft.AspNetCore.Mvc; // Nạp các thuộc tính và lớp nền của ASP.NET Core Web API ([ApiController], ControllerBase, ActionResult...).

namespace Lab04_Bai07_Api.Controllers; // Khai báo namespace thuộc tầng Controllers của API.

[ApiController] // Bật hành vi chuẩn Web API (tự động ràng buộc dữ liệu, trả 400 cho lỗi model binding...).
[Route("api/v1/auth")] // Định tuyến gốc cho các endpoint trong controller này là "/api/v1/auth/...".
public class AuthController : ControllerBase // Kế thừa ControllerBase (dùng cho Web API, không cần hỗ trợ View như Controller).
{
    // Tập hợp tên đăng nhập cấm (không phân biệt hoa/thường nhờ StringComparer.OrdinalIgnoreCase) để chặn mạo danh hệ thống.
    private static readonly HashSet<string> ForbiddenUsernames = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin", "administrator", "root", "support", "system", "moderator", "mod", "staff", "null", "undefined"
    };

    // Biểu thức chính quy cho username: chỉ chữ cái không dấu, số, dấu chấm hoặc gạch dưới (dài 1+ ký tự), biên dịch sẵn để chạy nhanh.
    private static readonly Regex UsernameRegex = new(@"^[A-Za-z0-9._]+$", RegexOptions.Compiled);

    // Biểu thức chính quy số điện thoại Việt Nam: bắt đầu bằng số 0 và theo sau đúng 9 chữ số (tổng 10 số).
    private static readonly Regex PhoneVnRegex = new(@"^0\d{9}$", RegexOptions.Compiled);

    // Biểu thức chính quy cho họ và tên: chỉ gồm ký tự chữ cái Unicode (\p{L}, hỗ trợ tiếng Việt có dấu) và khoảng trắng (\s).
    private static readonly Regex NameRegex = new(@"^[\p{L}\s]+$", RegexOptions.Compiled);

    private readonly InMemoryDataStore _store; // Lưu tham chiếu tới kho dữ liệu/dịch vụ được DI bơm vào.

    public AuthController(InMemoryDataStore store) // Constructor nhận InMemoryDataStore qua Dependency Injection.
    {
        _store = store; // Gán instance được cấp vào biến _store để dùng xuyên suốt controller.
    }

    [HttpPost("register")] // Ánh xạ HTTP POST tới đường dẫn "api/v1/auth/register".
    public ActionResult<UserResponse> Register([FromBody] RegisterRequest? req) // Đọc JSON body thành RegisterRequest; trả UserResponse hoặc mã lỗi HTTP.
    {
        if (req is null) // Kiểm tra nếu body rỗng hoặc parse JSON thất bại ra null.
        {
            return BadRequest(new ApiErrorResponse { Message = "Dữ liệu đăng ký không hợp lệ." }); // Trả HTTP 400 Bad Request kèm thông báo lỗi.
        }

        string? validationError = ValidateRegisterRequest(req); // Chạy toàn bộ luật kiểm tra dữ liệu đầu vào của form đăng ký.
        if (validationError is not null) // Nếu có thông báo lỗi nghĩa là dữ liệu không đạt yêu cầu.
        {
            return BadRequest(new ApiErrorResponse { Message = validationError }); // Trả HTTP 400 với đúng lý do không hợp lệ.
        }

        if (_store.UsernameExists(req.Username)) // Kiểm tra xem username đã tồn tại trong hệ thống chưa (không phân biệt hoa/thường).
        {
            return Conflict(new ApiErrorResponse { Message = "Tên đăng nhập đã tồn tại." }); // Trả HTTP 409 Conflict vì xung đột tên tài khoản.
        }

        UserAccount created = _store.AddUser(req); // Tạo tài khoản mới (kèm băm mật khẩu + salt) và lưu vào store/file JSON.
        UserResponse response = InMemoryDataStore.ToUserResponse(created); // Chuyển từ UserAccount nội bộ sang UserResponse an toàn cho client.

        return CreatedAtAction(nameof(GetCurrentUser), response); // Trả HTTP 201 Created kèm dữ liệu người dùng mới tạo.
    }

    [HttpPost("login")] // Ánh xạ HTTP POST tới đường dẫn "api/v1/auth/login".
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest? req) // Đọc thông tin đăng nhập từ body; trả LoginResponse hoặc mã lỗi.
    {
        // Kiểm tra body null hoặc thiếu username (trống/khoảng trắng) hoặc thiếu password.
        if (req is null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrEmpty(req.Password))
        {
            return BadRequest(new ApiErrorResponse { Message = "Vui lòng nhập tên đăng nhập và mật khẩu." }); // Trả HTTP 400 nếu thiếu thông tin bắt buộc.
        }

        var authResult = _store.Authenticate(req.Username, req.Password); // Xác thực tài khoản/mật khẩu và sinh token nếu hợp lệ.
        if (authResult is null) // Trả về null nếu không tìm thấy user hoặc mật khẩu sai.
        {
            return Unauthorized(new ApiErrorResponse { Message = "Tên đăng nhập hoặc mật khẩu không chính xác." }); // Trả HTTP 401 Unauthorized.
        }

        return Ok(new LoginResponse // Trả HTTP 200 OK kèm token và thông tin người dùng.
        {
            AccessToken = authResult.Value.Token, // Chuỗi token phiên đăng nhập được cấp cho client.
            TokenType = "Bearer", // Loại token chuẩn Bearer để client gửi kèm ở header Authorization.
            User = InMemoryDataStore.ToUserResponse(authResult.Value.User) // Ánh xạ UserAccount sang UserResponse để tránh lộ dữ liệu nhạy cảm.
        });
    }

    [HttpGet("/api/v1/user/me")] // Dấu '/' đầu tuyến ghi đè route gốc, tạo endpoint tuyệt đối "/api/v1/user/me".
    public ActionResult<UserResponse> GetCurrentUser() // Trả thông tin người dùng hiện tại dựa trên token gửi lên.
    {
        UserAccount? user = ResolveCurrentUser(); // Đọc header Authorization, trích xuất token và tìm user tương ứng.
        if (user is null) // Nếu không có header, token sai hoặc không tồn tại trong store.
        {
            return Unauthorized(new ApiErrorResponse { Message = "Token không hợp lệ hoặc đã hết hạn." }); // Trả HTTP 401 Unauthorized.
        }

        return Ok(InMemoryDataStore.ToUserResponse(user)); // Trả HTTP 200 OK kèm UserResponse của người dùng đang đăng nhập.
    }

    private UserAccount? ResolveCurrentUser() // Hàm nội bộ lấy UserAccount từ header Authorization của request hiện tại.
    {
        string? authHeader = Request.Headers.Authorization.FirstOrDefault(); // Lấy giá trị đầu tiên của header Authorization nếu có.
        if (string.IsNullOrWhiteSpace(authHeader)) // Nếu không gửi header Authorization hoặc chỉ có khoảng trắng.
        {
            return null; // Không xác định được người dùng.
        }

        const string bearerPrefix = "Bearer "; // Tiền tố chuẩn của Bearer token trong HTTP header.
        string token = authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase) // Kiểm tra xem có bắt đầu bằng "Bearer " không.
            ? authHeader[bearerPrefix.Length..].Trim() // Nếu có, cắt bỏ 7 ký tự "Bearer " và loại bỏ khoảng trắng thừa.
            : authHeader.Trim(); // Nếu client gửi thẳng chuỗi token không kèm "Bearer ", chỉ cần Trim().

        return _store.GetUserByToken(token); // Tra cứu và trả về UserAccount gắn với token (hoặc null nếu không khớp).
    }

    private static string? ValidateRegisterRequest(RegisterRequest req) // Trả chuỗi lỗi nếu dữ liệu sai, hoặc null nếu hợp lệ.
    {
        string username = req.Username.Trim(); // Cắt khoảng trắng hai đầu tên đăng nhập.
        if (username.Length < 4 || username.Length > 30) // Kiểm tra độ dài phải nằm trong khoảng 4..30 ký tự.
        {
            return "Tên đăng nhập phải dài từ 4 đến 30 ký tự."; // Báo lỗi độ dài username.
        }

        if (!UsernameRegex.IsMatch(username)) // Kiểm tra ký tự hợp lệ theo UsernameRegex.
        {
            return "Tên đăng nhập chỉ gồm chữ cái không dấu, số, dấu gạch dưới (_) hoặc dấu chấm (.), không chứa khoảng trắng."; // Báo lỗi ký tự username.
        }

        if (ForbiddenUsernames.Contains(username)) // Kiểm tra tên có nằm trong danh sách từ khóa cấm hay không.
        {
            return "Tên đăng nhập nằm trong danh sách từ cấm."; // Báo lỗi tên bị cấm.
        }

        if (req.Password.Length < 8 || req.Password.Length > 12) // Kiểm tra mật khẩu có độ dài từ 8 đến 12 ký tự (không Trim để giữ nguyên mật khẩu người dùng nhập).
        {
            return "Mật khẩu phải có độ dài từ 8 đến 12 ký tự."; // Báo lỗi độ dài mật khẩu.
        }

        string email = req.Email.Trim(); // Cắt khoảng trắng hai đầu email.
        if (!IsValidEmail(email)) // Gọi hàm phụ kiểm tra định dạng email chặt chẽ hơn.
        {
            return "Email không hợp lệ."; // Báo lỗi email sai định dạng.
        }

        string firstname = req.FirstName.Trim(); // Cắt khoảng trắng hai đầu tên.
        if (string.IsNullOrWhiteSpace(firstname) || !NameRegex.IsMatch(firstname)) // Kiểm tra tên không rỗng và chỉ chứa chữ cái + khoảng trắng.
        {
            return "Tên không được để trống và không chứa số hoặc ký tự đặc biệt."; // Báo lỗi trường FirstName.
        }

        string lastname = req.LastName.Trim(); // Cắt khoảng trắng hai đầu họ.
        if (string.IsNullOrWhiteSpace(lastname) || !NameRegex.IsMatch(lastname)) // Kiểm tra họ không rỗng và chỉ chứa chữ cái + khoảng trắng.
        {
            return "Họ không được để trống và không chứa số hoặc ký tự đặc biệt."; // Báo lỗi trường LastName.
        }

        string phone = req.Phone.Trim(); // Cắt khoảng trắng hai đầu số điện thoại.
        if (!PhoneVnRegex.IsMatch(phone)) // Kiểm tra số điện thoại có đúng định dạng 10 số bắt đầu bằng số 0 hay không.
        {
            return "Số điện thoại Việt Nam phải gồm đúng 10 chữ số và bắt đầu bằng số 0."; // Báo lỗi số điện thoại.
        }

        DateTime birthday = req.Birthday.Date; // Lấy phần ngày (bỏ giờ/phút/giây) của ngày sinh.
        DateTime today = DateTime.Today; // Lấy ngày hiện tại của máy chủ (giờ 00:00:00).
        if (birthday > today) // Chặn ngày sinh ở tương lai.
        {
            return "Ngày sinh không được lớn hơn ngày hiện tại."; // Báo lỗi ngày sinh lớn hơn hôm nay.
        }

        int age = today.Year - birthday.Year; // Tính tuổi sơ bộ theo hiệu năm.
        if (birthday > today.AddYears(-age)) // Nếu năm nay chưa tới ngày sinh nhật thì lùi tuổi đi 1.
        {
            age--; // Giảm tuổi chính xác 1 đơn vị.
        }

        if (age < 18) // Yêu cầu người dùng đủ từ 18 tuổi trở lên.
        {
            return "Người dùng phải đủ từ 18 tuổi trở lên."; // Báo lỗi chưa đủ 18 tuổi.
        }

        return null; // Tất cả điều kiện đều vượt qua -> dữ liệu hợp lệ.
    }

    private static bool IsValidEmail(string email) // Kiểm tra chuỗi email có hợp lệ không, trả true/false.
    {
        if (string.IsNullOrWhiteSpace(email) || email.Any(char.IsWhiteSpace)) // Loại ngay email rỗng hoặc chứa bất kỳ ký tự khoảng trắng nào.
        {
            return false; // Không hợp lệ.
        }

        try // Bắt ngoại lệ vì new MailAddress(...) sẽ ném FormatException nếu chuỗi sai cú pháp.
        {
            var addr = new MailAddress(email); // Phân tích chuỗi email bằng lớp MailAddress của .NET.
            if (!string.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase)) // Đảm bảo chuỗi gốc khớp với địa chỉ chuẩn hóa của MailAddress.
            {
                return false; // Không hợp lệ nếu bị biến dạng/chứa phần thừa.
            }

            string[] parts = email.Split('@'); // Tách email theo ký tự '@'.
            if (parts.Length != 2) // Địa chỉ email hợp lệ phải có đúng một ký tự '@' (tách thành 2 phần).
            {
                return false; // Không hợp lệ.
            }

            string domainPart = parts[1]; // Lấy phần tên miền sau '@'.
            if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(domainPart)) // Đảm bảo cả phần local và phần domain đều không rỗng.
            {
                return false; // Không hợp lệ.
            }

            // Tên miền phải chứa dấu '.', không bắt đầu bằng '.' và không kết thúc bằng '.' (tránh các dạng như "a@localhost" hoặc "a@.com").
            return domainPart.Contains('.') && !domainPart.StartsWith('.') && !domainPart.EndsWith('.');
        }
        catch // Nếu MailAddress ném lỗi định dạng.
        {
            return false; // Trả về false thay vì làm văng exception ra ngoài.
        }
    }
}