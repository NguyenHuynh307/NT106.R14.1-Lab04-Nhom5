namespace Lab04_Bai07;

partial class FormCNThemMonAn
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
        grpThongTinMon = new GroupBox();
        picPreview = new PictureBox();
        btnChonAnh = new Button();
        txtDuongDanAnh = new TextBox();
        lblHinhAnh = new Label();
        txtMoTa = new TextBox();
        lblMoTa = new Label();
        txtDiaChi = new TextBox();
        lblDiaChi = new Label();
        numGia = new NumericUpDown();
        lblGia = new Label();
        txtTenMon = new TextBox();
        lblTenMon = new Label();
        btnClear = new Button();
        btnLuu = new Button();
        grpThongTinMon.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numGia).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(204, 65, 65);
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(514, 45);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "THÊM MÓN ĂN MỚI";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // grpThongTinMon
        // 
        grpThongTinMon.Controls.Add(picPreview);
        grpThongTinMon.Controls.Add(btnChonAnh);
        grpThongTinMon.Controls.Add(txtDuongDanAnh);
        grpThongTinMon.Controls.Add(lblHinhAnh);
        grpThongTinMon.Controls.Add(txtMoTa);
        grpThongTinMon.Controls.Add(lblMoTa);
        grpThongTinMon.Controls.Add(txtDiaChi);
        grpThongTinMon.Controls.Add(lblDiaChi);
        grpThongTinMon.Controls.Add(numGia);
        grpThongTinMon.Controls.Add(lblGia);
        grpThongTinMon.Controls.Add(txtTenMon);
        grpThongTinMon.Controls.Add(lblTenMon);
        grpThongTinMon.Font = new Font("Segoe UI", 9F);
        grpThongTinMon.Location = new Point(21, 64);
        grpThongTinMon.Margin = new Padding(3, 4, 3, 4);
        grpThongTinMon.Name = "grpThongTinMon";
        grpThongTinMon.Padding = new Padding(3, 4, 3, 4);
        grpThongTinMon.Size = new Size(473, 459);
        grpThongTinMon.TabIndex = 1;
        grpThongTinMon.TabStop = false;
        grpThongTinMon.Text = "Thông tin món ăn";
        // 
        // picPreview
        // 
        picPreview.BorderStyle = BorderStyle.FixedSingle;
        picPreview.Location = new Point(119, 288);
        picPreview.Margin = new Padding(3, 4, 3, 4);
        picPreview.Name = "picPreview";
        picPreview.Size = new Size(242, 151);
        picPreview.SizeMode = PictureBoxSizeMode.Zoom;
        picPreview.TabIndex = 11;
        picPreview.TabStop = false;
        // 
        // btnChonAnh
        // 
        btnChonAnh.Location = new Point(368, 247);
        btnChonAnh.Margin = new Padding(3, 4, 3, 4);
        btnChonAnh.Name = "btnChonAnh";
        btnChonAnh.Size = new Size(85, 33);
        btnChonAnh.TabIndex = 10;
        btnChonAnh.Text = "Chọn ảnh";
        btnChonAnh.UseVisualStyleBackColor = true;
        btnChonAnh.Click += btnChonAnh_Click;
        // 
        // txtDuongDanAnh
        // 
        txtDuongDanAnh.BackColor = SystemColors.Window;
        txtDuongDanAnh.ForeColor = SystemColors.WindowText;
        txtDuongDanAnh.Location = new Point(119, 248);
        txtDuongDanAnh.Margin = new Padding(3, 4, 3, 4);
        txtDuongDanAnh.Name = "txtDuongDanAnh";
        txtDuongDanAnh.ReadOnly = true;
        txtDuongDanAnh.Size = new Size(242, 27);
        txtDuongDanAnh.TabIndex = 9;
        // 
        // lblHinhAnh
        // 
        lblHinhAnh.AutoSize = true;
        lblHinhAnh.Location = new Point(16, 252);
        lblHinhAnh.Name = "lblHinhAnh";
        lblHinhAnh.Size = new Size(68, 20);
        lblHinhAnh.TabIndex = 8;
        lblHinhAnh.Text = "Hình ảnh";
        // 
        // txtMoTa
        // 
        txtMoTa.Location = new Point(119, 161);
        txtMoTa.Margin = new Padding(3, 4, 3, 4);
        txtMoTa.Multiline = true;
        txtMoTa.Name = "txtMoTa";
        txtMoTa.ScrollBars = ScrollBars.Vertical;
        txtMoTa.Size = new Size(333, 73);
        txtMoTa.TabIndex = 7;
        // 
        // lblMoTa
        // 
        lblMoTa.AutoSize = true;
        lblMoTa.Location = new Point(16, 165);
        lblMoTa.Name = "lblMoTa";
        lblMoTa.Size = new Size(48, 20);
        lblMoTa.TabIndex = 6;
        lblMoTa.Text = "Mô tả";
        // 
        // txtDiaChi
        // 
        txtDiaChi.Location = new Point(119, 119);
        txtDiaChi.Margin = new Padding(3, 4, 3, 4);
        txtDiaChi.Name = "txtDiaChi";
        txtDiaChi.Size = new Size(333, 27);
        txtDiaChi.TabIndex = 5;
        // 
        // lblDiaChi
        // 
        lblDiaChi.AutoSize = true;
        lblDiaChi.Location = new Point(16, 123);
        lblDiaChi.Name = "lblDiaChi";
        lblDiaChi.Size = new Size(55, 20);
        lblDiaChi.TabIndex = 4;
        lblDiaChi.Text = "Địa chỉ";
        // 
        // numGia
        // 
        numGia.Increment = new decimal(new int[] { 5000, 0, 0, 0 });
        numGia.Location = new Point(119, 76);
        numGia.Margin = new Padding(3, 4, 3, 4);
        numGia.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
        numGia.Name = "numGia";
        numGia.Size = new Size(183, 27);
        numGia.TabIndex = 3;
        numGia.ThousandsSeparator = true;
        // 
        // lblGia
        // 
        lblGia.AutoSize = true;
        lblGia.Location = new Point(16, 80);
        lblGia.Name = "lblGia";
        lblGia.Size = new Size(76, 20);
        lblGia.TabIndex = 2;
        lblGia.Text = "Giá (VNĐ)";
        // 
        // txtTenMon
        // 
        txtTenMon.Location = new Point(119, 33);
        txtTenMon.Margin = new Padding(3, 4, 3, 4);
        txtTenMon.Name = "txtTenMon";
        txtTenMon.Size = new Size(333, 27);
        txtTenMon.TabIndex = 1;
        // 
        // lblTenMon
        // 
        lblTenMon.AutoSize = true;
        lblTenMon.Location = new Point(16, 37);
        lblTenMon.Name = "lblTenMon";
        lblTenMon.Size = new Size(66, 20);
        lblTenMon.TabIndex = 0;
        lblTenMon.Text = "Tên món";
        // 
        // btnClear
        // 
        btnClear.Location = new Point(295, 536);
        btnClear.Margin = new Padding(3, 4, 3, 4);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(94, 43);
        btnClear.TabIndex = 2;
        btnClear.Text = "Xóa";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += btnClear_Click;
        // 
        // btnLuu
        // 
        btnLuu.BackColor = Color.FromArgb(255, 228, 181);
        btnLuu.FlatAppearance.BorderColor = Color.FromArgb(235, 185, 110);
        btnLuu.FlatStyle = FlatStyle.Flat;
        btnLuu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLuu.ForeColor = Color.FromArgb(120, 65, 15);
        btnLuu.Location = new Point(400, 536);
        btnLuu.Margin = new Padding(3, 4, 3, 4);
        btnLuu.Name = "btnLuu";
        btnLuu.Size = new Size(94, 43);
        btnLuu.TabIndex = 3;
        btnLuu.Text = "Lưu";
        btnLuu.UseVisualStyleBackColor = false;
        btnLuu.Click += btnLuu_Click;
        // 
        // FormCNThemMonAn
        // 
        AcceptButton = btnLuu;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(514, 595);
        Controls.Add(btnLuu);
        Controls.Add(btnClear);
        Controls.Add(grpThongTinMon);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormCNThemMonAn";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thêm món ăn";
        grpThongTinMon.ResumeLayout(false);
        grpThongTinMon.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
        ((System.ComponentModel.ISupportInitialize)numGia).EndInit();
        ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.GroupBox grpThongTinMon;
    private System.Windows.Forms.Label lblTenMon;
    private System.Windows.Forms.TextBox txtTenMon;
    private System.Windows.Forms.Label lblGia;
    private System.Windows.Forms.NumericUpDown numGia;
    private System.Windows.Forms.Label lblDiaChi;
    private System.Windows.Forms.TextBox txtDiaChi;
    private System.Windows.Forms.Label lblMoTa;
    private System.Windows.Forms.TextBox txtMoTa;
    private System.Windows.Forms.Label lblHinhAnh;
    private System.Windows.Forms.TextBox txtDuongDanAnh;
    private System.Windows.Forms.Button btnChonAnh;
    private System.Windows.Forms.PictureBox picPreview;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnLuu;
}