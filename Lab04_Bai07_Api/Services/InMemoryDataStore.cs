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
InMemoryDataStore.cs là lớp trung tâm quản lý dữ liệu và phiên làm việc của API: 
giữ dữ liệu trên RAM để truy xuất nhanh, đồng thời tự động đồng bộ xuống file App_Data/store-data.json 
để không mất dữ liệu khi tắt/khởi động lại server.

1/ Chức năng chính đang đáp ứng: 
- Lưu trữ và khôi phục trạng thái (LoadOrSeed, SaveToDiskLocked, SeedSampleData): Nạp dữ liệu từ store-data.json khi khởi động; 
nếu chưa có hoặc file hỏng/rỗng thì tạo sẵn tài khoản demo và 8 món ăn mẫu rồi ghi an toàn qua file tạm .tmp.

- Quản lý tài khoản và xác thực (UsernameExists, AddUser, Authenticate, GetUserByToken): 
Kiểm tra trùng tên đăng nhập, băm mật khẩu + salt khi tạo tài khoản, xác thực đăng nhập, cấp token ngẫu nhiên gắn với UserId, 
và tra cứu người dùng từ token.

- Nghiệp vụ món ăn (GetFoods, AddFood, DeleteFood, GetRandomFood): Lọc món của người dùng hiện tại (mine = true) 
hoặc gom nhóm theo tên chuẩn hóa cho tab tất cả (mine = false), tính số lượng & danh sách người đóng góp, sắp xếp và phân trang (Paginate).

- Kiểm tra trùng nội dung của cùng một người dùng trước khi thêm món mới.

- Kiểm tra quyền sở hữu (CreatedByUserId) trước khi cho phép xóa món.

- Lấy ngẫu nhiên một món ăn theo toàn bộ danh sách hoặc chỉ món của người dùng hiện tại.

- Ánh xạ dữ liệu nội bộ sang DTO (ToUserResponse, ToFoodResponse): 
Chuyển đổi UserAccount và FoodItem sang UserResponse và FoodResponse để giấu các trường nội bộ/nhạy cảm 
(PasswordHash, PasswordSalt, CreatedByUserId) trước khi trả về cho client.

- Đồng bộ đa luồng (lock (_lock)): Bảo vệ các thao tác đọc/ghi danh sách, từ điển token và file JSON khi nhiều request gọi API cùng lúc.

2/ Những công việc cụ thể mà nó phải làm: 
- Chuẩn hóa và làm sạch dữ liệu đầu vào: Trim() khoảng trắng đầu/cuối, chuẩn hóa ngày sinh (Birthday.Date), 
gán giá trị mặc định (Language, Sex, Description, Image), và chuẩn hóa chuỗi (NormalizeText) để so sánh trùng lặp/gom nhóm chính xác.

- Đảm bảo tính toàn vẹn khi ghi file: Ghi nội dung JSON ra file .tmp trước rồi mới File.Move(..., overwrite: true) 
đè lên store-data.json, tránh làm hỏng file chính nếu ghi dở dang.

- Kiểm soát giới hạn truy vấn: Ép page tối thiểu là 1 và kẹp pageSize trong khoảng 1..100 
để tránh request truyền số trang âm hoặc kích thước trang quá lớn làm nặng server.

- Trả về tín hiệu trạng thái rõ ràng cho tầng trên: Ném InvalidOperationException khi thêm món trùng, 
trả về tuple (bool Found, bool Forbidden) khi xóa món, hoặc trả về null khi xác thực/tra cứu thất bại để 
Controller quy đổi thành mã HTTP phù hợp (400, 401, 403, 404, 409).

*/

// Nạp thư viện hỗ trợ chuyển đổi dữ liệu qua lại giữa object C# và định dạng JSON.
using System.Text.Json;

// Nạp các lớp model và DTO (UserAccount, FoodItem, PagedFoodsResponse...) thuộc tầng Models của API.
using Lab04_Bai07_Api.Models;

// Khai báo namespace thuộc tầng Services của project Web API "Lab04_Bai07_Api".
namespace Lab04_Bai07_Api.Services;

