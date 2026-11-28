using System; // Cung cấp các kiểu cơ bản như String, DateTime, Uri, StringSplitOptions...
using System.Collections.Generic; // Hỗ trợ các cấu trúc tập hợp generic.
using System.ComponentModel; // Hỗ trợ thành phần/thuộc tính thiết kế của WinForms.
using System.Data; // Cung cấp các kiểu dữ liệu cơ bản của ADO.NET/Data.
using System.Drawing; // Cung cấp Image, Bitmap để xử lý và hiển thị hình ảnh.
using System.IO; // Cung cấp FileStream để đọc file ảnh từ ổ đĩa an toàn.
using System.Linq; // Cung cấp phương thức mở rộng LINQ như .Any() để kiểm tra phần tử trùng.
using System.Text; // Hỗ trợ thao tác chuỗi và mã hóa ký tự.
using System.Threading.Tasks; // Cung cấp Task và async/await cho thao tác bất đồng bộ.
using System.Windows.Forms; // Nạp thư viện giao diện WinForms (Form, MessageBox, OpenFileDialog...).

using Lab04_Bai07.Services; // Nạp FoodApiClient, SessionContext và các DTO (FoodDto, CreateFoodPayload, PagedFoodsDto...).

namespace Lab04_Bai07; // Khai báo namespace thuộc ứng dụng WinForms Lab04_Bai07.

public partial class FormCNThemMonAn : Form // Form thêm món ăn mới, ghép với file Designer qua từ khóa partial.
{
    public FoodDto? CreatedFood { get; private set; } // Lưu món ăn vừa tạo thành công để form cha đọc lại; chỉ cho phép gán bên trong class.

    public FormCNThemMonAn() // Constructor khởi tạo form.
    {
        InitializeComponent(); // Khởi tạo và cấu hình toàn bộ control giao diện đã thiết kế.
    }

