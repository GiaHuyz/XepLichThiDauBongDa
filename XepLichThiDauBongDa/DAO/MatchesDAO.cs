using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using XepLichThiDauBongDa.DTO;

namespace XepLichThiDauBongDa.DAO
{
    public class MatchesDAO
    {
        private static MatchesDAO instance;

        private MatchesDAO() { }

        public static MatchesDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MatchesDAO();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }


        public DataTable GetDataMatches()
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery("SELECT Matches.MatchID, HomeTeam.TeamName as HomeTeam, HomeTeam.Logo as HomeTeamLogo, Matches.MatchDate,AwayTeam.Logo as AwayTeamLogo, AwayTeam.TeamName as AwayTeam FROM Matches INNER JOIN Teams AS HomeTeam ON Matches.HomeTeamID = HomeTeam.TeamID INNER JOIN Teams AS AwayTeam ON Matches.AwayTeamID = AwayTeam.TeamID WHERE LeagueID = '{id}'");
            return dt;
        }

        public DataTable GetDataMatchesByLeagueId(string id)
        {
            
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT Matches.MatchID, HomeTeam.TeamName as HomeTeam, HomeTeam.Logo as HomeTeamLogo, Matches.MatchDate,AwayTeam.Logo as AwayTeamLogo, AwayTeam.TeamName as AwayTeam FROM Matches INNER JOIN Teams AS HomeTeam ON Matches.HomeTeamID = HomeTeam.TeamID INNER JOIN Teams AS AwayTeam ON Matches.AwayTeamID = AwayTeam.TeamID WHERE LeagueID = '{id}'");
            return dt;
        }

        public string GenerateID()
        {
            string s = "SELECT MAX(MatchID) FROM dbo.Matches";
            try
            {
                string id = (string)DataProvider.Instance.ExecuteScalar(s);
                s = id.Substring(id.Length - 3, 3);
                int amount = int.Parse(s) + 1;
                if (amount < 10)
                    s = "M" + "00" + amount.ToString();
                else if (amount < 100)
                    s = "M" + "0" + amount.ToString();
                else
                    s = "M" + amount.ToString();
            }
            catch (Exception)
            {
                s = "M" + "001";
            }
            return s;
        }

        public bool InsertMatches(string id, string homeTeamId, string awayTeamId, string leagueID, string turn)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"INSERT INTO dbo.Matches(MatchID,HomeTeamID,AwayTeamID,LeagueID, Turn) VALUES ('{id}', '{homeTeamId}' , '{awayTeamId}' , '{leagueID}', N'{turn}')");
            return result > 0;
        }
       
        public DataTable LoadMatchesByNumPage(int numPage, int @rowPerPages, string leagueID)
        {
            int count = (int)DataProvider.Instance.ExecuteScalar("SELECT COUNT(*) FROM dbo.Matches");
            if(count > 0) {
                return DataProvider.Instance.ExecuteQuery("EXEC USP_PaginationMatches @numPages , @rowPerPages , @leagueID", new object[] { numPage, rowPerPages, leagueID });
            }
            return null;
        }

        public void DeleteMatchesByLeagueID(string leagueID)
        {
            StandingsDAO.Instance.DeleteStandingsByLeagueID(leagueID);
            DataProvider.Instance.ExecuteQuery($"DELETE FROM Matches WHERE LeagueID = '{leagueID}'");
        }

        public void UpdateMatchDate(DateTime matchDate, string matchID)
        {
            DataProvider.Instance.ExecuteNonQuery($"UPDATE dbo.Matches SET MatchDate = @matchDate WHERE MatchID = '{matchID}'", new object[] { matchDate });
        }

        public DataTable LoadAllMatchesByLeagueID(string leagueID)
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"select Matches.MatchID, HomeTeam.TeamID as HomeTeamID, AwayTeam.TeamID as AwayTeamID, HomeTeam.TeamName as HomeTeam, HomeTeam.Logo as HomeTeamLogo, CONVERT(varchar, Matches.HomeTeamScore) + ' - ' + CONVERT(varchar, Matches.AwayTeamScore) as Result, AwayTeam.Logo as AwayTeamLogo, AwayTeam.TeamName as AwayTeam, Matches.MatchDate, Matches.Status, Matches.Turn FROM Matches INNER JOIN Teams AS HomeTeam ON Matches.HomeTeamID = HomeTeam.TeamID INNER JOIN Teams AS AwayTeam ON Matches.AwayTeamID = AwayTeam.TeamID  WHERE LeagueID = '{leagueID}'");
            return dt;
        }

        public bool UpdateMatch(int homeScore, int awayScore, string matchID)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"UPDATE dbo.Matches SET HomeTeamScore = {homeScore}, AwayTeamScore = {awayScore}, Status = 'Finished' WHERE MatchID = '{matchID}'");
            return result > 0;
        }

        public DateTime GetMaxDateHomeTurn(string leagueID)
        {
            DateTime maxDate = Convert.ToDateTime(DataProvider.Instance.ExecuteScalar($"SELECT MAX(MatchDate) AS MaxDate FROM Matches WHERE Turn = 'Home' and LeagueID = '{leagueID}'"));
            return maxDate;
        }

        public bool CheckDateHomeTurn(string leagueID)
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT MatchID, MatchDate FROM Matches WHERE Turn = 'Home' and MatchDate IS NULL");
            return dt.Rows.Count > 0;
        }

        public DateTime GetMaxDate(string leagueID)
        {
            try
            {
                DateTime maxDate = Convert.ToDateTime(DataProvider.Instance.ExecuteScalar($"SELECT MAX(MatchDate) FROM Matches WHERE LeagueID = '{leagueID}'"));
                return maxDate;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public DateTime GetMinDateAwayTurn(string leagueID)
        {
            try
            {
                DateTime minDate = Convert.ToDateTime(DataProvider.Instance.ExecuteScalar($"SELECT MIN(MatchDate) FROM Matches WHERE LeagueID = '{leagueID}' AND Turn = 'Away'"));
                return minDate;
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }

        public DataTable SearchMatch(string txt, string leagueID)
        {
            return DataProvider.Instance.ExecuteQuery($"select Matches.MatchID, HomeTeam.TeamID as HomeTeamID,  AwayTeam.TeamID as AwayTeamID, HomeTeam.TeamName as HomeTeam, HomeTeam.Logo as HomeTeamLogo, CONVERT(varchar, Matches.HomeTeamScore) + ' - ' + CONVERT(varchar, Matches.AwayTeamScore) as Result, AwayTeam.Logo as AwayTeamLogo, AwayTeam.TeamName as AwayTeam, Matches.MatchDate, Matches.Status, Matches.Turn FROM Matches INNER JOIN Teams AS HomeTeam ON Matches.HomeTeamID = HomeTeam.TeamID INNER JOIN Teams AS AwayTeam ON Matches.AwayTeamID = AwayTeam.TeamID WHERE (MatchID like '%{txt}%' OR HomeTeam.TeamName LIKE '%{txt}%' OR AwayTeam.TeamName LIKE '%{txt}%' OR FORMAT(MatchDate, 'dd-MM-yyyy') LIKE '%{txt}%' OR Status LIKE '%{txt}%' OR Turn LIKE '%{txt}%') AND (LeagueID = '{leagueID}')");
        }
    }
}
