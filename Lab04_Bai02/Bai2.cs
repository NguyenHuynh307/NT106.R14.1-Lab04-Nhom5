// Thư viện làm việc với tệp tin, thư mục và luồng dữ liệu (File, Path, Directory, Stream, StreamReader).
using System.IO;
// Thư viện xử lý kết nối và truyền dữ liệu qua mạng (WebClient, WebException, HttpWebResponse).
using System.Net;
// Thư viện xử lý bảng mã ký tự và nối chuỗi hiệu quả (Encoding, StringBuilder).
using System.Text;
// Thư viện hỗ trợ biểu thức chính quy để tìm kiếm, tách và phân tích chuỗi (Regex).
using System.Text.RegularExpressions;

// Khai báo namespace theo cú pháp file-scoped namespace của C# hiện đại.
namespace Lab04_Bai02;

// Định nghĩa lớp Bai2 kế thừa Form; từ khóa partial cho phép chia phần giao diện sang file khác (Bai2.Designer.cs).
public partial class Bai2 : Form
{
    // Hàm khởi tạo (constructor) của Form Bai2.
    public Bai2()
    {
        // Khởi tạo và cấu hình các control trên Form (txtUrl, txtFilePath, btnDownload, txtHtmlContent...).
        InitializeComponent();
    }

    // Sự kiện được kích hoạt khi người dùng nhấn vào nút btnDownload.
    private void btnDownload_Click(object sender, EventArgs e)
    {
        // Lấy chuỗi địa chỉ URL từ ô txtUrl và loại bỏ khoảng trắng thừa ở hai đầu.
        string url = txtUrl.Text.Trim();
        // Lấy đường dẫn lưu tệp từ ô txtFilePath và loại bỏ khoảng trắng thừa ở hai đầu.
        string filePath = txtFilePath.Text.Trim();

        // Kiểm tra nếu URL hoặc đường dẫn lưu tệp rỗng hoặc chỉ toàn khoảng trắng.
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(filePath))
        {
            // Hiển thị hộp thoại cảnh báo yêu cầu nhập đủ thông tin.
            MessageBox.Show("Vui lòng nhập đầy đủ địa chỉ URL và đường dẫn lưu file.",
                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // Dừng thực thi hàm sự kiện nếu thiếu dữ liệu đầu vào.
            return;
        }

        // Kiểm tra nếu URL chưa bắt đầu bằng "http://" hoặc "https://" (không phân biệt chữ hoa/thường).
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // Tự động thêm tiền tố "https://" vào đầu URL.
            url = "https://" + url;
        }

        // Bắt đầu khối try để xử lý an toàn các lỗi phát sinh khi thao tác thư mục, mạng hoặc tệp.
        try
        {
            // Trích xuất đường dẫn thư mục chứa tệp đích từ filePath.
            string? directory = Path.GetDirectoryName(filePath);
            // Nếu tên thư mục hợp lệ và thư mục chưa tồn tại trên ổ đĩa.
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                // Tạo mới toàn bộ thư mục cần thiết trước khi ghi tệp.
                Directory.CreateDirectory(directory);
            }

            // Khởi tạo WebClient với từ khóa using để tự động giải phóng tài nguyên sau khi dùng xong.
            using WebClient client = new WebClient();
            // Thêm header User-Agent giả lập trình duyệt Chrome trên Windows để giảm nguy cơ bị chặn request.
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            // Thêm header Accept khai báo các định dạng nội dung phản hồi mà client chấp nhận.
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            // 1. Tải nội dung trang web và ghi thành file HTML
            client.DownloadFile(url, filePath);

            // 2. Mở Stream bằng OpenRead để đọc nội dung và hiển thị lên Form
            using Stream stream = client.OpenRead(url);
            // Bọc Stream trong StreamReader với mã hóa UTF-8 và bật tự nhận diện Byte Order Mark (BOM).
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            // Đọc toàn bộ nội dung HTML từ luồng dữ liệu thành một chuỗi.
            string rawHtml = reader.ReadToEnd();

            // Định dạng lại mã HTML cho dễ đọc và gán vào ô hiển thị txtHtmlContent trên Form.
            txtHtmlContent.Text = FormatHtml(rawHtml);

