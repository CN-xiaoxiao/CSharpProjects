using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LibraryManagerBLL;
using LibraryManagerModels;
using LibraryManagerCommon;

namespace LibraryManagerPro
{
    public partial class FrmAdminLogin : Form
    {
        private SysAdminManager objAdminManager = new SysAdminManager();

        public FrmAdminLogin()
        {
            InitializeComponent();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            #region 数据验证

            if (this.txtAdminId.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入登录账号！", "登录提示");
                this.txtAdminId.Focus();
                return;
            }
            if (!DataValidate.IsInteger(this.txtAdminId.Text.Trim()))
            {
                MessageBox.Show("登录账号必须是四位整数！", "登录提示");
                this.txtAdminId.Focus();
                return;
            }
            if (this.txtLoginPwd.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入登录密码！", "登录提示");
                this.txtLoginPwd.Focus();
                return;
            }

            #endregion

            // 封装对象
            SysAdmin objAdmin = new SysAdmin()
            {
                AdminId = Convert.ToInt32(this.txtAdminId.Text.Trim()),
                LoginPwd = this.txtLoginPwd.Text.Trim()
            };

            try
            {
                objAdmin = objAdminManager.AdminLogin(objAdmin);

                if (objAdmin != null)
                {
                    if (objAdmin.StatusId == 1)
                    {
                        Program.objCurrentAdmin = objAdmin;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("当前登录账号被禁用！无法登录，请联系管理员！", "登录提示");
                    }
                }
                else
                {
                    MessageBox.Show("登录账号或密码错误！", "登录提示");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("登录出现异常:" + ex.Message, "登录提示");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
