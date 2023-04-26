-- drop DATABASE QLGD
CREATE DATABASE QLGD;
go
USE QLGD;
go

CREATE TABLE Account (
    Username VARCHAR(50) PRIMARY KEY,
    Password VARCHAR(50) NOT NULL,
    Roles BIT NOT NULL DEFAULT 0
);
select * from Account
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

SELECT * FROM Teams 
WHERE (TeamName = 'Manchester United' OR abbreviation = 'MNU') 
AND (TeamName NOT IN ('Manchester United') OR abbreviation NOT IN ('MNU'))
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
delete from Standings
ALTER TABLE Standings
ALTER COLUMN GoalsAgainst INT NOT NULL DEFAULT 0;
GO 
CREATE PROC USP_Login @userName varchar(50), @pass varchar(50)
AS	
BEGIN
    SELECT * FROM dbo.Account WHERE Username = @userName AND Password = @pass
END
go
INSERT INTO Account VALUES('a', '1962026656160185351301320480154111117132155',1)
INSERT INTO Account VALUES('b', '1962026656160185351301320480154111117132155',0)
go

INSERT INTO Leagues (LeagueID, LeagueName, StartDate, EndDate, NumberOfTeams, Username) VALUES
('LG001', 'Premier League', '2022-08-01', '2023-05-31', 6,'b'),
('LG002', 'La Liga', '2022-08-01', '2023-05-31', 4,'b'),
('LG003', 'Bundesliga', '2022-08-01', '2023-05-31', 6,'b'),
('LG004', 'Serie A', '2022-08-01', '2023-05-31', 6,'b'),
('LG005', 'Ligue 1', '2022-08-01', '2023-05-31', 6,'b'),
('LG006', 'Eredivisie', '2022-08-01', '2023-05-31', 6,'b'),
('LG007', 'Primeira Liga', '2022-08-01', '2023-05-31', 6,'b'),
('LG008', 'Jupiler Pro League', '2022-08-01', '2023-05-31', 4,'b'),
('LG009', 'Super Lig', '2022-08-01', '2023-05-31', 4,'b'),
('LG010', 'MLS', '2022-03-01', '2022-12-11', 4,'b');

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
('LG003', 'TE003'),
('LG004', 'TE003'),
('LG004', 'TE004'),
('LG005', 'TE005'),
('LG005', 'TE006');

go

alter PROC USP_PaginationMatches @numPages int, @rowPerPages int, @leagueID varchar(10)
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
exec USP_PaginationMatches 2, 2, 'LG006'
select * from Matches where LeagueID = 'LG006'
go

CREATE TRIGGER TRG_LeagueTeam ON LeagueTeams
AFTER INSERT, DELETE
AS
BEGIN
  UPDATE Leagues
  SET NumberOfTeams = (SELECT COUNT(*) FROM LeagueTeams WHERE LeagueID = Leagues.LeagueID)
  WHERE LeagueID IN (SELECT LeagueID FROM inserted UNION SELECT LeagueID FROM deleted)
END

delete from Matches
select * from Matches where LeagueID = 'LG002'
delete from Matches

delete from Standings

