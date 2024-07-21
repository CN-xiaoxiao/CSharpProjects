using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LibraryManagerBLL;
using LibraryManagerCommon;
using LibraryManagerModels;

namespace LibraryManagerPro
{
    public partial class FrmBorrowBook : Form
    {

        private ReaderManager objReaderManager = new ReaderManager();
        private BorrowManager objBorrowManager = new BorrowManager();
        private BookManager objBookManager = new BookManager();

        private Reader objCurrentRearer = null;
        private List<BorrowDetail> borrowsDetailList = new List<BorrowDetail>();

        public FrmBorrowBook()
        {
            InitializeComponent();

            this.txtBarCode.Enabled = false;
            this.btnDel.Enabled = false;
            this.btnSave.Enabled = false;

            this.dgvBookList.AutoGenerateColumns = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtReadingCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtReadingCard.Text.Trim().Length != 0 && e.KeyCode == Keys.Enter)
            {
                this.objCurrentRearer = this.objReaderManager.GetReaderByReadingCard(this.txtReadingCard.Text.Trim());
                if (objCurrentRearer != null)
                {
                    if (objCurrentRearer.StatusId == 1)
                    {
                        // 显示读者信息
                        this.lblReaderName.Text = objCurrentRearer.ReaderName;
                        this.lblRoleName.Text = objCurrentRearer.RoleName;
                        this.lblAllowCounts.Text = objCurrentRearer.AllowCounts.ToString();
                        // 显示照片
                        this.pbReaderImage.Image = objCurrentRearer.ReaderImage != "" ? (Image)new SerializeObjectToString().DeserializeObject(objCurrentRearer.ReaderImage) : null;
                        // 显示已借图书总数和剩余可解图书总数
                        int borrowCount = objBorrowManager.GetBorrowCount(this.txtReadingCard.Text.Trim());
                        this.lblBorrowCount.Text = borrowCount.ToString();
                        this.lbl_Remainder.Text = (objCurrentRearer.AllowCounts - borrowCount).ToString();
                        if (objCurrentRearer.AllowCounts > borrowCount)
                        {
                            // 开启图书条码扫描文本框
                            this.txtBarCode.Enabled = true;
                            this.txtBarCode.Focus();
                        }
                        else
                        {
                            MessageBox.Show("当前读者借书总数已经达到上限！", "借书提示");
                        }
                    }
                    else if (objCurrentRearer.StatusId == 0)
                    {
                        MessageBox.Show("当前借阅证已经被挂失，不能继续借书！", "结束提示");
                    }
                }
                else
                {
                    MessageBox.Show("当前借阅证不存在！", "借书提示");
                }
            }

        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length != 0 && e.KeyCode == Keys.Enter)
            {
                if (Convert.ToInt32(this.lbl_Remainder.Text) == 0)
                {
                    MessageBox.Show("当天读者借书总数已达上限！", "借书提示");
                    return;
                }
                // 根据条码从数据库中查询图书信息
                Book objBook = objBookManager.GetBookByBarCode(this.txtBarCode.Text.Trim());
                if (objBook != null)
                {
                    //  判断当前集合中是否已经存在该图书对象
                    int count = (from b in this.borrowsDetailList where b.BarCode.Equals(objBook.BarCode) select b).Count();
                    if (count == 0)
                    {
                        BorrowDetail bookDetail = new BorrowDetail()
                        {
                            BarCode = objBook.BarCode,
                            BookId = objBook.BookId,
                            BookName = objBook.BookName,
                            Expire = DateTime.Now.AddDays(objCurrentRearer.AllowDay),
                            BorrowCount = 1
                        };
                        borrowsDetailList.Add(bookDetail);
                        // 同步刷新列表数据
                        this.dgvBookList.DataSource = null;
                        this.dgvBookList.DataSource = borrowsDetailList;
                    }
                    else
                    {
                        BorrowDetail bookDetail = (from b in this.borrowsDetailList where b.BarCode.Equals(objBook.BarCode) select b).First<BorrowDetail>();
                        bookDetail.BorrowCount += 1;
                        this.dgvBookList.Refresh();
                    }
                    // 检查当前借书总数是否已经达到上限
                    this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text) + 1).ToString();
                    this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text) - 1).ToString();
                    
                    this.txtBarCode.Clear();

                    this.btnSave.Enabled = true;
                    this.btnDel.Enabled = true;
                }
                else
                {
                    MessageBox.Show("当前图书不存在！", "借书提示");
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string barCode = this.dgvBookList.CurrentRow.Cells["BarCode"].Value.ToString();
            BorrowDetail bookDetail = (from b in this.borrowsDetailList where b.BarCode.Equals(barCode) select b).First<BorrowDetail>();

            this.borrowsDetailList.Remove(bookDetail);

            this.dgvBookList.DataSource = null;
            this.dgvBookList.DataSource = this.borrowsDetailList;
            this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text) - bookDetail.BorrowCount).ToString();
            this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text) + bookDetail.BorrowCount).ToString();

            if (this.borrowsDetailList.Count == 0)
            {
                this.btnSave.Enabled = false;
                this.btnDel.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 数据验证

            #endregion

            BorrowInfo main = new BorrowInfo()
            {
                ReaderId = this.objCurrentRearer.ReaderId,
                BorrowId = DateTime.Now.ToString("yyyyMMddhhmmssms"),
                AdminName_B = Program.objCurrentAdmin.AdminName
            };

            for (int i = 0; i < this.borrowsDetailList.Count; i++)
            {
                borrowsDetailList[i].BorrowId = main.BorrowId;
                borrowsDetailList[i].Expire = DateTime.Now.AddDays(objCurrentRearer.AllowDay);
                borrowsDetailList[i].NonReturnCount = borrowsDetailList[i].BorrowCount;
            }

            try
            {
                objBorrowManager.BorrowBook(main, borrowsDetailList);

                this.txtBarCode.Clear();
                this.txtBarCode.Enabled = false;
                this.btnDel.Enabled=false;
                this.btnSave.Enabled = false;
                this.lblRoleName.Text = "";
                this.lblBorrowCount.Text = "";
                this.lblReaderName.Text = "";
                this.lblAllowCounts.Text = "";
                this.lbl_Remainder.Text = "";
                this.pbReaderImage.Image = null;
                this.dgvBookList.DataSource = null;
                this.txtReadingCard.Clear();
                this.objCurrentRearer = null;

                MessageBox.Show("借书成功！", "借书提示");
                this.txtReadingCard.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "借书提示");
            }
        }
    }
}
