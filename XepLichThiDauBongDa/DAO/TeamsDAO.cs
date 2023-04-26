using System;
using System.Collections.Generic;
using System.Data;
using XepLichThiDauBongDa.DTO;

namespace XepLichThiDauBongDa.DAO
{
    public class TeamsDAO
    {
        private static TeamsDAO instance;

        private TeamsDAO() { }

        public static TeamsDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TeamsDAO();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }


        public DataTable GetDataTeams()
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Teams");
            return dt;
        }

        public string GenerateID()
        {
            string s = "SELECT MAX(TeamID) FROM dbo.Teams";
            try
            {
                string id = (string)DataProvider.Instance.ExecuteScalar(s);
                s = id.Substring(id.Length - 3, 3);
                int amount = int.Parse(s) + 1;
                if (amount < 10)
                    s = "TE" + "00" + amount.ToString();
                else if (amount < 100)
                    s = "TE" + "0" + amount.ToString();
                else
                    s = "TE" + amount.ToString();
            }
            catch (Exception)
            {
                s = "TE" + "001";
            }
            return s;
        }

        public bool InsertTeams(string id, string name, string abbreviation, string leagueID, byte[] logo)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"INSERT INTO dbo.Teams VALUES ('{id}','{name}' , '{abbreviation}', @logo )", new object[] { logo });
            DataProvider.Instance.ExecuteNonQuery($"INSERT INTO dbo.LeagueTeams VALUES('{leagueID}','{id}')");
            return result > 0;
        }

        public bool UpdateTeams(string id, string name, string abbreviation, byte[] logo)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"UPDATE dbo.Teams SET TeamName = '{name}',  Abbreviation = '{abbreviation}', Logo = @logo WHERE TeamID = '{id}'", new object[] {logo});
            return result > 0;
        }

        public DataTable GetTeamsByLeagueID(string id)
        {
            return DataProvider.Instance.ExecuteQuery($"SELECT t.* FROM Teams t INNER JOIN LeagueTeams lt ON t.TeamID = lt.TeamID WHERE lt.LeagueID = '{id}'");
        }

        public List<Teams> GetAllTeamsByLeagueID(string id)
        {
            List<Teams> teams = new List<Teams>();
            DataTable dt = DataProvider.Instance.ExecuteQuery($"SELECT t.* FROM Teams t INNER JOIN LeagueTeams lt ON t.TeamID = lt.TeamID WHERE lt.LeagueID = '{id}'");
            foreach (DataRow row in dt.Rows)
            {
                teams.Add(new Teams(row));
            }
            return teams;
        }

        public bool InsertLeagueTeam(string leagueID, string teamID)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"INSERT INTO LeagueTeams VALUES ('{leagueID}', '{teamID}')");
            return result > 0;
        }

        public int CountTeamsByLeagueID(string leagueID)
        {
            int result = (int)DataProvider.Instance.ExecuteScalar($"SELECT COUNT(*) FROM LeagueTeams WHERE LeagueID = '{leagueID}'");
            return result;
        }

        public bool CheckTeamInLeague(string leagueID, string teamID)
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT * FROM dbo.LeagueTeams WHERE LeagueID = '{leagueID}' and TeamID = '{teamID}'");
            return dt.Rows.Count > 0;
        }

        public bool CheckDuplicateTeamName(string teamName, string abbreviation)
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Teams WHERE TeamName = '{teamName}' OR abbreviation = '{abbreviation}'");
            return dt.Rows.Count > 0;
        }

        public bool CheckDuplicateUpdateTeamName(string teamName, string abbreviation)
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Teams WHERE (TeamName = '{teamName}' OR abbreviation = '{abbreviation}') AND (TeamName NOT IN ('{teamName}') OR abbreviation NOT IN ('{abbreviation}'))");
            return dt.Rows.Count > 0;
        }

        public void DeleteTeam(string teamID)
        {
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM LeagueTeams where TeamID = '{teamID}'");
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Matches WHERE HomeTeamID = '{teamID}' OR AwayTeamID = '{teamID}'");
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Standings WHERE TeamID = '{teamID}'");
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Teams where TeamID = '{teamID}'");
        }

        public DataTable SearchTeam(string txt, string leagueID)
        {
            return DataProvider.Instance.ExecuteQuery($"SELECT t.* FROM Teams t INNER JOIN LeagueTeams lt ON t.TeamID = lt.TeamID WHERE lt.LeagueID = '{leagueID}' AND (t.TeamID LIKE '%{txt}%' OR t.TeamName LIKE '%{txt}%' OR t.Abbreviation LIKE '%{txt}%')");
        }
    }
}
