using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagerDBUtility;
using LibraryManagerModels;

using System.Data.SqlClient;

namespace LibraryManagerDAL
{
    /// <summary>
    /// 借书数据访问类
    /// </summary>
    public class BorrowService
    {
        /// <summary>
        /// 根据借阅证查询当前读者借书总数
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public int GetBorrowCount(string readingCard)
        {
            object count = SQLHelper.GetSingleResultByProcedure("usp_QueryBorrowCount", new SqlParameter[] { new SqlParameter("@ReadingCard", readingCard) });

            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 保存借书信息
        /// </summary>
        /// <param name="main"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool BorrowBook(BorrowInfo main, List<BorrowDetail> detail)
        {
            string mainSql = "insert into BorrowInfo (BorrowId,ReaderId,AdminName_B) values(@BorrowId,@ReaderId,@AdminName_B)";
            StringBuilder detailSql = new StringBuilder();
            detailSql.Append("insert into BorrowDetail(BorrowId,BookId,BorrowCount,ReturnCount,NonReturnCount,Expire)");
            detailSql.Append(" values(@BorrowId,@BookId,@BorrowCount,@ReturnCount,@NonReturnCount,@Expire)");

            SqlParameter[] mainParam = new SqlParameter[]
            {
                new SqlParameter("@BorrowId", main.BorrowId),
                new SqlParameter("@ReaderId", main.ReaderId),
                new SqlParameter("@AdminName_B", main.AdminName_B)
            };

            List<SqlParameter[]> detailParam = new List<SqlParameter[]>();

            foreach (BorrowDetail item in detail)
            {
                detailParam.Add(new SqlParameter[]
                {
                    new SqlParameter("BorrowId",item.BorrowId),
                    new SqlParameter("BookId",item.BookId),
                    new SqlParameter("BorrowCount",item.BorrowCount),
                    new SqlParameter("ReturnCount",item.ReturnCount),
                    new SqlParameter("Expire", item.Expire),
                    new SqlParameter("NonReturnCount", item.NonReturnCount)
                });
            }

            return SQLHelper.UpdateByTran(mainSql, mainParam, detailSql.ToString(), detailParam);
        }
    }
}
