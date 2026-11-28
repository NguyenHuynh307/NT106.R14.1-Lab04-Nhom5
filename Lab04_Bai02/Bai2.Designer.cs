namespace Lab04_Bai02;

partial class Bai2
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        txtUrl = new TextBox();
        txtFilePath = new TextBox();
        btnDownload = new Button();
        txtHtmlContent = new TextBox();
        SuspendLayout();
        // 
        // txtUrl
        // 
        txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtUrl.Font = new Font("Segoe UI", 10F);
        txtUrl.Location = new Point(16, 18);
        txtUrl.Margin = new Padding(4, 5, 4, 5);
        txtUrl.Name = "txtUrl";
        txtUrl.Size = new Size(500, 30);
        txtUrl.TabIndex = 0;
        // 
        // txtFilePath
        // 
        txtFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtFilePath.Font = new Font("Segoe UI", 10F);
        txtFilePath.Location = new Point(16, 69);
        txtFilePath.Margin = new Padding(4, 5, 4, 5);
        txtFilePath.Name = "txtFilePath";
        txtFilePath.Size = new Size(500, 30);
        txtFilePath.TabIndex = 1;
        // 
        // btnDownload
        // 
        btnDownload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDownload.Font = new Font("Segoe UI", 9.5F);
        btnDownload.Location = new Point(531, 18);
        btnDownload.Margin = new Padding(4, 5, 4, 5);
        btnDownload.Name = "btnDownload";
        btnDownload.Size = new Size(120, 89);
        btnDownload.TabIndex = 2;
        btnDownload.Text = "Download";
        btnDownload.UseVisualStyleBackColor = true;
        btnDownload.Click += this.btnDownload_Click;
        // 
        // txtHtmlContent
        // 
        txtHtmlContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtHtmlContent.Font = new Font("Consolas", 9.5F);
        txtHtmlContent.Location = new Point(16, 123);
        txtHtmlContent.Margin = new Padding(4, 5, 4, 5);
        txtHtmlContent.Multiline = true;
        txtHtmlContent.Name = "txtHtmlContent";
        txtHtmlContent.ScrollBars = ScrollBars.Both;
        txtHtmlContent.Size = new Size(633, 518);
        txtHtmlContent.TabIndex = 3;
        // 
        // Bai2
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(667, 662);
        Controls.Add(txtHtmlContent);
        Controls.Add(btnDownload);
        Controls.Add(txtFilePath);
        Controls.Add(txtUrl);
        Margin = new Padding(4, 5, 4, 5);
        MinimumSize = new Size(421, 313);
        Name = "Bai2";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Bai2";
        ResumeLayout(false);
        PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtUrl;
    private System.Windows.Forms.TextBox txtFilePath;
    private System.Windows.Forms.Button btnDownload;
    private System.Windows.Forms.TextBox txtHtmlContent;
}