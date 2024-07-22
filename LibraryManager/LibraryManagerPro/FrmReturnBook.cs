
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
    public partial class FrmReturnBook : Form
    {
        private ReaderManager objReaderManager = new ReaderManager();
        private BorrowManager objBorrowManager = new BorrowManager();

        private Reader objCurrentReader = null;
        private List<BorrowDetail> nonReturnList = null;
        private List<BorrowDetail> returnList = new List<BorrowDetail>();

        private int returnBookCount = 0; 

        #region 初始化
        public FrmReturnBook()
        {
            InitializeComponent();

            this.txtBarCode.Enabled = false;
            this.btnConfirmReturn.Enabled = false;
            this.dgvNonReturnList.AutoGenerateColumns = false;
            this.dgvReturnList.AutoGenerateColumns = false;

            this.dgvNonReturnList.Columns["BorrowCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["BorrowCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
            this.dgvNonReturnList.Columns["ReturnCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["ReturnCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
            this.dgvNonReturnList.Columns["NonReturnCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["NonReturnCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);

            this.dgvReturnList.Columns["ReturnCount_"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvReturnList.Columns["ReturnCount_"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
        }

        #endregion

        #region 显示读者信息（个人信息+图书借阅信息）

        private void txtReadingCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtReadingCard.Text.Trim().Length != 0 && e.KeyCode == Keys.Enter)
            {
                this.objCurrentReader = this.objReaderManager.GetReaderByReadingCard(this.txtReadingCard.Text.Trim());
                if (objCurrentReader != null)
                {
                    if (objCurrentReader.StatusId == 1)
                    {
                        // 显示读者信息
                        this.lblReaderName.Text = objCurrentReader.ReaderName;
                        this.lblRoleName.Text = objCurrentReader.RoleName;
                        this.lblAllowCounts.Text = objCurrentReader.AllowCounts.ToString();
                        // 显示照片
                        this.pbReaderImage.Image = objCurrentReader.ReaderImage != "" ? (Image)new SerializeObjectToString().DeserializeObject(objCurrentReader.ReaderImage) : null;

                        // 开启图书条码扫描文本框
                        this.txtBarCode.Enabled = true;
                        this.txtBarCode.Focus();

                        // 显示该读者未归还的图书
                        nonReturnList = objBorrowManager.GetBorrowDetailByReadingCard(this.txtReadingCard.Text.Trim());
                        this.dgvNonReturnList.DataSource = nonReturnList;

                        int borrowCount = (from n in nonReturnList select n).Sum(u => u.NonReturnCount);
                        int remainder = objCurrentReader.AllowCounts - borrowCount;
                        this.lblBorrowCount.Text = borrowCount.ToString();
                        this.lbl_Remainder.Text = remainder.ToString();
                    }
                    else if (objCurrentReader.StatusId == 0)
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

        #endregion

        #region 显示还书列表
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length != 0 && e.KeyCode == Keys.Enter)
            {
                // 检查当前借书列表中是否存在该图书
                if ((from b in this.nonReturnList where b.BarCode.Equals(this.txtBarCode.Text.Trim()) select b).Count() == 0)
                {
                    MessageBox.Show("当前图书不在借书列表中，请和读者确认该图书的信息！", "还书提示");
                    this.txtBarCode.SelectAll();
                    return;
                }
                // 从还书集合中查询扫描的图书总数
                int count = (from b in this.returnList where b.BarCode == this.txtBarCode.Text.Trim() select b).Count();
                if (count == 0)
                {
                    this.returnList.Add(new BorrowDetail()
                    {
                        BarCode = this.txtBarCode.Text,
                        ReturnCount = 1,
                        BookName = (from b in this.nonReturnList where b.BarCode.Equals(this.txtBarCode.Text.Trim()) select b).First<BorrowDetail>().BookName
                    });
                    this.dgvReturnList.DataSource = null;
                    this.dgvReturnList.DataSource = this.returnList;
                }
                else
                {
                    BorrowDetail objReturnDetail = (from b in this.returnList where b.BarCode == this.txtBarCode.Text.Trim() select b).First<BorrowDetail>();

                    int nonReturnCount = (from b in this.nonReturnList where b.BarCode == this.txtBarCode.Text.Trim() select b).Sum(u => u.NonReturnCount);

                    // 判断当前还书总数是否等于借书总数
                    if (nonReturnCount == objReturnDetail.ReturnCount)
                    {
                        MessageBox.Show("还书总数不能大于借书总数！", "还书提示");
                        this.txtBarCode.Clear();
                        return;
                    }
                    else
                    {
                        objReturnDetail.ReturnCount += 1;
                        this.dgvReturnList.Refresh();
                    }
                }
                // 同步显示还书总数
                returnBookCount++;
                this.lblReturnCount.Text = returnBookCount.ToString();
                this.btnConfirmReturn.Enabled = true;
                this.txtBarCode.Clear();
                this.txtBarCode.Focus();
            }
        }

        #endregion

        #region 还书操作
        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            objBorrowManager.ReturnBook(this.returnList, this.nonReturnList, Program.objCurrentAdmin.AdminName);

            this.returnBookCount = 0;
            this.nonReturnList.Clear();
            this.returnList.Clear();
            this.txtReadingCard.Clear();
            this.btnConfirmReturn.Enabled=false;
            this.txtBarCode.Enabled=false;

            this.dgvNonReturnList.DataSource = null;
            this.dgvReturnList.DataSource= null;

            this.lblReturnCount.Text = "0";
            this.lbl_Remainder.Text = "";
            this.lblAllowCounts.Text = "0";
            this.lblBorrowCount.Text = "0";
            this.lblReaderName.Text = "";
            this.lblRoleName.Text = "";
            this.pbReaderImage.Image = null;

            MessageBox.Show("还书成功！", "还书提示");
            this.txtReadingCard.Focus();
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
