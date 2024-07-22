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

        /// <summary>
        /// 根据借阅证号查询读者借书信息列表
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public List<BorrowDetail> GetBorrowDetailByReadingCard(string readingCard)
        {
            return objBorrowService.GetBorrowDetailByReadingCard(readingCard);
        }

        /// <summary>
        /// 还书操作
        /// </summary>
        /// <param name="returnList">还书对象集合</param>
        /// <param name="nonReturnList">未还对象集合</param>
        /// <param name="adminName">管理员姓名</param>
        /// <returns></returns>
        public bool ReturnBook(List<BorrowDetail> returnList, List<BorrowDetail> nonReturnList, string adminName)
        {
            List<ReturnBook> reutrnBookList = new List<ReturnBook>();

            foreach (BorrowDetail returnItem in returnList)
            {
                // 取出当前图书本次归还的总数
                int returnCount = returnItem.ReturnCount;
                // 在未归还图书结合中找到该书的出借记录
                List<BorrowDetail> borrowList = (from b in nonReturnList where b.BarCode.Equals(returnItem.BarCode) select b).ToList<BorrowDetail>();
                // 遍历当前的图书出借记录
                foreach (BorrowDetail borrowItem in borrowList)
                {
                    if (borrowItem.NonReturnCount == returnCount || borrowItem.NonReturnCount > returnCount)
                    {
                        reutrnBookList.Add(new ReturnBook()
                        {
                            BorrowDetailId = borrowItem.BorrowDetailId,
                            ReturnCount = returnCount,
                            BookName = borrowItem.BookName
                        });
                        break;
                    }
                    else
                    {
                        reutrnBookList.Add(new ReturnBook()
                        {
                            BorrowDetailId = borrowItem.BorrowDetailId,
                            ReturnCount = borrowItem.NonReturnCount,
                            BookName = borrowItem.BookName
                        });
                        returnCount -= borrowItem.NonReturnCount;
                    }
                }
            }

            for (int i = 0; i < reutrnBookList.Count; i++)
            {
                reutrnBookList[i].AdminName_R = adminName;
            }

            return objBorrowService.ReturnBook(reutrnBookList);
        }
    }
}