    private async void btnLuu_Click(object sender, EventArgs e) // Xử lý sự kiện bất đồng bộ khi người dùng bấm nút Lưu.
    {
        if (string.IsNullOrWhiteSpace(SessionContext.AccessToken)) // Kiểm tra người dùng đã đăng nhập và có token chưa.
        {
            MessageBox.Show("Bạn cần đăng nhập trước khi thêm món ăn.", "Thông báo", // Cảnh báo yêu cầu đăng nhập.
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Dừng xử lý nếu chưa đăng nhập.
        }

        string name = txtTenMon.Text.Trim(); // Lấy tên món ăn và cắt khoảng trắng hai đầu.
        decimal price = numGia.Value; // Lấy giá món ăn từ control NumericUpDown.
        string address = txtDiaChi.Text.Trim(); // Lấy địa chỉ quán và cắt khoảng trắng hai đầu.
        string description = txtMoTa.Text.Trim(); // Lấy mô tả món ăn và cắt khoảng trắng hai đầu.
        string image = txtDuongDanAnh.Text.Trim(); // Lấy đường dẫn hoặc URL ảnh và cắt khoảng trắng hai đầu.

        if (string.IsNullOrWhiteSpace(name)) // Kiểm tra tên món ăn không được rỗng hoặc chỉ toàn khoảng trắng.
        {
            MessageBox.Show("Vui lòng nhập tên món ăn.", "Thiếu thông tin", // Thông báo thiếu tên món.
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTenMon.Focus(); // Đưa con trỏ nhập liệu về ô tên món để người dùng sửa ngay.
            return; // Dừng lưu.
        }

        if (price < 0) // Kiểm tra giá món ăn không được âm.
        {
            MessageBox.Show("Giá món ăn phải lớn hơn hoặc bằng 0.", "Lỗi dữ liệu", // Báo lỗi giá âm.
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Dừng lưu.
        }

        if (string.IsNullOrWhiteSpace(address)) // Kiểm tra địa chỉ quán không được để trống.
        {
            MessageBox.Show("Vui lòng nhập địa chỉ quán.", "Thiếu thông tin", // Thông báo thiếu địa chỉ quán.
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDiaChi.Focus(); // Đưa focus vào ô địa chỉ quán.
            return; // Dừng lưu.
        }

        btnLuu.Enabled = false; // Vô hiệu hóa tạm thời nút Lưu khi đang gửi request để tránh bấm đúp.
        try
        {
            using var api = new FoodApiClient("http://localhost:5167"); // Khởi tạo FoodApiClient trỏ tới địa chỉ Web API.

            // Kiểm tra xem người dùng hiện tại đã từng thêm món trùng tên, địa chỉ, giá và mô tả (sau chuẩn hóa) chưa.
            if (await IsDuplicateForCurrentUserAsync(api, name, address, price, description))
            {
                MessageBox.Show(
                    "Bạn đã đóng góp món ăn với nội dung này rồi, vui lòng nhập thông tin khác.", // Cảnh báo món bị trùng ở tài khoản hiện tại.
                    "Trùng món ăn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtTenMon.Focus(); // Đưa focus trở lại ô tên món.
                return; // Không gọi API tạo món mới.
            }

            CreatedFood = await api.CreateFoodAsync(new CreateFoodPayload // Gửi POST api/v1/foods kèm payload món ăn mới.
            {
                Name = name, // Gán tên món.
                Price = price, // Gán giá món.
                Address = address, // Gán địa chỉ quán.
                Description = description, // Gán mô tả.
                Image = image // Gán đường dẫn/URL ảnh.
            });

            MessageBox.Show($"Đã thêm món \"{CreatedFood.Name}\"!", "Thành công", // Thông báo thêm món thành công kèm tên món trả về từ server.
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK; // Đặt kết quả dialog là OK để form gọi biết thao tác thành công.
            this.Close(); // Đóng form thêm món ăn.
        }
        catch (ApiException ex) // Bắt lỗi từ API hoặc lỗi kết nối/timeout do FoodApiClient ném ra.
        {
            MessageBox.Show(ex.Message, "Thêm món thất bại", // Hiển thị thông báo lỗi cho người dùng.
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLuu.Enabled = true; // Luôn bật lại nút Lưu sau khi hoàn tất hoặc khi có lỗi xảy ra.
        }
    }

    // Chuẩn hóa chuỗi văn bản: cắt khoảng trắng hai đầu, chuyển chữ thường bất biến văn hóa và gộp nhiều khoảng trắng liên tiếp thành 1 dấu cách.
    private static string NormalizeFoodText(string? value)
    {
        return string.Join(" ", (value ?? string.Empty) // Nối các từ bằng đúng một dấu cách.
            .Trim() // Xóa khoảng trắng đầu và cuối chuỗi.
            .ToLowerInvariant() // Chuyển toàn bộ sang chữ thường theo quy tắc invariant culture.
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)); // Tách theo dấu cách và loại bỏ các phần tử rỗng do nhiều dấu cách liên tiếp.
    }

    // Duyệt toàn bộ trang món ăn của người dùng hiện tại (mine = true) để kiểm tra có bản ghi nào trùng nội dung không.
    private static async Task<bool> IsDuplicateForCurrentUserAsync(
        FoodApiClient api, // Client dùng để gọi GET api/v1/foods.
        string name, // Tên món cần kiểm tra.
        string address, // Địa chỉ quán cần kiểm tra.
        decimal price, // Giá món cần kiểm tra.
        string description) // Mô tả món cần kiểm tra.
    {
        string normName = NormalizeFoodText(name); // Chuẩn hóa tên món nhập vào.
        string normAddress = NormalizeFoodText(address); // Chuẩn hóa địa chỉ nhập vào.
        string normDesc = NormalizeFoodText(description); // Chuẩn hóa mô tả nhập vào.

        int page = 1; // Bắt đầu kiểm tra từ trang thứ 1.
        const int pageSize = 100; // Lấy tối đa 100 bản ghi mỗi lần gọi để giảm số request khi duyệt danh sách.

        while (true) // Lặp qua từng trang cho tới khi hết dữ liệu hoặc tìm thấy bản ghi trùng.
        {
            PagedFoodsDto myFoods = await api.GetFoodsAsync(page, pageSize, mine: true); // Lấy danh sách món ăn của user hiện tại ở trang hiện tại.

            bool matched = myFoods.Items.Any(f => // Kiểm tra có món nào trong trang hiện tại khớp cả 4 thuộc tính đã chuẩn hóa và giá không.
                NormalizeFoodText(f.Name) == normName && // Khớp tên món đã chuẩn hóa.
                NormalizeFoodText(f.Address) == normAddress && // Khớp địa chỉ đã chuẩn hóa.
                f.Price == price && // Khớp chính xác giá tiền.
                NormalizeFoodText(f.Description) == normDesc); // Khớp mô tả đã chuẩn hóa.

            if (matched) // Nếu tìm thấy món trùng nội dung.
            {
                return true; // Báo có trùng lặp.
            }

            if (page >= myFoods.TotalPages || myFoods.Items.Count == 0) // Nếu đã tới trang cuối hoặc trang hiện tại không còn item nào.
            {
                break; // Thoát vòng lặp.
            }

            page++; // Tăng số trang để kiểm tra trang kế tiếp.
        }

        return false; // Không phát hiện món trùng nội dung trong danh sách của người dùng hiện tại.
    }

    private void btnClear_Click(object sender, EventArgs e) // Đặt lại toàn bộ ô nhập liệu và ảnh xem trước về trạng thái ban đầu.
    {
        txtTenMon.Clear(); // Xóa nội dung ô tên món.
        numGia.Value = numGia.Minimum; // Đưa giá trị số về mức nhỏ nhất (Minimum) của NumericUpDown.
        txtDiaChi.Clear(); // Xóa nội dung ô địa chỉ quán.
        txtMoTa.Clear(); // Xóa nội dung ô mô tả.
        txtDuongDanAnh.Clear(); // Xóa nội dung ô đường dẫn ảnh.
        if (picPreview.Image is not null) // Kiểm tra xem PictureBox xem trước có đang giữ ảnh nào không.
        {
            var oldImage = picPreview.Image; // Giữ tham chiếu ảnh cũ.
            picPreview.Image = null; // Gỡ ảnh khỏi PictureBox trước khi giải phóng.
            oldImage.Dispose(); // Giải phóng tài nguyên ảnh cũ để tránh rò rỉ bộ nhớ.
        }
        txtTenMon.Focus(); // Đưa con trỏ quay lại ô tên món.
    }

    private void btnChonAnh_Click(object sender, EventArgs e) // Mở hộp thoại chọn file ảnh trên máy và hiển thị xem trước.
    {
        using var dialog = new OpenFileDialog // Khởi tạo hộp thoại chọn tệp tin.
        {
            Title = "Chọn hình ảnh món ăn", // Tiêu đề cửa sổ chọn file.
            Filter = "Tệp hình ảnh (*.jpg;*.jpeg;*.png;*.bmp;*.webp)|*.jpg;*.jpeg;*.png;*.bmp;*.webp|Tất cả tệp (*.*)|*.*", // Lọc định dạng ảnh phổ biến.
            CheckFileExists = true, // Yêu cầu tệp phải tồn tại.
            Multiselect = false // Chỉ cho phép chọn một tệp ảnh duy nhất.
        };

        if (dialog.ShowDialog(this) != DialogResult.OK) // Nếu người dùng bấm Cancel hoặc đóng hộp thoại.
        {
            return; // Không thay đổi gì và thoát hàm.
        }

        string selectedPath = dialog.FileName; // Lấy đường dẫn đầy đủ của tệp ảnh đã chọn.
        txtDuongDanAnh.Text = selectedPath; // Hiển thị đường dẫn tệp lên textbox đường dẫn ảnh.

        if (picPreview.Image is not null) // Nếu ô xem trước đang có ảnh cũ.
        {
            var oldImage = picPreview.Image; // Lưu tham chiếu ảnh cũ.
            picPreview.Image = null; // Ngắt liên kết ảnh cũ khỏi PictureBox.
            oldImage.Dispose(); // Giải phóng tài nguyên ảnh cũ.
        }

        // Đọc qua FileStream rồi clone sang Bitmap để tránh khóa file ảnh trên đĩa
        using var stream = new FileStream(selectedPath, FileMode.Open, FileAccess.Read); // Mở file ảnh ở chế độ chỉ đọc.
        using var tempImage = Image.FromStream(stream); // Tạo đối tượng Image tạm từ stream.
        picPreview.Image = new Bitmap(tempImage); // Nhân bản sang Bitmap mới để ảnh tồn tại độc lập sau khi đóng stream.
        picPreview.SizeMode = PictureBoxSizeMode.Zoom; // Co giãn ảnh vừa khung PictureBox nhưng vẫn giữ đúng tỷ lệ.
    }
}