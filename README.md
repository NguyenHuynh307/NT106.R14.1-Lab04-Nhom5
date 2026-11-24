# NT106.R14.1 - Lab 04 - Nhóm 5

> Bài Lab 04 nhóm 5 lớp NT106.R14.1: 5 ứng dụng Windows Forms minh họa truyền thông với Web Server trong C# (thực hiện Bài 1, 2, 3, 4, 7; miễn Bài 5, 6).

## Các bài

* **Bài 01 - Lấy HTML (WebRequest)**: Nhập URL bất kỳ, gửi HTTP GET bằng `WebRequest` / `WebResponse`, đọc `StreamReader` (UTF-8) và hiển thị mã nguồn HTML lên `RichTextBox` (kèm test IIS `localhost`).
* **Bài 02 - Download HTML (WebClient)**: Dùng `WebClient` (`OpenRead`, `DownloadFile`) tải nội dung trang web từ URL về file `.html` cục bộ và hiển thị lại lên Form.
* **Bài 03 - Web Browser cơ bản**: Duyệt web bằng `WebView2` (`Load`, `Reload`), tải file HTML (`Down Files`), trích xuất và tải toàn bộ hình ảnh bằng `HtmlAgilityPack` (`Down Resources`) và xem mã nguồn ở Form mới (`View Source`).
* **Bài 04 - Quản lý phòng vé (v4)**: Crawl danh sách phim đang chiếu từ `betacinemas.vn/phim.htm` kèm `ProgressBar`, lưu tóm tắt ra JSON, bấm banner mở chi tiết bằng `WebView2`, hỗ trợ đặt đơn/nhiều vé và lưu dữ liệu khách hàng/vé bằng SQLite.
* **Bài 07 - Hôm nay ăn gì? (v4)**: Ứng dụng gọi REST API (`HttpClient` + JSON + Bearer Token) hỗ trợ đăng ký, đăng nhập, thêm/xóa món ăn, phân trang `All` / `Tôi đóng góp` và chọn ngẫu nhiên món ăn.

## Cài đặt và chạy

1. Cài **.NET 10 SDK** và **Visual Studio 2022/2026** (workload .NET Desktop).
2. Mở `NT106.R14.1-Lab04-Nhom5.slnx`, chọn project `NT106.R14.1-Lab04-Nhom5` làm Startup Project rồi bấm `F5`. Hoặc chạy:
   ```bash
   dotnet run --project .\NT106.R14.1-Lab04-Nhom5
   ```
3. Chọn bài tập từ menu chính để mở Form điều hướng riêng của từng bài.

## Cấu trúc mã nguồn

| Tệp / thư mục | Vai trò | Phụ trách |
| :--- | :--- | :--- |
| `NT106.R14.1-Lab04-Nhom5.slnx` | Solution gom 6 project | `NguyenHuynh307` |
| `NT106.R14.1-Lab04-Nhom5/` | App chính, menu điều hướng 5 bài | `NguyenHuynh307` |
| `Lab04_Bai01/` | Hiển thị HTML với `WebRequest` | `hoanganh120507` |
| `Lab04_Bai02/` | Download file HTML với `WebClient` | `Liamz07` |
| `Lab04_Bai03/` | Web Browser (`WebView2` + `HtmlAgilityPack`) | `hoanganh120507` |
| `Lab04_Bai04/` | Quản lý phòng vé (Crawl + JSON + SQLite) | `NguyenHuynh307` |
| `Lab04_Bai07/` | Hôm nay ăn gì? (REST API + Auth + Phân trang) | `Liamz07` |

## Ghi chú

* Không thực hiện Bài 5 và Bài 6 theo thông báo của giảng viên; kiến thức HTTP POST/GET và Bearer Token được tích hợp trực tiếp vào Bài 7.
* Mỗi bài là một project độc lập trong cùng solution, có form điều hướng riêng; hỗ trợ tiếng Việt UTF-8 và bắt lỗi kết nối/đầu vào đầy đủ.
