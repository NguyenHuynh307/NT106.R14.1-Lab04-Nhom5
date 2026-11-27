using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Web.WebView2.WinForms;

namespace Lab04_Bai03;

partial class Form1
{
    private IContainer? components = null;

    private TableLayoutPanel rootLayout = null!;
    private TableLayoutPanel toolbarLayout = null!;
    private Label lblUrl = null!;
    private TextBox txtUrl = null!;
    private Button btnLoad = null!;
    private Button btnReload = null!;
    private Button btnDownloadHtml = null!;
    private Button btnDownloadResources = null!;
    private Button btnViewSource = null!;
    private WebView2 webView21 = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        rootLayout = new TableLayoutPanel();
        toolbarLayout = new TableLayoutPanel();
        lblUrl = new Label();
        txtUrl = new TextBox();
        btnLoad = new Button();
        btnReload = new Button();
        btnDownloadHtml = new Button();
        btnDownloadResources = new Button();
        btnViewSource = new Button();
        webView21 = new WebView2();
        rootLayout.SuspendLayout();
        toolbarLayout.SuspendLayout();
        ((ISupportInitialize)webView21).BeginInit();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(toolbarLayout, 0, 0);
        rootLayout.Controls.Add(webView21, 0, 1);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Margin = new Padding(0);
        rootLayout.Name = "rootLayout";
        rootLayout.RowCount = 2;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.Size = new Size(1180, 760);
        rootLayout.TabIndex = 0;
        // 
        // toolbarLayout
        // 
        toolbarLayout.ColumnCount = 6;
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
        toolbarLayout.Controls.Add(lblUrl, 0, 0);
        toolbarLayout.Controls.Add(txtUrl, 0, 1);
        toolbarLayout.Controls.Add(btnLoad, 1, 1);
        toolbarLayout.Controls.Add(btnReload, 2, 1);
        toolbarLayout.Controls.Add(btnDownloadHtml, 3, 1);
        toolbarLayout.Controls.Add(btnDownloadResources, 4, 1);
        toolbarLayout.Controls.Add(btnViewSource, 5, 1);
        toolbarLayout.Dock = DockStyle.Fill;
        toolbarLayout.Location = new Point(3, 3);
        toolbarLayout.Name = "toolbarLayout";
        toolbarLayout.Padding = new Padding(12, 8, 12, 8);
        toolbarLayout.RowCount = 2;
        toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        toolbarLayout.Size = new Size(1174, 84);
        toolbarLayout.TabIndex = 0;
        // 
        // lblUrl
        // 
        lblUrl.Dock = DockStyle.Fill;
        lblUrl.Font = new Font("Segoe UI", 9F);
        lblUrl.Location = new Point(15, 8);
        lblUrl.Name = "lblUrl";
        lblUrl.Size = new Size(523, 25);
        lblUrl.TabIndex = 0;
        lblUrl.Text = "URL";
        lblUrl.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtUrl
        // 
        txtUrl.Dock = DockStyle.Fill;
        txtUrl.Font = new Font("Segoe UI", 10F);
        txtUrl.Location = new Point(15, 37);
        txtUrl.Margin = new Padding(3, 4, 8, 3);
        txtUrl.Name = "txtUrl";
        txtUrl.Size = new Size(518, 30);
        txtUrl.TabIndex = 1;
        // 
        // btnLoad
        // 
        btnLoad.Location = new Point(544, 36);
        btnLoad.Name = "btnLoad";
        btnLoad.Size = new Size(75, 23);
        btnLoad.TabIndex = 2;
        // 
        // btnReload
        // 
        btnReload.Location = new Point(632, 36);
        btnReload.Name = "btnReload";
        btnReload.Size = new Size(75, 23);
        btnReload.TabIndex = 3;
        // 
        // btnDownloadHtml
        // 
        btnDownloadHtml.Location = new Point(720, 36);
        btnDownloadHtml.Name = "btnDownloadHtml";
        btnDownloadHtml.Size = new Size(75, 23);
        btnDownloadHtml.TabIndex = 4;
        // 
        // btnDownloadResources
        // 
        btnDownloadResources.Location = new Point(860, 36);
        btnDownloadResources.Name = "btnDownloadResources";
        btnDownloadResources.Size = new Size(75, 23);
        btnDownloadResources.TabIndex = 5;
        // 
        // btnViewSource
        // 
        btnViewSource.Location = new Point(1040, 36);
        btnViewSource.Name = "btnViewSource";
        btnViewSource.Size = new Size(75, 23);
        btnViewSource.TabIndex = 6;
        // 
        // webView21
        // 
        webView21.AllowExternalDrop = true;
        webView21.CreationProperties = null;
        webView21.DefaultBackgroundColor = Color.White;
        webView21.Dock = DockStyle.Fill;
        webView21.Location = new Point(0, 90);
        webView21.Margin = new Padding(0);
        webView21.Name = "webView21";
        webView21.Size = new Size(1180, 670);
        webView21.TabIndex = 1;
        webView21.ZoomFactor = 1D;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 760);
        Controls.Add(rootLayout);
        MinimumSize = new Size(1000, 650);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Lab 4 - Bai 3 | Web Browser";
        rootLayout.ResumeLayout(false);
        toolbarLayout.ResumeLayout(false);
        toolbarLayout.PerformLayout();
        ((ISupportInitialize)webView21).EndInit();
        ResumeLayout(false);
    }

    private static void ConfigureButton(
        Button button,
        string text,
        string name)
    {
        button.Name = name;
        button.Text = text;

        button.Dock = DockStyle.Fill;
        button.Margin = new Padding(4, 4, 4, 4);

        button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        button.ForeColor = SystemColors.ControlText;
        button.TextAlign = ContentAlignment.MiddleCenter;

        button.AutoSize = false;
        button.AutoEllipsis = true;
        button.UseVisualStyleBackColor = true;
    }
}
