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
    public partial class FrmBookManage : Form
    {
     

        public FrmBookManage()
        {
            InitializeComponent();
        }

      
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