// Lớp quản lý dữ liệu người dùng, phiên token và món ăn trên bộ nhớ RAM, đồng thời lưu xuống file JSON.
public class InMemoryDataStore
{
    // Lớp nội bộ đóng gói toàn bộ trạng thái cần ghi ra/đọc từ file JSON trên đĩa.
    // Đóng gói chung để lưu và đọc đồng bộ cả người dùng, token và món ăn trên một file JSON duy nhất, đồng thời ẩn cấu trúc lưu trữ khỏi bên ngoài.
    private sealed class PersistedState
    {
        // Danh sách tất cả tài khoản người dùng.
        public List<UserAccount> Users { get; set; } = [];

        // Bảng ánh xạ từ chuỗi AccessToken sang UserId để xác định người dùng đang đăng nhập.
        public Dictionary<string, string> TokensToUserId { get; set; } = new();

        // Danh sách tất cả món ăn được tạo hoặc đóng góp.
        public List<FoodItem> Foods { get; set; } = [];
    }

    // Cấu hình serialize/deserialize JSON chung, bật WriteIndented để file JSON dễ đọc hơn.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    // Đối tượng khóa đồng bộ (lock) giúp tránh xung đột dữ liệu khi nhiều request truy cập cùng lúc.
    private readonly object _lock = new();

    // Đường dẫn đầy đủ tới file lưu trữ dữ liệu "store-data.json".
    private readonly string _dataFilePath;

    // Danh sách tài khoản giữ trên RAM để tra cứu và xử lý nhanh.
    private readonly List<UserAccount> _users = [];

    // Từ điển ánh xạ token -> userId giữ trên RAM.
    private readonly Dictionary<string, string> _tokensToUserId = new();

    // Danh sách món ăn giữ trên RAM.
    private readonly List<FoodItem> _foods = [];

    // Constructor nhận IWebHostEnvironment để xác định thư mục gốc của ứng dụng API.
    public InMemoryDataStore(IWebHostEnvironment env)
    {
        // Ghép đường dẫn thư mục App_Data nằm trong ContentRootPath của API.
        string dataDir = Path.Combine(env.ContentRootPath, "App_Data");

        // Tạo thư mục App_Data nếu chưa tồn tại (nếu đã có thì bỏ qua an toàn).
        Directory.CreateDirectory(dataDir);

        // Xác định đường dẫn file lưu trữ App_Data/store-data.json.
        _dataFilePath = Path.Combine(dataDir, "store-data.json");

        // Nạp dữ liệu từ file JSON nếu có, hoặc tạo dữ liệu mẫu ban đầu nếu chưa có/rỗng.
        LoadOrSeed();
    }

    // Đọc dữ liệu từ file JSON vào RAM; nếu chưa có file, file lỗi hoặc rỗng thì seed dữ liệu mẫu.
    private void LoadOrSeed()
    {
        // Khóa luồng để đảm bảo quá trình đọc/khởi tạo dữ liệu diễn ra an toàn.
        lock (_lock)
        {
            // Kiểm tra xem file store-data.json có tồn tại trên đĩa không.
            if (File.Exists(_dataFilePath))
            {
                try
                {
                    // Đọc toàn bộ nội dung văn bản từ file JSON.
                    string json = File.ReadAllText(_dataFilePath);

                    // Chuyển chuỗi JSON thành đối tượng PersistedState.
                    PersistedState? state = JsonSerializer.Deserialize<PersistedState>(json, JsonOptions);

                    // Nếu deserialize thành công (không null).
                    if (state is not null)
                    {
                        // Nạp danh sách người dùng từ file vào bộ nhớ _users.
                        _users.AddRange(state.Users);

                        // Nạp từng cặp token - userId vào từ điển _tokensToUserId.
                        foreach (var pair in state.TokensToUserId)
                        {
                            _tokensToUserId[pair.Key] = pair.Value;
                        }

                        // Nạp danh sách món ăn từ file vào bộ nhớ _foods.
                        _foods.AddRange(state.Foods);

                        // Nếu đã có dữ liệu user hoặc món ăn thì kết thúc, không cần seed lại.
                        if (_users.Count > 0 || _foods.Count > 0)
                        {
                            return;
                        }
                    }
                }
                catch
                {
                    // Nếu file JSON bị lỗi định dạng thì seed lại dữ liệu mặc định
                }
            }

            // Tạo tài khoản demo và danh sách món ăn mẫu ban đầu.
            SeedSampleData();

            // Ghi dữ liệu mẫu vừa tạo xuống file JSON.
            SaveToDiskLocked();
        }
    }

