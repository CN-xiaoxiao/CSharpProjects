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

        /// <summary>
        /// 根据借阅证号查询读者借书信息列表
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public List<BorrowDetail> GetBorrowDetailByReadingCard(string readingCard)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("ReadingCard", readingCard)
            };
            SqlDataReader objReader = SQLHelper.GetReaderByProcedure("usp_QueryBookByReadingCard", param);
            List<BorrowDetail> detailList = new List<BorrowDetail>();

            while (objReader.Read())
            {
                detailList.Add(new BorrowDetail()
                {
                    ReadingCard = objReader["ReadingCard"].ToString(),
                    BorrowDetailId = Convert.ToInt32(objReader["BorrowDetailId"]),
                    BookId = Convert.ToInt32(objReader["BookId"]),
                    BorrowId = objReader["BorrowId"].ToString(),
                    BookName = objReader["BookName"].ToString(),
                    BarCode = objReader["BarCode"].ToString(),
                    BorrowCount = Convert.ToInt32(objReader["BorrowCount"]),
                    ReturnCount = Convert.ToInt32(objReader["ReturnCount"]),
                    NonReturnCount = Convert.ToInt32(objReader["NonReturnCount"]),
                    BorrowDate = Convert.ToDateTime(objReader["BorrowDate"]),
                    Expire = Convert.ToDateTime(objReader["Expire"]),
                    StatusDesc = objReader["StatusDesc"].ToString()
                });
            }
            objReader.Close();
            return detailList;
        }

        public bool ReturnBook(List<ReturnBook> bookList)
        {
            List<SqlParameter[]> paramArray = new List<SqlParameter[]>();
            foreach (ReturnBook item in bookList)
            {
                paramArray.Add(new SqlParameter[]
                {
                    new SqlParameter("@BorrowDetailId", item.BorrowDetailId),
                    new SqlParameter("@ReturnCount", item.ReturnCount),
                    new SqlParameter("@AdminName_R", item.AdminName_R)
                });
            }

            return SQLHelper.UpdateByTran("usp_ReturnBook", paramArray);
        }
    }
}
