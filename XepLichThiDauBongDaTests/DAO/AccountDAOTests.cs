using Microsoft.VisualStudio.TestTools.UnitTesting;
using XepLichThiDauBongDa.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XepLichThiDauBongDa.DAO.Tests
{
    [TestClass()]
    public class AccountDAOTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            bool result = AccountDAO.Instance.Login("a", "1");
            Assert.IsTrue(result);
            bool result2 = AccountDAO.Instance.Login("a", "2");
            Assert.IsFalse(result2);
            bool result3 = AccountDAO.Instance.Login("c", "2");
            Assert.IsFalse(result3);
        }
    }
}