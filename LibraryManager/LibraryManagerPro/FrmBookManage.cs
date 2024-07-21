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
using MyVideo;
using LibraryManagerCommon;

namespace LibraryManagerPro
{
    public partial class FrmBookManage : Form
    {
        private BookManager objBookManager = new BookManager();
        private List<Book> bookList = null;
        private Video objVideo;

        public FrmBookManage()
        {
            InitializeComponent();

            #region 初始化

            List<Category> categoryList = objBookManager.GetAllCategory();
            List<Category> list = new List<Category>();
            list.AddRange(categoryList);

            categoryList.Insert(0, new Category() { CategoryId = -1, CategoryName = "" });
            this.cboCategory.DataSource = categoryList;
            this.cboCategory.DisplayMember = "CategoryName";
            this.cboCategory.ValueMember = "CategoryId";
            this.cboCategory.SelectedIndex = -1;

            // 禁用删除
            this.btnDel.Enabled = false;
            this.btnSave.Enabled = false;
            // 禁止数据列表自动生成列
            this.dgvBookList.AutoGenerateColumns = false;

            this.cbo_BookCategory.DataSource = list;
            this.cbo_BookCategory.DisplayMember = "CategoryName";
            this.cbo_BookCategory.ValueMember = "CategoryId";
            this.cbo_BookCategory.SelectedIndex = -1;

            this.cbo_Publisher.DataSource = objBookManager.GetAllPublisher();
            this.cbo_Publisher.DisplayMember = "PublisherName";
            this.cbo_Publisher.ValueMember = "PublisherId";
            this.cbo_Publisher.SelectedIndex = -1;

            #endregion
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            // 断开选择改变事件(防止出现异常情况)
            this.dgvBookList.SelectionChanged -= new EventHandler(this.dgvBookList_SelectionChanged);

            // 判断用户是否输入查询条件
            if ((this.cboCategory.SelectedIndex == -1 || this.cboCategory.SelectedValue.ToString() == "-1") && this.txtBarCode.Text.Trim().Length == 0 && this.txtBookName.Text.Trim().Length == 0)
            {
                MessageBox.Show("请至少选择一个查询条件！", "查询提示");
            }
            else
            {
                // 根据条件组合查询
                bookList = objBookManager.GetBooks((this.cboCategory.SelectedIndex == -1) ? -1 : Convert.ToInt32(this.cboCategory.SelectedValue), this.txtBarCode.Text.Trim(), this.txtBookName.Text.Trim());
                // 在列表中显示查询结果
                this.dgvBookList.DataSource = bookList;
                // 根据查询结果决定是否开启和禁用“删除”按钮
                if (bookList.Count == 0)
                {
                    this.btnDel.Enabled = false;
                    this.btnSave.Enabled = false;
                }
                else
                {
                    this.btnDel.Enabled = true;
                    this.btnSave.Enabled = true;
                    this.dgvBookList.Focus();
                }
            }

            // 开启选择改变事件
            this.dgvBookList.SelectionChanged += new EventHandler(this.dgvBookList_SelectionChanged);
            this.dgvBookList_SelectionChanged(null, null);
        }

        private void dgvBookList_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dgvBookList.RowCount == 0)
            {
                return;
            }
            // 找到要修改的图书对象
            string barCode = this.dgvBookList.CurrentRow.Cells["BarCode"].Value.ToString();
            Book objBook = (from b in bookList where b.BarCode.Equals(barCode) select b).First<Book>();
            // 显示当前图书对象信息
            this.lbl_BarCode.Text = objBook.BarCode;
            this.txt_Author.Text = objBook.Author;
            this.lbl_BookCount.Text = objBook.BookCount.ToString();
            this.txt_BookName.Text = objBook.BookName;
            this.txt_BookPosition.Text = objBook.BookPosition;
            this.txt_UnitPrice.Text = objBook.UnitPrice.ToString();
            this.cbo_BookCategory.SelectedValue = objBook.BookCategory;
            this.cbo_Publisher.SelectedValue = objBook.PublisherId;
            this.lbl_BookId.Text = objBook.BookId.ToString();

