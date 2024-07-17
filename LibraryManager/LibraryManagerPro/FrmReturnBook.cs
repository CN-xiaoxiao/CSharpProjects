
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
    public partial class FrmReturnBook : Form
    {
     

        #region 初始化
        public FrmReturnBook()
        {
            InitializeComponent();
        }
          

        #endregion

        #region 显示读者信息（个人信息+图书借阅信息）
      
        #endregion

        #region 显示还书列表
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        #endregion

        #region 还书操作
        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
          
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
