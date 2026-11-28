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
FoodController.cs là controller xử lý các request HTTP liên quan đến tài nguyên món ăn (api/v1/foods). 
Lớp này nhận tham số truy vấn/body/route, kiểm tra xác thực và dữ liệu đầu vào cơ bản, ủy quyền xử lý dữ liệu cho InMemoryDataStore, 
và trả về mã trạng thái HTTP cùng kết quả tương ứng cho client.

1/ Chức năng chính đang thực hiện
- Lấy danh sách món ăn (GET /api/v1/foods): Hỗ trợ phân trang (page, pageSize) và chế độ lọc mine 
(mine = true để xem món của người dùng hiện tại, yêu cầu đăng nhập; mine = false để xem danh sách chung đã gom nhóm).

- Chọn ngẫu nhiên một món ăn (GET /api/v1/foods/random): Lấy ngẫu nhiên một món ăn từ toàn bộ danh sách 
hoặc từ món của người dùng hiện tại (mine = true), trả 404 Not Found nếu không có món phù hợp.

- Tạo mới món ăn (POST /api/v1/foods): Yêu cầu đăng nhập, kiểm tra tính hợp lệ cơ bản của dữ liệu món ăn, 
gọi _store.AddFood(...) để lưu món mới và trả 201 Created, hoặc 409 Conflict nếu trùng lặp.

Xóa món ăn (DELETE /api/v1/foods/{id}): Yêu cầu đăng nhập, gọi _store.DeleteFood(id, currentUser.Id)
để kiểm tra tồn tại và quyền sở hữu, trả 204 No Content khi xóa thành công hoặc các mã lỗi 404 / 403.

2/ Những công việc cụ thể phải làm
- Xác thực người dùng qua token (ResolveCurrentUser): Đọc header Authorization, tách tiền tố "Bearer " 
(không phân biệt hoa/thường) nếu có, cắt khoảng trắng và tra cứu UserAccount qua _store.GetUserByToken(token).

- Kiểm tra quyền truy cập theo từng endpoint: Chặn người dùng chưa đăng nhập (401 Unauthorized) khi gọi CreateFood, 
DeleteFood, hoặc khi gọi GetFoods / GetRandomFood với mine = true.

- Kiểm tra hợp lệ dữ liệu đầu vào khi tạo món: Đảm bảo req không null, Name và Address không rỗng/toàn khoảng trắng, 
và Price không nhỏ hơn 0 (trả 400 Bad Request nếu vi phạm).

- Ánh xạ kết quả nghiệp vụ sang mã HTTP chuẩn: Quy đổi kết quả từ InMemoryDataStore và ngoại lệ InvalidOperationException 
thành 200 OK, 201 Created, 204 No Content, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found hoặc 
409 Conflict kèm ApiErrorResponse.
*/

using Lab04_Bai07_Api.Models;
using Lab04_Bai07_Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Lab04_Bai07_Api.Controllers;

[ApiController] // Bật tự động validate model, binding dữ liệu và trả về lỗi 400 chuẩn API
[Route("api/v1/foods")] // Tiền tố đường dẫn chung cho toàn bộ controller: /api/v1/foods
public class FoodController : ControllerBase
{
    private readonly InMemoryDataStore _store;

    // Tiêm (DI) kho dữ liệu in-memory vào controller qua constructor
    public FoodController(InMemoryDataStore store)
    {
        _store = store;
    }

