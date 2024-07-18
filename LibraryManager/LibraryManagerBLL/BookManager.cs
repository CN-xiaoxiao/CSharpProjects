using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagerDAL;
using LibraryManagerModels;

namespace LibraryManagerBLL
{
    /// <summary>
    /// 图书管理业务逻辑
    /// </summary>
    public class BookManager
    {
        private BookService objBookService = new BookService();

        /// <summary>
        /// 获取全部的图书分类
        /// </summary>
        /// <returns></returns>
        public List<Category> GetAllCategory()
        {
            return objBookService.GetAllCategory();
        }

        /// <summary>
        /// 获取全部的出版社信息
        /// </summary>
        /// <returns></returns>
        public List<Publisher> GetAllPublisher()
        {
            return objBookService.GetAllPublisher();
        }

        /// <summary>
        /// 判断图书条码是否已经存在
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public bool BarCodeIsExisted(string barCode)
        {
            int count = objBookService.GetCountByCarCode(barCode);

            return count == 1 ? true : false;
        }

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="objBook"></param>
        /// <returns></returns>
        public int AddBook(Book objBook)
        {
            return objBookService.AddBook(objBook);
        }
    }


}
