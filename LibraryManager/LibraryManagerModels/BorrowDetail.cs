using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerModels
{
    /// <summary>
    /// 借阅详细表
    /// </summary>
    [Serializable]
    public class BorrowDetail
    {
        public int BorrowDetailId { get; set; }
        public string BorrowId { get; set; }
        public int BookId { get; set; }
        public int BorrowCount { get; set; }
        public int ReturnCount { get; set; }
        public int NonReturnCount { get; set; }
        public DateTime Expire {  get; set; }

        public string BarCode { get; set; }
        public string BookName { get; set; }
        public string ReadingCard { get; set; }
        public DateTime BorrowDate { get; set; }
        public string StatusDesc { get; set; }
    }
}
