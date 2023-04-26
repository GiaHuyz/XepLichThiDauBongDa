using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XepLichThiDauBongDa.DTO
{
    public class Account
    {
        private string username;
        private int role;

        public Account(string username, int role)
        {
            this.username = username;
            this.role = role;
        }

        public Account(DataRow row)
        {
            this.username = row["Username"].ToString();
            this.role = (bool)row["Roles"] == true ? 1 : 0;
        }

        public string Username { get => username; set => username = value; }
        public int Role { get => role; set => role = value; }
    }
}
