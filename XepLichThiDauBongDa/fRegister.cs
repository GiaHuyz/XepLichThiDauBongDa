using System;
using System.Windows.Forms;
using XepLichThiDauBongDa.DAO;

namespace XepLichThiDauBongDa
{
    public partial class fRegister : Form
    {
        public fRegister()
        {
            InitializeComponent();

            txtUsername.Focus();
        }

        bool InsertAccount(string username, string password)
        {
            return AccountDAO.Instance.InsertAccount(username, password);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string pass = txtPass.Text;
            string comPass = txtComPass.Text;
            if (username == "" && pass == "" && comPass == "")
            {
                MessageBox.Show("Điền không đủ thông tin", "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (pass == comPass)
            {
                InsertAccount(username, pass);
                MessageBox.Show("Tài khoản được tạo thành công", "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUsername.Text = "";
                txtPass.Text = "";
                txtComPass.Text = "";
                this.Hide();
                fLogin f = new fLogin();
                f.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Mật khẩu không khớp, hãy nhập lại", "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPass.Text = "";
                txtComPass.Text = "";
                txtPass.Focus();
            }
        }

        private void cbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPass.Checked)
            {
                txtPass.PasswordChar = '\0';
                txtComPass.PasswordChar = '\0';
            }
            else
            {
                txtPass.PasswordChar = '•';
                txtComPass.PasswordChar = '•';
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPass.Text = "";
            txtComPass.Text = "";
            txtUsername.Focus();
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            fLogin fLogin = new fLogin();
            fLogin.ShowDialog();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