            if (objBook.BookImage.Length != 0)
            {
                this.pbCurrentImage.Image = (Image)new SerializeObjectToString().DeserializeObject(objBook.BookImage);
            }
            else
            {
                this.pbCurrentImage.Image = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 数据验证

            if (this.txt_BookName.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入图书名称", "修改提示");
                this.txt_BookName.Focus();
                return;
            }
            if (this.cbo_BookCategory.SelectedIndex == -1)
            {
                MessageBox.Show("请选择图书类别", "修改提示");
                return;
            }
            if (this.cbo_Publisher.SelectedIndex == -1)
            {
                MessageBox.Show("请选择出版社", "修改提示");
                return;
            }
            if (this.dtp_PublishDate.Value == null || this.dtp_PublishDate.Text.Length == 0)
            {
                MessageBox.Show("请选择图书出版日期", "修改提示");
                return;
            }
            if (this.txt_Author.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入作者名称", "修改提示");
                this.txt_Author.Focus();
                return;
            }
            if (this.txt_UnitPrice.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入图书价格", "修改提示");
                this.txt_UnitPrice.Focus();
                return;
            }
            if (this.txt_BookPosition.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入图书位置", "修改提示");
                this.txt_BookPosition.Focus();
                return;
            }
            if (this.pbCurrentImage.Image == null)
            {
                MessageBox.Show("请选择图书封面", "修改提示");
                return;
            }

            #endregion

            // 封装对象
            Book objBook = new Book()
            {
                BookName = this.txt_BookName.Text.Trim(),
                BookCategory = Convert.ToInt32(this.cbo_BookCategory.SelectedValue),
                PublisherId = Convert.ToInt32(this.cbo_Publisher.SelectedValue),
                PublishDate = Convert.ToDateTime(this.dtp_PublishDate.Text),
                Author = this.txt_Author.Text.Trim(),
                UnitPrice = Convert.ToDouble(this.txt_UnitPrice.Text.Trim()),
                BookCount = Convert.ToInt32(this.lbl_BookCount.Text.Trim()),
                BookPosition = this.txt_BookPosition.Text.Trim(),
                BookImage = new SerializeObjectToString().SerializeObject(this.pbCurrentImage.Image),
                BookId = Convert.ToInt32(this.lbl_BookId.Text)
            };

            try
            {
                objBookManager.EditBook(objBook);

                MessageBox.Show("修改成功！", "提示信息");
                // 同步更新显示
                Book editedBook = (from b in this.bookList where b.BookId.Equals(Convert.ToInt32(this.lbl_BookId.Text)) select b).First<Book>();
                editedBook.BookName = objBook.BookName;
                editedBook.Author = objBook.Author;
                editedBook.BookCategory = objBook.BookCategory;
                editedBook.PublisherId = objBook.PublisherId;
                editedBook.PublishDate = objBook.PublishDate;
                editedBook.UnitPrice = objBook.UnitPrice;
                editedBook.BookCount = objBook.BookCount;
                editedBook.BookPosition = objBook.BookPosition;
                editedBook.BookImage = objBook.BookImage;
                editedBook.BookId = objBook.BookId;

                // 刷新dgv的显示
                this.dgvBookList.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误提示");
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确认要删除【" + this.txt_BookName.Text + "】这本书吗？", "删除询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            this.dgvBookList.SelectionChanged -= new EventHandler(this.dgvBookList_SelectionChanged);

            try
            {
                if (objBookManager.DeleteBook(lbl_BookId.Text) == 1)
                {
                    Book deleteBook = (from b in this.bookList where b.BookId.Equals(Convert.ToInt32(this.lbl_BookId.Text)) select b).First<Book>();
                    this.bookList.Remove(deleteBook);
                    this.dgvBookList.DataSource = null;
                    this.dgvBookList.DataSource = this.bookList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "删除提示");
            }

            this.dgvBookList.SelectionChanged += new EventHandler(this.dgvBookList_SelectionChanged);
            this.dgvBookList_SelectionChanged(null, null);
        }

        //启动摄像头
        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建摄像头操作类
                this.objVideo = new Video(this.pbImage.Handle, this.pbImage.Left, this.pbImage.Top, this.pbImage.Width, (short)this.pbImage.Height);
                // 打开摄像头
                objVideo.OpenVideo();
                // 禁用打开摄像头按钮
                this.btnStartVideo.Enabled = false;
                this.btnTake.Enabled = true;
                this.btnCloseVideo.Enabled = true;

                this.btnCloseVideo.BackColor = Color.Red;
                this.btnTake.BackColor = Color.YellowGreen;
                this.btnTake.ForeColor = Color.White;
            }
            catch (Exception ex)
            {
                MessageBox.Show("摄像头启动失败: " + ex.Message, "错误信息");
            }
        }
        private void btnCloseVideo_Click(object sender, EventArgs e)
        {
            this.objVideo.CloseVideo();
            this.btnCloseVideo.Enabled = false;
            this.btnCloseVideo.BackColor = default;
            this.btnTake.Enabled = false;
            this.btnTake.BackColor = default;
            this.btnStartVideo.Enabled = true;
        }
        //开始拍照
        private void btnTake_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.Image = objVideo.CatchVideo();
        }

        //选择图片
        private void btnChoseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog objOpenFile = new OpenFileDialog(); // 创建一个文件选择对象
            DialogResult result = objOpenFile.ShowDialog();

            if (result == DialogResult.OK) // 如果选择了文件
            {
                // 判断是否是图片格式
                Image image = isImageFile(objOpenFile.FileName);
                if (image != null)
                {
                    this.pbCurrentImage.Image = image; // 给图片展示对象赋值
                }
            }
        }
        // 判断是否是图片文件
        private Image isImageFile(string fileName)
        {
            try
            {
                return Image.FromFile(fileName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
