using System.IO;
using System.Net;

namespace Lab04_Bai01;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private string getHTML(string szURL)
    {
        // Tạo yêu cầu gửi tới URL
        WebRequest request = WebRequest.Create(szURL);
        // Nhận phản hồi từ máy chủ trả về
        WebResponse response = request.GetResponse();
        // Mở luồng đọc dữ liệu từ phản hồi
        Stream dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        // Đọc toàn bộ nội dung thành chuỗi HTML
        string responseFromServer = reader.ReadToEnd();
        // Đóng các kết nối để giải phóng bộ nhớ
        reader.Close();
        response.Close();

        return responseFromServer;
    }

    private void btnGet_Click(object sender, EventArgs e)
    {
        string url = txtUrl.Text.Trim();
        if (string.IsNullOrEmpty(url))
        {
            MessageBox.Show("Vui lòng nhập địa chỉ URL!", "Thông báo");
            return;
        }

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "http://" + url;
        }

        try
        {
            rtbContent.Text = getHTML(url);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi lấy dữ liệu: " + ex.Message, "Lỗi");
        }
    }
}