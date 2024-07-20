using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MyVideo;
using LibraryManagerModels;
using LibraryManagerBLL;
using LibraryManagerCommon;

namespace LibraryManagerPro
{
    public partial class FrmEidtReader : Form
    {
        private Video objVideo = null;
        private Reader objEditedReader = null;
        private ReaderManager objReaderManger = new ReaderManager();

        public FrmEidtReader()
        {
            InitializeComponent();

        }

        public FrmEidtReader(Reader objReader) : this()
        {
            this.cboReaderRole.DataSource = objReaderManger.GetAllReaderRole();
            this.cboReaderRole.DisplayMember = "RoleName";
            this.cboReaderRole.ValueMember = "RoleId";

            this.txtAddress.Text = objReader.ReaderAddress;
            this.txtPhone.Text = objReader.PhoneNumber;
            this.txtPostcode.Text = objReader.PostCode;
            this.txtReaderName.Text = objReader.ReaderName;
            if (objReader.Gender == "男")
            {
                this.rdoMale.Checked = true;
            }
            else
            {
                this.rdoFemale.Checked = true;
            }
            this.txtReadingCard.Text = objReader.ReadingCard;
            this.cboReaderRole.Text = objReader.RoleName;
            this.pbReaderPhoto.Image = objReader.ReaderImage != "" ? (Image)new SerializeObjectToString().DeserializeObject(objReader.ReaderImage) : null;

            objEditedReader = objReader; // 保存当前读者对象
        }

        public event EventHandler ParentRefreshRequested;
        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 数据验证

            if (this.txtReaderName.Text.Trim().Length == 0)
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
            if (!this.rdoMale.Checked && !this.rdoFemale.Checked)
            {
                MessageBox.Show("请选择读者性别！", "提示信息");
                return;
            }
            if (this.cboReaderRole.SelectedIndex == -1)
            {
                MessageBox.Show("请选择会员角色！", "提示信息");
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
                Gender = this.rdoFemale.Checked ? "女" : "男",
                RoleId = Convert.ToInt32(this.cboReaderRole.SelectedValue),
                PostCode = this.txtPostcode.Text.Trim(),
                PhoneNumber = this.txtPhone.Text.Trim(),
                ReaderAddress = this.txtAddress.Text.Trim(),
                ReaderImage = this.pbReaderPhoto.Image != null ? new SerializeObjectToString().SerializeObject(this.pbReaderPhoto.Image) : "",
                ReaderId = this.objEditedReader.ReaderId
            };

            try
            {
                objReaderManger.EditReader(objReader);
                MessageBox.Show("提交成功！", "提示信息");
                // 同步更新(使用委托)
                ParentRefreshRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
    }
}
