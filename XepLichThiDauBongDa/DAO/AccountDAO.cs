using System.Data;
using System.Security.Cryptography;
using System.Text;
using XepLichThiDauBongDa.DTO;

namespace XepLichThiDauBongDa.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        private AccountDAO() { }

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDAO();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public DataTable GetDataAcc()
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT Username, Roles FROM Account");
            return dt;
        }

        private string hashPass(string password)
        {
            MD5 mh = MD5.Create();
            byte[] passwordByte = Encoding.ASCII.GetBytes(password);
            byte[] hashPassArr = mh.ComputeHash(passwordByte);
            string hashPass = "";
            foreach (byte item in hashPassArr)
            {
                hashPass += item;
            }
            return hashPass;
        }

        public bool Login(string username, string password)
        {
            string query = "USP_Login @userName , @pass";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { username, 
                hashPass(password) });

            return result.Rows.Count > 0;
        }

        public bool CheckDuplicateAccount(string username)
        {
            DataTable result = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Account WHERE Username = '{username}'");
            return result.Rows.Count > 0;
        }

        public bool InsertAccount(string username, string password)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"INSERT INTO Account VALUES ('{username}','{hashPass(password)}', 0)");
            return result > 0;
        }

        public void UpdateAccount(string username, string password)
        {
            DataProvider.Instance.ExecuteNonQuery($"UPDATE Account SET Password = '{hashPass(password)}' WHERE Username = '{username}'");
        }

        public Account GetAccountByUserName(string username)
        {
            DataTable result = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Account WHERE Username = '{username}'");
            foreach (DataRow row in result.Rows)
            {
                return new Account(row);
            }
            return null;
        }

        public void UpdateRoleAccount(int roles, string username)
        {
            DataProvider.Instance.ExecuteNonQuery($"UPDATE Account SET Roles = {roles} WHERE Username = '{username}'");
        }
    }
}
