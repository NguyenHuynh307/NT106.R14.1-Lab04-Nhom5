using System.Xml.Linq;

using static System.Net.Mime.MediaTypeNames;

namespace Lab04_Bai01;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label lblUrl;
    private System.Windows.Forms.TextBox txtUrl;
    private System.Windows.Forms.Button btnGet;
    private System.Windows.Forms.RichTextBox rtbContent;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblUrl = new Label();
        txtUrl = new TextBox();
        btnGet = new Button();
        rtbContent = new RichTextBox();
        SuspendLayout();
        // 
        // lblUrl
        // 
        lblUrl.AutoSize = true;
        lblUrl.Location = new Point(23, 31);
        lblUrl.Name = "lblUrl";
        lblUrl.Size = new Size(38, 20);
        lblUrl.TabIndex = 3;
        lblUrl.Text = "URL:";
        // 
        // txtUrl
        // 
        txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtUrl.Location = new Point(69, 27);
        txtUrl.Margin = new Padding(3, 4, 3, 4);
        txtUrl.Name = "txtUrl";
        txtUrl.Size = new Size(662, 27);
        txtUrl.TabIndex = 2;
        txtUrl.Text = "http://localhost";
        // 
        // btnGet
        // 
        btnGet.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnGet.Location = new Point(749, 25);
        btnGet.Margin = new Padding(3, 4, 3, 4);
        btnGet.Name = "btnGet";
        btnGet.Size = new Size(109, 33);
        btnGet.TabIndex = 1;
        btnGet.Text = "Get HTML";
        btnGet.UseVisualStyleBackColor = true;
        btnGet.Click += btnGet_Click;
        // 
        // rtbContent
        // 
        rtbContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        rtbContent.Location = new Point(23, 80);
        rtbContent.Margin = new Padding(3, 4, 3, 4);
        rtbContent.Name = "rtbContent";
        rtbContent.ReadOnly = true;
        rtbContent.Size = new Size(834, 505);
        rtbContent.TabIndex = 0;
        rtbContent.Text = "";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(880, 613);
        Controls.Add(rtbContent);
        Controls.Add(btnGet);
        Controls.Add(txtUrl);
        Controls.Add(lblUrl);
        Margin = new Padding(3, 4, 3, 4);
        Name = "Form1";
        Text = "Lab04 - Bài 1: Get HTML";
        ResumeLayout(false);
        PerformLayout();
    }
}