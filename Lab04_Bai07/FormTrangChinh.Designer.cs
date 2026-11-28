namespace Lab04_Bai07;

partial class FormTrangChinh
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
        lblBrandTitle = new Label();
        btnAnGiGio = new Button();
        btnThemMonAn = new Button();
        tabMain = new TabControl();
        tabAll = new TabPage();
        pnlAllItems = new FlowLayoutPanel();
        tabMyContributions = new TabPage();
        pnlMyItems = new FlowLayoutPanel();
        lblPage = new Label();
        cboPage = new ComboBox();
        lblPageSize = new Label();
        cboPageSize = new ComboBox();
        pnlStatusBar = new Panel();
        prgStatus = new ProgressBar();
        lnkLogout = new LinkLabel();
        lblWelcome = new Label();
        tabMain.SuspendLayout();
        tabAll.SuspendLayout();
        tabMyContributions.SuspendLayout();
        pnlStatusBar.SuspendLayout();
        SuspendLayout();
        // 
        // lblBrandTitle
        // 
        lblBrandTitle.AutoSize = true;
        lblBrandTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblBrandTitle.ForeColor = Color.FromArgb(204, 65, 65);
        lblBrandTitle.Location = new Point(18, 16);
        lblBrandTitle.Name = "lblBrandTitle";
        lblBrandTitle.Size = new Size(267, 41);
        lblBrandTitle.TabIndex = 0;
        lblBrandTitle.Text = "HÔM NAY ĂN GÌ?";
        // 
        // btnAnGiGio
        // 
        btnAnGiGio.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAnGiGio.BackColor = Color.FromArgb(255, 228, 181);
        btnAnGiGio.FlatAppearance.BorderColor = Color.FromArgb(235, 185, 110);
        btnAnGiGio.FlatStyle = FlatStyle.Flat;
        btnAnGiGio.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnAnGiGio.ForeColor = Color.FromArgb(120, 65, 15);
        btnAnGiGio.Location = new Point(322, 34);
        btnAnGiGio.Margin = new Padding(3, 4, 3, 4);
        btnAnGiGio.Name = "btnAnGiGio";
        btnAnGiGio.Size = new Size(145, 59);
        btnAnGiGio.TabIndex = 1;
        btnAnGiGio.Text = "Ăn gì giờ?";
        btnAnGiGio.UseVisualStyleBackColor = false;
        btnAnGiGio.Click += btnAnGiGio_Click;
        // 
        // btnThemMonAn
        // 
        btnThemMonAn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnThemMonAn.BackColor = Color.FromArgb(255, 228, 181);
        btnThemMonAn.FlatAppearance.BorderColor = Color.FromArgb(235, 185, 110);
        btnThemMonAn.FlatStyle = FlatStyle.Flat;
        btnThemMonAn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnThemMonAn.ForeColor = Color.FromArgb(120, 65, 15);
        btnThemMonAn.Location = new Point(481, 34);
        btnThemMonAn.Margin = new Padding(3, 4, 3, 4);
        btnThemMonAn.Name = "btnThemMonAn";
        btnThemMonAn.Size = new Size(145, 59);
        btnThemMonAn.TabIndex = 2;
        btnThemMonAn.Text = "Thêm món ăn";
        btnThemMonAn.UseVisualStyleBackColor = false;
        btnThemMonAn.Click += btnThemMonAn_Click;
        // 
        // tabMain
        // 
        tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabMain.Controls.Add(tabAll);
        tabMain.Controls.Add(tabMyContributions);
        tabMain.Font = new Font("Segoe UI", 9F);
        tabMain.Location = new Point(18, 101);
        tabMain.Margin = new Padding(3, 4, 3, 4);
        tabMain.Name = "tabMain";
        tabMain.SelectedIndex = 0;
        tabMain.Size = new Size(619, 565);
        tabMain.TabIndex = 3;
        // 
        // tabAll
        // 
        tabAll.Controls.Add(pnlAllItems);
        tabAll.Location = new Point(4, 29);
        tabAll.Margin = new Padding(3, 4, 3, 4);
        tabAll.Name = "tabAll";
        tabAll.Padding = new Padding(7, 8, 7, 8);
        tabAll.Size = new Size(611, 532);
        tabAll.TabIndex = 0;
        tabAll.Text = "All";
        tabAll.UseVisualStyleBackColor = true;
        // 
        // pnlAllItems
        // 
        pnlAllItems.AutoScroll = true;
        pnlAllItems.Dock = DockStyle.Fill;
        pnlAllItems.FlowDirection = FlowDirection.TopDown;
        pnlAllItems.Location = new Point(7, 8);
        pnlAllItems.Margin = new Padding(3, 4, 3, 4);
        pnlAllItems.Name = "pnlAllItems";
        pnlAllItems.Size = new Size(597, 516);
        pnlAllItems.TabIndex = 0;
        pnlAllItems.WrapContents = false;
        // 
        // tabMyContributions
        // 
        tabMyContributions.Controls.Add(pnlMyItems);
        tabMyContributions.Location = new Point(4, 29);
        tabMyContributions.Margin = new Padding(3, 4, 3, 4);
        tabMyContributions.Name = "tabMyContributions";
        tabMyContributions.Padding = new Padding(7, 8, 7, 8);
        tabMyContributions.Size = new Size(611, 532);
        tabMyContributions.TabIndex = 1;
        tabMyContributions.Text = "Tôi đóng góp";
        tabMyContributions.UseVisualStyleBackColor = true;
        // 
        // pnlMyItems
        // 
        pnlMyItems.AutoScroll = true;
        pnlMyItems.Dock = DockStyle.Fill;
        pnlMyItems.FlowDirection = FlowDirection.TopDown;
        pnlMyItems.Location = new Point(7, 8);
        pnlMyItems.Margin = new Padding(3, 4, 3, 4);
        pnlMyItems.Name = "pnlMyItems";
        pnlMyItems.Size = new Size(597, 516);
        pnlMyItems.TabIndex = 0;
        pnlMyItems.WrapContents = false;
        // 
        // lblPage
        // 
        lblPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblPage.AutoSize = true;
        lblPage.Font = new Font("Segoe UI", 9F);
        lblPage.Location = new Point(414, 680);
        lblPage.Name = "lblPage";
        lblPage.Size = new Size(41, 20);
        lblPage.TabIndex = 4;
        lblPage.Text = "Page";
        // 
        // cboPage
        // 
        cboPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        cboPage.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPage.Font = new Font("Segoe UI", 9F);
        cboPage.FormattingEnabled = true;
        cboPage.Items.AddRange(new object[] { "1" });
        cboPage.Location = new Point(461, 675);
        cboPage.Margin = new Padding(3, 4, 3, 4);
        cboPage.Name = "cboPage";
        cboPage.Size = new Size(54, 28);
        cboPage.TabIndex = 5;
        // 
        // lblPageSize
        // 
        lblPageSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblPageSize.AutoSize = true;
        lblPageSize.Font = new Font("Segoe UI", 9F);
        lblPageSize.Location = new Point(525, 680);
        lblPageSize.Name = "lblPageSize";
        lblPageSize.Size = new Size(70, 20);
        lblPageSize.TabIndex = 6;
        lblPageSize.Text = "Page size";
        // 
        // cboPageSize
        // 
        cboPageSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPageSize.Font = new Font("Segoe UI", 9F);
        cboPageSize.FormattingEnabled = true;
        cboPageSize.Items.AddRange(new object[] { "5", "10", "20" });
        cboPageSize.Location = new Point(592, 675);
        cboPageSize.Margin = new Padding(3, 4, 3, 4);
        cboPageSize.Name = "cboPageSize";
        cboPageSize.Size = new Size(45, 28);
        cboPageSize.TabIndex = 7;
        // 
        // pnlStatusBar
        // 
        pnlStatusBar.BackColor = Color.FromArgb(245, 245, 245);
        pnlStatusBar.Controls.Add(prgStatus);
        pnlStatusBar.Controls.Add(lnkLogout);
        pnlStatusBar.Controls.Add(lblWelcome);
        pnlStatusBar.Dock = DockStyle.Bottom;
        pnlStatusBar.Location = new Point(0, 715);
        pnlStatusBar.Margin = new Padding(3, 4, 3, 4);
        pnlStatusBar.Name = "pnlStatusBar";
        pnlStatusBar.Size = new Size(656, 37);
        pnlStatusBar.TabIndex = 8;
        // 
        // prgStatus
        // 
        prgStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        prgStatus.ForeColor = Color.FromArgb(46, 164, 79);
        prgStatus.Location = new Point(500, 9);
        prgStatus.Margin = new Padding(3, 4, 3, 4);
        prgStatus.Name = "prgStatus";
        prgStatus.Size = new Size(137, 21);
        prgStatus.Style = ProgressBarStyle.Continuous;
        prgStatus.TabIndex = 2;
        prgStatus.Value = 100;
        // 
        // lnkLogout
        // 
        lnkLogout.AutoSize = true;
        lnkLogout.Font = new Font("Segoe UI", 8.5F);
        lnkLogout.Location = new Point(429, 10);
        lnkLogout.Name = "lnkLogout";
        lnkLogout.Size = new Size(56, 20);
        lnkLogout.TabIndex = 1;
        lnkLogout.TabStop = true;
        lnkLogout.Text = "Logout";
        lnkLogout.LinkClicked += lnkLogout_LinkClicked;
        // 
        // lblWelcome
        // 
        lblWelcome.AutoSize = true;
        lblWelcome.Font = new Font("Segoe UI", 8.5F);
        lblWelcome.ForeColor = Color.FromArgb(46, 139, 87);
        lblWelcome.Location = new Point(14, 9);
        lblWelcome.Name = "lblWelcome";
        lblWelcome.Size = new Size(111, 20);
        lblWelcome.TabIndex = 0;
        lblWelcome.Text = "Welcome, User ";
        // 
        // FormTrangChinh
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(656, 752);
        Controls.Add(pnlStatusBar);
        Controls.Add(cboPageSize);
        Controls.Add(lblPageSize);
        Controls.Add(cboPage);
        Controls.Add(lblPage);
        Controls.Add(tabMain);
        Controls.Add(btnThemMonAn);
        Controls.Add(btnAnGiGio);
        Controls.Add(lblBrandTitle);
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(592, 598);
        Name = "FormTrangChinh";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hôm nay ăn gì?";
        tabMain.ResumeLayout(false);
        tabAll.ResumeLayout(false);
        tabMyContributions.ResumeLayout(false);
        pnlStatusBar.ResumeLayout(false);
        pnlStatusBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblBrandTitle;
    private System.Windows.Forms.Button btnAnGiGio;
    private System.Windows.Forms.Button btnThemMonAn;
    private System.Windows.Forms.TabControl tabMain;
    private System.Windows.Forms.TabPage tabAll;
    private System.Windows.Forms.FlowLayoutPanel pnlAllItems;
    private System.Windows.Forms.TabPage tabMyContributions;
    private System.Windows.Forms.FlowLayoutPanel pnlMyItems;
    private System.Windows.Forms.Label lblPage;
    private System.Windows.Forms.ComboBox cboPage;
    private System.Windows.Forms.Label lblPageSize;
    private System.Windows.Forms.ComboBox cboPageSize;
    private System.Windows.Forms.Panel pnlStatusBar;
    private System.Windows.Forms.Label lblWelcome;
    private System.Windows.Forms.LinkLabel lnkLogout;
    private System.Windows.Forms.ProgressBar prgStatus;
}