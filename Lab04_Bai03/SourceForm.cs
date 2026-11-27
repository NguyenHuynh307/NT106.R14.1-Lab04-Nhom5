
using System.Drawing;
using System.Windows.Forms;

namespace Lab04_Bai03;

public partial class SourceForm : Form
{
    private readonly RichTextBox txtSource;

    public SourceForm(string html)
    {
        InitializeComponent();

        Text = "View Source - HTML";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(1000, 700);
        MinimumSize = new Size(700, 450);

        txtSource = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            WordWrap = false,
            Font = new Font("Consolas", 10F),
            DetectUrls = false,
            BackColor = Color.White,
            ForeColor = Color.Black
        };

        txtSource.Text = html;

        Controls.Add(txtSource);
    }
}
