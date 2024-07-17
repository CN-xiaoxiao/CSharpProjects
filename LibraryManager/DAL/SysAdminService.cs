using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagerModels;
using LibraryManagerDBUtility;

using System.Data;
using System.Data.SqlClient;

namespace LibraryManagerDAL
{
    /// <summary>
    /// 管理员数据访问类
    /// </summary>
    public class SysAdminService
    {
        /// <summary>
        /// 根据登录账户和密码从数据库比对
        /// </summary>
        /// <param name="objAdmin">包含账号和密码的管理员对象</param>
        /// <returns>返回管理员对象信息</returns>
        public SysAdmin AdminLogin(SysAdmin objAdmin)
        {
            // 定义登录的SQL语句
            string sql = "select AdminName,StatusId,RoleId from SysAdmins where";
            sql += " AdminId=@AdminId and LoginPwd=@LoginPwd";

            // 封装参数
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@AdminId", objAdmin.AdminId),
                new SqlParameter("@LoginPwd", objAdmin.LoginPwd)
            };

            // 执行查询
            SqlDataReader objReader = SQLHelper.GetReader(sql, param);

            // 处理查询结果
            if (objReader.Read())
            {
                objAdmin.StatusId = Convert.ToInt32(objReader["StatusId"]);
                objAdmin.RoleId = Convert.ToInt32(objReader["RoleId"]);
                objAdmin.AdminName = objReader["AdminName"].ToString();
            }
            else
            {
                objAdmin = null;
            }

            objReader.Close();

            return objAdmin;
        }
    }
}
