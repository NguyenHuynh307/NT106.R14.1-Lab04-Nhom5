namespace Lab04_Bai07;

partial class FormDangNhap
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
        lblTitle = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        btnLogin = new Button();
        lblPrompt = new Label();
        lnkSignUp = new LinkLabel();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(204, 65, 65);
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(479, 75);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "HÔM NAY ĂN GÌ?";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9.5F);
        lblUsername.Location = new Point(25, 96);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(101, 21);
        lblUsername.TabIndex = 1;
        lblUsername.Text = "Tên tài khoản";
        // 
        // txtUsername
        // 
        txtUsername.Font = new Font("Segoe UI", 9.5F);
        txtUsername.Location = new Point(133, 92);
        txtUsername.Margin = new Padding(3, 4, 3, 4);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(189, 29);
        txtUsername.TabIndex = 2;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Font = new Font("Segoe UI", 9.5F);
        lblPassword.Location = new Point(33, 144);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(75, 21);
        lblPassword.TabIndex = 3;
        lblPassword.Text = "Mật khẩu";
        // 
        // txtPassword
        // 
        txtPassword.Font = new Font("Segoe UI", 9.5F);
        txtPassword.Location = new Point(133, 140);
        txtPassword.Margin = new Padding(3, 4, 3, 4);
        txtPassword.Name = "txtPassword";
        txtPassword.PasswordChar = '●';
        txtPassword.Size = new Size(189, 29);
        txtPassword.TabIndex = 4;
        // 
        // btnLogin
        // 
        btnLogin.Font = new Font("Segoe UI", 9.5F);
        btnLogin.Location = new Point(345, 92);
        btnLogin.Margin = new Padding(3, 4, 3, 4);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(112, 77);
        btnLogin.TabIndex = 5;
        btnLogin.Text = "Đăng nhập";
        btnLogin.UseVisualStyleBackColor = true;
        btnLogin.Click += btnLogin_Click;
        // 
        // lblPrompt
        // 
        lblPrompt.AutoSize = true;
        lblPrompt.Font = new Font("Segoe UI", 8.5F);
        lblPrompt.ForeColor = SystemColors.GrayText;
        lblPrompt.Location = new Point(150, 198);
        lblPrompt.Name = "lblPrompt";
        lblPrompt.Size = new Size(144, 20);
        lblPrompt.TabIndex = 6;
        lblPrompt.Text = "Không có tài khoản?";
        // 
        // lnkSignUp
        // 
        lnkSignUp.AutoSize = true;
        lnkSignUp.Font = new Font("Segoe UI", 8.5F);
        lnkSignUp.Location = new Point(289, 198);
        lnkSignUp.Name = "lnkSignUp";
        lnkSignUp.Size = new Size(63, 20);
        lnkSignUp.TabIndex = 7;
        lnkSignUp.TabStop = true;
        lnkSignUp.Text = "Đăng ký";
        lnkSignUp.LinkClicked += lnkSignUp_LinkClicked;
        // 
        // FormDangNhap
        // 
        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(479, 247);
        Controls.Add(lnkSignUp);
        Controls.Add(lblPrompt);
        Controls.Add(btnLogin);
        Controls.Add(txtPassword);
        Controls.Add(lblPassword);
        Controls.Add(txtUsername);
        Controls.Add(lblUsername);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormDangNhap";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hôm nay ăn gì? - Đăng nhập";
        ResumeLayout(false);
        PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Label lblPrompt;
    private System.Windows.Forms.LinkLabel lnkSignUp;
}