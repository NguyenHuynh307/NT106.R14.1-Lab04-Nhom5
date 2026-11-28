/*
FoodApiClient.cs là tầng dịch vụ gọi HTTP API ở phía client (nằm trong Lab04_Bai07.Services), dùng HttpClient để 
giao tiếp với Web API (Lab04_Bai07_Api) qua JSON và quản lý phiên đăng nhập ở phía ứng dụng client.

1/ Chức năng chính đang thực hiện
Xác thực và quản lý phiên người dùng:
RegisterAsync: gửi RegisterPayload tới POST api/v1/auth/register để đăng ký tài khoản và nhận UserDto.
LoginAsync: gửi LoginPayload tới POST api/v1/auth/login, nhận LoginResultDto rồi tự động lưu AccessToken và CurrentUser vào SessionContext.
GetCurrentUserAsync: gọi GET api/v1/user/me kèm token để lấy thông tin người dùng hiện tại và cập nhật SessionContext.CurrentUser.
SessionContext: lưu trạng thái phiên đăng nhập hiện tại trên client (AccessToken, CurrentUser) và hỗ trợ xóa phiên qua Clear().

Thao tác dữ liệu món ăn:
GetFoodsAsync: gọi GET api/v1/foods với tham số page, pageSize, mine để lấy danh sách phân trang PagedFoodsDto.
CreateFoodAsync: gửi CreateFoodPayload tới POST api/v1/foods kèm xác thực để tạo món mới và nhận FoodDto.
DeleteFoodAsync: gọi DELETE api/v1/foods/{id} kèm token để xóa món ăn theo mã.
GetRandomFoodAsync: gọi GET api/v1/foods/random?mine=... để lấy ngẫu nhiên một món ăn (FoodDto).

2/ Những công việc cụ thể phải làm
- Cấu hình kết nối HTTP và giải phóng tài nguyên: Khởi tạo HttpClient với BaseAddress chuẩn hóa dấu / cuối và 
Timeout (mặc định 15 giây), cấu hình PropertyNameCaseInsensitive = true khi đọc JSON, và cài đặt Dispose() để giải phóng _http.

- Định nghĩa bộ Payload, DTO và ngoại lệ riêng: Cung cấp các lớp dữ liệu gửi/nhận 
(RegisterPayload, LoginPayload, UserDto, CreateFoodPayload, FoodDto, PagedFoodsDto, LoginResultDto, ApiErrorDto) 
cùng ApiException mang theo StatusCode và thông báo lỗi.

- Đóng gói request và gắn Bearer Token: Tuần tự hóa object thành JSON UTF-8 (application/json) và 
gắn header Authorization: Bearer <token> khi thực hiện các thao tác cần xác thực.

- Bắt lỗi mạng, parse lỗi server và deserialize kết quả:
Quy đổi timeout thành mã lỗi 408 và lỗi kết nối mạng (HttpRequestException) thành 503 dưới dạng ApiException.
Khi HTTP trả về trạng thái lỗi, đọc nội dung phản hồi, ưu tiên trích xuất Message từ ApiErrorDto để ném ApiException rõ ràng cho tầng giao diện.
Khi thành công, kiểm tra mã ID hợp lệ trước khi xóa (DeleteFoodAsync), hoặc chuyển JSON sang kiểu TResponse và báo lỗi nếu phản hồi rỗng.

*/

using System.Net.Http.Headers; // Cung cấp AuthenticationHeaderValue để gắn header "Authorization: Bearer <token>".
using System.Text; // Cung cấp Encoding.UTF8 khi tạo nội dung chuỗi JSON gửi đi.
using System.Text.Json; // Cung cấp JsonSerializer và JsonSerializerOptions để serialize/deserialize JSON.

namespace Lab04_Bai07.Services; // Khai báo namespace thuộc tầng Services của ứng dụng client (Lab04_Bai07).

