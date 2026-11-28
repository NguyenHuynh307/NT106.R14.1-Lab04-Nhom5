namespace Lab04_Bai07;

partial class FormDangKy
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
        grpSignUp = new GroupBox();
        txtPassword = new TextBox();
        lblPassword = new Label();
        txtUsername = new TextBox();
        lblUsername = new Label();
        grpUserInfo = new GroupBox();
        rdoFemale = new RadioButton();
        rdoMale = new RadioButton();
        lblSex = new Label();
        cboLanguage = new ComboBox();
        lblLanguage = new Label();
        dtpBirthday = new DateTimePicker();
        lblBirthday = new Label();
        txtPhone = new TextBox();
        lblPhone = new Label();
        txtLastname = new TextBox();
        lblLastname = new Label();
        txtFirstname = new TextBox();
        lblFirstname = new Label();
        txtEmail = new TextBox();
        lblEmail = new Label();
        btnClear = new Button();
        btnSubmit = new Button();
        grpSignUp.SuspendLayout();
        grpUserInfo.SuspendLayout();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(204, 65, 65);
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(491, 48);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "HÔM NAY ĂN GÌ?";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // grpSignUp
        // 
        grpSignUp.Controls.Add(txtPassword);
        grpSignUp.Controls.Add(lblPassword);
        grpSignUp.Controls.Add(txtUsername);
        grpSignUp.Controls.Add(lblUsername);
        grpSignUp.Font = new Font("Segoe UI", 9F);
        grpSignUp.Location = new Point(27, 69);
        grpSignUp.Margin = new Padding(3, 4, 3, 4);
        grpSignUp.Name = "grpSignUp";
        grpSignUp.Padding = new Padding(3, 4, 3, 4);
        grpSignUp.Size = new Size(437, 123);
        grpSignUp.TabIndex = 1;
        grpSignUp.TabStop = false;
        grpSignUp.Text = "Đăng ký";
        // 
        // txtPassword
        // 
        txtPassword.Location = new Point(128, 73);
        txtPassword.Margin = new Padding(3, 4, 3, 4);
        txtPassword.Name = "txtPassword";
        txtPassword.PasswordChar = '●';
        txtPassword.Size = new Size(290, 27);
        txtPassword.TabIndex = 3;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(16, 77);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(70, 20);
        lblPassword.TabIndex = 2;
        lblPassword.Text = "Mật khẩu";
        // 
        // txtUsername
        // 
        txtUsername.Location = new Point(128, 33);
        txtUsername.Margin = new Padding(3, 4, 3, 4);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(290, 27);
        txtUsername.TabIndex = 1;
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(16, 37);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(97, 20);
        lblUsername.TabIndex = 0;
        lblUsername.Text = "Tên tài khoản";
        // 
        // grpUserInfo
        // 
        grpUserInfo.Controls.Add(rdoFemale);
        grpUserInfo.Controls.Add(rdoMale);
        grpUserInfo.Controls.Add(lblSex);
        grpUserInfo.Controls.Add(cboLanguage);
        grpUserInfo.Controls.Add(lblLanguage);
        grpUserInfo.Controls.Add(dtpBirthday);
        grpUserInfo.Controls.Add(lblBirthday);
        grpUserInfo.Controls.Add(txtPhone);
        grpUserInfo.Controls.Add(lblPhone);
        grpUserInfo.Controls.Add(txtLastname);
        grpUserInfo.Controls.Add(lblLastname);
        grpUserInfo.Controls.Add(txtFirstname);
        grpUserInfo.Controls.Add(lblFirstname);
        grpUserInfo.Controls.Add(txtEmail);
        grpUserInfo.Controls.Add(lblEmail);
        grpUserInfo.Font = new Font("Segoe UI", 9F);
        grpUserInfo.Location = new Point(27, 203);
        grpUserInfo.Margin = new Padding(3, 4, 3, 4);
        grpUserInfo.Name = "grpUserInfo";
        grpUserInfo.Padding = new Padding(3, 4, 3, 4);
        grpUserInfo.Size = new Size(437, 363);
        grpUserInfo.TabIndex = 2;
        grpUserInfo.TabStop = false;
        grpUserInfo.Text = "Thông tin người dùng";
        // 
        // rdoFemale
        // 
        rdoFemale.AutoSize = true;
        rdoFemale.Location = new Point(206, 295);
        rdoFemale.Margin = new Padding(3, 4, 3, 4);
        rdoFemale.Name = "rdoFemale";
        rdoFemale.Size = new Size(50, 24);
        rdoFemale.TabIndex = 14;
        rdoFemale.Text = "Nữ";
        rdoFemale.UseVisualStyleBackColor = true;
        // 
        // rdoMale
        // 
        rdoMale.AutoSize = true;
        rdoMale.Checked = true;
        rdoMale.Location = new Point(128, 295);
        rdoMale.Margin = new Padding(3, 4, 3, 4);
        rdoMale.Name = "rdoMale";
        rdoMale.Size = new Size(62, 24);
        rdoMale.TabIndex = 13;
        rdoMale.TabStop = true;
        rdoMale.Text = "Nam";
        rdoMale.UseVisualStyleBackColor = true;
        // 
        // lblSex
        // 
        lblSex.AutoSize = true;
        lblSex.Location = new Point(16, 296);
        lblSex.Name = "lblSex";
        lblSex.Size = new Size(65, 20);
        lblSex.TabIndex = 12;
        lblSex.Text = "Giới tính";
        // 
        // cboLanguage
        // 
        cboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLanguage.FormattingEnabled = true;
        cboLanguage.Items.AddRange(new object[] { "Tiếng Việt" });
        cboLanguage.Location = new Point(128, 247);
        cboLanguage.Margin = new Padding(3, 4, 3, 4);
        cboLanguage.Name = "cboLanguage";
        cboLanguage.Size = new Size(290, 28);
        cboLanguage.TabIndex = 11;
        // 
        // lblLanguage
        // 
        lblLanguage.AutoSize = true;
        lblLanguage.Location = new Point(16, 251);
        lblLanguage.Name = "lblLanguage";
        lblLanguage.Size = new Size(76, 20);
        lblLanguage.TabIndex = 10;
        lblLanguage.Text = "Ngôn ngữ";
        // 
        // dtpBirthday
        // 
        dtpBirthday.CustomFormat = "dd/MM/yyyy";
        dtpBirthday.Format = DateTimePickerFormat.Custom;
        dtpBirthday.Location = new Point(128, 204);
        dtpBirthday.Margin = new Padding(3, 4, 3, 4);
        dtpBirthday.Name = "dtpBirthday";
        dtpBirthday.Size = new Size(290, 27);
        dtpBirthday.TabIndex = 9;
        // 
        // lblBirthday
        // 
        lblBirthday.AutoSize = true;
        lblBirthday.Location = new Point(16, 208);
        lblBirthday.Name = "lblBirthday";
        lblBirthday.Size = new Size(74, 20);
        lblBirthday.TabIndex = 8;
        lblBirthday.Text = "Ngày sinh";
        // 
        // txtPhone
        // 
        txtPhone.Location = new Point(128, 161);
        txtPhone.Margin = new Padding(3, 4, 3, 4);
        txtPhone.Name = "txtPhone";
        txtPhone.Size = new Size(290, 27);
        txtPhone.TabIndex = 7;
        // 
        // lblPhone
        // 
        lblPhone.AutoSize = true;
        lblPhone.Location = new Point(16, 165);
        lblPhone.Name = "lblPhone";
        lblPhone.Size = new Size(36, 20);
        lblPhone.TabIndex = 6;
        lblPhone.Text = "SĐT";
        // 
        // txtLastname
        // 
        txtLastname.Location = new Point(128, 119);
        txtLastname.Margin = new Padding(3, 4, 3, 4);
        txtLastname.Name = "txtLastname";
        txtLastname.Size = new Size(290, 27);
        txtLastname.TabIndex = 5;
        // 
        // lblLastname
        // 
        lblLastname.AutoSize = true;
        lblLastname.Location = new Point(16, 123);
        lblLastname.Name = "lblLastname";
        lblLastname.Size = new Size(29, 20);
        lblLastname.TabIndex = 4;
        lblLastname.Text = "Họ";
        // 
        // txtFirstname
        // 
        txtFirstname.Location = new Point(128, 76);
        txtFirstname.Margin = new Padding(3, 4, 3, 4);
        txtFirstname.Name = "txtFirstname";
        txtFirstname.Size = new Size(290, 27);
        txtFirstname.TabIndex = 3;
        // 
        // lblFirstname
        // 
        lblFirstname.AutoSize = true;
        lblFirstname.Location = new Point(16, 80);
        lblFirstname.Name = "lblFirstname";
        lblFirstname.Size = new Size(32, 20);
        lblFirstname.TabIndex = 2;
        lblFirstname.Text = "Tên";
        // 
        // txtEmail
        // 
        txtEmail.Location = new Point(128, 33);
        txtEmail.Margin = new Padding(3, 4, 3, 4);
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(290, 27);
        txtEmail.TabIndex = 1;
        // 
        // lblEmail
        // 
        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(16, 37);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(46, 20);
        lblEmail.TabIndex = 0;
        lblEmail.Text = "Email";
        // 
        // btnClear
        // 
        btnClear.Location = new Point(277, 579);
        btnClear.Margin = new Padding(3, 4, 3, 4);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(87, 40);
        btnClear.TabIndex = 3;
        btnClear.Text = "Xóa";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += btnClear_Click;
        // 
        // btnSubmit
        // 
        btnSubmit.Location = new Point(377, 579);
        btnSubmit.Margin = new Padding(3, 4, 3, 4);
        btnSubmit.Name = "btnSubmit";
        btnSubmit.Size = new Size(87, 40);
        btnSubmit.TabIndex = 4;
        btnSubmit.Text = "Đăng ký";
        btnSubmit.UseVisualStyleBackColor = true;
        btnSubmit.Click += btnSubmit_Click;
        // 
        // FormDangKy
        // 
        AcceptButton = btnSubmit;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(491, 637);
        Controls.Add(btnSubmit);
        Controls.Add(btnClear);
        Controls.Add(grpUserInfo);
        Controls.Add(grpSignUp);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "FormDangKy";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hôm nay ăn gì? - Đăng ký";
        grpSignUp.ResumeLayout(false);
        grpSignUp.PerformLayout();
        grpUserInfo.ResumeLayout(false);
        grpUserInfo.PerformLayout();
        ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.GroupBox grpSignUp;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.GroupBox grpUserInfo;
    private System.Windows.Forms.Label lblEmail;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.Label lblFirstname;
    private System.Windows.Forms.TextBox txtFirstname;
    private System.Windows.Forms.Label lblLastname;
    private System.Windows.Forms.TextBox txtLastname;
    private System.Windows.Forms.Label lblPhone;
    private System.Windows.Forms.TextBox txtPhone;
    private System.Windows.Forms.Label lblBirthday;
    private System.Windows.Forms.DateTimePicker dtpBirthday;
    private System.Windows.Forms.Label lblLanguage;
    private System.Windows.Forms.ComboBox cboLanguage;
    private System.Windows.Forms.Label lblSex;
    private System.Windows.Forms.RadioButton rdoMale;
    private System.Windows.Forms.RadioButton rdoFemale;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnSubmit;
}