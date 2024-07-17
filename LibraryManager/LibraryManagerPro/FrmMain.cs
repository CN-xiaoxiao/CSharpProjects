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
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            this.tssl_AdminName.Text = Program.objCurrentAdmin.AdminName;
        }

        private void OpenForm(Form objFrm)
        {
            foreach (Control item in this.spContainer.Panel2.Controls)
            {
                if (item is Form)
                {
                    ((Form)item).Close();
                }
            }

            objFrm.TopLevel = false;    // 将主窗体设置为非顶级控件
            objFrm.FormBorderStyle = FormBorderStyle.None; // 去掉子窗体的边框
            objFrm.Parent = this.spContainer.Panel2;
            objFrm.Dock = DockStyle.Fill;
            objFrm.Show();
        }

        // 新增图书
        private void btnAddBook_Click(object sender, EventArgs e)
        {
            this.OpenForm(new FrmAddBook());
            this.lblOperationName.Text = "新增图书";

        }

        private void btnBookManage_Click(object sender, EventArgs e)
        {
            this.OpenForm(new FrmBookManage());
            this.lblOperationName.Text = "图书信息维护";
        }

        private void btnBorrowBook_Click(object sender, EventArgs e)
        {
            this.OpenForm(new FrmBorrowBook());
            this.lblOperationName.Text = "图书出借";
        }

        private void btnBookNew_Click(object sender, EventArgs e)
        {
            this.OpenForm(new FrmNewBook());
            this.lblOperationName.Text = "图书上架";
        }

        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            this.OpenForm(new FrmReturnBook());
            this.lblOperationName.Text = "图书归还";
        }

        private void btnReaderManager_Click(object sender, EventArgs e)
        {
            this.OpenForm(new FrmReaderManger());
            this.lblOperationName.Text = "会员管理";
        }

        // 密码修改
        private void btnModifyPwd_Click(object sender, EventArgs e)
        {
            FrmModifyPwd objFrm = new FrmModifyPwd();
            DialogResult result = objFrm.ShowDialog();
        }
        //退出系统
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确认退出系统吗？", "退出询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true; // 取消窗体关闭操作
            }
        }
    }
}