    // GET /api/v1/foods?page=1&pageSize=5&mine=false
    // Lấy danh sách món ăn có phân trang; hỗ trợ lọc theo món của người dùng hiện tại (mine = true)
    [HttpGet]
    public ActionResult<PagedFoodsResponse> GetFoods(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] bool mine = false)
    {
        // Xác định người dùng từ token trong header Authorization (có thể null nếu chưa đăng nhập)
        UserAccount? currentUser = ResolveCurrentUser();

        // Nếu yêu cầu lọc món của bản thân (mine = true) nhưng chưa đăng nhập -> trả về 401 Unauthorized
        if (mine && currentUser is null)
        {
            return Unauthorized(new ApiErrorResponse { Message = "Cần đăng nhập để xem danh sách món ăn bạn đã đóng góp." });
        }

        // Truy vấn dữ liệu phân trang từ store và trả về HTTP 200 OK
        PagedFoodsResponse result = _store.GetFoods(page, pageSize, mine, currentUser?.Id);
        return Ok(result);
    }

    // GET /api/v1/foods/random?mine=false
    // Chọn ngẫu nhiên một món ăn (tất cả hoặc chỉ thuộc về người dùng hiện tại)
    [HttpGet("random")]
    public ActionResult<FoodResponse> GetRandomFood([FromQuery] bool mine = false)
    {
        UserAccount? currentUser = ResolveCurrentUser();

        // Chưa đăng nhập mà đòi lấy ngẫu nhiên món của mình -> trả về 401 Unauthorized
        if (mine && currentUser is null)
        {
            return Unauthorized(new ApiErrorResponse { Message = "Cần đăng nhập để chọn ngẫu nhiên từ món ăn của bạn." });
        }

        FoodResponse? food = _store.GetRandomFood(mine, currentUser?.Id);

        // Không có món nào thỏa điều kiện -> trả về 404 Not Found
        if (food is null)
        {
            return NotFound(new ApiErrorResponse { Message = "Không tìm thấy món ăn phù hợp." });
        }

        return Ok(food);
    }

    // POST /api/v1/foods
    // Thêm mới một món ăn (yêu cầu đăng nhập và kiểm tra tính hợp lệ của dữ liệu đầu vào)
    [HttpPost]
    public ActionResult<FoodResponse> CreateFood([FromBody] CreateFoodRequest? req)
    {
        UserAccount? currentUser = ResolveCurrentUser();

        // Bắt buộc phải đăng nhập mới được thêm món ăn
        if (currentUser is null)
        {
            return Unauthorized(new ApiErrorResponse { Message = "Vui lòng đăng nhập trước khi thêm món ăn." });
        }

        // Kiểm tra tên món ăn không rỗng / toàn khoảng trắng
        if (req is null || string.IsNullOrWhiteSpace(req.Name))
        {
            return BadRequest(new ApiErrorResponse { Message = "Tên món ăn không được để trống." });
        }

        // Kiểm tra giá món ăn không âm
        if (req.Price < 0)
        {
            return BadRequest(new ApiErrorResponse { Message = "Giá món ăn không được nhỏ hơn 0." });
        }

        // Kiểm tra địa chỉ quán không rỗng / toàn khoảng trắng
        if (string.IsNullOrWhiteSpace(req.Address))
        {
            return BadRequest(new ApiErrorResponse { Message = "Địa chỉ quán không được để trống." });
        }

        try
        {
            // Lưu món ăn mới vào store và trả về HTTP 201 Created kèm dữ liệu món vừa tạo
            FoodResponse created = _store.AddFood(req, currentUser);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (InvalidOperationException ex)
        {
            // Trường hợp xung đột nghiệp vụ (ví dụ trùng tên/dữ liệu) -> trả về 409 Conflict
            return Conflict(new ApiErrorResponse { Message = ex.Message });
        }
    }

    // DELETE /api/v1/foods/{id}
    // Xóa món ăn theo id; chỉ chủ sở hữu (người tạo) mới có quyền xóa
    [HttpDelete("{id}")]
    public IActionResult DeleteFood(string id)
    {
        UserAccount? currentUser = ResolveCurrentUser();

        // Chưa đăng nhập -> trả về 401 Unauthorized
        if (currentUser is null)
        {
            return Unauthorized(new ApiErrorResponse { Message = "Vui lòng đăng nhập trước khi xóa món ăn." });
        }

        // Thực hiện xóa trong store và kiểm tra kết quả (tìm thấy / không đủ quyền)
        var result = _store.DeleteFood(id, currentUser.Id);
        if (!result.Found)
        {
            return NotFound(new ApiErrorResponse { Message = "Không tìm thấy món ăn cần xóa." });
        }

        if (result.Forbidden)
        {
            // Người dùng đã đăng nhập nhưng không phải người tạo món ăn -> trả về 403 Forbidden
            return StatusCode(StatusCodes.Status403Forbidden,
                new ApiErrorResponse { Message = "Bạn chỉ có thể xóa món ăn do chính mình tạo." });
        }

        // Xóa thành công -> trả về 204 No Content (không có nội dung body)
        return NoContent();
    }

    // Đọc header Authorization, tách token (bỏ tiền tố "Bearer " nếu có) và tìm tài khoản tương ứng
    private UserAccount? ResolveCurrentUser()
    {
        string? authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return null;
        }

        const string bearerPrefix = "Bearer ";

        // Nếu header bắt đầu bằng "Bearer " (không phân biệt hoa thường) thì cắt phần prefix và trim khoảng trắng
        string token = authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            ? authHeader[bearerPrefix.Length..].Trim()
            : authHeader.Trim();

        // Tra cứu người dùng theo token; trả về null nếu token không hợp lệ hoặc đã hết hạn
        return _store.GetUserByToken(token);
    }
}