public class RegisterPayload // Dữ liệu gửi lên khi đăng ký tài khoản mới.
{
    public string Username { get; set; } = string.Empty; // Tên đăng nhập, mặc định chuỗi rỗng.
    public string Password { get; set; } = string.Empty; // Mật khẩu thuần gửi lên qua request.
    public string Email { get; set; } = string.Empty; // Địa chỉ email của người dùng.
    public string FirstName { get; set; } = string.Empty; // Tên người dùng.
    public string LastName { get; set; } = string.Empty; // Họ người dùng.
    public string Phone { get; set; } = string.Empty; // Số điện thoại.
    public DateTime Birthday { get; set; } // Ngày sinh.
    public string Language { get; set; } = "Tiếng Việt"; // Ngôn ngữ ưu tiên, mặc định "Tiếng Việt".
    public string Sex { get; set; } = "Nam"; // Giới tính, mặc định "Nam".
}

public class LoginPayload // Dữ liệu gửi lên khi đăng nhập.
{
    public string Username { get; set; } = string.Empty; // Tên đăng nhập.
    public string Password { get; set; } = string.Empty; // Mật khẩu.
}

public class UserDto // Thông tin người dùng nhận về từ API (không có hash/salt mật khẩu).
{
    public string Id { get; set; } = string.Empty; // Mã định danh người dùng.
    public string Username { get; set; } = string.Empty; // Tên đăng nhập.
    public string Email { get; set; } = string.Empty; // Email.
    public string FirstName { get; set; } = string.Empty; // Tên.
    public string LastName { get; set; } = string.Empty; // Họ.
    public string Phone { get; set; } = string.Empty; // Số điện thoại.
    public DateTime Birthday { get; set; } // Ngày sinh.
    public string Language { get; set; } = "Tiếng Việt"; // Ngôn ngữ.
    public string Sex { get; set; } = "Nam"; // Giới tính.
}

public class CreateFoodPayload // Dữ liệu gửi lên khi tạo món ăn mới.
{
    public string Name { get; set; } = string.Empty; // Tên món ăn.
    public decimal Price { get; set; } // Giá món ăn (dùng decimal để chính xác với tiền tệ).
    public string Address { get; set; } = string.Empty; // Địa chỉ quán.
    public string Description { get; set; } = string.Empty; // Mô tả thêm.
    public string Image { get; set; } = string.Empty; // Đường dẫn hoặc thông tin ảnh món ăn.
}

public class FoodDto // Thông tin một món ăn nhận về từ API.
{
    public string Id { get; set; } = string.Empty; // Mã món ăn.
    public string Name { get; set; } = string.Empty; // Tên món ăn.
    public decimal Price { get; set; } // Giá món ăn.
    public string Address { get; set; } = string.Empty; // Địa chỉ quán.
    public string Description { get; set; } = string.Empty; // Mô tả.
    public string Image { get; set; } = string.Empty; // Ảnh.
    public string CreatedByUsername { get; set; } = string.Empty; // Username người tạo món.
    public DateTime CreatedAt { get; set; } // Thời điểm tạo.
    public int ContributorCount { get; set; } = 1; // Số lượng người đóng góp món (mặc định 1).
    public List<string> Contributors { get; set; } = []; // Danh sách người đóng góp (khởi tạo list rỗng bằng collection expression []).
}

public class PagedFoodsDto // Kết quả danh sách món ăn có phân trang trả về từ GET /api/v1/foods.
{
    public int Page { get; set; } // Trang hiện tại.
    public int PageSize { get; set; } // Số phần tử mỗi trang.
    public int TotalItems { get; set; } // Tổng số món/nhóm món phù hợp.
    public int TotalPages { get; set; } // Tổng số trang.
    public List<FoodDto> Items { get; set; } = []; // Danh sách món ăn thuộc trang hiện tại.
}

public class LoginResultDto // Kết quả trả về khi đăng nhập thành công.
{
    public string AccessToken { get; set; } = string.Empty; // Token phiên để gửi kèm các request cần xác thực.
    public string TokenType { get; set; } = "Bearer"; // Kiểu token, mặc định "Bearer".
    public UserDto User { get; set; } = new(); // Thông tin tài khoản đã đăng nhập.
}

public class ApiErrorDto // Cấu trúc JSON lỗi chuẩn mà server trả về khi thất bại.
{
    public string Message { get; set; } = string.Empty; // Thông báo lỗi từ server.
}

