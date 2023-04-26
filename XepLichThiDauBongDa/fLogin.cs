using System;
using System.Windows.Forms;
using XepLichThiDauBongDa.DAO;
using XepLichThiDauBongDa.DTO;

namespace XepLichThiDauBongDa
{
    public partial class fLogin : Form
    {
        public fLogin()
        {
            InitializeComponent();
        }

        public bool Login(string username, string password)
        {
            return AccountDAO.Instance.Login(username, password);
        }

        Account GetAccountByUser(string username)
        {
            return AccountDAO.Instance.GetAccountByUserName(username);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (Login(txtUsername.Text, txtPass.Text))
            {
                Account acc = GetAccountByUser(txtUsername.Text);
                if (acc.Role == 0)
                {
                    this.Hide();
                    fUser f = new fUser();
                    f.acc = acc;
                    f.ShowDialog();
                    this.Close();
                } else
                {
                    this.Hide();
                    fAdmin f = new fAdmin();
                    f.acc = acc;
                    f.ShowDialog();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Text = "";
                txtPass.Text = "";
                txtUsername.Focus();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPass.Text = "";
            txtUsername.Focus();
        }

        private void cbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPass.Checked)
            {
                txtPass.PasswordChar = '\0';
            }
            else
            {
                txtPass.PasswordChar = '•';
            }
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            fRegister fRegister = new fRegister();
            fRegister.ShowDialog();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
