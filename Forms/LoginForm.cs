using System;
using System.Windows.Forms;
using MagazaIadeOtomasyonu.Data;
using MagazaIadeOtomasyonu.Repositories;

namespace MagazaIadeOtomasyonu.Forms;

public class LoginForm : Form
{
    private readonly IUserRepository _userRepository;

    private TextBox _txtUser = null!;
    private TextBox _txtPass = null!;
    private Button _btnLogin = null!;

    public LoginForm()
    {
        _userRepository = new UserRepository();

        BuildUi();
    }

    private void BuildUi()
    {
        Text = "Giriş";
        Width = 320;
        Height = 220;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var lblUser = new Label { Text = "Kullanıcı Adı", Left = 20, Top = 20, AutoSize = true };
        _txtUser = new TextBox { Left = 20, Top = 40, Width = 250 };

        var lblPass = new Label { Text = "Şifre", Left = 20, Top = 75, AutoSize = true };
        _txtPass = new TextBox { Left = 20, Top = 95, Width = 250, PasswordChar = '*' };

        _btnLogin = new Button { Text = "Giriş Yap", Left = 20, Top = 135, Width = 100 };
        _btnLogin.Click += (_, _) => TryLogin(_txtUser.Text, _txtPass.Text);

        // Enter ile giriş yapılsın
        AcceptButton = _btnLogin;

        Controls.Add(lblUser);
        Controls.Add(_txtUser);
        Controls.Add(lblPass);
        Controls.Add(_txtPass);
        Controls.Add(_btnLogin);
    }

    public bool TryLogin(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.", "Giriş");
            return false;
        }

        var user = _userRepository.FindByCredentials(username, password);
        if (user is null)
        {
            MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Giriş Başarısız");
            return false;
        }

        SessionContext.SignIn(user);
        DialogResult = DialogResult.OK;
        Close();
        return true;
    }
}
