namespace Lab04_Bai07;

partial class FormCNAnGiGio
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
        grpNguonDuLieu = new GroupBox();
        rdoCuaToi = new RadioButton();
        rdoCaCongDong = new RadioButton();
        pnlKetQua = new Panel();
        txtMoTa = new TextBox();
        lblNguoiDongGop = new Label();
        lblDiaChi = new Label();
        lblGia = new Label();
        lblTenMon = new Label();
        picMonAn = new PictureBox();
        btnChonLai = new Button();
        btnDong = new Button();
        grpNguonDuLieu.SuspendLayout();
        pnlKetQua.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picMonAn).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(204, 65, 65);
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(526, 45);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "HÔM NAY ĂN GÌ?";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // grpNguonDuLieu
        // 
        grpNguonDuLieu.Controls.Add(rdoCuaToi);
        grpNguonDuLieu.Controls.Add(rdoCaCongDong);
        grpNguonDuLieu.Font = new Font("Segoe UI", 9F);
        grpNguonDuLieu.Location = new Point(21, 64);
        grpNguonDuLieu.Margin = new Padding(3, 4, 3, 4);
        grpNguonDuLieu.Name = "grpNguonDuLieu";
        grpNguonDuLieu.Padding = new Padding(3, 4, 3, 4);
        grpNguonDuLieu.Size = new Size(485, 69);
        grpNguonDuLieu.TabIndex = 1;
        grpNguonDuLieu.TabStop = false;
        grpNguonDuLieu.Text = "Nguồn món ăn";
        // 
        // rdoCuaToi
        // 
        rdoCuaToi.AutoSize = true;
        rdoCuaToi.Location = new Point(206, 29);
        rdoCuaToi.Margin = new Padding(3, 4, 3, 4);
        rdoCuaToi.Name = "rdoCuaToi";
        rdoCuaToi.Size = new Size(181, 24);
        rdoCuaToi.TabIndex = 1;
        rdoCuaToi.Text = "Của tôi (Tôi đóng góp)";
        rdoCuaToi.UseVisualStyleBackColor = true;
        // 
        // rdoCaCongDong
        // 
        rdoCaCongDong.AutoSize = true;
        rdoCaCongDong.Checked = true;
        rdoCaCongDong.Location = new Point(21, 29);
        rdoCaCongDong.Margin = new Padding(3, 4, 3, 4);
        rdoCaCongDong.Name = "rdoCaCongDong";
        rdoCaCongDong.Size = new Size(155, 24);
        rdoCaCongDong.TabIndex = 0;
        rdoCaCongDong.TabStop = true;
        rdoCaCongDong.Text = "Cả cộng đồng (All)";
        rdoCaCongDong.UseVisualStyleBackColor = true;
        // 
        // pnlKetQua
        // 
        pnlKetQua.BackColor = Color.White;
        pnlKetQua.BorderStyle = BorderStyle.FixedSingle;
        pnlKetQua.Controls.Add(txtMoTa);
        pnlKetQua.Controls.Add(lblNguoiDongGop);
        pnlKetQua.Controls.Add(lblDiaChi);
        pnlKetQua.Controls.Add(lblGia);
        pnlKetQua.Controls.Add(lblTenMon);
        pnlKetQua.Controls.Add(picMonAn);
        pnlKetQua.Location = new Point(21, 147);
        pnlKetQua.Margin = new Padding(3, 4, 3, 4);
        pnlKetQua.Name = "pnlKetQua";
        pnlKetQua.Size = new Size(484, 285);
        pnlKetQua.TabIndex = 2;
        // 
        // txtMoTa
        // 
        txtMoTa.BackColor = Color.FromArgb(250, 250, 250);
        txtMoTa.BorderStyle = BorderStyle.FixedSingle;
        txtMoTa.Font = new Font("Segoe UI", 9F);
        txtMoTa.Location = new Point(16, 200);
        txtMoTa.Margin = new Padding(3, 4, 3, 4);
        txtMoTa.Multiline = true;
        txtMoTa.Name = "txtMoTa";
        txtMoTa.ReadOnly = true;
        txtMoTa.ScrollBars = ScrollBars.Vertical;
        txtMoTa.Size = new Size(450, 63);
        txtMoTa.TabIndex = 5;
        // 
        // lblNguoiDongGop
        // 
        lblNguoiDongGop.Font = new Font("Segoe UI", 9F);
        lblNguoiDongGop.ForeColor = Color.FromArgb(46, 139, 87);
        lblNguoiDongGop.Location = new Point(174, 155);
        lblNguoiDongGop.Name = "lblNguoiDongGop";
        lblNguoiDongGop.Size = new Size(293, 32);
        lblNguoiDongGop.TabIndex = 4;
        lblNguoiDongGop.Text = "Đóng góp: -";
        // 
        // lblDiaChi
        // 
        lblDiaChi.Font = new Font("Segoe UI", 9.5F);
        lblDiaChi.Location = new Point(174, 99);
        lblDiaChi.Name = "lblDiaChi";
        lblDiaChi.Size = new Size(293, 51);
        lblDiaChi.TabIndex = 3;
        lblDiaChi.Text = "Địa chỉ: -";
        // 
        // lblGia
        // 
        lblGia.Font = new Font("Segoe UI", 9.5F);
        lblGia.Location = new Point(174, 64);
        lblGia.Name = "lblGia";
        lblGia.Size = new Size(293, 29);
        lblGia.TabIndex = 2;
        lblGia.Text = "Giá: -";
        // 
        // lblTenMon
        // 
        lblTenMon.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTenMon.ForeColor = Color.FromArgb(204, 65, 65);
        lblTenMon.Location = new Point(174, 19);
        lblTenMon.Name = "lblTenMon";
        lblTenMon.Size = new Size(293, 40);
        lblTenMon.TabIndex = 1;
        lblTenMon.Text = "Chưa chọn món nào";
        // 
        // picMonAn
        // 
        picMonAn.BackColor = Color.FromArgb(248, 248, 248);
        picMonAn.BorderStyle = BorderStyle.FixedSingle;
        picMonAn.Location = new Point(16, 19);
        picMonAn.Margin = new Padding(3, 4, 3, 4);
        picMonAn.Name = "picMonAn";
        picMonAn.Size = new Size(146, 170);
        picMonAn.SizeMode = PictureBoxSizeMode.Zoom;
        picMonAn.TabIndex = 0;
        picMonAn.TabStop = false;
        // 
        // btnChonLai
        // 
        btnChonLai.BackColor = Color.FromArgb(255, 228, 181);
        btnChonLai.FlatAppearance.BorderColor = Color.FromArgb(235, 185, 110);
        btnChonLai.FlatStyle = FlatStyle.Flat;
        btnChonLai.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnChonLai.ForeColor = Color.FromArgb(120, 65, 15);
        btnChonLai.Location = new Point(256, 448);
        btnChonLai.Margin = new Padding(3, 4, 3, 4);
        btnChonLai.Name = "btnChonLai";
        btnChonLai.Size = new Size(142, 45);
        btnChonLai.TabIndex = 3;
        btnChonLai.Text = "Chọn món khác";
        btnChonLai.UseVisualStyleBackColor = false;
        btnChonLai.Click += btnChonLai_Click;
        // 
        // btnDong
        // 
        btnDong.Font = new Font("Segoe UI", 9.5F);
        btnDong.Location = new Point(409, 448);
        btnDong.Margin = new Padding(3, 4, 3, 4);
        btnDong.Name = "btnDong";
        btnDong.Size = new Size(96, 45);
        btnDong.TabIndex = 4;
        btnDong.Text = "Đóng";
        btnDong.UseVisualStyleBackColor = true;
        btnDong.Click += btnDong_Click;
        // 
        // FormCNAnGiGio
        // 
        AcceptButton = btnChonLai;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(526, 512);
        Controls.Add(btnDong);
        Controls.Add(btnChonLai);
        Controls.Add(pnlKetQua);
        Controls.Add(grpNguonDuLieu);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormCNAnGiGio";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Ăn gì giờ?";
        grpNguonDuLieu.ResumeLayout(false);
        grpNguonDuLieu.PerformLayout();
        pnlKetQua.ResumeLayout(false);
        pnlKetQua.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picMonAn).EndInit();
        ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.GroupBox grpNguonDuLieu;
    private System.Windows.Forms.RadioButton rdoCaCongDong;
    private System.Windows.Forms.RadioButton rdoCuaToi;
    private System.Windows.Forms.Panel pnlKetQua;
    private System.Windows.Forms.PictureBox picMonAn;
    private System.Windows.Forms.Label lblTenMon;
    private System.Windows.Forms.Label lblGia;
    private System.Windows.Forms.Label lblDiaChi;
    private System.Windows.Forms.Label lblNguoiDongGop;
    private System.Windows.Forms.TextBox txtMoTa;
    private System.Windows.Forms.Button btnChonLai;
    private System.Windows.Forms.Button btnDong;
}