using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagerDAL;
using LibraryManagerModels;

namespace LibraryManagerBLL
{
    public class ReaderManager
    {
        private ReaderService objReaderService = new ReaderService();

        public int AddReader(Reader objReader)
        {
            return objReaderService.AddReader(objReader);
        }

        public int EditReader(Reader objReader)
        {
            return objReaderService.EditReader(objReader);
        }

        public int ForbiddenReaderCard(string readerId)
        {
            return objReaderService.ForbiddenReaderCard(readerId);
        }

        public DataTable GetAllReaderRole()
        {
            return objReaderService.GetAllReaderRole();
        }
        public Reader GetReaderByReadingCard(string readingCard)
        {
            return objReaderService.GetReaderByReadingCard(readingCard);
        }
        public Reader GetReaderByIDCard(string IDCard)
        {
            return objReaderService.GetReaderByIDCard(IDCard);
        }

        /// <summary>
        /// 根据会员角色查询读者信息
        /// </summary>
        /// <param name="roleID">角色编号</param>
        /// <param name="readerCount">返回的读者总数</param>
        /// <returns>返回读者对象列表</returns>
        public List<Reader> GetReaderByRole(string roleID, out int readerCount)
        {
            List<Reader> readerList = objReaderService.GetReaderByRole(roleID, out readerCount);

            foreach (Reader reader in readerList)
            {
                switch (reader.StatusId)
                {
                    case 1:
                        reader.StatusDesc = "正常";
                        break;
                    case 0:
                        reader.StatusDesc = "禁用";
                        break;
                }
            }

            return readerList;  
        }
    }
}