public class ApiException : Exception // Ngoại lệ tùy chỉnh đại diện cho lỗi gọi API hoặc lỗi mạng/timeout.
{
    public int StatusCode { get; } // Lưu mã trạng thái HTTP (ví dụ 400, 401, 403, 404, 409) hoặc mã lỗi tự quy ước (408, 503).

    public ApiException(int statusCode, string message) : base(message) // Khởi tạo exception kèm status code và truyền message lên lớp Exception cha.
    {
        StatusCode = statusCode; // Gán mã trạng thái cho thuộc tính StatusCode.
    }
}

public static class SessionContext // Lưu trạng thái phiên đăng nhập hiện tại ở phía client (dạng tĩnh dùng chung).
{
    public static string? AccessToken { get; set; } // Lưu Bearer token hiện tại (null nếu chưa đăng nhập).
    public static UserDto? CurrentUser { get; set; } // Lưu thông tin người dùng hiện tại (null nếu chưa đăng nhập).

    public static void Clear() // Xóa toàn bộ thông tin phiên khi đăng xuất hoặc reset phiên.
    {
        AccessToken = null; // Xóa token.
        CurrentUser = null; // Xóa thông tin user hiện tại.
    }
}

public sealed class FoodApiClient : IDisposable // Client gọi Web API, đóng băng kế thừa (sealed) và hỗ trợ giải phóng HttpClient qua IDisposable.
{
    // Cấu hình JSON cho chiều deserialize: bỏ qua phân biệt hoa/thường giữa tên property C# và key JSON.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http; // Đối tượng HttpClient thực hiện gửi request HTTP tới API.

    // Constructor nhận baseUrl (mặc định http://localhost:5167) và timeout (mặc định 15 giây).
    public FoodApiClient(string baseUrl = "http://localhost:5167", int timeoutSeconds = 15)
    {
        _http = new HttpClient // Tạo instance HttpClient mới.
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"), // Đảm bảo BaseAddress luôn kết thúc bằng '/' để ghép đường dẫn tương đối chuẩn.
            Timeout = TimeSpan.FromSeconds(timeoutSeconds) // Đặt giới hạn thời gian chờ phản hồi cho mỗi request.
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterPayload payload, CancellationToken ct = default) // Gọi POST api/v1/auth/register để đăng ký tài khoản.
    {
        return await SendJsonAsync<RegisterPayload, UserDto>( // Gửi JSON payload và nhận về UserDto.
            HttpMethod.Post, "api/v1/auth/register", payload, requiresAuth: false, ct);
    }

    public async Task<LoginResultDto> LoginAsync(LoginPayload payload, CancellationToken ct = default) // Gọi POST api/v1/auth/login để đăng nhập.
    {
        LoginResultDto result = await SendJsonAsync<LoginPayload, LoginResultDto>( // Gửi username/password và nhận LoginResultDto.
            HttpMethod.Post, "api/v1/auth/login", payload, requiresAuth: false, ct);

        SessionContext.AccessToken = result.AccessToken; // Lưu token vừa nhận vào SessionContext để dùng cho các request sau.
        SessionContext.CurrentUser = result.User; // Lưu thông tin user hiện tại vào SessionContext.
        return result; // Trả kết quả đăng nhập cho tầng UI xử lý tiếp.
    }

    public async Task<UserDto> GetCurrentUserAsync(CancellationToken ct = default) // Gọi GET api/v1/user/me để lấy thông tin user hiện tại.
    {
        UserDto user = await SendJsonAsync<object, UserDto>( // Không có body (body: null), yêu cầu xác thực (requiresAuth: true).
            HttpMethod.Get, "api/v1/user/me", body: null, requiresAuth: true, ct);

        SessionContext.CurrentUser = user; // Cập nhật lại thông tin user mới nhất vào SessionContext.
        return user; // Trả về UserDto.
    }

