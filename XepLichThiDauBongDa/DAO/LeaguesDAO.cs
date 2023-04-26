using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace XepLichThiDauBongDa.DAO
{
    public class LeaguesDAO
    {
        private static LeaguesDAO instance;

        private LeaguesDAO() { }

        public static LeaguesDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LeaguesDAO();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }


        public DataTable GetDataLeagues()
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Leagues");
            return dt;
        }

        public bool CheckLeagueAdmin(string username, string leagueID)
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT LeagueID FROM dbo.Leagues WHERE Username = '{username}' AND LeagueID = '{leagueID}'");
            return dt.Rows.Count > 0;
        }

        public string GenerateID()
        {
            string s = "SELECT MAX(LeagueID) FROM dbo.Leagues";
            try
            {
                string id = (string)DataProvider.Instance.ExecuteScalar(s);
                s = id.Substring(id.Length - 3, 3);
                int amount = int.Parse(s) + 1;
                if (amount < 10)
                    s = "LG" + "00" + amount.ToString();
                else if (amount < 100)
                    s = "LG" + "0" + amount.ToString();
                else
                    s = "LG" + amount.ToString();
            }
            catch (Exception)
            {
                s = "LG" + "001";
            }
            return s;
        }

        public bool InsertLeagues(string id, string name, DateTime startDate, DateTime endDate, string username)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"INSERT INTO dbo.Leagues VALUES ('{id}','{name}' , @startDate , @endDate , 0, '{username}')", new object[] { startDate.Date, endDate.Date });
            return result > 0;
        }

        public bool UpdateLeagues(string id, string name, DateTime startDate, DateTime endDate)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"UPDATE dbo.Leagues SET LeagueName = '{name}', StartDate = @startDate , EndDate = @endDate WHERE LeagueID = '{id}'", new object[] { startDate.Date, endDate.Date });
            return result > 0;
        }

        public DataTable GetLeaguesByUser(string username)
        {
            return DataProvider.Instance.ExecuteQuery($"SELECT LeagueID, LeagueName, StartDate, EndDate, NumberOfTeams FROM dbo.Leagues WHERE Username = '{username}'");
        }

        public List<(DateTime, DateTime)> GetDateLeague(string leagueID)
        {
            List<(DateTime, DateTime)> result = new List<(DateTime, DateTime)>();
            DataTable data = DataProvider.Instance.ExecuteQuery($"SELECT StartDate, EndDate FROM Leagues WHERE LeagueID = '{leagueID}'");
            foreach (DataRow row in data.Rows)
            {
                result.Add(((Convert.ToDateTime(row["StartDate"])), (Convert.ToDateTime(row["EndDate"]))));
            }
            return result;
        }

        public void DeleteLeague(string leagueID)
        {
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM LeagueTeams where LeagueID = '{leagueID}'");
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Matches where LeagueID = '{leagueID}'");
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Standings where LeagueID = '{leagueID}'");
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Leagues where LeagueID = '{leagueID}'");
        }

        public List<string> GetAllLeagueIdByUsername(string username)
        {
            List<string> ids = new List<string>();
            DataTable data = DataProvider.Instance.ExecuteQuery($"SELECT LeagueID FROM Leagues WHERE Username = '{username}'");
            foreach (DataRow row in data.Rows)
            {
                ids.Add(row["LeagueID"].ToString());
            }
            return ids;
        }

        public void DeleteLeagueByUsername(string username)
        {
            List<string> ids = GetAllLeagueIdByUsername(username);
            foreach (var id in ids)
            {
                DeleteLeague(id);
            }
        }

        public DataTable SearchLeagues(string txt)
        {
            return DataProvider.Instance.ExecuteQuery($"SELECT * FROM Leagues WHERE LeagueID LIKE '%{txt}%' OR LeagueName LIKE N'%{txt}%' OR FORMAT(StartDate, 'dd-MM-yyyy') LIKE '%{txt}%' OR FORMAT(EndDate, 'dd-MM-yyyy') LIKE '%{txt}%' OR NumberOfTeams LIKE '{txt}'");
        }
    }
}
