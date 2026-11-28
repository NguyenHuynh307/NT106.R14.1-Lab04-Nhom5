using Lab04_Bai07.Services; // Nạp FoodApiClient và SessionContext từ tầng Services của client.

namespace Lab04_Bai07; // Khai báo namespace thuộc ứng dụng WinForms Lab04_Bai07.

public partial class FormCNAnGiGio : Form // Form chức năng "Hôm nay ăn gì", chia tách code-behind với file Designer.
{
    public FormCNAnGiGio() // Constructor khởi tạo form.
    {
        InitializeComponent(); // Khởi tạo các control trên giao diện (nút, label, textbox, picturebox, radio button...).
        this.Load += async (_, _) => await PickRandomFoodAsync(showErrorPopup: false); // Khi form tải xong, tự chọn ngẫu nhiên 1 món nhưng không bật popup lỗi ban đầu.
    }

    private bool IsMineOnlySelected() => rdoCuaToi.Checked; // Kiểm tra xem người dùng đang chọn radio "Của tôi" (true) hay "Cả cộng đồng" (false).

    private async Task PickRandomFoodAsync(bool showErrorPopup) // Gọi API chọn ngẫu nhiên món ăn bất đồng bộ và cập nhật giao diện.
    {
        bool mineOnly = IsMineOnlySelected(); // Lấy trạng thái lọc theo món của người dùng hiện tại.

        if (mineOnly && string.IsNullOrWhiteSpace(SessionContext.AccessToken)) // Nếu chọn "Của tôi" nhưng phiên hiện tại chưa có AccessToken (chưa đăng nhập).
        {
            ResetResultView("Chưa đăng nhập"); // Đặt lại khu vực hiển thị kết quả với thông báo chưa đăng nhập.
            if (showErrorPopup) // Chỉ hiện MessageBox cảnh báo nếu được yêu cầu (ví dụ khi bấm nút "Chọn lại").
            {
                MessageBox.Show("Vui lòng đăng nhập để chọn ngẫu nhiên từ món ăn của bạn.", // Nội dung cảnh báo.
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Tiêu đề, nút OK và biểu tượng cảnh báo.
            }
            return; // Dừng sớm, không gọi API.
        }

        btnChonLai.Enabled = false; // Khóa tạm nút "Chọn lại" trong lúc đang chờ phản hồi từ server để tránh bấm liên tiếp.
        try
        {
            using var api = new FoodApiClient("http://localhost:5167"); // Tạo client trỏ tới Web API và tự giải phóng khi hết khối using.
            FoodDto food = await api.GetRandomFoodAsync(mineOnly); // Gọi GET api/v1/foods/random?mine=... để lấy ngẫu nhiên một món ăn.
            BindFoodResult(food); // Đổ dữ liệu món ăn nhận được lên các control trên form.
        }
        catch (ApiException ex) // Bắt lỗi từ API hoặc lỗi mạng/timeout do FoodApiClient ném ra.
        {
            ResetResultView(mineOnly ? "Chưa có món ăn của bạn" : "Chưa có món ăn nào"); // Hiển thị trạng thái rỗng phù hợp với chế độ đang chọn.
            if (showErrorPopup) // Nếu cần hiện thông báo lỗi cho người dùng.
            {
                MessageBox.Show(ex.Message, "Không thể chọn món", // Hiển thị thông báo lỗi cụ thể từ ApiException.
                    MessageBoxButtons.OK, MessageBoxIcon.Information); // Tiêu đề, nút OK và biểu tượng thông tin.
            }
        }
        finally
        {
            btnChonLai.Enabled = true; // Luôn bật lại nút "Chọn lại" sau khi hoàn tất hoặc gặp lỗi.
        }
    }

    private void BindFoodResult(FoodDto food) // Gán thông tin từ FoodDto lên các label, textbox và picturebox.
    {
        lblTenMon.Text = food.Name; // Hiển thị tên món ăn.
        lblGia.Text = $"Giá: {food.Price:N0} đ"; // Định dạng giá tiền theo dấu phân tách hàng nghìn kèm đơn vị "đ".
        lblDiaChi.Text = $"Địa chỉ: {food.Address}"; // Hiển thị địa chỉ quán.
        lblNguoiDongGop.Text = $"Đóng góp: {food.CreatedByUsername}"; // Hiển thị username người đóng góp món.
        txtMoTa.Text = string.IsNullOrWhiteSpace(food.Description) // Kiểm tra mô tả có rỗng hoặc chỉ là khoảng trắng không.
            ? "Không có mô tả." // Nội dung thay thế nếu không có mô tả.
            : food.Description; // Hiển thị mô tả gốc nếu có.

        LoadFoodImage(food.Image); // Tải và hiển thị ảnh món ăn từ URL hoặc đường dẫn file cục bộ.
    }

    private void ResetResultView(string titleMessage) // Xóa thông tin món ăn cũ và hiển thị thông báo trạng thái trên tiêu đề.
    {
        lblTenMon.Text = titleMessage; // Đặt thông báo trạng thái vào label tên món.
        lblGia.Text = "Giá: -"; // Đặt lại label giá về mặc định.
        lblDiaChi.Text = "Địa chỉ: -"; // Đặt lại label địa chỉ về mặc định.
        lblNguoiDongGop.Text = "Đóng góp: -"; // Đặt lại label người đóng góp về mặc định.
        txtMoTa.Clear(); // Xóa nội dung ô mô tả.

        if (picMonAn.Image is not null) // Kiểm tra xem PictureBox đang giữ ảnh cũ hay không.
        {
            var oldImage = picMonAn.Image; // Giữ tham chiếu ảnh cũ.
            picMonAn.Image = null; // Gỡ ảnh khỏi PictureBox trước khi giải phóng.
            oldImage.Dispose(); // Giải phóng bộ nhớ GDI+ của ảnh cũ tránh rò rỉ tài nguyên.
        }
    }

    private void LoadFoodImage(string? imagePathOrUrl) // Tải ảnh từ URL HTTP/HTTPS hoặc đường dẫn file trên máy.
    {
        if (picMonAn.Image is not null) // Nếu đang có ảnh cũ hiển thị trên picMonAn.
        {
            var oldImage = picMonAn.Image; // Lưu tạm ảnh cũ.
            picMonAn.Image = null; // Ngắt liên kết ảnh cũ khỏi control.
            oldImage.Dispose(); // Giải phóng tài nguyên ảnh cũ.
        }

        if (string.IsNullOrWhiteSpace(imagePathOrUrl)) // Nếu không có đường dẫn hoặc URL ảnh thì kết thúc.
        {
            return;
        }

        try
        {
            // Kiểm tra xem chuỗi có phải URI tuyệt đối với scheme http hoặc https hay không.
            if (Uri.TryCreate(imagePathOrUrl, UriKind.Absolute, out Uri? uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                picMonAn.LoadAsync(uri.ToString()); // Tải ảnh bất đồng bộ từ URL web.
                picMonAn.SizeMode = PictureBoxSizeMode.Zoom; // Co giãn ảnh giữ đúng tỷ lệ khung hình.
                return; // Đã bắt đầu tải URL thì thoát hàm.
            }

            if (File.Exists(imagePathOrUrl)) // Nếu không phải URL web, kiểm tra xem có phải file tồn tại trên ổ đĩa không.
            {
                using var stream = new FileStream(imagePathOrUrl, FileMode.Open, FileAccess.Read); // Mở file ở chế độ chỉ đọc để tránh khóa file trên đĩa.
                using var tempImage = Image.FromStream(stream); // Đọc dữ liệu ảnh từ stream.
                picMonAn.Image = new Bitmap(tempImage); // Sao chép sang Bitmap độc lập để stream có thể đóng an toàn mà ảnh vẫn hiển thị.
                picMonAn.SizeMode = PictureBoxSizeMode.Zoom; // Hiển thị ảnh theo chế độ Zoom giữ tỷ lệ.
            }
        }
        catch
        {
            // Bỏ qua lỗi tải ảnh để vẫn hiển thị đầy đủ thông tin chữ của món ăn.
        }
    }

    private async void btnChonLai_Click(object sender, EventArgs e) // Xử lý sự kiện khi người dùng bấm nút "Chọn lại".
    {
        await PickRandomFoodAsync(showErrorPopup: true); // Gọi chọn lại món ngẫu nhiên và hiện popup thông báo nếu có lỗi.
    }

    private async void rdoCaCongDong_CheckedChanged(object sender, EventArgs e) // Xử lý khi trạng thái radio "Cả cộng đồng" thay đổi.
    {
        if (rdoCaCongDong.Checked && IsHandleCreated) // Chỉ gọi khi radio này được chọn và handle WinForms đã được tạo xong.
        {
            await PickRandomFoodAsync(showErrorPopup: false); // Tự động lấy ngẫu nhiên món từ toàn cộng đồng (không hiện popup lỗi).
        }
    }

    private async void rdoCuaToi_CheckedChanged(object sender, EventArgs e) // Xử lý khi trạng thái radio "Của tôi" thay đổi.
    {
        if (rdoCuaToi.Checked && IsHandleCreated) // Chỉ chạy khi radio "Của tôi" được tích và window handle đã sẵn sàng.
        {
            await PickRandomFoodAsync(showErrorPopup: false); // Tự động thử lấy ngẫu nhiên món của bản thân (không hiện popup lỗi).
        }
    }

    private void btnDong_Click(object sender, EventArgs e) // Xử lý sự kiện khi bấm nút "Đóng".
    {
        this.Close(); // Đóng form hiện tại.
    }
}