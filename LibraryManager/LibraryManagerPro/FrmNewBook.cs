using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LibraryManagerModels;
using LibraryManagerBLL;
using LibraryManagerCommon;

namespace LibraryManagerPro
{
    public partial class FrmNewBook : Form
    {
        private BookManager objBookManager = new BookManager();
        private List<Book> bookList = new List<Book>();

        public FrmNewBook()
        {
            InitializeComponent();

            this.txtAddCount.Enabled = false;
            this.dgvBookList.AutoGenerateColumns = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length != 0 && e.KeyCode == Keys.Enter)
            {

                Book objBook = objBookManager.GetBookByBarCode(this.txtBarCode.Text.Trim());
                if (objBook != null)
                {
                    this.lblBookName.Text = objBook.BookName;
                    this.lblBookPosition.Text = objBook.BookPosition;
                    this.lblBookCount.Text = objBook.BookCount.ToString();
                    this.lblCategory.Text = objBook.BookCategory.ToString();
                    this.lblBookId.Text = objBook.BookId.ToString();
                    this.pbImage.Image = objBook.BookImage.Length == 0 ? null : (Image)new SerializeObjectToString().DeserializeObject(objBook.BookImage);

                    this.txtAddCount.Enabled = true;
                    this.txtAddCount.Focus();

                    int count = (from b in bookList where b.BookId == objBook.BookId select b).Count();

                    if (count == 0)
                    {
                        this.bookList.Add(objBook);
                        this.dgvBookList.DataSource = null;
                        this.dgvBookList.DataSource = this.bookList;
                    }
                }
                else
                {
                    MessageBox.Show("图书不存在！", "查询提示");
                    this.txtBarCode.Focus();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 数据认证
            if (this.txtAddCount.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入新增图书总数！", "提示信息");
                this.txtAddCount.Focus();
                return;
            }
            if (!DataValidate.IsInteger(this.txtAddCount.Text.Trim()))
            {
                MessageBox.Show("新增图书总数必须是一个正整数！", "提示信息");
                this.txtAddCount.SelectAll();
                this.txtAddCount.Focus();

                return;
            }

            try
            {
                int result = objBookManager.AddBookCount(this.txtBarCode.Text.Trim(), Convert.ToInt32(this.txtAddCount.Text.Trim()));
                if (result == 1)
                {
                    Book objBook = (from b in this.bookList where b.BarCode == this.txtBarCode.Text.Trim() select b).First<Book>();

                    objBook.BookCount += Convert.ToInt32(this.txtAddCount.Text.Trim());
                    this.dgvBookList.Refresh();

                    this.lblBookName.Text = "";
                    this.lblBookPosition.Text = "";
                    this.lblBookCount.Text = "";
                    this.lblCategory.Text = "";
                    this.lblBookId.Text = "";
                    this.pbImage.Image = null;

                    this.txtBarCode.Clear();
                    this.txtAddCount.Clear();
                    this.txtAddCount.Enabled = false;
                    this.txtBarCode.Focus();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("当前操作出现问题：" + ex.Message);
            }
        }

        private void txtAddCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtAddCount.Text.Trim().Length != 0 && e.KeyCode == Keys.Enter) {
                this.btnSave_Click(null, null);
            }
        }
    }
}
