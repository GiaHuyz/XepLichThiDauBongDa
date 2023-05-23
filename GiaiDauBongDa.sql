CREATE DATABASE QLGD;
go
USE QLGD;
go

CREATE TABLE Account (
    Username VARCHAR(50) PRIMARY KEY,
    Password VARCHAR(50) NOT NULL,
    Roles BIT NOT NULL DEFAULT 0
);

CREATE TABLE Leagues (
    LeagueID VARCHAR(10) PRIMARY KEY,
    LeagueName NVARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    NumberOfTeams INT NOT NULL,
	Username varchar(50) NOT NULL,
	FOREIGN KEY (Username) REFERENCES Account(Username),
);

CREATE TABLE Teams (
    TeamID VARCHAR(10) PRIMARY KEY,
    TeamName NVARCHAR(50) NOT NULL,
    Abbreviation varchar(6) NOT NULL,
    Logo IMAGE
);

CREATE TABLE LeagueTeams (
    LeagueID VARCHAR(10) NOT NULL,
    TeamID VARCHAR(10) NOT NULL,
    PRIMARY KEY (LeagueID, TeamID),
    FOREIGN KEY (LeagueID) REFERENCES Leagues(LeagueID),
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID)
);

CREATE TABLE Matches (
    MatchID VARCHAR(10) PRIMARY KEY,
    HomeTeamID VARCHAR(10) NOT NULL,
    AwayTeamID VARCHAR(10) NOT NULL,
    LeagueID VARCHAR(10) NOT NULL,
    MatchDate DATETIME,
    HomeTeamScore INT NOT NULL DEFAULT 0,
    AwayTeamScore INT NOT NULL DEFAULT 0,
    Status VARCHAR(30) NOT NULL DEFAULT 'Not Finished',
    Turn NVARCHAR(10) NOT NULL,
    FOREIGN KEY (HomeTeamID) REFERENCES Teams(TeamID),
    FOREIGN KEY (AwayTeamID) REFERENCES Teams(TeamID),
    FOREIGN KEY (LeagueID) REFERENCES Leagues(LeagueID)
);

CREATE TABLE Standings (
    StandingsID VARCHAR(10) PRIMARY KEY,
    LeagueID VARCHAR(10) NOT NULL,
    TeamID VARCHAR(10) NOT NULL,
    Points INT NOT NULL,
    Wins INT NOT NULL,
    Draws INT NOT NULL,
    Losses INT NOT NULL,
    GoalsFor INT NOT NULL DEFAULT 0,
    GoalsAgainst INT NOT NULL DEFAULT 0,
    FOREIGN KEY (LeagueID) REFERENCES Leagues(LeagueID),
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID)
); 

GO 

CREATE PROC USP_Login @userName varchar(50), @pass varchar(50)
AS	
BEGIN
    SELECT * FROM dbo.Account WHERE Username = @userName AND Password = @pass
END

go

CREATE PROC USP_PaginationMatches @numPages int, @rowPerPages int, @leagueID varchar(10)
AS
BEGIN
    select top(@rowPerPages) Matches.MatchID,
    HomeTeam.TeamID as HomeTeamID, 
    AwayTeam.TeamID as AwayTeamID,
    HomeTeam.TeamName as HomeTeam,
    HomeTeam.Logo as HomeTeamLogo,
	CONVERT(varchar, Matches.HomeTeamScore) + ' - ' + CONVERT(varchar, Matches.AwayTeamScore) as Result,
    AwayTeam.Logo as AwayTeamLogo,
    AwayTeam.TeamName as AwayTeam,
    Matches.MatchDate,
	Matches.Status,
    Matches.Turn
    FROM Matches
    INNER JOIN Teams AS HomeTeam ON Matches.HomeTeamID = HomeTeam.TeamID
    INNER JOIN Teams AS AwayTeam ON Matches.AwayTeamID = AwayTeam.TeamID 
    WHERE MatchID not in (select top((@numPages - 1) * @rowPerPages) MatchID from Matches where LeagueID = @leagueID) AND
    LeagueID = @leagueID ORDER BY MatchID;
END

go

CREATE TRIGGER TRG_LeagueTeam ON LeagueTeams
AFTER INSERT, DELETE
AS
BEGIN
  UPDATE Leagues
  SET NumberOfTeams = (SELECT COUNT(*) FROM LeagueTeams WHERE LeagueID = Leagues.LeagueID)
  WHERE LeagueID IN (SELECT LeagueID FROM inserted UNION SELECT LeagueID FROM deleted)
END


go

INSERT INTO Account VALUES('admin', '1962026656160185351301320480154111117132155',1)
INSERT INTO Account VALUES('user', '1962026656160185351301320480154111117132155',0)

go


INSERT INTO Leagues (LeagueID, LeagueName, StartDate, EndDate, NumberOfTeams, Username) VALUES
('LG001', 'Premier League', '2023-08-10', '2023-09-20', 0,'user'),
('LG002', 'La Liga', '2023-08-01', '2023-09-25', 0,'user'),
('LG003', 'Bundesliga', '2023-08-01', '2023-09-27', 0,'user');


go

INSERT INTO Teams (TeamID, TeamName, Abbreviation, Logo) VALUES
('TE001', 'Manchester United', 'MNU', NULL),
('TE002', 'Real Madrid', 'RMD', NULL),
('TE003', 'Bayern Munich', 'FCB', NULL),
('TE004', 'Juventus', 'JUV', NULL),
('TE005', 'Paris Saint-Germain', 'PSG', NULL),
('TE006', 'Ajax', 'AJA', NULL),
('TE007', 'FC Porto', 'POR', NULL),
('TE008', 'Club Brugge','CLB', NULL),
('TE009', 'Galatasaray', 'GAL', NULL);

go

INSERT INTO LeagueTeams (LeagueID, TeamID)
VALUES 
('LG001', 'TE001'),
('LG001', 'TE002'),
('LG001', 'TE003'),
('LG001', 'TE004'),
('LG001', 'TE005'),
('LG001', 'TE006'),
('LG002', 'TE001'),
('LG002', 'TE003'),
('LG002', 'TE005'),
('LG002', 'TE006'),
('LG003', 'TE002'),
('LG003', 'TE003');

