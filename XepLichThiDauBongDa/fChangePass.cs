using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XepLichThiDauBongDa.DAO;
using XepLichThiDauBongDa.DTO;

namespace XepLichThiDauBongDa
{
    public partial class fChangePass : Form
    {
        public Account acc;
        public fChangePass()
        {
            InitializeComponent();
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            string cur = txtCurPass.Text;
            string newPass = txtNewPass.Text;
            string rePass = txtReEnterPass.Text;
            if(AccountDAO.Instance.Login(acc.Username, cur)) 
            {
                if(newPass.Equals(rePass))
                {
                    AccountDAO.Instance.UpdateAccount(acc.Username, newPass);
                    MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } 
                else
                {
                    MessageBox.Show("Xác nhận mật khẩu không khớp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Sai mật khẩu cũ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
