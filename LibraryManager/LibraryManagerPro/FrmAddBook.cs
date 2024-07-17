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
    public partial class FrmAddBook : Form
    {       
        public FrmAddBook()
        {
            InitializeComponent();
       
        }
        //启动摄像头
        private void btnStartVideo_Click(object sender, EventArgs e)
        {
          
        }
        private void btnCloseVideo_Click(object sender, EventArgs e)
        {
         
        }
        //开始拍照
        private void btnTake_Click(object sender, EventArgs e)
        {
        }
        //选择图片
        private void btnChoseImage_Click(object sender, EventArgs e)
        {
          
        }
        //清除   
        private void btnClear_Click(object sender, EventArgs e)
        {
          
        }
        //判断图书条码是否已经存在
        private void txtBarCode_Leave(object sender, EventArgs e)
        {
        }
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
        }
        //确认添加
        private void btnAdd_Click(object sender, EventArgs e)
        {
          
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
    }
}
