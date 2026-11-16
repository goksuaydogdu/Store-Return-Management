using System;
using System.Windows.Forms;

namespace MagazaIadeOtomasyonu;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var login = new Forms.LoginForm();
        var result = login.ShowDialog();

        if (result == DialogResult.OK)
        {
            Application.Run(new Forms.OperationsForm());
        }
        // If login is canceled or fails, the app exits.
    }
}