    // Ghi dữ liệu hiện tại trên RAM xuống file JSON một cách an toàn bằng file tạm (.tmp).
    private void SaveToDiskLocked()
    {
        // Gom các collection hiện tại vào một đối tượng PersistedState.
        var state = new PersistedState
        {
            Users = _users,
            TokensToUserId = _tokensToUserId,
            Foods = _foods
        };

        // Serialize đối tượng state thành chuỗi JSON đẹp mắt.
        string json = JsonSerializer.Serialize(state, JsonOptions);

        // Tạo đường dẫn file tạm để tránh ghi đè dở dang làm hỏng file chính nếu có lỗi.
        string tempFile = _dataFilePath + ".tmp";

        // Ghi nội dung JSON ra file tạm.
        File.WriteAllText(tempFile, json);

        // Thay thế file chính bằng file tạm (ghi đè nếu đã tồn tại).
        File.Move(tempFile, _dataFilePath, overwrite: true);
    }

    // Khởi tạo dữ liệu mẫu gồm 1 tài khoản demo và 8 món ăn gợi ý ban đầu.
    private void SeedSampleData()
    {
        // Băm mật khẩu mẫu "Demo@1234" kèm salt ngẫu nhiên.
        var (hash, salt) = PasswordHasher.HashPassword("Demo@1234");

        // Tạo tài khoản người dùng mẫu.
        var demoUser = new UserAccount
        {
            Id = "user-demo-1",
            Username = "demo_user",
            Email = "demo@example.com",
            FirstName = "Minh",
            LastName = "Nguyễn",
            Phone = "0901234567",
            Birthday = new DateTime(2005, 1, 1),
            Language = "vi",
            Sex = "Nam",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        // Thêm tài khoản mẫu vào danh sách _users.
        _users.Add(demoUser);

        // Mảng tên các món ăn mẫu.
        string[] sampleNames =
        [
            "Cơm tấm sườn bì chả",
            "Phở bò tái nạm",
            "Bún chả Hà Nội",
            "Bánh mì thịt nướng",
            "Hủ tiếu Nam Vang",
            "Cơm gà xối mỡ",
            "Bún riêu cua đồng",
            "Mì quảng gà"
        ];

        // Mảng giá tương ứng cho từng món ăn mẫu.
        decimal[] samplePrices = [45000, 55000, 50000, 25000, 48000, 45000, 42000, 45000];

        // Mảng địa chỉ tương ứng cho từng món ăn mẫu.
        string[] sampleAddresses =
        [
            "Quận 1, TP.HCM",
            "Quận 3, TP.HCM",
            "Quận 5, TP.HCM",
            "Thủ Đức, TP.HCM",
            "Quận 10, TP.HCM",
            "Phú Nhuận, TP.HCM",
            "Bình Thạnh, TP.HCM",
            "Quận 1, TP.HCM"
        ];

        // Vòng lặp tạo từng đối tượng FoodItem mẫu và thêm vào _foods.
        for (int i = 0; i < sampleNames.Length; i++)
        {
            _foods.Add(new FoodItem
            {
                Id = $"food-{i + 1}",
                Name = sampleNames[i],
                Price = samplePrices[i],
                Address = sampleAddresses[i],
                Description = "Món ăn gợi ý mặc định để kiểm thử API",
                Image = string.Empty,
                CreatedByUserId = demoUser.Id,
                CreatedByUsername = demoUser.Username,
                // Gán thời điểm tạo lệch nhau từng phút để thứ tự ban đầu rõ ràng.
                CreatedAt = DateTime.UtcNow.AddMinutes(-sampleNames.Length + i)
            });
        }
    }

    // Kiểm tra xem tên đăng nhập đã tồn tại chưa (không phân biệt hoa thường).
    public bool UsernameExists(string username)
    {
        // Khóa luồng khi đọc danh sách _users.
        lock (_lock)
        {
            // Kiểm tra có tài khoản nào trùng tên đăng nhập (không phân biệt hoa/thường) hay chưa để chặn đăng ký trùng.
            return _users.Any(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Tạo người dùng mới từ RegisterRequest, lưu vào RAM và ghi xuống file JSON.
    public UserAccount AddUser(RegisterRequest req)
    {
        // Khóa luồng để đảm bảo thêm user và ghi file đồng bộ.
        lock (_lock)
        {
            // Băm mật khẩu người dùng nhập trước khi lưu.
            var (hash, salt) = PasswordHasher.HashPassword(req.Password);

            // Tạo thực thể UserAccount với dữ liệu đã chuẩn hóa (Trim, giá trị mặc định).
            var user = new UserAccount
            {
                Username = req.Username.Trim(),
                Email = req.Email.Trim(),
                FirstName = req.FirstName.Trim(),
                LastName = req.LastName.Trim(),
                Phone = req.Phone.Trim(),
                Birthday = req.Birthday.Date,
                Language = string.IsNullOrWhiteSpace(req.Language) ? "Tiếng Việt" : req.Language.Trim(),
                Sex = string.IsNullOrWhiteSpace(req.Sex) ? "Nam" : req.Sex.Trim(),
                PasswordHash = hash,
                PasswordSalt = salt
            };

            // Thêm tài khoản mới vào bộ nhớ _users.
            _users.Add(user);

            // Lưu trạng thái mới xuống file JSON.
            SaveToDiskLocked();

            // Trả về tài khoản vừa tạo.
            return user;
        }
    }

    // Xác thực đăng nhập bằng username và password; nếu đúng thì cấp token mới và trả về (Token, User).
    public (string Token, UserAccount User)? Authenticate(string username, string password)
    {
        // Khóa luồng khi kiểm tra thông tin đăng nhập và lưu token.
        lock (_lock)
        {
            // Tìm tài khoản khớp username (không phân biệt chữ hoa/thường).
            var user = _users.FirstOrDefault(u => string.Equals(u.Username, username.Trim(), StringComparison.OrdinalIgnoreCase));

            // Nếu không tìm thấy user hoặc mật khẩu không khớp hash+salt thì trả về null.
            if (user is null || !PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            // Sinh chuỗi token ngẫu nhiên kết hợp Base64 của GUID và chuỗi GUID dạng hex.
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +Guid.NewGuid().ToString("N");

            // Lưu ánh xạ token -> user.Id vào từ điển phiên làm việc.
            _tokensToUserId[token] = user.Id;

            // Lưu cập nhật xuống file JSON.
            SaveToDiskLocked();

            // Trả về tuple chứa token và thông tin người dùng.
            return (token, user);
        }
    }

    // Tra cứu tài khoản người dùng tương ứng với access token.
    public UserAccount? GetUserByToken(string? token)
    {
        // Nếu token rỗng hoặc null thì không hợp lệ.
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        // Khóa luồng khi tra cứu token và danh sách người dùng.
        lock (_lock)
        {
            // Tìm userId gắn với token; nếu không tồn tại thì trả về null.
            if (!_tokensToUserId.TryGetValue(token, out string? userId))
            {
                return null;
            }

            // Trả về UserAccount có Id khớp với userId.
            return _users.FirstOrDefault(u => u.Id == userId);
        }
    }

    // Lấy danh sách món ăn có phân trang; hỗ trợ lọc theo món của người dùng hiện tại hoặc gom nhóm tất cả.
    public PagedFoodsResponse GetFoods(int page, int pageSize, bool mine, string? currentUserId)
    {
        // Đảm bảo số trang tối thiểu là 1.
        page = Math.Max(1, page);

        // Giới hạn kích thước trang trong khoảng [1, 100].
        pageSize = Math.Clamp(pageSize, 1, 100);

        // Chụp bản sao danh sách _foods trong vùng lock để thao tác an toàn.
        List<FoodItem> sourceItems;
        lock (_lock)
        {
            sourceItems = _foods.ToList();
        }

        // Trường hợp xem tab "Của tôi" (mine == true).
        if (mine)
        {
            // Nếu chưa có currentUserId hợp lệ thì trả về danh sách rỗng phân trang.
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return new PagedFoodsResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = 0,
                    TotalPages = 1,
                    Items = []
                };
            }

            // Lọc các món do currentUserId tạo, sắp xếp mới nhất trước và chuyển sang FoodResponse.
            var myFoods = sourceItems
                .Where(f => f.CreatedByUserId == currentUserId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => ToFoodResponse(f, contributorCount: 1, contributors: [f.CreatedByUsername]))
                .ToList();

            // Phân trang kết quả món ăn của người dùng.
            return Paginate(myFoods, page, pageSize);
        }

        // Tab All: gom theo tên món (có thể trùng tên do nhiều người đóng góp)
        var groupedFoods = sourceItems
            // Gom nhóm các món có cùng tên sau khi chuẩn hóa chữ thường và khoảng trắng.
            .GroupBy(f => NormalizeText(f.Name))
            .Select(g =>
            {
                // Lấy bản ghi tạo mới nhất trong nhóm làm đại diện hiển thị.
                FoodItem latest = g.OrderByDescending(x => x.CreatedAt).First();

                // Lấy danh sách username đóng góp duy nhất (không phân biệt hoa/thường), bỏ qua giá trị rỗng.
                List<string> distinctContributors = g
                    .Select(x => x.CreatedByUsername)
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // Số lượng người đóng góp ít nhất là 1.
                int count = Math.Max(1, distinctContributors.Count);

                // Tạo FoodResponse kèm số lượng và danh sách người đóng góp.
                return ToFoodResponse(latest, count, distinctContributors);
            })
            // Ưu tiên món được nhiều người đóng góp lên trước, sau đó theo thời gian tạo mới nhất
            .OrderByDescending(x => x.ContributorCount)
            .ThenByDescending(x => x.CreatedAt)
            .ToList();

        // Phân trang danh sách món đã gom nhóm.
        return Paginate(groupedFoods, page, pageSize);
    }

    // Cắt danh sách FoodResponse theo số trang (page) và kích thước trang (pageSize).
    private static PagedFoodsResponse Paginate(List<FoodResponse> items, int page, int pageSize)
    {
        // Tổng số phần tử trước khi phân trang.
        int totalItems = items.Count;

        // Tính tổng số trang, làm tròn lên và đảm bảo ít nhất là 1 trang.
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));

        // Đảm bảo trang hiện tại nằm trong khoảng hợp lệ [1, totalPages].
        int safePage = Math.Clamp(page, 1, totalPages);

        // Bỏ qua các phần tử của các trang trước và lấy đúng số lượng phần tử của trang hiện tại.
        var pagedItems = items
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Trả về đối tượng PagedFoodsResponse hoàn chỉnh.
        return new PagedFoodsResponse
        {
            Page = safePage,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = pagedItems
        };
    }

    // Chuẩn hóa chuỗi văn bản: cắt khoảng trắng đầu/cuối, chuyển chữ thường và gộp nhiều khoảng trắng thành 1 dấu cách.
    private static string NormalizeText(string? value)
    {
        return string.Join(" ", (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    // Thêm món ăn mới cho người dùng hiện tại, kiểm tra trùng nội dung của cùng một người dùng.
    public FoodResponse AddFood(CreateFoodRequest req, UserAccount user)
    {
        // Chuẩn hóa các trường văn bản để so sánh chống trùng lặp chính xác hơn.
        string normName = NormalizeText(req.Name);
        string normAddress = NormalizeText(req.Address);
        string normDescription = NormalizeText(req.Description);

        // Khóa luồng khi kiểm tra trùng lặp, thêm món và lưu file.
        lock (_lock)
        {
            // Kiểm tra xem cùng user này đã từng thêm món có cùng tên, địa chỉ, giá và mô tả chưa.
            bool duplicatedBySameUser = _foods.Any(f =>
                f.CreatedByUserId == user.Id &&
                NormalizeText(f.Name) == normName &&
                NormalizeText(f.Address) == normAddress &&
                f.Price == req.Price &&
                NormalizeText(f.Description) == normDescription);

            // Nếu đã đóng góp nội dung y hệt thì ném ngoại lệ nghiệp vụ.
            if (duplicatedBySameUser)
            {
                throw new InvalidOperationException("Bạn đã đóng góp món ăn với nội dung này rồi.");
            }

            // Khởi tạo bản ghi FoodItem mới.
            var food = new FoodItem
            {
                Name = req.Name.Trim(),
                Price = req.Price,
                Address = req.Address.Trim(),
                Description = (req.Description ?? string.Empty).Trim(),
                Image = (req.Image ?? string.Empty).Trim(),
                CreatedByUserId = user.Id,
                CreatedByUsername = user.Username
            };

            // Thêm món ăn mới vào danh sách _foods.
            _foods.Add(food);

            // Ghi cập nhật xuống file JSON.
            SaveToDiskLocked();

            // Trả về FoodResponse cho client.
            return ToFoodResponse(food);
        }
    }

    // Xóa món ăn theo foodId; trả về (Found: có tồn tại món không, Forbidden: có phải không phải chủ sở hữu không).
    public (bool Found, bool Forbidden) DeleteFood(string foodId, string currentUserId)
    {
        // Khóa luồng khi tìm, kiểm tra quyền và xóa món ăn.
        lock (_lock)
        {
            // Tìm món ăn theo Id.
            var food = _foods.FirstOrDefault(f => f.Id == foodId);
            if (food is null)
            {
                // Không tìm thấy món ăn.
                return (false, false);
            }

            // Chỉ người tạo món (CreatedByUserId == currentUserId) mới có quyền xóa.
            if (food.CreatedByUserId != currentUserId)
            {
                // Tìm thấy món nhưng bị cấm xóa (Forbidden).
                return (true, true);
            }

            // Xóa món ăn khỏi danh sách trên RAM.
            _foods.Remove(food);

            // Ghi trạng thái sau khi xóa xuống file JSON.
            SaveToDiskLocked();

            // Xóa thành công.
            return (true, false);
        }
    }

    // Chọn ngẫu nhiên một món ăn (có thể lọc chỉ lấy món của người dùng hiện tại nếu mineOnly == true).
    public FoodResponse? GetRandomFood(bool mineOnly, string? currentUserId)
    {
        // Khóa luồng khi đọc danh sách món ăn.
        lock (_lock)
        {
            IEnumerable<FoodItem> query = _foods;

            // Nếu chỉ lấy món của người dùng hiện tại.
            if (mineOnly)
            {
                query = string.IsNullOrEmpty(currentUserId)
                    ? []
                    : query.Where(f => f.CreatedByUserId == currentUserId);
            }

            // Chuyển sang List để biết tổng số lượng và truy cập theo chỉ số.
            List<FoodItem> list = query.ToList();
            if (list.Count == 0)
            {
                return null;
            }

            // Chọn ngẫu nhiên một chỉ số hợp lệ và trả về FoodResponse tương ứng.
            int index = Random.Shared.Next(list.Count);
            return ToFoodResponse(list[index]);
        }
    }

    // Chuyển từ UserAccount sang UserResponse để ẩn PasswordHash/PasswordSalt/CreatedAt khỏi client và giữ cấu trúc phản hồi API ổn định.
    public static UserResponse ToUserResponse(UserAccount user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Phone = user.Phone,
        Birthday = user.Birthday,
        Language = user.Language,
        Sex = user.Sex
    };

    // Chuyển từ FoodItem sang FoodResponse để ẩn CreatedByUserId nội bộ và giữ đúng cấu trúc dữ liệu trả về cho client dù FoodItem đổi cách lưu trữ.
    public static FoodResponse ToFoodResponse(FoodItem food) => new()
    {
        Id = food.Id,
        Name = food.Name,
        Price = food.Price,
        Address = food.Address,
        Description = food.Description,
        Image = food.Image,
        CreatedByUsername = food.CreatedByUsername,
        CreatedAt = food.CreatedAt
    };

    // Chuyển từ FoodItem sang FoodResponse để ẩn CreatedByUserId nội bộ và gắn thêm dữ liệu tổng hợp (số lượng và danh sách người đóng góp) phục vụ hiển thị cho client.
    private static FoodResponse ToFoodResponse(FoodItem item, int contributorCount, List<string> contributors)
    {
        return new FoodResponse
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Address = item.Address,
            Description = item.Description,
            Image = item.Image,
            CreatedByUsername = item.CreatedByUsername,
            CreatedAt = item.CreatedAt,
            ContributorCount = contributorCount,
            Contributors = contributors
        };
    }
}