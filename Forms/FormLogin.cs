using NoteApp.JWT;
using NoteApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteApp.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormRegister formRegister = new FormRegister();
            formRegister.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            UserService userService = new UserService();

            var userName = txtUserName.Text.Trim();
            var password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please fill in all fields.";
                lblStatus.ForeColor = Color.Red;
                return;
            }
            var user = userService.UserLogin(userName, password);

            if (user != null)
            {
                TokenGenerator tokenGenerator = new TokenGenerator();
                string token = tokenGenerator.GenerateJwtToken(user.UserName, user.Id);

                TokenValidator tokenValidator = new TokenValidator();
                var principal = tokenValidator.ValidateJwtToken(token);

                if (principal == null)
                {
                    lblStatus.Text = "Token doğrulanamadı.";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                FormMain formMain = new FormMain(token);
                formMain.Show();
                this.Hide();
            }
            else
            {
                lblStatus.Text = "Invalid username or password.";
                lblStatus.ForeColor = Color.Red;
                txtPassword.Clear();
                txtUserName.Clear();
                txtUserName.Focus();
            }
        }
    }
}
