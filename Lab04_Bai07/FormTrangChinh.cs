using System; // Cung cấp các kiểu cơ bản của .NET như String, Math, Uri, EventArgs.
using System.Collections.Generic; // Cung cấp IEnumerable<T> và List<T>.
using System.ComponentModel; // Hỗ trợ thuộc tính thiết kế của WinForms.
using System.Data; // Cung cấp các kiểu dữ liệu cơ bản của Data.
using System.Drawing; // Cung cấp Color, Font, Point, Image, Bitmap cho giao diện.
using System.Linq; // Cung cấp các phương thức mở rộng LINQ như ToList() và OfType<T>().
using System.Threading.Tasks; // Cung cấp Task và async/await cho thao tác bất đồng bộ.
using System.Windows.Forms; // Nạp thư viện giao diện WinForms (Form, FlowLayoutPanel, ComboBox, MessageBox...).

using Lab04_Bai07.Services; // Nạp FoodApiClient, SessionContext và các DTO (PagedFoodsDto, FoodDto, UserDto...).

namespace Lab04_Bai07; // Khai báo namespace thuộc ứng dụng WinForms Lab04_Bai07.

public partial class FormTrangChinh : Form // Form màn hình chính hiển thị danh sách món ăn, phân trang và điều hướng tính năng.
{
    // Cờ chặn sự kiện SelectedIndexChanged của ComboBox chạy chồng chéo khi code đang nạp lại danh sách trang.
    private bool _isUpdatingPagination;

    public FormTrangChinh() // Constructor khởi tạo form chính.
    {
        InitializeComponent(); // Khởi tạo các control trên giao diện theo file Designer.
        InitializePageSizeDefault(); // Nạp các mức kích thước trang mặc định (5, 10, 20) vào cboPageSize.
        UpdateWelcomeLabel(); // Cập nhật lời chào trên lblWelcome theo thông tin người dùng hiện tại.

        this.Load += async (_, _) => await LoadCurrentTabAsync(page: 1); // Tải dữ liệu trang 1 của tab hiện tại khi form mở xong.
        tabMain.SelectedIndexChanged += async (_, _) => await LoadCurrentTabAsync(page: 1); // Khi chuyển tab (Cộng đồng / Của tôi), tải lại từ trang 1.
        cboPage.SelectedIndexChanged += async (_, _) => // Khi người dùng chọn số trang mới trên cboPage.
        {
            if (!_isUpdatingPagination && cboPage.SelectedItem is int page) // Chỉ tải nếu không đang cập nhật nội bộ và chọn đúng số trang int.
            {
                await LoadCurrentTabAsync(page); // Tải danh sách món ăn ở trang được chọn.
            }
        };
        cboPageSize.SelectedIndexChanged += async (_, _) => // Khi người dùng đổi số bản ghi mỗi trang (5/10/20).
        {
            if (!_isUpdatingPagination) // Chỉ phản ứng khi người dùng chủ động thay đổi.
            {
                await LoadCurrentTabAsync(page: 1); // Quay về trang 1 và tải lại theo pageSize mới.
            }
        };
    }

    private void InitializePageSizeDefault() // Thiết lập danh sách lựa chọn số món/trang mặc định.
    {
        if (cboPageSize.Items.Count == 0) // Chỉ thêm nếu ComboBox chưa có item nào.
        {
            cboPageSize.Items.AddRange([5, 10, 20]); // Thêm các lựa chọn 5, 10, 20 món mỗi trang.
        }

        if (cboPageSize.SelectedIndex < 0) // Nếu chưa chọn mục nào.
        {
            cboPageSize.SelectedIndex = 0; // Mặc định chọn mục đầu tiên (5 món/trang).
        }
    }