            // Hiển thị thông báo thao tác tải và lưu tệp thành công.
            MessageBox.Show("Tải và lưu file HTML thành công!",
                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Bắt riêng ngoại lệ WebException liên quan đến lỗi kết nối hoặc phản hồi HTTP.
        catch (WebException wex)
        {
            // Lấy thông điệp lỗi chi tiết (ưu tiên InnerException nếu có).
            string detail = wex.InnerException?.Message ?? wex.Message;
            // Kiểm tra nếu phản hồi trả về là HttpWebResponse để lấy thêm mã trạng thái HTTP.
            if (wex.Response is HttpWebResponse resp)
            {
                // Nối thêm mã số HTTP (ví dụ 404, 403, 500) và mô tả trạng thái vào chuỗi chi tiết lỗi.
                detail += $" (HTTP {(int)resp.StatusCode} {resp.StatusDescription})";
            }
            // Hiển thị hộp thoại thông báo lỗi mạng/HTTP cho người dùng.
            MessageBox.Show($"Lỗi khi tải trang web: {detail}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        // Bắt các ngoại lệ chung khác (như lỗi đường dẫn, quyền ghi tệp, lỗi hệ thống...).
        catch (Exception ex)
        {
            // Lấy thông điệp lỗi chi tiết (ưu tiên InnerException nếu có).
            string detail = ex.InnerException?.Message ?? ex.Message;
            // Hiển thị hộp thoại thông báo lỗi cho người dùng.
            MessageBox.Show($"Lỗi khi tải trang web: {detail}",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Hàm tĩnh hỗ trợ xuống dòng và thụt lề chuỗi HTML thô để dễ quan sát.
    private static string FormatHtml(string html)
    {
        // Nếu chuỗi HTML đầu vào rỗng hoặc chỉ chứa khoảng trắng thì trả về chuỗi rỗng.
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Tách các thẻ liền nhau thành từng dòng để dễ đọc
        string normalized = Regex.Replace(html, @">\s*<", ">\n<");
        // Tách chuỗi đã chuẩn hóa thành mảng các dòng theo "\r\n" hoặc "\n", bỏ qua phần tử rỗng.
        string[] lines = normalized.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Khởi tạo StringBuilder để xây dựng chuỗi kết quả tối ưu bộ nhớ.
        var sb = new StringBuilder();
        // Biến theo dõi cấp độ thụt lề hiện tại (mỗi cấp tương ứng 2 dấu cách).
        int indent = 0;

        // Danh sách các thẻ rỗng (void tags) không có thẻ đóng và khai báo !doctype.
        string[] voidTags =
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input",
            "link", "meta", "param", "source", "track", "wbr", "!doctype"
        };

        // Duyệt qua từng dòng đã tách trong mảng lines.
        foreach (string rawLine in lines)
        {
            // Cắt bỏ khoảng trắng thừa ở đầu và cuối dòng hiện tại.
            string line = rawLine.Trim();
            // Bỏ qua dòng nếu sau khi Trim trở thành chuỗi rỗng.
            if (string.IsNullOrEmpty(line))
                continue;

            // Kiểm tra xem dòng có bắt đầu bằng thẻ đóng (dạng </tagName>) hay không.
            bool isClosingTag = Regex.IsMatch(line, @"^<\s*/\s*[a-zA-Z0-9:-]+");
            // Kiểm tra xem dòng có phải comment hoặc khai báo đặc biệt bắt đầu bằng "<!" hoặc "<?".
            bool isCommentOrDeclaration = line.StartsWith("<!") || line.StartsWith("<?");

            // Nếu là thẻ đóng và mức thụt lề lớn hơn 0 thì giảm indent trước khi ghi dòng này.
            if (isClosingTag && indent > 0)
            {
                indent--;
            }

            // Thêm số dấu cách tương ứng với mức thụt lề hiện tại vào StringBuilder.
            sb.Append(' ', indent * 2);
            // Ghi nội dung dòng hiện tại kèm ký tự xuống dòng.
            sb.AppendLine(line);

            // Tìm tên thẻ mở ở đầu dòng bằng biểu thức chính quy.
            var openMatch = Regex.Match(line, @"^<\s*([a-zA-Z0-9:-]+)");
            // Nếu khớp tên thẻ mở và dòng không phải thẻ đóng cũng không phải comment/khai báo.
            if (openMatch.Success && !isClosingTag && !isCommentOrDeclaration)
            {
                // Chuẩn hóa tên thẻ sang chữ thường không phụ thuộc văn hóa vùng miền.
                string tagName = openMatch.Groups[1].Value.ToLowerInvariant();
                // Kiểm tra xem dòng có kết thúc bằng "/>" (thẻ tự đóng).
                bool isSelfClosing = line.EndsWith("/>");
                // Kiểm tra xem tên thẻ có nằm trong danh sách voidTags hay không.
                bool isVoidTag = Array.Exists(voidTags, t => t == tagName);

                // Nếu không phải thẻ tự đóng và không phải thẻ rỗng thì tăng mức thụt lề cho các dòng sau.
                if (!isSelfClosing && !isVoidTag)
                {
                    indent++;
                }
            }
        }

        // Chuyển StringBuilder thành string, xóa khoảng trắng/xuống dòng thừa ở cuối và trả về.
        return sb.ToString().TrimEnd();
    }
}