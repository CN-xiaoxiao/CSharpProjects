using LibraryManagerBLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibraryManagerPro
{
    public partial class FrmModifyPwd : Form
    {
        public FrmModifyPwd()
        {
            InitializeComponent();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (this.txtNewPwd.Text.Trim().Length == 0 || this.txtOldPwd.Text.Trim().Length == 0 || this.txtNewPwdConfirm.Text.Trim().Length == 0)
            {
                MessageBox.Show("所有内容必须填写！", "修改提示");
                return;
            }
            if (this.txtOldPwd.Text.Trim() != Program.objCurrentAdmin.LoginPwd)
            {
                MessageBox.Show("原密码不正确！", "修改提示");
                this.txtOldPwd.SelectAll();
                this.txtOldPwd.Focus();
                return;
            }
            if (this.txtNewPwd.Text.Trim() != this.txtNewPwdConfirm.Text.Trim())
            {
                MessageBox.Show("两次输入的密码不一致！", "修改提示");
                return;
            }

            try
            {
                new SysAdminManager().ModifyPwd(Program.objCurrentAdmin.AdminId.ToString(), this.txtNewPwd.Text.Trim());
                Program.objCurrentAdmin.LoginPwd = this.txtNewPwd.Text.Trim();

                MessageBox.Show("密码修改成功！", "修改提示");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
