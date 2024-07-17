using LibraryManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace LibraryManagerPro
{
    static class Program
    {
        // 定义一个全局变量
        public static SysAdmin objCurrentAdmin = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 显示登录窗体
            FrmAdminLogin frmLogin = new FrmAdminLogin();
            DialogResult result = frmLogin.ShowDialog();

            if (result == DialogResult.OK)
            {
                Application.Run(new FrmMain());
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