    public async Task<PagedFoodsDto> GetFoodsAsync( // Gọi GET api/v1/foods có phân trang và lọc theo mine.
        int page = 1, // Trang cần lấy (mặc định 1).
        int pageSize = 5, // Số mục trên mỗi trang (mặc định 5).
        bool mine = false, // true nếu chỉ lấy món của người dùng hiện tại.
        CancellationToken ct = default) // Token hỗ trợ hủy bất đồng bộ.
    {
        // Ghép query string với mine viết thường ("true"/"false") theo chuẩn URL query.
        string url = $"api/v1/foods?page={page}&pageSize={pageSize}&mine={mine.ToString().ToLowerInvariant()}";
        return await SendJsonAsync<object, PagedFoodsDto>( // Gửi GET và deserialize kết quả thành PagedFoodsDto.
            HttpMethod.Get, url, body: null, requiresAuth: mine, ct);
    }

    public async Task<FoodDto> CreateFoodAsync(CreateFoodPayload payload, CancellationToken ct = default) // Gọi POST api/v1/foods để thêm món mới.
    {
        return await SendJsonAsync<CreateFoodPayload, FoodDto>( // Gửi CreateFoodPayload kèm token (requiresAuth: true) và nhận FoodDto.
            HttpMethod.Post, "api/v1/foods", payload, requiresAuth: true, ct);
    }

