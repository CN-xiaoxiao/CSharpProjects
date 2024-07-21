using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagerDAL;
using LibraryManagerModels;

namespace LibraryManagerBLL
{
    public class BorrowManager
    {
        private BorrowService objBorrowService = new BorrowService();

        /// <summary>
        /// 根据借阅证查询当前读者借书总数
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public int GetBorrowCount(string readingCard)
        {
            return objBorrowService.GetBorrowCount(readingCard);
        }

        /// <summary>
        /// 保存借书信息
        /// </summary>
        /// <param name="main"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool BorrowBook(BorrowInfo main, List<BorrowDetail> detail)
        {
            return objBorrowService.BorrowBook(main, detail);
        }
    }
}
