using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace LibraryManagerDBUtility
{
    /// <summary>
    /// 针对SQLServer数据库的通用访问类
    /// </summary>
    public class SQLHelper
    {
        // 封装数据库连接字符串
        private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();

        #region 封装格式化SQL语句执行的各种方法

        public static int Update(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static int Update(string sql)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        public static object GetSingleResult(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static object GetSingleResult(string sql)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
        }

        public static SqlDataReader GetReader(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();

                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                conn.Close();
                string errorInfo = "调用public static SqlDataReader GetReader(string sql)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }

        }

        public static DataSet GetDataSet(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static DataSet GetDataSet(string sql)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 封装带参数SQL语句执行的各种方法

        public static int Update(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);

                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static int Update(string sql, SqlParameter[] param)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public static object GetSingleResult(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);

                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static object GetSingleResult(string sql, SqlParameter[] param)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        public static SqlDataReader GetReader(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);

                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                conn.Close();
                string errorInfo = "调用public static SqlDataReader GetReader(string sql, SqlParameter[] param)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
        }

        /// <summary>
        /// 启用事务提交多条带参数的SQL语句
        /// </summary>
        /// <param name="mainSql">主表SQL语句</param>
        /// <param name="mainParam">主表SQL语句对应的参数</param>
        /// <param name="detailSql">明细表SQL语句</param>
        /// <param name="detailParam">明细表SQL语句对应的参数数组集合</param>
        /// <returns>返回事务释放执行成功</returns>
        /// <exception cref="Exception"></exception>
        public static bool UpdateByTran(string mainSql, SqlParameter[] mainParam, string detailSql, List<SqlParameter[]> detailParam)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                conn.Open();
                cmd.Transaction = conn.BeginTransaction();

                if (mainSql != null && mainSql.Length != 0)
                {
                    cmd.CommandText = mainSql;
                    cmd.Parameters.AddRange(mainParam);
                    cmd.ExecuteNonQuery();
                }
                foreach (SqlParameter[] param in detailParam)
                {
                    cmd.CommandText = detailSql;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }

                cmd.Transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();
                }

                string errorInfo = "调用public static bool UpdateByTran(string mainSql, SqlParameter[] mainParam, string detailSql, List<SqlParameter[]> detailParam)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;
                }
                conn.Close();
            }
        }

        #endregion

        #region 封装调用存储过程执行的各种方法

        public static int UpdateByProcedure(string spName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(spName, conn);

            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static int UpdateByProcedure(string spName, SqlParameter[] param)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        public static object GetSingleResultByProcedure(string spName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(spName, conn);

            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static object GetSingleResultByProcedure(string spName, SqlParameter[] param)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        public static SqlDataReader GetReaderByProcedure(string spName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(spName, conn);

            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                conn.Close();
                string errorInfo = "调用public static SqlDataReader GetReaderByProcedure(string spName, SqlParameter[] param)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
        }

        /// <summary>
        /// 启用事务调用带参数的存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="paramArray">存储过程参数数组集合</param>
        /// <returns>返回基于事务的存储过程调用是否成功</returns>
        /// <exception cref="Exception"></exception>
        public static bool UpdateByTran(string procedureName, List<SqlParameter[]> paramArray)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedureName;
                cmd.Transaction = conn.BeginTransaction();

                foreach (SqlParameter[] param in paramArray)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }
                
                cmd.Transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                string errorInfo = "调用public static bool UpdateByTran(string procedureName, List<SqlParameter[]> paramArray)方法时发生错误：" + ex.Message;
                WriteLog(errorInfo);

                throw new Exception(errorInfo);
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;
                }
                conn.Close();
            }
        }

        #endregion

        #region 其他方法

        private static void WriteLog(string log)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream("sqlheaper.log", FileMode.Create)))
            {
                sw.WriteLine(DateTime.Now.ToString() + "  " + log);
            }
        }

        #endregion
    }
}