    private void UpdateWelcomeLabel() // Hiển thị lời chào kèm tên người dùng hoặc "User" nếu chưa đăng nhập.
    {
        var user = SessionContext.CurrentUser; // Lấy thông tin người dùng đang đăng nhập từ SessionContext.
        if (user is null) // Nếu chưa có thông tin user.
        {
            lblWelcome.Text = "Welcome, User"; // Hiển thị lời chào mặc định.
            return; // Kết thúc hàm.
        }

        string fullName = $"{user.LastName} {user.FirstName}".Trim(); // Ghép Họ và Tên và cắt khoảng trắng thừa.
        string displayName = !string.IsNullOrWhiteSpace(fullName) ? fullName : user.Username; // Ưu tiên họ tên đầy đủ, nếu trống thì dùng username.
        lblWelcome.Text = $"Welcome, {displayName}"; // Gán chuỗi lời chào lên label.
    }

    private bool IsMineTabSelected() // Kiểm tra xem tab hiện tại có phải tab "Món ăn của tôi" hay không.
    {
        return tabMain.SelectedTab == tabMyContributions; // Trả về true nếu đang ở tabMyContributions.
    }

    private async Task LoadCurrentTabAsync(int page = 1) // Điều phối tải dữ liệu đúng theo tab đang mở.
    {
        if (IsMineTabSelected()) // Nếu đang ở tab món ăn của tôi.
        {
            await LoadMyFoodsAsync(page); // Tải danh sách món ăn do user hiện tại đóng góp.
        }
        else // Ngược lại là tab danh sách chung của cộng đồng.
        {
            await LoadAllFoodsAsync(page); // Tải danh sách món ăn cộng đồng.
        }
    }

    private void btnAnGiGio_Click(object sender, EventArgs e) // Mở form chức năng chọn ngẫu nhiên "Ăn gì giờ".
    {
        using var randomForm = new FormCNAnGiGio(); // Khởi tạo FormCNAnGiGio.
        randomForm.ShowDialog(this); // Hiển thị dạng hộp thoại modal gắn với FormTrangChinh.
    }

    private async void btnThemMonAn_Click(object sender, EventArgs e) // Mở form thêm món ăn và làm mới danh sách nếu tạo thành công.
    {
        using var addForm = new FormCNThemMonAn(); // Khởi tạo FormCNThemMonAn.
        if (addForm.ShowDialog(this) == DialogResult.OK) // Nếu người dùng thêm món thành công (DialogResult.OK).
        {
            await LoadCurrentTabAsync(page: 1); // Tải lại danh sách từ trang 1 để cập nhật món mới.
        }
    }