    public async Task DeleteFoodAsync(string id, CancellationToken ct = default) // Gọi DELETE api/v1/foods/{id} để xóa món ăn.
    {
        if (string.IsNullOrWhiteSpace(id)) // Kiểm tra phía client trước khi gọi mạng nếu id rỗng/khoảng trắng.
        {
            throw new ApiException(400, "Thiếu mã món ăn cần xóa."); // Ném ApiException mã 400.
        }

        // Tạo request HTTP DELETE, escape id để an toàn khi đưa vào đường dẫn URL.
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/foods/{Uri.EscapeDataString(id)}");
        if (!string.IsNullOrWhiteSpace(SessionContext.AccessToken)) // Nếu đang có token trong phiên.
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SessionContext.AccessToken); // Gắn header Authorization: Bearer <token>.
        }

        HttpResponseMessage response; // Biến lưu phản hồi HTTP từ server.
        try
        {
            response = await _http.SendAsync(request, ct); // Gửi request bất đồng bộ tới server.
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) // Bắt timeout (bị hủy nội bộ bởi HttpClient.Timeout chứ không phải do người dùng hủy ct).
        {
            throw new ApiException(408, "Yêu cầu tới máy chủ API bị quá thời gian chờ (timeout)."); // Quy đổi thành ApiException 408.
        }
        catch (HttpRequestException ex) // Bắt lỗi kết nối mạng (server tắt, sai cổng, không có mạng...).
        {
            throw new ApiException(503, $"Không thể kết nối tới máy chủ API ({_http.BaseAddress}): {ex.Message}"); // Quy đổi thành ApiException 503.
        }

        using (response) // Đảm bảo giải phóng HttpResponseMessage sau khi xử lý xong.
        {
            if (response.IsSuccessStatusCode) // Với DELETE thành công, server thường trả 204 No Content (IsSuccessStatusCode = true).
            {
                return; // Kết thúc bình thường, không cần đọc body.
            }

            string content = await response.Content.ReadAsStringAsync(ct); // Đọc nội dung lỗi trả về từ server.
            string message = $"Lỗi máy chủ ({(int)response.StatusCode})."; // Thông báo lỗi mặc định nếu không parse được JSON.
            if (!string.IsNullOrWhiteSpace(content)) // Nếu có nội dung phản hồi.
            {
                try
                {
                    var err = JsonSerializer.Deserialize<ApiErrorDto>(content, JsonOptions); // Thử đọc JSON theo dạng { "message": "..." }.
                    if (!string.IsNullOrWhiteSpace(err?.Message)) // Nếu có Message hợp lệ.
                    {
                        message = err.Message; // Dùng thông báo lỗi chi tiết từ server.
                    }
                }
                catch
                {
                    message = content; // Nếu body không phải JSON ApiErrorDto thì dùng nguyên văn chuỗi nội dung.
                }
            }

            throw new ApiException((int)response.StatusCode, message); // Ném ApiException kèm HTTP status code và message để UI hiển thị.
        }
    }

    public async Task<FoodDto> GetRandomFoodAsync(bool mine = false, CancellationToken ct = default) // Gọi GET api/v1/foods/random.
    {
        string url = $"api/v1/foods/random?mine={mine.ToString().ToLowerInvariant()}"; // Tạo URL kèm tham số query mine.
        return await SendJsonAsync<object, FoodDto>( // Gửi GET và nhận FoodDto của món ngẫu nhiên.
            HttpMethod.Get, url, body: null, requiresAuth: mine, ct);
    }

    // Hàm helper nội bộ gửi request JSON chung cho các endpoint trả về dữ liệu JSON.
    private async Task<TResponse> SendJsonAsync<TRequest, TResponse>(
        HttpMethod method, // Phương thức HTTP (GET, POST, PUT, DELETE...).
        string relativeUrl, // Đường dẫn tương đối so với BaseAddress.
        TRequest? body, // Dữ liệu object cần serialize thành JSON body (null nếu không có body).
        bool requiresAuth, // Cờ báo có cần gắn Bearer token nếu có hay không.
        CancellationToken ct) // Token hủy tác vụ.
    {
        using var request = new HttpRequestMessage(method, relativeUrl); // Khởi tạo request mới cho từng lần gọi.

        if (requiresAuth && !string.IsNullOrWhiteSpace(SessionContext.AccessToken)) // Nếu cần auth và có token hợp lệ.
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SessionContext.AccessToken); // Gắn header Bearer token.
        }

        if (body is not null) // Nếu có dữ liệu gửi lên trong body.
        {
            string json = JsonSerializer.Serialize(body); // Chuyển object C# thành chuỗi JSON.
            request.Content = new StringContent(json, Encoding.UTF8, "application/json"); // Đóng gói JSON UTF-8 với Content-Type application/json.
        }

        HttpResponseMessage response; // Biến giữ phản hồi từ server.
        try
        {
            response = await _http.SendAsync(request, ct); // Gửi request đi và chờ phản hồi.
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) // Nếu quá thời gian Timeout của HttpClient.
        {
            throw new ApiException(408, "Yêu cầu tới máy chủ API bị quá thời gian chờ (timeout)."); // Ném lỗi 408.
        }
        catch (HttpRequestException ex) // Nếu lỗi kết nối tới host/port.
        {
            throw new ApiException(503, $"Không thể kết nối tới máy chủ API ({_http.BaseAddress}): {ex.Message}"); // Ném lỗi 503.
        }

        using (response) // Giải phóng tài nguyên response sau khi dùng xong.
        {
            string content = await response.Content.ReadAsStringAsync(ct); // Đọc toàn bộ body phản hồi dạng chuỗi.

            if (!response.IsSuccessStatusCode) // Nếu HTTP status code không nằm trong 200..299.
            {
                string message = $"Lỗi máy chủ ({(int)response.StatusCode})."; // Thông báo mặc định.
                if (!string.IsNullOrWhiteSpace(content)) // Nếu server có trả nội dung lỗi.
                {
                    try
                    {
                        var err = JsonSerializer.Deserialize<ApiErrorDto>(content, JsonOptions); // Parse JSON thành ApiErrorDto.
                        if (!string.IsNullOrWhiteSpace(err?.Message)) // Nếu có trường Message.
                        {
                            message = err.Message; // Lấy thông báo lỗi từ server.
                        }
                    }
                    catch
                    {
                        message = content; // Fallback sang chuỗi thô nếu không đúng định dạng ApiErrorDto.
                    }
                }

                throw new ApiException((int)response.StatusCode, message); // Ném ApiException kèm mã HTTP để tầng gọi xử lý.
            }

            TResponse? parsed = JsonSerializer.Deserialize<TResponse>(content, JsonOptions); // Chuyển chuỗi JSON thành công sang kiểu TResponse.
            if (parsed is null) // Trường hợp JSON là "null" hoặc rỗng bất thường.
            {
                throw new ApiException((int)response.StatusCode, "Phản hồi JSON từ máy chủ rỗng."); // Ném ApiException báo phản hồi rỗng.
            }

            return parsed; // Trả về đối tượng kết quả đã deserialize.
        }
    }

    public void Dispose() => _http.Dispose(); // Giải phóng HttpClient khi FoodApiClient được dispose.
}