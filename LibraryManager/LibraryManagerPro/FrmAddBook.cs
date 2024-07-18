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
using MyVideo;

namespace LibraryManagerPro
{
    public partial class FrmAddBook : Form
    {
        private BookManager objBookManger = new BookManager();
        private Video objVideo = null; // 摄像头操作的成员变量
        private List<Book> bookList = new List<Book>(); // 保存当前已经添加到数据库的图书对象

        public FrmAddBook()
        {
            InitializeComponent();

            // 初始化图书分类下拉框
            this.cboBookCategory.DataSource = objBookManger.GetAllCategory();
            this.cboBookCategory.DisplayMember = "CategoryName";
            this.cboBookCategory.ValueMember = "CategoryId";
            this.cboBookCategory.SelectedIndex = -1;

            // 初始化出版社下拉框
            this.cboPublisher.DataSource = objBookManger.GetAllPublisher();
            this.cboPublisher.DisplayMember = "PublisherName";
            this.cboPublisher.ValueMember = "PublisherId";
            this.cboPublisher.SelectedIndex = -1;

            this.btnCloseVideo.Enabled = false;
            this.btnTake.Enabled = false;

            // 禁止数据列表控件dgv自动生成列
            this.dgvBookList.AutoGenerateColumns = false;
            // 添加列表样式
            new DataGridViewStyle().DgvStyle1(this.dgvBookList);
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

        //清除   
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.Image = null;
        }
        //判断图书条码是否已经存在
        private void txtBarCode_Leave(object sender, EventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length > 0)
            {
                if (objBookManger.BarCodeIsExisted(this.txtBarCode.Text.Trim()))
                {
                    MessageBox.Show("该条码已存在", "验证信息");
                    this.txtBarCode.SelectAll();
                    this.txtBarCode.Focus();
                    return;
                }
            }
        }
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtBarCode_Leave(null, null);
            }
        }
        //确认添加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region 数据验证

            if (this.txtBookName.Text.Trim().Length == 0)
            {
                MessageBox.Show("请填写图书名称", "验证信息");
                this.txtBookName.Focus();
                return;
            }
            if (this.cboBookCategory.SelectedIndex == -1)
            {
                MessageBox.Show("请选择图书分类", "验证信息");
                return;
            }
            if (this.cboPublisher.SelectedIndex == -1)
            {
                MessageBox.Show("请选择图书出版社", "验证信息");
                return;
            }
            if (this.txtAuthor.Text.Trim().Length == 0)
            {
                MessageBox.Show("请填写主编人", "验证信息");
                this.txtAuthor.Focus();
                return;
            }
            if (this.txtUnitPrice.Text.Trim().Length == 0)
            {
                MessageBox.Show("请填写图书原价", "验证信息");
                this.txtUnitPrice.Focus();
                return;
            }

            // 验证图书条码
            if (this.txtBarCode.Text.Trim().Length == 0)
            {
                MessageBox.Show("请填写图书条码", "验证信息");
                this.txtBarCode.Focus();
                return;
            }

            if (this.txtBookCount.Text.Trim().Length == 0)
            {
                MessageBox.Show("请填写收藏总数", "验证信息");
                this.txtBookCount.Focus();
                return;
            }
            if (this.txtBookPosition.Text.Trim().Length == 0)
            {
                MessageBox.Show("请填写书架位置", "验证信息");
                this.txtBookPosition.Focus();
                return;
            }

            // 验证图片
            if (this.pbCurrentImage.Image == null)
            {
                MessageBox.Show("请选择图书封面", "验证信息");
                return;
            }

            #endregion

            // 封装图书对象
            Book objBook = new Book()
            {
                BookName = this.txtBookName.Text.Trim(),
                BookCategory = Convert.ToInt32(this.cboBookCategory.SelectedValue),
                PublisherId = Convert.ToInt32(this.cboPublisher.SelectedValue),
                PublishDate = Convert.ToDateTime(this.dtpPublishDate.Text),
                Author = this.txtAuthor.Text.Trim(),
                UnitPrice = Convert.ToDouble(this.txtUnitPrice.Text.Trim()),
                BarCode = this.txtBarCode.Text.Trim(),
                BookCount = Convert.ToInt32(this.txtBookCount.Text.Trim()),
                BookPosition = this.txtBookPosition.Text.Trim(),
                BookImage = new SerializeObjectToString().SerializeObject(this.pbCurrentImage.Image),
                PublisherName = this.cboPublisher.Text
            };

            try
            {
                objBookManger.AddBook(objBook);
                // 在下面的列表中同步显示
                bookList.Add(objBook);
                this.dgvBookList.DataSource = null;
                this.dgvBookList.DataSource = bookList;

                // 清空表单中的内容，以便下次填写
                foreach (Control item in this.gbBook.Controls)
                {
                    if (item is TextBox)
                    {
                        item.Text = "";
                    }
                    else if (item is ComboBox)
                    {
                        ((ComboBox)item).SelectedIndex = -1;
                    }
                }
                this.pbCurrentImage.Image = null;
                this.txtBookName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加出现异常: " + ex.Message, "错误提示");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 添加行号
        private void dgvBookList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridViewStyle.DgvRowPostPaint(this.dgvBookList, e);
        }
    }
}