    private async Task LoadAllFoodsAsync(int page = 1) // Gọi API lấy danh sách món ăn cộng đồng (mine: false).
    {
        int pageSize = GetSelectedPageSize(); // Lấy số lượng món mỗi trang hiện tại.

        try
        {
            using var api = new FoodApiClient("http://localhost:5167"); // Khởi tạo client gọi Web API.
            PagedFoodsDto result = await api.GetFoodsAsync(page, pageSize, mine: false); // Gọi GET api/v1/foods với mine = false.

            RenderFoodsIntoPanel(pnlAllItems, result.Items, isAllTab: true); // Render danh sách thẻ món ăn vào pnlAllItems.
            UpdatePageComboBox(result.Page, result.TotalPages); // Cập nhật số trang hiện tại và tổng số trang lên cboPage.
        }
        catch (ApiException ex) // Bắt lỗi khi gọi API hoặc lỗi kết nối mạng/timeout.
        {
            MessageBox.Show(ex.Message, "Lỗi tải danh sách món ăn", // Hiển thị thông báo lỗi.
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadMyFoodsAsync(int page = 1) // Gọi API lấy danh sách món ăn của người dùng hiện tại (mine: true).
    {
        int pageSize = GetSelectedPageSize(); // Lấy pageSize đang chọn.

        if (string.IsNullOrWhiteSpace(SessionContext.AccessToken)) // Nếu chưa đăng nhập / không có token.
        {
            pnlMyItems.Controls.Clear(); // Xóa các control cũ đang hiển thị trên pnlMyItems.
            MessageBox.Show("Bạn cần đăng nhập để xem các món bạn đã đóng góp.", // Thông báo yêu cầu đăng nhập.
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Dừng không gọi API.
        }

        try
        {
            using var api = new FoodApiClient("http://localhost:5167"); // Khởi tạo client gọi Web API.
            PagedFoodsDto result = await api.GetFoodsAsync(page, pageSize, mine: true); // Gọi GET api/v1/foods với mine = true (kèm Bearer token).

            RenderFoodsIntoPanel(pnlMyItems, result.Items, isAllTab: false); // Render các thẻ món ăn của user vào pnlMyItems.
            UpdatePageComboBox(result.Page, result.TotalPages); // Cập nhật ComboBox phân trang.
        }
        catch (ApiException ex) // Bắt lỗi từ API hoặc lỗi mạng.
        {
            MessageBox.Show(ex.Message, "Lỗi tải món ăn đã đóng góp", // Báo lỗi cho người dùng.
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RenderFoodsIntoPanel(FlowLayoutPanel targetPanel, IEnumerable<FoodDto> items, bool isAllTab) // Tạo các card món ăn trong FlowLayoutPanel.
    {
        DisposeCardImages(targetPanel); // Giải phóng ảnh cũ trước khi xóa control để tránh rò rỉ bộ nhớ GDI+.
        targetPanel.SuspendLayout(); // Tạm dừng bố cục panel trong lúc thêm nhiều control để render mượt hơn.
        targetPanel.Controls.Clear(); // Xóa toàn bộ control cũ trong panel.

        List<FoodDto> foodList = items.ToList(); // Chuyển IEnumerable sang List để đếm số lượng và duyệt nhiều lần nếu cần.
        if (foodList.Count == 0) // Nếu trang hiện tại không có món ăn nào.
        {
            var lblEmpty = new Label // Tạo label thông báo danh sách rỗng.
            {
                Text = isAllTab
                    ? "Chưa có món ăn nào trong cộng đồng." // Thông báo khi tab cộng đồng rỗng.
                    : "Bạn chưa đóng góp món ăn nào.", // Thông báo khi tab cá nhân rỗng.
                AutoSize = true, // Tự động co giãn theo độ dài chữ.
                Margin = new Padding(10) // Khoảng cách lề ngoài 10px.
            };
            targetPanel.Controls.Add(lblEmpty); // Thêm label vào panel.
            targetPanel.ResumeLayout(); // Khôi phục tính toán bố cục panel.
            return; // Kết thúc hàm.
        }

        int cardWidth = Math.Max(300, targetPanel.ClientSize.Width - 28); // Tính chiều rộng thẻ theo panel (trừ chừa khoảng trống/scrollbar, tối thiểu 300px).
        const int cardHeight = 104; // Chiều cao cố định mỗi thẻ món ăn.
        const int imgSize = 80; // Kích thước vuông (80x80) của ảnh món ăn trên thẻ.
        const int textLeft = 100; // Tọa độ X bắt đầu của các label thông tin chữ bên phải ảnh.

        foreach (FoodDto food in foodList) // Tạo một Panel card cho từng món ăn.
        {
            var card = new Panel // Khung thẻ chứa ảnh và thông tin một món ăn.
            {
                Width = cardWidth, // Gán chiều rộng thẻ.
                Height = cardHeight, // Gán chiều cao thẻ.
                BorderStyle = BorderStyle.FixedSingle, // Vẽ viền mảnh xung quanh thẻ.
                Margin = new Padding(4), // Khoảng cách giữa các thẻ.
                Padding = new Padding(8) // Khoảng cách đệm bên trong thẻ.
            };

            var picFood = CreateFoodPictureBox(food.Image, imgSize, left: 10, top: 11); // Tạo PictureBox hiển thị ảnh món ăn ở bên trái thẻ.

            string contributorsText = food.Contributors.Count > 0 // Nếu có danh sách người đóng góp thì ghép tên bằng dấu phẩy.
                ? string.Join(", ", food.Contributors)
                : food.CreatedByUsername; // Ngược lại dùng username người tạo ban đầu.

            string titleText = isAllTab // Ở tab cộng đồng hiển thị thêm số người đóng góp; ở tab cá nhân chỉ hiển thị tên và giá.
                ? $"{food.Name}  —  {food.Price:N0} đ  ({food.ContributorCount} người đóng góp)"
                : $"{food.Name}  —  {food.Price:N0} đ";

            var lblTitle = new Label // Label tiêu đề (tên món, giá, số người đóng góp).
            {
                Text = titleText, // Gán nội dung tiêu đề.
                Font = new Font(Font, FontStyle.Bold), // In đậm chữ tiêu đề dựa trên font hiện tại của form.
                AutoSize = true, // Tự điều chỉnh kích thước theo nội dung.
                Location = new Point(textLeft, 10) // Đặt ở hàng trên cùng bên phải ảnh.
            };

            var lblAddress = new Label // Label hiển thị địa chỉ quán.
            {
                Text = $"Địa chỉ: {food.Address}", // Gán địa chỉ.
                AutoSize = true, // Tự động co giãn.
                Location = new Point(textLeft, 36) // Đặt ở hàng giữa bên phải ảnh.
            };

            var lblContributors = new Label // Label hiển thị danh sách người đóng góp.
            {
                Text = $"Đóng góp: {contributorsText}", // Gán tên người đóng góp.
                AutoSize = true, // Tự động co giãn.
                Location = new Point(textLeft, 62) // Đặt ở hàng dưới bên phải ảnh.
            };

            card.Controls.Add(picFood); // Gắn PictureBox vào card.
            card.Controls.Add(lblTitle); // Gắn label tiêu đề vào card.
            card.Controls.Add(lblAddress); // Gắn label địa chỉ vào card.
            card.Controls.Add(lblContributors); // Gắn label người đóng góp vào card.
            targetPanel.Controls.Add(card); // Gắn card hoàn chỉnh vào FlowLayoutPanel.
        }

        targetPanel.ResumeLayout(); // Bật lại layout và sắp xếp các card trên panel.
    }

    private static PictureBox CreateFoodPictureBox(string? imagePathOrUrl, int size, int left, int top) // Tạo PictureBox và tải ảnh từ URL hoặc file local.
    {
        var pic = new PictureBox // Khởi tạo control hiển thị ảnh.
        {
            Width = size, // Chiều rộng ảnh.
            Height = size, // Chiều cao ảnh.
            Location = new Point(left, top), // Vị trí của PictureBox bên trong card.
            SizeMode = PictureBoxSizeMode.Zoom, // Phóng to/thu nhỏ giữ đúng tỷ lệ khung hình.
            BorderStyle = BorderStyle.FixedSingle, // Viền mảnh quanh khung ảnh.
            BackColor = Color.FromArgb(245, 245, 245) // Màu nền xám nhạt khi chưa có ảnh hoặc đang tải.
        };

        if (string.IsNullOrWhiteSpace(imagePathOrUrl)) // Nếu không có thông tin ảnh thì trả về khung trống.
        {
            return pic;
        }

        try
        {
            // Kiểm tra xem có phải URL tuyệt đối dùng giao thức HTTP hoặc HTTPS không.
            if (Uri.TryCreate(imagePathOrUrl, UriKind.Absolute, out Uri? uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                pic.LoadAsync(uri.ToString()); // Tải ảnh bất đồng bộ từ mạng.
                return pic; // Trả về PictureBox.
            }

            if (System.IO.File.Exists(imagePathOrUrl)) // Nếu là đường dẫn file trên máy và file tồn tại.
            {
                using var stream = new System.IO.FileStream(imagePathOrUrl, System.IO.FileMode.Open, System.IO.FileAccess.Read); // Mở file chế độ chỉ đọc để tránh khóa file.
                using var tempImage = Image.FromStream(stream); // Đọc Image tạm từ stream.
                pic.Image = new Bitmap(tempImage); // Clone sang Bitmap riêng để an toàn khi đóng stream.
            }
        }
        catch
        {
            // Giữ khung ảnh trống nếu đường dẫn lỗi hoặc không tải được ảnh
        }

        return pic; // Trả về PictureBox đã cấu hình.
    }

    private static void DisposeCardImages(FlowLayoutPanel panel) // Giải phóng toàn bộ Image trong các PictureBox của các card thuộc panel.
    {
        foreach (Control ctrl in panel.Controls) // Duyệt từng control con trong panel (thường là các card).
        {
            foreach (PictureBox pic in ctrl.Controls.OfType<PictureBox>()) // Tìm mọi PictureBox nằm trong control đó.
            {
                if (pic.Image is not null) // Nếu PictureBox đang giữ ảnh.
                {
                    var img = pic.Image; // Lưu tham chiếu ảnh.
                    pic.Image = null; // Gỡ khỏi PictureBox.
                    img.Dispose(); // Giải phóng bộ nhớ ảnh.
                }
            }
        }
    }

    private int GetSelectedPageSize() // Lấy số lượng món/trang đang chọn trong cboPageSize.
    {
        return int.TryParse(cboPageSize.SelectedItem?.ToString(), out int size) && size > 0 // Chuyển sang int và kiểm tra > 0.
            ? size // Trả về giá trị đã chọn.
            : 5; // Mặc định 5 nếu chưa parse được.
    }

    private void UpdatePageComboBox(int currentPage, int totalPages) // Cập nhật danh sách trang (1..totalPages) và trang đang chọn vào cboPage.
    {
        _isUpdatingPagination = true; // Bật cờ để tránh SelectedIndexChanged gọi LoadCurrentTabAsync lặp vô hạn.
        try
        {
            int safeTotalPages = Math.Max(1, totalPages); // Đảm bảo tổng số trang ít nhất là 1 kể cả khi không có dữ liệu.
            cboPage.Items.Clear(); // Xóa danh sách trang cũ.
            for (int i = 1; i <= safeTotalPages; i++) // Thêm từng số trang từ 1 đến safeTotalPages.
            {
                cboPage.Items.Add(i);
            }
            cboPage.SelectedItem = Math.Clamp(currentPage, 1, safeTotalPages); // Chọn trang hiện tại (kẹp trong khoảng hợp lệ).
        }
        finally
        {
            _isUpdatingPagination = false; // Tắt cờ sau khi cập nhật xong ComboBox.
        }
    }

    private void lnkLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Xử lý khi người dùng bấm liên kết Đăng xuất.
    {
        DialogResult confirm = MessageBox.Show( // Hỏi xác nhận trước khi đăng xuất.
            "Bạn có chắc chắn muốn đăng xuất không?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) // Nếu người dùng chọn No hoặc đóng hộp thoại.
        {
            return; // Hủy thao tác đăng xuất.
        }

        // 1. Xóa phiên đăng nhập hiện tại
        SessionContext.AccessToken = null; // Xóa access token.
        SessionContext.CurrentUser = null; // Xóa thông tin user hiện tại.

        // 2. Giải phóng ảnh trên các panel trước khi đóng form
        DisposeCardImages(pnlAllItems); // Giải phóng ảnh ở tab cộng đồng.
        DisposeCardImages(pnlMyItems); // Giải phóng ảnh ở tab của tôi.

        // 3. Đánh dấu đăng xuất để Program.cs mở lại FormDangNhap mới
        this.Tag = "REOPEN_LOGIN"; // Gắn cờ tín hiệu cho Program.cs biết cần mở lại form đăng nhập.
        this.Close(); // Đóng FormTrangChinh.
    }
}