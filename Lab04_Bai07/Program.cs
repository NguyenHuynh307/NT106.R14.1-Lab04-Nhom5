using System;
using System.Windows.Forms;

using Lab04_Bai07.Services;

namespace Lab04_Bai07;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        while (true)
        {
            DialogResult loginResult;
            using (var loginForm = new FormDangNhap())
            {
                Application.Run(loginForm);
                loginResult = loginForm.DialogResult;
            }

            if (loginResult != DialogResult.OK || string.IsNullOrWhiteSpace(SessionContext.AccessToken))
            {
                break;
            }

            string? tagAfterClose;
            using (var mainForm = new FormTrangChinh())
            {
                Application.Run(mainForm);
                tagAfterClose = mainForm.Tag as string;
            }

            if (tagAfterClose == "REOPEN_LOGIN")
            {
                continue;
            }

            break;
        }
    }
}