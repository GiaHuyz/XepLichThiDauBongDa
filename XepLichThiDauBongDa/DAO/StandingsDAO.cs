using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace XepLichThiDauBongDa.DAO
{
    public class StandingsDAO
    {
        private static StandingsDAO instance;

        private StandingsDAO() { }

        public static StandingsDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StandingsDAO();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }


        public DataTable GetDataStandingsByLeagueID(string leagueID)
        {
            DataTable dt = new DataTable();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT Standings.StandingsID, Teams.TeamName, Teams.Logo, Points,Wins,Draws,Losses,GoalsFor,GoalsAgainst FROM Standings INNER JOIN Teams ON Standings.TeamID = Teams.TeamID WHERE LeagueID = '{leagueID}' ORDER BY Points DESC");
            return dt;
        }

        public List<(int, int, int, int, int, int)> GetPointStandingsByTeamIDAndLeagueID(string teamID, string leagueID)
        {
            DataTable dt = new DataTable();
            List<(int, int, int, int, int, int)> values = new List<(int, int, int, int, int, int)>();
            dt = DataProvider.Instance.ExecuteQuery($"SELECT Points,Wins,Draws,Losses,GoalsFor,GoalsAgainst FROM Standings WHERE LeagueID = '{leagueID}' AND TeamID = '{teamID}'");
            foreach (DataRow row in dt.Rows)
            {
                values.Add((Convert.ToInt32(row["Points"]), Convert.ToInt32(row["Wins"]), Convert.ToInt32(row["Draws"]), Convert.ToInt32(row["Losses"]), Convert.ToInt32(row["GoalsFor"]), Convert.ToInt32(row["GoalsAgainst"])));
            }
            return values;
        }

        public string GenerateID()
        {
            string s = "SELECT MAX(StandingsID) FROM dbo.Standings";
            try
            {
                string id = (string)DataProvider.Instance.ExecuteScalar(s);
                s = id.Substring(id.Length - 3, 3);
                int amount = int.Parse(s) + 1;
                if (amount < 10)
                    s = "STD" + "00" + amount.ToString();
                else if (amount < 100)
                    s = "STD" + "0" + amount.ToString();
                else
                    s = "STD" + amount.ToString();
            }
            catch (Exception)
            {
                s = "STD" + "001";
            }
            return s;
        }

        public bool InsertStandings(string id, string leagueID, string teamID, int points, int wins, int draws, int losses)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"INSERT INTO dbo.Standings VALUES ('{id}', '{leagueID}' , '{teamID}', {points}, {wins}, {draws}, {losses}, 0, 0)");
            return result > 0;
        }

        public bool UpdateStandings(string teamID, string leagueID, int point, int win, int draw, int lose, int goalsFor, int goalAgainst)
        {
            int result = DataProvider.Instance.ExecuteNonQuery($"UPDATE dbo.Standings SET Points = {point}, Wins = {win}, Draws = {draw}, Losses = {lose}, GoalsFor = {goalsFor}, GoalsAgainst = {goalAgainst} WHERE TeamID = '{teamID}' AND LeagueID = '{leagueID}'");
            return result > 0;
        }

        public void DeleteStandingsByLeagueID(string leagueID)
        {
            DataProvider.Instance.ExecuteNonQuery($"DELETE FROM Standings WHERE LeagueID = '{leagueID}'");
        }

        public DataTable SearchStandings(string txt, string leagueID)
        {
            return DataProvider.Instance.ExecuteQuery($"SELECT Standings.StandingsID, Teams.TeamName, Teams.Logo, Points,Wins,Draws,Losses,GoalsFor,GoalsAgainst FROM Standings INNER JOIN Teams ON Standings.TeamID = Teams.TeamID WHERE LeagueID = '{leagueID}' AND (StandingsID LIKE '%{txt}%' OR Teams.TeamName LIKE '%{txt}%') ORDER BY Points DESC");
        }
    }

}
