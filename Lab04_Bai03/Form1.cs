
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

using HtmlAgilityPack;

using HapHtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Lab04_Bai03;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        ConfigureButton(btnLoad, "Load");
        ConfigureButton(btnReload, "Reload");
        ConfigureButton(btnDownloadHtml, "Download HTML");
        ConfigureButton(btnDownloadResources, "Download Resources");
        ConfigureButton(btnViewSource, "View Source");

        Load += Form1_Load;

        btnLoad.Click += BtnLoad_Click;
        btnReload.Click += BtnReload_Click;
        btnDownloadHtml.Click += BtnDownloadHtml_Click;
        btnDownloadResources.Click += BtnDownloadResources_Click;
        btnViewSource.Click += BtnViewSource_Click;

        txtUrl.Text = "https://uit.edu.vn";
    }

    private static void ConfigureButton(Button button, string text)
    {
        button.Text = text;
        button.ForeColor = Color.Black;
        button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
    }

    // 1. Khởi tạo WebView2
    private async void Form1_Load(object? sender, EventArgs e)
    {
        try
        {
            await webView21.EnsureCoreWebView2Async();

            if (TryGetWebsiteUrl(out Uri? url))
                webView21.Source = url;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khởi tạo WebView2: " + ex.Message);
        }
    }

    // 2. Load Website
    private async void BtnLoad_Click(object? sender, EventArgs e)
    {
        if (!TryGetWebsiteUrl(out Uri? url))
        {
            MessageBox.Show("URL không hợp lệ!");
            return;
        }

        try
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.Source = url!;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi Load Website: " + ex.Message);
        }
    }

    // 3. Reload Website
    private async void BtnReload_Click(object? sender, EventArgs e)
    {
        try
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi Reload: " + ex.Message);
        }
    }

    // 4. Download HTML
    private async void BtnDownloadHtml_Click(object? sender, EventArgs e)
    {
        if (!TryGetWebsiteUrl(out Uri? url))
        {
            MessageBox.Show("URL không hợp lệ!");
            return;
        }

        using SaveFileDialog saveDialog = new SaveFileDialog();

        saveDialog.Title = "CHỌN NƠI LƯU FILE HTML";
        saveDialog.Filter = "HTML Files (*.html)|*.html";
        saveDialog.FileName = "website.html";

        if (saveDialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            using WebClient client = new WebClient();

            await client.DownloadFileTaskAsync(
                url!,
                saveDialog.FileName
            );

            MessageBox.Show(
                "DOWNLOAD HTML THÀNH CÔNG!\n\n" +
                saveDialog.FileName
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi Download HTML: " + ex.Message);
        }
    }

    // 5. Download Resources
    private async void BtnDownloadResources_Click(object? sender, EventArgs e)
    {
        if (!TryGetWebsiteUrl(out Uri? pageUrl))
        {
            MessageBox.Show("URL không hợp lệ!");
            return;
        }

        using FolderBrowserDialog folderDialog = new FolderBrowserDialog();

        folderDialog.Description = "CHỌN THƯ MỤC LƯU HÌNH ẢNH";

        if (folderDialog.ShowDialog() != DialogResult.OK)
            return;

        string folderPath = folderDialog.SelectedPath;

        try
        {
            string html;

            using (WebClient htmlClient = new WebClient())
            {
                html = await htmlClient.DownloadStringTaskAsync(pageUrl!);
            }

            HapHtmlDocument document = new HapHtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection? images =
                document.DocumentNode.SelectNodes("//img[@src]");

            if (images == null || images.Count == 0)
            {
                MessageBox.Show("Không tìm thấy hình ảnh!");
                return;
            }

            Uri resourceBaseUri = pageUrl!;

            HtmlNode? baseNode =
                document.DocumentNode.SelectSingleNode("//base[@href]");

            if (baseNode != null)
            {
                string baseHref =
                    baseNode.GetAttributeValue("href", "").Trim();

                if (Uri.TryCreate(resourceBaseUri, baseHref, out Uri? resolvedBase)
                    && resolvedBase != null
                    && (resolvedBase.Scheme == Uri.UriSchemeHttp ||
                        resolvedBase.Scheme == Uri.UriSchemeHttps))
                {
                    resourceBaseUri = resolvedBase;
                }
            }

            using HttpClient imageClient = new HttpClient();

            imageClient.Timeout = TimeSpan.FromSeconds(30);

            imageClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36"
            );

            HashSet<string> downloadedUrls = new HashSet<string>();

            HashSet<string> usedFileNames =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int successCount = 0;
            int skippedCount = 0;
            int failedCount = 0;
            int index = 0;

            foreach (HtmlNode image in images)
            {
                index++;

                string src = image.GetAttributeValue("src", "").Trim();
                src = HtmlEntity.DeEntitize(src);

                if (string.IsNullOrWhiteSpace(src))
                {
                    skippedCount++;
                    continue;
                }

                if (!Uri.TryCreate(src, UriKind.Absolute, out Uri? imageUrl))
                {
                    if (!Uri.TryCreate(resourceBaseUri, src, out imageUrl))
                    {
                        skippedCount++;
                        continue;
                    }
                }

                if (imageUrl == null ||
                    (imageUrl.Scheme != Uri.UriSchemeHttp &&
                     imageUrl.Scheme != Uri.UriSchemeHttps))
                {
                    skippedCount++;
                    continue;
                }

                if (!downloadedUrls.Add(imageUrl.AbsoluteUri))
                {
                    skippedCount++;
                    continue;
                }

                try
                {
                    using HttpResponseMessage response =
                        await imageClient.GetAsync(
                            imageUrl,
                            HttpCompletionOption.ResponseHeadersRead
                        );

                    response.EnsureSuccessStatusCode();

                    byte[] imageData =
                        await response.Content.ReadAsByteArrayAsync();

                    string? contentType =
                        response.Content.Headers.ContentType?.MediaType;

                    string? extension =
                        GetImageExtension(imageData, contentType);

                    if (extension == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    string baseName = GetImageBaseName(imageUrl, index);

                    string fileName = GetUniqueFileName(
                        baseName,
                        extension,
                        folderPath,
                        usedFileNames
                    );

                    string filePath = Path.Combine(folderPath, fileName);

                    await File.WriteAllBytesAsync(filePath, imageData);

                    successCount++;
                }
                catch
                {
                    failedCount++;
                }
            }

            MessageBox.Show(
                "DOWNLOAD RESOURCES HOÀN TẤT!\n\n" +
                $"Tải thành công: {successCount}\n" +
                $"Bỏ qua: {skippedCount}\n" +
                $"Thất bại: {failedCount}\n\n" +
                "Thư mục lưu:\n" + folderPath
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi Download Resources: " + ex.Message);
        }
    }

    // 6. View Source
    private async void BtnViewSource_Click(object? sender, EventArgs e)
    {
        if (!TryGetWebsiteUrl(out Uri? url))
        {
            MessageBox.Show("URL không hợp lệ!");
            return;
        }

        try
        {
            string html;

            using (WebClient client = new WebClient())
            {
                html = await client.DownloadStringTaskAsync(url!);
            }

            using SourceForm sourceForm = new SourceForm(html);

            sourceForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi View Source: " + ex.Message);
        }
    }

    // Xác định định dạng ảnh
    private static string? GetImageExtension(byte[] data, string? contentType)
    {
        if (data.Length >= 8 &&
            data[0] == 0x89 && data[1] == 0x50 &&
            data[2] == 0x4E && data[3] == 0x47)
            return ".png";

        if (data.Length >= 3 &&
            data[0] == 0xFF && data[1] == 0xD8 &&
            data[2] == 0xFF)
            return ".jpg";

        if (data.Length >= 6)
        {
            string signature = Encoding.ASCII.GetString(data, 0, 6);

            if (signature == "GIF87a" || signature == "GIF89a")
                return ".gif";
        }

        if (data.Length >= 12 &&
            Encoding.ASCII.GetString(data, 0, 4) == "RIFF" &&
            Encoding.ASCII.GetString(data, 8, 4) == "WEBP")
            return ".webp";

        if (data.Length >= 2 &&
            data[0] == 0x42 && data[1] == 0x4D)
            return ".bmp";

        string type = (contentType ?? "").ToLowerInvariant();

        if (type == "image/svg+xml")
        {
            string content = Encoding.UTF8.GetString(
                data, 0, Math.Min(data.Length, 2048));

            return content.Contains("<svg", StringComparison.OrdinalIgnoreCase)
                ? ".svg" : null;
        }

        return type switch
        {
            "image/png" or "image/x-png" => ".png",
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            "image/bmp" => ".bmp",
            "image/x-icon" or "image/vnd.microsoft.icon" => ".ico",
            "image/tiff" => ".tiff",
            "image/avif" => ".avif",
            "image/heic" => ".heic",
            _ => null
        };
    }

    private static string GetImageBaseName(Uri imageUrl, int index)
    {
        string name = Path.GetFileNameWithoutExtension(imageUrl.AbsolutePath);

        name = Uri.UnescapeDataString(name);

        if (string.IsNullOrWhiteSpace(name))
            name = $"image_{index:D3}";

        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        return name;
    }

    private static string GetUniqueFileName(
        string baseName,
        string extension,
        string folderPath,
        HashSet<string> usedFileNames)
    {
        string candidate = baseName + extension;
        int number = 2;

        while (File.Exists(Path.Combine(folderPath, candidate)) ||
               !usedFileNames.Add(candidate))
        {
            candidate = $"{baseName}_{number}{extension}";
            number++;
        }

        return candidate;
    }

    // Kiểm tra URL
    private bool TryGetWebsiteUrl(out Uri? url)
    {
        string input = txtUrl.Text.Trim();

        if (!Uri.TryCreate(input, UriKind.Absolute, out url) || url == null)
            return false;

        return url.Scheme == Uri.UriSchemeHttp ||
               url.Scheme == Uri.UriSchemeHttps;
    }
}
