using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using LibraryManagerModels;
using LibraryManagerDBUtility;

namespace LibraryManagerDAL
{
    public class BookService
    {
        #region 封装图书分类和出版社对象

        /// <summary>
        /// 获取全部的图书分类
        /// </summary>
        /// <returns></returns>
        public List<Category> GetAllCategory()
        {
            string sql = "select CategoryId,CategoryName from Categories";
            List<Category> list = new List<Category>();
            SqlDataReader objReader = SQLHelper.GetReader(sql);

            while (objReader.Read())
            {
                list.Add(new Category()
                {
                    CategoryId = Convert.ToInt32(objReader["CategoryId"]),
                    CategoryName = objReader["CategoryName"].ToString()
                });

            }

            objReader.Close();

            return list;
        }

        /// <summary>
        /// 获取全部的出版社信息
        /// </summary>
        /// <returns></returns>
        public List<Publisher> GetAllPublisher()
        {
            string sql = "select PublisherId,PublisherName from Publishers";
            List<Publisher> list = new List<Publisher>();
            SqlDataReader objReader = SQLHelper.GetReader(sql);

            while (objReader.Read())
            {
                list.Add(new Publisher()
                {
                    PublisherId = Convert.ToInt32(objReader["PublisherId"]),
                    PublisherName = objReader["PublisherName"].ToString()
                });
            }

            objReader.Close();

            return list;
        }

        #endregion

        #region 添加图书

        /// <summary>
        /// 判断条码是否已经存在
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public int GetCountByCarCode(string barCode)
        {
            string sql = "select count(*) from Books where BarCode=@BarCode";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BarCode",barCode)
            };

            return Convert.ToInt32(SQLHelper.GetSingleResult(sql, param));
        }

        /// <summary>
        /// 添加图书对象
        /// </summary>
        /// <param name="objBook"></param>
        /// <returns></returns>
        public int AddBook(Book objBook)
        {
            // 封装参数
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BarCode", objBook.BarCode),
                new SqlParameter("@BookName", objBook.BookName),
                new SqlParameter("@Author", objBook.Author),
                new SqlParameter("@PublisherId", objBook.PublisherId),
                new SqlParameter("@PublishDate", objBook.PublishDate),
                new SqlParameter("@BookCategory", objBook.BookCategory),
                new SqlParameter("@UnitPrice", objBook.UnitPrice),
                new SqlParameter("@BookImage", objBook.BookImage),
                new SqlParameter("@BookCount", objBook.BookCount),
                new SqlParameter("@Remainder", objBook.Remainder),
                new SqlParameter("@BookPosition", objBook.BookPosition)
            };

            return SQLHelper.UpdateByProcedure("usp_AddBook", param);
        }

        #endregion

        #region 图书上架

        /// <summary>
        /// 根据图书条码查询图书对象
        /// </summary>
        /// <param name="barCode">图书条码</param>
        /// <returns></returns>
        public Book GetBookByBarCode(string barCode)
        {
            string sql = "select BookId, BarCode,BookName,Author,Books.PublisherId,PublishDate,BookCategory,UnitPrice,BookImage,BookCount,Remainder,BookPosition,RegTime,PublisherName,CategoryName from Books";
            sql += " inner join Publishers on Publishers.PublisherId=Books.PublisherId";
            sql += " inner join Categories on Books.BookCategory=Categories.CategoryId";
            sql += " where BarCode=@BarCode";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BarCode", barCode)
            };
            SqlDataReader objReader = SQLHelper.GetReader(sql, param);
            Book objBook = null;

            if (objReader.Read())
            {
                objBook = new Book()
                {
                    BookId = Convert.ToInt32(objReader["BookId"].ToString()),
                    BarCode = objReader["BarCode"].ToString(),
                    BookName = objReader["BookName"].ToString(),
                    Author = objReader["Author"].ToString(),
                    PublisherId = Convert.ToInt32(objReader["PublisherId"].ToString()),
                    PublishDate = Convert.ToDateTime(objReader["PublishDate"].ToString()),
                    BookCategory = Convert.ToInt32(objReader["BookCategory"].ToString()),
                    UnitPrice = Convert.ToDouble(objReader["UnitPrice"].ToString()),
                    BookImage = objReader["BookImage"] is DBNull ? "" : objReader["BookImage"].ToString(),
                    BookCount = Convert.ToInt32(objReader["BookCount"].ToString()),
                    Remainder = Convert.ToInt32(objReader["Remainder"].ToString()),
                    BookPosition = objReader["BookPosition"].ToString(),
                    RegTime = Convert.ToDateTime(objReader["RegTime"].ToString()),
                    PublisherName = objReader["PublisherName"].ToString(),
                    CategoryName = objReader["CategoryName"].ToString()

                };
            }
            objReader.Close();

            return objBook;
        }

        /// <summary>
        /// 更新图书收藏总数
        /// </summary>
        /// <param name="barCode">图书条码</param>
        /// <param name="bookCount">新增总数</param>
        /// <returns></returns>
        public int AddBookCount(string barCode, int bookCount)
        {
            string sql = "update Books set BookCount=BookCount+@BookCount where BarCode=@BarCode";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BookCount", bookCount),
                new SqlParameter("@BarCode", barCode)
            };

            return SQLHelper.Update(sql, param);
        }

        #endregion
    }
}
