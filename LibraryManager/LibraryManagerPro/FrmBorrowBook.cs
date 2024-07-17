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
    public partial class FrmBorrowBook : Form
    {
     
        public FrmBorrowBook()
        {
            InitializeComponent();
       
        }

    

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
