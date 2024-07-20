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
using MyVideo;

namespace LibraryManagerPro
{
    public partial class FrmReaderManger : Form
    {
        private ReaderManager objReaderManager = new ReaderManager();
        private Video objVideo = null; // 摄像头操作的成员变量
        private Reader objCurrentReader = null;

        public FrmReaderManger()
        {
            InitializeComponent();

            DataTable dt = objReaderManager.GetAllReaderRole();
            this.cboReaderRole.DataSource = dt;
            this.cboReaderRole.DisplayMember = "RoleName";
            this.cboReaderRole.ValueMember = "RoleId";

            this.cboRole.DataSource = dt.Copy();
            this.cboRole.DisplayMember = "RoleName";
            this.cboRole.ValueMember = "RoleId";
            this.cboRole.SelectedIndex = -1;

            this.btnEdit.Enabled = false;
            this.btnEnable.Enabled = false;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnQueryReader_Click(object sender, EventArgs e)
        {
            this.lvReader.Items.Clear(); // 清空所有内容
            int readerCount = 0;
            List<Reader> readerList = objReaderManager.GetReaderByRole(this.cboRole.SelectedValue.ToString(), out readerCount);
            foreach (Reader reader in readerList)
            {
                // 创建一个listviewItem对象
                ListViewItem lvItem = new ListViewItem(reader.ReaderId.ToString());
                // ListViewItem对象添加到ListView中
                this.lvReader.Items.Add(lvItem);
                lvItem.SubItems.AddRange(new string[]
                {
                    reader.ReadingCard,
                    reader.ReaderName,
                    reader.Gender,
                    reader.PhoneNumber,
                    reader.RegTime.ToShortDateString(),
                    reader.StatusDesc,
                    reader.ReaderAddress
                });
            }
            this.lblReaderCount.Text = readerCount.ToString();
        }

        //启动摄像头
        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建摄像头操作类
                this.objVideo = new Video(this.pbReaderVideo.Handle, this.pbReaderVideo.Left, this.pbReaderVideo.Top, this.pbReaderVideo.Width, (short)this.pbReaderVideo.Height);
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
            this.pbReaderPhoto.Image = objVideo.CatchVideo();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region 数据验证

            if(this.txtReaderName.Text.Trim().Length == 0)
            {
                MessageBox.Show("读者姓名不能为空！", "提示信息");
                this.txtReaderName.Focus();
                return;
            }
            if (this.txtReadingCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("借阅证编号不能为空！", "提示信息");
                this.txtReadingCard.Focus();
                return;
            }
            if (!this.rdoMale.Checked&& !this.rdoFemale.Checked) 
            {
                MessageBox.Show("请选择读者性别！", "提示信息");
                return;
            }
            if (this.cboReaderRole.SelectedIndex == -1)
            {
                MessageBox.Show("请选择会员角色！", "提示信息");
                return;
            }
            if (this.txtIDCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("身份证号不能为空！", "提示信息");
                this.txtIDCard.Focus();
                return;
            }
            if (this.txtPostcode.Text.Trim().Length == 0)
            {
                MessageBox.Show("邮政编码不能为空！", "提示信息");
                this.txtPostcode.Focus();
                return;
            }
            if (this.txtAddress.Text.Trim().Length == 0)
            {
                MessageBox.Show("现在地址不能为空！", "提示信息");
                this.txtAddress.Focus();
                return;
            }
            if (this.txtPhone.Text.Trim().Length == 0)
            {
                MessageBox.Show("联系地址不能为空！", "提示信息");
                this.txtPhone.Focus();
                return;
            }

            #endregion

            Reader objReader = new Reader()
            {
                ReaderName = this.txtReaderName.Text.Trim(),
                ReadingCard = this.txtReadingCard.Text.Trim(),
                IDCard = this.txtIDCard.Text.Trim(),
                Gender = this.rdoFemale.Checked ? "女" : "男",
                RoleId = Convert.ToInt32(this.cboReaderRole.SelectedValue),
                PostCode = this.txtPostcode.Text.Trim(),
                PhoneNumber = this.txtPhone.Text.Trim(),
                ReaderAddress = this.txtAddress.Text.Trim(),
                ReaderPwd = "111111",
                AdminId = Program.objCurrentAdmin.AdminId,
                ReaderImage = this.pbReaderPhoto.Image != null ? new SerializeObjectToString().SerializeObject(this.pbReaderPhoto.Image) : ""
            };

            try
            {
                objReaderManager.AddReader(objReader);
                MessageBox.Show("添加成功！", "提示信息");

                this.ClearReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ClearReader()
        {
            this.txtReaderName.Text = "";
            this.txtReadingCard.Text = "";
            this.cboReaderRole.SelectedIndex = -1;
            this.txtPostcode.Text = "";
            this.txtPhone.Text = "";
            this.txtIDCard.Clear();
            this.txtAddress.Text = "";
            this.pbReaderPhoto.Image = null;

            this.btnEdit.Enabled = false;
            this.btnEnable.Enabled = false;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (this.txt_ReadingCard.Text.Trim().Length == 0 && this.txt_IDCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入一个查询内容！", "查询提示");
                return;
            }

            if (this.rdoIDCard.Checked && this.txt_IDCard.Text.Trim().Length > 0)
            {
                this.objCurrentReader = objReaderManager.GetReaderByIDCard(this.txt_IDCard.Text.Trim());
            }
            else
            {
                this.objCurrentReader = objReaderManager.GetReaderByReadingCard(this.txt_ReadingCard.Text.Trim());
            }

            if (this.objCurrentReader != null)
            {
                if (objCurrentReader.StatusId != 0)
                {
                    this.btnEdit.Enabled = true;
                    this.btnEnable.Enabled = true;
                }

                this.lblAddress.Text = objCurrentReader.ReaderAddress;
                this.lblPhone.Text = objCurrentReader.PhoneNumber;
                this.lblPostCode.Text = objCurrentReader.PostCode;
                this.lblReaderName.Text = objCurrentReader.ReaderName;
                this.lblReadingCard.Text = objCurrentReader.ReadingCard;
                this.lblRoleName.Text = objCurrentReader.RoleName;
                this.lblGender.Text = objCurrentReader.Gender;
                this.pbReaderImg.Image = objCurrentReader.ReaderImage != "" ? (Image)new SerializeObjectToString().DeserializeObject(objCurrentReader.ReaderImage) : null;
            }
            else
            {
                MessageBox.Show("当前读者不存在！", "查询提示");

                this.ClearReader();
            }
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确认挂失当前借阅证吗？", "挂失询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                try
                {
                    objReaderManager.ForbiddenReaderCard(objCurrentReader.ReaderId.ToString());
                    MessageBox.Show("挂失成功！", "提示信息");
                    this.ClearReader();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FrmEidtReader frmEdit = new FrmEidtReader(this.objCurrentReader);
            frmEdit.ParentRefreshRequested += RefreshFrom;
            frmEdit.Show();
        }

        private void RefreshFrom(object sender, EventArgs e)
        {
            RefreshParent();
        }

        private void RefreshParent()
        {
            this.btnQuery_Click(null,null);
            if (this.cboRole.SelectedIndex != -1) {
                this.btnQueryReader_Click(null, null);
            }
        }
    }
}
