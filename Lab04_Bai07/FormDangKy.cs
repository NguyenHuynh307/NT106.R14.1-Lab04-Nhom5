using System.Net.Mail; // Cung cấp MailAddress để kiểm tra cú pháp địa chỉ email.
using System.Text.RegularExpressions; // Cung cấp Regex để khớp mẫu biểu thức chính quy cho username, số điện thoại và họ tên.

using Lab04_Bai07.Services; // Nạp FoodApiClient, RegisterPayload, UserDto và ApiException từ tầng Services phía client.

namespace Lab04_Bai07; // Khai báo namespace thuộc ứng dụng WinForms Lab04_Bai07.

public partial class FormDangKy : Form // Form đăng ký tài khoản mới, ghép với file Designer qua từ khóa partial.
{
    // Tập hợp tên tài khoản bị cấm (không phân biệt hoa/thường) nhằm tránh giả mạo tài khoản hệ thống/quản trị.
    private static readonly HashSet<string> ForbiddenUsernames = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin", "administrator", "root", "support", "system", "moderator", "mod", "staff", "null", "undefined"
    };

    private static readonly Regex UsernameRegex = new(@"^[A-Za-z0-9._]+$", RegexOptions.Compiled); // Cho phép chữ không dấu, số, '.' và '_', biên dịch sẵn để tối ưu hiệu năng.
    private static readonly Regex PhoneVnRegex = new(@"^0\d{9}$", RegexOptions.Compiled); // Số điện thoại Việt Nam gồm đúng 10 chữ số và bắt đầu bằng 0.
    private static readonly Regex NameRegex = new(@"^[\p{L}\s]+$", RegexOptions.Compiled); // Cho phép mọi ký tự chữ cái Unicode (\p{L}, gồm cả tiếng Việt có dấu) và khoảng trắng.

    public FormDangKy() // Constructor khởi tạo form.
    {
        InitializeComponent(); // Khởi tạo toàn bộ control giao diện của form theo thiết kế Designer.
    }

    private bool inputValidator() // Kiểm tra hợp lệ lần lượt các trường nhập liệu trên form đăng ký từ trên xuống dưới.
    {
        // 1. Tên tài khoản (từ trên xuống dưới: ô đầu tiên)
        string username = txtUsername.Text.Trim(); // Lấy tên đăng nhập và cắt khoảng trắng hai đầu.
        if (string.IsNullOrEmpty(username)) // Kiểm tra tên đăng nhập có rỗng sau khi Trim hay không.
        {
            MessageBox.Show("Vui lòng nhập Tên tài khoản.", "Lỗi Tên tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo thiếu tên tài khoản.
            txtUsername.Focus(); // Đưa con trỏ nhập liệu về ô txtUsername.
            return false; // Không vượt qua kiểm tra.
        }
        if (username.Length < 4 || username.Length > 30) // Kiểm tra độ dài tên đăng nhập phải nằm trong khoảng 4..30 ký tự.
        {
            MessageBox.Show("Tên tài khoản phải có độ dài từ 4 đến 30 ký tự.", "Lỗi Tên tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi độ dài.
            txtUsername.Focus(); // Trả focus về ô tên tài khoản.
            return false; // Không vượt qua kiểm tra.
        }
        if (!UsernameRegex.IsMatch(username)) // Kiểm tra tên tài khoản có chứa ký tự ngoài [A-Za-z0-9._] hoặc khoảng trắng không.
        {
            MessageBox.Show("Tên tài khoản chỉ gồm chữ cái không dấu, số, dấu gạch dưới (_) hoặc dấu chấm (.), không chứa khoảng trắng.", "Lỗi Tên tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi ký tự không hợp lệ.
            txtUsername.Focus(); // Trả focus về ô tên tài khoản.
            return false; // Không vượt qua kiểm tra.
        }
        if (ForbiddenUsernames.Contains(username)) // Kiểm tra tên tài khoản có nằm trong danh sách từ cấm không.
        {
            MessageBox.Show("Tên tài khoản nằm trong danh sách từ cấm (admin, root, support...).", "Lỗi Tên tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo tên bị cấm.
            txtUsername.Focus(); // Trả focus về ô tên tài khoản.
            return false; // Không vượt qua kiểm tra.
        }

        // 2. Mật khẩu
        string password = txtPassword.Text; // Lấy mật khẩu nguyên bản (không Trim để giữ đúng ký tự người dùng nhập).
        if (string.IsNullOrEmpty(password)) // Kiểm tra mật khẩu có đang để trống không.
        {
            MessageBox.Show("Vui lòng nhập Mật khẩu.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo thiếu mật khẩu.
            txtPassword.Focus(); // Đưa focus về ô mật khẩu.
            return false; // Không vượt qua kiểm tra.
        }
        if (password.Length < 8 || password.Length > 12) // Kiểm tra độ dài mật khẩu phải từ 8 đến 12 ký tự.
        {
            MessageBox.Show("Mật khẩu phải có độ dài từ 8 đến 12 ký tự.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi độ dài mật khẩu.
            txtPassword.Focus(); // Trả focus về ô mật khẩu.
            return false; // Không vượt qua kiểm tra.
        }

        // 3. Email
        string email = txtEmail.Text.Trim(); // Lấy email và cắt khoảng trắng hai đầu.
        if (string.IsNullOrEmpty(email)) // Kiểm tra email có rỗng không.
        {
            MessageBox.Show("Vui lòng nhập Email.", "Lỗi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo thiếu email.
            txtEmail.Focus(); // Đưa focus về ô email.
            return false; // Không vượt qua kiểm tra.
        }
        if (!IsValidEmail(email)) // Kiểm tra định dạng email qua hàm IsValidEmail.
        {
            MessageBox.Show("Email không đúng định dạng ten@ten_mien hoặc chứa ký tự không hợp lệ.", "Lỗi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi định dạng email.
            txtEmail.Focus(); // Trả focus về ô email.
            return false; // Không vượt qua kiểm tra.
        }

        // 4. Tên (trên form của bạn ô Tên nằm trước ô Họ)
        string firstname = txtFirstname.Text.Trim(); // Lấy phần Tên và cắt khoảng trắng hai đầu.
        if (string.IsNullOrEmpty(firstname)) // Kiểm tra Tên có rỗng không.
        {
            MessageBox.Show("Vui lòng nhập Tên.", "Lỗi Tên", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo thiếu Tên.
            txtFirstname.Focus(); // Đưa focus về ô Tên.
            return false; // Không vượt qua kiểm tra.
        }
        if (!NameRegex.IsMatch(firstname)) // Kiểm tra Tên chỉ gồm chữ cái và khoảng trắng.
        {
            MessageBox.Show("Tên không được chứa chữ số hoặc ký tự đặc biệt.", "Lỗi Tên", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi ký tự trong Tên.
            txtFirstname.Focus(); // Trả focus về ô Tên.
            return false; // Không vượt qua kiểm tra.
        }

        // 5. Họ (nằm ngay dưới ô Tên)
        string lastname = txtLastname.Text.Trim(); // Lấy phần Họ và cắt khoảng trắng hai đầu.
        if (string.IsNullOrEmpty(lastname)) // Kiểm tra Họ có rỗng không.
        {
            MessageBox.Show("Vui lòng nhập Họ.", "Lỗi Họ", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo thiếu Họ.
            txtLastname.Focus(); // Đưa focus về ô Họ.
            return false; // Không vượt qua kiểm tra.
        }
        if (!NameRegex.IsMatch(lastname)) // Kiểm tra Họ chỉ gồm chữ cái và khoảng trắng.
        {
            MessageBox.Show("Họ không được chứa chữ số hoặc ký tự đặc biệt.", "Lỗi Họ", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi ký tự trong Họ.
            txtLastname.Focus(); // Trả focus về ô Họ.
            return false; // Không vượt qua kiểm tra.
        }

        // 6. Số điện thoại
        string phone = txtPhone.Text.Trim(); // Lấy số điện thoại và cắt khoảng trắng hai đầu.
        if (string.IsNullOrEmpty(phone)) // Kiểm tra số điện thoại có rỗng không.
        {
            MessageBox.Show("Vui lòng nhập Số điện thoại.", "Lỗi Số điện thoại", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo thiếu số điện thoại.
            txtPhone.Focus(); // Đưa focus về ô số điện thoại.
            return false; // Không vượt qua kiểm tra.
        }
        if (!PhoneVnRegex.IsMatch(phone)) // Kiểm tra số điện thoại có đúng 10 số và bắt đầu bằng 0 không.
        {
            MessageBox.Show("Số điện thoại Việt Nam phải gồm đúng 10 chữ số và bắt đầu bằng số 0.", "Lỗi Số điện thoại", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo lỗi định dạng số điện thoại.
            txtPhone.Focus(); // Trả focus về ô số điện thoại.
            return false; // Không vượt qua kiểm tra.
        }

        // 7. Ngày sinh
        DateTime birthday = dtpBirthday.Value.Date; // Lấy phần ngày (bỏ giờ/phút/giây) của ngày sinh đã chọn.
        DateTime today = DateTime.Today; // Lấy ngày hiện tại.
        if (birthday > today) // Chặn ngày sinh nằm ở tương lai.
        {
            MessageBox.Show("Ngày sinh không được lớn hơn ngày hiện tại.", "Lỗi Ngày sinh", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo ngày sinh không hợp lệ.
            dtpBirthday.Focus(); // Đưa focus về DateTimePicker ngày sinh.
            return false; // Không vượt qua kiểm tra.
        }

        int age = today.Year - birthday.Year; // Tính hiệu số năm sơ bộ giữa năm hiện tại và năm sinh.
        if (birthday > today.AddYears(-age)) // Nếu chưa tới sinh nhật trong năm nay thì trừ bớt 1 tuổi.
        {
            age--; // Giảm tuổi đi 1.
        }

        if (age < 18) // Yêu cầu người dùng đủ ít nhất 18 tuổi.
        {
            MessageBox.Show("Người dùng phải đủ từ 18 tuổi trở lên.", "Lỗi Ngày sinh", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Báo chưa đủ 18 tuổi.
            dtpBirthday.Focus(); // Trả focus về DateTimePicker ngày sinh.
            return false; // Không vượt qua kiểm tra.
        }

        return true; // Toàn bộ trường dữ liệu đều hợp lệ.
    }

    private static bool IsValidEmail(string email) // Kiểm tra tính hợp lệ của địa chỉ email.
    {
        if (string.IsNullOrWhiteSpace(email) || email.Any(char.IsWhiteSpace)) // Từ chối chuỗi rỗng hoặc có bất kỳ ký tự khoảng trắng nào.
        {
            return false; // Email không hợp lệ.
        }

        try
        {
            var addr = new MailAddress(email); // Thử phân tích chuỗi bằng MailAddress của .NET.
            if (!string.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase)) // Đảm bảo địa chỉ chuẩn hóa khớp chuỗi đầu vào.
            {
                return false; // Không khớp thì trả về false.
            }

            string[] parts = email.Split('@'); // Tách email theo ký tự '@'.
            if (parts.Length != 2) // Bắt buộc có đúng 1 dấu '@' (tức 2 phần: local và domain).
            {
                return false; // Sai số lượng dấu '@'.
            }

            string localPart = parts[0]; // Phần tên trước '@'.
            string domainPart = parts[1]; // Phần tên miền sau '@'.

            if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domainPart)) // Cả local và domain đều không được rỗng.
            {
                return false; // Thiếu phần tên hoặc tên miền.
            }

            if (!domainPart.Contains('.') || domainPart.StartsWith('.') || domainPart.EndsWith('.')) // Tên miền phải có dấu '.', không bắt đầu hoặc kết thúc bằng '.'.
            {
                return false; // Tên miền không hợp lệ.
            }

            return true; // Email vượt qua tất cả điều kiện kiểm tra.
        }
        catch
        {
            return false; // Nếu MailAddress ném ngoại lệ định dạng thì trả về false.
        }
    }

    private async void btnSubmit_Click(object sender, EventArgs e) // Xử lý sự kiện bất đồng bộ khi bấm nút Đăng ký.
    {
        if (!inputValidator()) // Kiểm tra dữ liệu đầu vào trước khi gọi API.
        {
            return; // Dừng lại nếu dữ liệu chưa hợp lệ.
        }

        string language = "Tiếng Việt"; // Giá trị ngôn ngữ mặc định gửi lên API.
        string sex = "Nam"; // Giá trị giới tính mặc định gửi lên API.

        btnSubmit.Enabled = false; // Khóa tạm nút Đăng ký khi đang gửi request để tránh bấm nhiều lần.
        try
        {
            using var api = new FoodApiClient("http://localhost:5167"); // Khởi tạo client gọi Web API.
            UserDto created = await api.RegisterAsync(new RegisterPayload // Gửi POST api/v1/auth/register kèm thông tin đăng ký.
            {
                Username = txtUsername.Text.Trim(), // Gán username đã Trim.
                Password = txtPassword.Text, // Gán mật khẩu gốc.
                Email = txtEmail.Text.Trim(), // Gán email đã Trim.
                FirstName = txtFirstname.Text.Trim(), // Gán tên đã Trim.
                LastName = txtLastname.Text.Trim(), // Gán họ đã Trim.
                Phone = txtPhone.Text.Trim(), // Gán số điện thoại đã Trim.
                Birthday = dtpBirthday.Value.Date, // Gán ngày sinh (chỉ lấy phần Date).
                Language = language, // Gán ngôn ngữ.
                Sex = sex // Gán giới tính.
            });

            MessageBox.Show($"Đăng ký tài khoản '{created.Username}' thành công!", "Thành công", // Thông báo tạo tài khoản thành công.
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK; // Đặt DialogResult là OK để form cha biết đăng ký thành công.
            this.Close(); // Đóng form đăng ký.
        }
        catch (ApiException ex) // Bắt lỗi nghiệp vụ từ API (như trùng username 409, lỗi validation 400) hoặc lỗi mạng/timeout.
        {
            MessageBox.Show(ex.Message, "Đăng ký thất bại", // Hiển thị thông báo lỗi cho người dùng.
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSubmit.Enabled = true; // Luôn mở lại nút Đăng ký sau khi hoàn tất hoặc gặp lỗi.
        }
    }

    private void btnClear_Click(object sender, EventArgs e) // Xóa/đặt lại toàn bộ các trường nhập liệu trên form đăng ký.
    {
        txtUsername.Clear(); // Xóa ô tên tài khoản.
        txtPassword.Clear(); // Xóa ô mật khẩu.
        txtEmail.Clear(); // Xóa ô email.
        txtFirstname.Clear(); // Xóa ô tên.
        txtLastname.Clear(); // Xóa ô họ.
        txtPhone.Clear(); // Xóa ô số điện thoại.
        dtpBirthday.Value = DateTime.Today; // Đặt lại ngày sinh về ngày hiện tại.
        if (cboLanguage is not null) // Kiểm tra combobox ngôn ngữ tồn tại trên form.
        {
            cboLanguage.SelectedIndex = -1; // Bỏ chọn mục hiện tại trong combobox.
            cboLanguage.Text = string.Empty; // Xóa chữ hiển thị trong combobox.
        }
        rdoMale.Checked = true; // Chọn lại radio button giới tính mặc định (Nam).
        txtUsername.Focus(); // Đưa con trỏ quay lại ô tên tài khoản.
    }
}