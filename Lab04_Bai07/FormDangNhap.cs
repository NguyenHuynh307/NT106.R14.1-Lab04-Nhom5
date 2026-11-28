using System; // Cung cấp các kiểu dữ liệu cơ bản của .NET như String, EventArgs.
using System.Collections.Generic; // Hỗ trợ các cấu trúc tập hợp generic.
using System.ComponentModel; // Hỗ trợ thuộc tính và thành phần thiết kế của WinForms.
using System.Data; // Cung cấp các kiểu dữ liệu cơ bản của Data.
using System.Drawing; // Cung cấp các kiểu đồ họa cơ bản cho giao diện WinForms.
using System.Text; // Hỗ trợ xử lý chuỗi và mã hóa ký tự.
using System.Windows.Forms; // Nạp thư viện giao diện WinForms (Form, MessageBox, LinkLabelLinkClickedEventArgs...).

using Lab04_Bai07.Services; // Nạp FoodApiClient, LoginPayload, LoginResultDto, SessionContext và ApiException.

namespace Lab04_Bai07; // Khai báo namespace thuộc ứng dụng WinForms Lab04_Bai07.

public partial class FormDangNhap : Form // Form đăng nhập tài khoản, ghép với file Designer qua từ khóa partial.
{
    public FormDangNhap() // Constructor khởi tạo form đăng nhập.
    {
        InitializeComponent(); // Khởi tạo và cấu hình các control trên giao diện đã thiết kế.
    }

    private async void btnLogin_Click(object sender, EventArgs e) // Xử lý sự kiện bất đồng bộ khi người dùng bấm nút Đăng nhập.
    {
        string username = txtUsername.Text.Trim(); // Lấy tên đăng nhập và xóa khoảng trắng thừa ở hai đầu.
        string password = txtPassword.Text; // Lấy mật khẩu nguyên bản (không Trim để giữ đúng ký tự người dùng nhập).

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) // Kiểm tra người dùng có bỏ trống username hoặc password không.
        {
            MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", // Cảnh báo yêu cầu nhập đủ thông tin.
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Dừng xử lý nếu thiếu thông tin.
        }

        btnLogin.Enabled = false; // Khóa tạm nút Đăng nhập trong lúc gọi API để tránh người dùng bấm liên tiếp.
        try
        {
            using var api = new FoodApiClient("http://localhost:5167"); // Tạo client kết nối tới Web API và tự giải phóng khi hết khối using.
            LoginResultDto result = await api.LoginAsync(new LoginPayload // Gửi POST api/v1/auth/login kèm thông tin đăng nhập.
            {
                Username = username, // Gán tên đăng nhập.
                Password = password // Gán mật khẩu.
            });

            if (!string.IsNullOrWhiteSpace(result.AccessToken)) // Nếu phản hồi có token hợp lệ.
            {
                SessionContext.AccessToken = result.AccessToken; // Lưu token phiên vào SessionContext (LoginAsync cũng đã gán tương tự).
            }

            if (result.User is not null) // Nếu phản hồi có thông tin người dùng.
            {
                SessionContext.CurrentUser = result.User; // Lưu thông tin user hiện tại vào SessionContext.
            }

            MessageBox.Show($"Xin chào {result.User?.LastName} {result.User?.FirstName}!", "Đăng nhập thành công", // Chào mừng người dùng theo Họ và Tên.
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK; // Đặt kết quả dialog là OK để form cha biết đăng nhập thành công.
            this.Close(); // Đóng form đăng nhập.
        }
        catch (ApiException ex) // Bắt lỗi từ API (như sai tài khoản/mật khẩu 401) hoặc lỗi mạng/timeout.
        {
            MessageBox.Show(ex.Message, "Đăng nhập thất bại", // Hiển thị thông báo lỗi cho người dùng.
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true; // Luôn bật lại nút Đăng nhập sau khi hoàn tất hoặc khi có lỗi xảy ra.
        }
    }

    private void lnkSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Xử lý khi người dùng bấm vào liên kết đăng ký tài khoản mới.
    {
        this.Hide(); // Tạm ẩn form đăng nhập khi chuyển sang form đăng ký.
        using var registerForm = new FormDangKy(); // Khởi tạo form đăng ký tài khoản.
        registerForm.ShowDialog(this); // Mở FormDangKy dạng modal và chờ người dùng thao tác xong.
        this.Show(); // Hiện lại form đăng nhập sau khi FormDangKy đóng.
    }
}