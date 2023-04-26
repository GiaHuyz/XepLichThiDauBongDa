using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using XepLichThiDauBongDa.DAO;
using XepLichThiDauBongDa.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Image = System.Drawing.Image;

namespace XepLichThiDauBongDa
{
    public partial class fAdmin : Form
    {
        public Account acc;
        private int totalPages = 0;
        private int rowPerPages = 0;
        public fAdmin()
        {
            InitializeComponent();
        }

        #region Function Leagues
        void LoadLeagues()
        {
            grdLeagues.DataSource = LeaguesDAO.Instance.GetDataLeagues();
            cbbLeaguesNameTeams.DataSource = LeaguesDAO.Instance.GetDataLeagues();
            cbbLeaguesNameTeams.DisplayMember = "LeagueName";
            cbbLeaguesNameTeams.ValueMember = "LeagueID";
            grdLeagues.DataSource = LeaguesDAO.Instance.GetDataLeagues();
            if (grdLeagues.Rows.Count > 1)
            {
                grdLeagues_CellClick(grdLeagues, new DataGridViewCellEventArgs(0, 0));
            }
            cbbLeaguesNameMatch.DataSource = LeaguesDAO.Instance.GetDataLeagues();
            cbbLeaguesNameMatch.DisplayMember = "LeagueName";
            cbbLeaguesNameMatch.ValueMember = "LeagueID";
            cbbLeagueStandings.DataSource = LeaguesDAO.Instance.GetDataLeagues();
            cbbLeagueStandings.DisplayMember = "LeagueName";
            cbbLeagueStandings.ValueMember = "LeagueID";
        }

        void GenerateIDLegues()
        {
            txtIDLeagues.Text = LeaguesDAO.Instance.GenerateID();
        }

        bool InsertLeagues(string id, string name, DateTime startDate, DateTime endDate, string username)
        {
            return LeaguesDAO.Instance.InsertLeagues(id, name, startDate, endDate, username);
        }

        bool UpdateLeagues(string id, string name, DateTime startDate, DateTime endDate)
        {
            return LeaguesDAO.Instance.UpdateLeagues(id, name, startDate, endDate);
        }
        #endregion

        #region Function Teams
        void LoadCbbTeams()
        {
            cbbTeams.DataSource = TeamsDAO.Instance.GetDataTeams();
            cbbTeams.DisplayMember = "TeamName";
            cbbTeams.ValueMember = "TeamID";
        }

        void GenerateIDTeams()
        {
            txtIDTeam.Text = TeamsDAO.Instance.GenerateID();
        }

        bool InsertTeams(string id, string name, string abbreviation,string leagueID , byte[] logo)
        {
            return TeamsDAO.Instance.InsertTeams(id, name, abbreviation, leagueID, logo);
        }

        bool UpdateTeams(string id, string name, string abbreviation, byte[] logo)
        {
            return TeamsDAO.Instance.UpdateTeams(id, name, abbreviation, logo);
        }

        void LoadTeamsByLeagueID(string leagueID)
        {
            grdTeams.DataSource = TeamsDAO.Instance.GetTeamsByLeagueID(leagueID);
            if (grdTeams.Rows.Count > 1)
            {
                txtLeguesIDTeams.Text = cbbLeaguesNameTeams.GetItemText(cbbLeaguesNameTeams.SelectedValue);
                DataGridViewImageColumn img = new DataGridViewImageColumn();
                img = (DataGridViewImageColumn)grdTeams.Columns["Logo"];
                img.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            txtNumOfTeams.Text = TeamsDAO.Instance.CountTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString()).ToString();
        }
        #endregion

        void LoadStandingsByLeagueID(string leagueID)
        {
            grdStandings.DataSource = StandingsDAO.Instance.GetDataStandingsByLeagueID(leagueID);
            if (grdStandings.Rows.Count > 1)
            {
                DataGridViewImageColumn Logo = new DataGridViewImageColumn();
                Logo = (DataGridViewImageColumn)grdStandings.Columns["Logo"];
                Logo.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
        }

        private List<Teams> GetAllTeamsByLeagueID(string id)
        {
            return TeamsDAO.Instance.GetAllTeamsByLeagueID(id);
        }

        void LoadMatchesByNumPage(int numPage, int rowPerPages, string leagueID)
        {
            grdMatches.DataSource = MatchesDAO.Instance.LoadMatchesByNumPage(numPage, rowPerPages, leagueID);
            if (grdMatches.Rows.Count > 0)
            {
                SetImageColumn();
                grdMatches.Columns[1].Visible = false;
                grdMatches.Columns[2].Visible = false;
            }
        }

        void SetImageColumn()
        {
            DataGridViewImageColumn homeLogo = new DataGridViewImageColumn();
            homeLogo = (DataGridViewImageColumn)grdMatches.Columns["HomeTeamLogo"];
            homeLogo.ImageLayout = DataGridViewImageCellLayout.Zoom;
            DataGridViewImageColumn awayLogo = new DataGridViewImageColumn();
            awayLogo = (DataGridViewImageColumn)grdMatches.Columns["AwayTeamLogo"];
            awayLogo.ImageLayout = DataGridViewImageCellLayout.Zoom;
        }

        void LoadAcc()
        {
            grdAcc.DataSource = AccountDAO.Instance.GetDataAcc();
            if(grdAcc.Rows.Count > 1)
            {
                grdAcc.CurrentCell = grdAcc.Rows[0].Cells[0];
                grdAcc_CellClick(grdMatches, new DataGridViewCellEventArgs(0, 0));
            }
        }

        private void fAdmin_Load(object sender, EventArgs e)
        {
            pnInfoLeagues.Enabled = false;
            LoadLeagues();
            LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
            LoadCbbTeams();
            LoadStandingsByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
            LoadAcc();
            List<Teams> teams = GetAllTeamsByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
            int countTeams = teams.Count;
            totalPages = (countTeams - 1) * 2;
            rowPerPages = countTeams / 2;
            LoadMatchesByNumPage(1, rowPerPages, cbbLeaguesNameMatch.SelectedValue.ToString());
            if (grdMatches.Rows.Count > 1)
            {
                pnInfoMatch.Visible = true;
                pnPage.Visible = true;
                nmPage.Maximum = totalPages;
                nmPage.Minimum = 1;
                txtNumPage.Text = "Vòng " + nmPage.Value.ToString() + " / " + totalPages;
                grdMatches_CellClick_1(grdMatches, new DataGridViewCellEventArgs(0, 0));
            }
            else
            {
                pnInfoMatch.Visible = false;
                pnPage.Visible = false;
            }
            cbbTurn.SelectedIndex = 0;
            if (grdLeagues.Rows.Count < 1)
            {
                btnDeleteLeagues.Enabled = false;
                btnEditLeagues.Enabled = false;
                btnSaveLeagues.Enabled = false;
            }
            if(grdTeams.Rows.Count < 1)
            {
                btnDeleteTeam.Enabled = false;
                btnSaveTeam.Enabled = false;
                btnSaveTeam.Enabled = false;
            }
        }

        #region EventLeagues
        private void btnAddLeagues_Click(object sender, EventArgs e)
        {
            pnInfoLeagues.Enabled = true;
            dtpStart.Enabled = true;
            txtNameLeagues.Enabled = true;
            GenerateIDLegues();
            grdLeagues.ClearSelection();
            txtNameLeagues.Text = "";
            txtNameLeagues.Select();
            btnSaveLeagues.Enabled = true;
            btnEditLeagues.Enabled = false;
            btnDeleteLeagues.Enabled = false;
            dtpStart.Value = DateTime.Now;
            dtpkEnd.Value = DateTime.Now.AddDays(1);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtNameLeagues.Text == "")
            {
                MessageBox.Show("Chưa nhập tên giải đấu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dtpStart.Value < DateTime.Now.Date)
            {
                MessageBox.Show("Thời gian bắt đầu phải từ ngày hôm nay về sau !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dtpkEnd.Value <= dtpStart.Value)
            {
                MessageBox.Show("Thời gian kết thúc sau thời gian bắt đầu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (grdLeagues.SelectedCells.Count > 0)
            {
                DateTime maxDateMatches = MatchesDAO.Instance.GetMaxDate(grdLeagues.CurrentRow.Cells["LeagueID"].Value.ToString());
                if (dtpkEnd.Value.Date < maxDateMatches.Date)
                {
                    MessageBox.Show($"Đã có trận diễn ra vào ngày {maxDateMatches}. Ngày kết thúc phải sau ngày này !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                UpdateLeagues(txtIDLeagues.Text, txtNameLeagues.Text, dtpStart.Value.Date, dtpkEnd.Value.Date);
                LoadLeagues();
            }
            else if (InsertLeagues(txtIDLeagues.Text, txtNameLeagues.Text, dtpStart.Value.Date, dtpkEnd.Value.Date, acc.Username))
            {
                LoadLeagues();
                grdLeagues.ClearSelection();
                txtNameLeagues.Select();
                GenerateIDLegues();
            }
        }

        private void grdLeagues_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pnInfoLeagues.Enabled = false;
            btnDeleteLeagues.Enabled = true;
            btnEditLeagues.Enabled = true;
            btnSaveLeagues.Enabled = false;
            if (grdLeagues.Rows.Count > 1)
            {
                txtIDLeagues.Text = grdLeagues.CurrentRow.Cells["LeagueID"].Value.ToString();
                txtNameLeagues.Text = grdLeagues.CurrentRow.Cells["LeagueName"].Value.ToString();
                dtpStart.Value = Convert.ToDateTime(grdLeagues.CurrentRow.Cells["StartDate"].Value);
                dtpkEnd.Value = Convert.ToDateTime(grdLeagues.CurrentRow.Cells["EndDate"].Value);
            }
        }

        private void btnEditLeagues_Click(object sender, EventArgs e)
        {
            if(grdLeagues.CurrentRow.Cells["Username"].Value.ToString() != acc.Username)
            {
                return;
            }
            List<(DateTime, DateTime)> dateLeague = LeaguesDAO.Instance.GetDateLeague(grdLeagues.CurrentRow.Cells["LeagueID"].Value.ToString());
            if (DateTime.Now.Date >= dateLeague[0].Item1 && DateTime.Now.Date <= dateLeague[0].Item2)
            {
                pnInfoLeagues.Enabled = true;
                dtpStart.Enabled = false;
                txtNameLeagues.Enabled = false;
                btnSaveLeagues.Enabled = true;
                return;
            }
            pnInfoLeagues.Enabled = true;
            dtpStart.Enabled = true;
            txtNameLeagues.Enabled = true;
            txtNameLeagues.Select();
            btnSaveLeagues.Enabled = true;
        }
        #endregion

        #region EventTeams

        private void btnAddTeam_Click(object sender, EventArgs e)
        {
            if(cbbLeaguesNameTeams.SelectedValue == null)
            {
                return;
            }
            if(!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameTeams.SelectedValue.ToString()))
            {
                return;
            }
            List<(DateTime, DateTime)> dateLeague = LeaguesDAO.Instance.GetDateLeague(cbbLeaguesNameTeams.SelectedValue.ToString());
            if (DateTime.Now.Date >= dateLeague[0].Item1 && DateTime.Now.Date <= dateLeague[0].Item2)
            {
                MessageBox.Show("Giải đấu đang diễn ra không thể chỉnh sửa !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pnInfoTeams.Enabled = true;
            txtIDTeam.Enabled = true;
            txtNameTeam.Enabled = true;
            txtAbbreviationTeam.Enabled = true;
            ptbLogo.Enabled = true;
            btnBrowseImage.Enabled = true;
            GenerateIDTeams();
            grdTeams.ClearSelection();
            txtNameTeam.Text = "";
            txtAbbreviationTeam.Text = "";
            ptbLogo.Image = null;
            txtNameTeam.Select();
            btnSaveTeam.Enabled = true;
            btnEditTeam.Enabled = false;
            btnDeleteTeam.Enabled = false;
        }

        private void btnEditTeam_Click(object sender, EventArgs e)
        {
            if(cbbLeaguesNameTeams.SelectedValue == null)
            {
                return;
            }
            if (!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameTeams.SelectedValue.ToString()))
            {
                return;
            }
            List<(DateTime, DateTime)> dateLeague = LeaguesDAO.Instance.GetDateLeague(cbbLeaguesNameTeams.SelectedValue.ToString());
            if (DateTime.Now.Date >= dateLeague[0].Item1 && DateTime.Now.Date <= dateLeague[0].Item2)
            {
                MessageBox.Show("Giải đấu đang diễn ra không thể chỉnh sửa !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pnInfoTeams.Enabled = true;
            txtNameTeam.Enabled = true;
            txtAbbreviationTeam.Enabled = true;
            ptbLogo.Enabled = true;
            btnBrowseImage.Enabled = true;
            txtNameTeam.Select();
            btnSaveTeam.Enabled = true;
        }

        private void btnDeleteTeam_Click(object sender, EventArgs e)
        {
            if (cbbLeaguesNameTeams.SelectedValue == null)
            {
                return;
            }
            if (!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameTeams.SelectedValue.ToString()))
            {
                return;
            }
            List<(DateTime, DateTime)> dateLeague = LeaguesDAO.Instance.GetDateLeague(cbbLeaguesNameTeams.SelectedValue.ToString());
            if (DateTime.Now.Date >= dateLeague[0].Item1 && DateTime.Now.Date <= dateLeague[0].Item2)
            {
                MessageBox.Show("Giải đấu đang diễn ra không thể chỉnh sửa !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataTable dt = MatchesDAO.Instance.GetDataMatchesByLeagueId(cbbLeaguesNameTeams.SelectedValue.ToString());
            if (dt.Rows.Count > 1)
            {
                DialogResult dialogResult = MessageBox.Show($"Tất cả trận đấu và thành tích của đội {txtNameTeam.Text} đều sẽ bị xoá. Bạn có chắc không ? ", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.OK)
                {
                    TeamsDAO.Instance.DeleteTeam(txtIDTeam.Text);
                    List<Teams> teams = GetAllTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
                    int count = teams.Count;
                    totalPages = (count - 1) * 2;
                    rowPerPages = count / 2;
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show($"Bạn có chắc muốn xoá đội {txtNameTeam.Text} ? ", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.OK)
                {
                    TeamsDAO.Instance.DeleteTeam(txtIDTeam.Text);
                    LoadCbbTeams();
                    LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
                }
            }
        }

        private void btnSaveTeam_Click(object sender, EventArgs e)
        {
            byte[] image = { 0 };
            if (ptbLogo.Image != null)
            {
                ImageConverter imageConverter = new ImageConverter();
                image = (byte[])imageConverter.ConvertTo(ptbLogo.Image, typeof(byte[]));
            }
            if (txtNameTeam.Text == "")
            {
                MessageBox.Show("Chưa nhập tên đội bóng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txtAbbreviationTeam.Text == "")
            {
                MessageBox.Show("Chưa nhập tên viết tắt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (grdTeams.SelectedCells.Count > 0)
            {
                if (TeamsDAO.Instance.CheckDuplicateUpdateTeamName(txtNameTeam.Text, txtAbbreviationTeam.Text))
                {
                    MessageBox.Show("Trùng tên đội bóng hoặc tên viết tắt !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                UpdateTeams(txtIDTeam.Text, txtNameTeam.Text, txtAbbreviationTeam.Text, image);
                LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
            }
            else
            {
                if (TeamsDAO.Instance.CheckDuplicateTeamName(txtNameTeam.Text, txtAbbreviationTeam.Text))
                {
                    MessageBox.Show("Trùng tên đội bóng hoặc tên viết tắt !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                InsertTeams(txtIDTeam.Text, txtNameTeam.Text, txtAbbreviationTeam.Text, cbbLeaguesNameTeams.SelectedValue.ToString(), image);
                LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
                LoadCbbTeams();
                grdTeams.ClearSelection();
                txtNameTeam.Select();
                GenerateIDTeams();
            }
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog() { Filter = "Image files(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png", Multiselect = false })
            {
                if(openFile.ShowDialog() == DialogResult.OK)
                {
                    ptbLogo.Image = Image.FromFile(openFile.FileName);
                }
            }
        }

        private void grdTeams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pnInfoTeams.Enabled = true;
            txtIDTeam.Enabled = false;
            txtNameTeam.Enabled = false;
            txtAbbreviationTeam.Enabled = false;
            ptbLogo.Enabled = false;
            btnBrowseImage.Enabled = false;
            btnDeleteTeam.Enabled = true;
            btnEditTeam.Enabled = true;
            btnSaveTeam.Enabled = false;
            try
            {
                if (grdTeams.Rows.Count > 1)
                {
                    txtIDTeam.Text = grdTeams.CurrentRow.Cells["TeamID"].Value.ToString();
                    txtNameTeam.Text = grdTeams.CurrentRow.Cells["TeamName"].Value.ToString();
                    txtAbbreviationTeam.Text = grdTeams.CurrentRow.Cells["Abbreviation"].Value.ToString();
                }
                MemoryStream ms = new MemoryStream((byte[])grdTeams.CurrentRow.Cells["Logo"].Value);
                ptbLogo.Image = Image.FromStream(ms);
            }
            catch (Exception)
            {
                ptbLogo.Image = null;
            }
        }

        private void grdTeams_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion

        private void cbbLeaguesNameTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbbLeaguesNameTeams.SelectedValue == null)
            {
                grdTeams.DataSource = null;
                return;
            }
            LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
            if (grdTeams.Rows.Count > 1)
            {
                grdTeams_CellClick(grdTeams, new DataGridViewCellEventArgs(0, 0));
            }
            else
            {
                txtIDTeam.Text = string.Empty;
                txtNameTeam.Text = string.Empty;
                txtAbbreviationTeam.Text = string.Empty;
                ptbLogo.Image = null;
            }
        }

        private void txtLeguesIDTeams_TextChanged(object sender, EventArgs e)
        {
            string leagueID = txtLeguesIDTeams.Text;
            if(leagueID == "") { 
                cbbLeaguesNameTeams.SelectedItem = null;
            } 
            foreach (DataRowView item in cbbLeaguesNameTeams.Items)
            {
                if (item["LeagueID"].ToString() == leagueID)
                {
                    cbbLeaguesNameTeams.SelectedItem = item;
                    return;
                }
            }
        }

        private void tcAdmin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcAdmin.SelectedIndex == 1)
            {
                pnInfoTeams.Enabled = true;
                txtIDTeam.Enabled = false;
                txtNameTeam.Enabled = false;
                txtAbbreviationTeam.Enabled = false;
                ptbLogo.Enabled = false;
                btnBrowseImage.Enabled = false;
                LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
                grdTeams_CellClick(grdTeams, new DataGridViewCellEventArgs(0, 0));
            }
            if (tcAdmin.SelectedIndex == 2)
            {
                grdMatches.ClearSelection();
                LoadMatchesByNumPage(1, rowPerPages, cbbLeaguesNameMatch.SelectedValue.ToString());
                grdMatches_CellClick(grdMatches, new DataGridViewCellEventArgs(0, 0));
            }
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            fChangePass f = new fChangePass();
            f.acc = acc;
            f.ShowDialog();
        }

        private void btnDeleteLeagues_Click(object sender, EventArgs e)
        {
            if (grdLeagues.CurrentRow.Cells["Username"].Value.ToString() != acc.Username)
            {
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Tất cả dữ liệu của giải đấu sẽ bị xoá. Bạn có chắc không ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                LeaguesDAO.Instance.DeleteLeague(grdLeagues.CurrentRow.Cells["LeagueID"].Value.ToString());
                LoadLeagues();
            }
        }

        private void btnSearchLeagues_Click(object sender, EventArgs e)
        {
            if (txtSearchLeagues.Text == "")
            {
                LoadLeagues();
            }
            else
            {
                grdLeagues.DataSource = LeaguesDAO.Instance.SearchLeagues(txtSearchLeagues.Text);
            }
        }

        private void btnAddTeamCbb_Click(object sender, EventArgs e)
        {
            if (cbbLeaguesNameTeams.SelectedValue == null)
            {
                return;
            }
            List<(DateTime, DateTime)> dateLeague = LeaguesDAO.Instance.GetDateLeague(cbbLeaguesNameTeams.SelectedValue.ToString());
            if (DateTime.Now.Date >= dateLeague[0].Item1 && DateTime.Now.Date <= dateLeague[0].Item2)
            {
                MessageBox.Show("Giải đấu đang diễn ra không thể chỉnh sửa !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (TeamsDAO.Instance.CheckTeamInLeague(cbbLeaguesNameTeams.SelectedValue.ToString(), cbbTeams.SelectedValue.ToString()))
            {
                MessageBox.Show("Đội bóng đã có trong giải đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TeamsDAO.Instance.InsertLeagueTeam(cbbLeaguesNameTeams.SelectedValue.ToString(), cbbTeams.SelectedValue.ToString());
            LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            fLogin f = new fLogin();
            f.ShowDialog();
            this.Close();
        }

        #region Function Matches
        string GenerateMatchesId()
        {
            return MatchesDAO.Instance.GenerateID();
        }
        bool InsertMatches(string id, string homeTeamId, string awayTeamId, string leagueId, string turn)
        {
            return MatchesDAO.Instance.InsertMatches(id, homeTeamId, awayTeamId, leagueId, turn);
        }


        bool UpdateMatch(int homeScore, int awayScore, string matchID)
        {
            return MatchesDAO.Instance.UpdateMatch(homeScore, awayScore, matchID);
        }

        bool UpdateStandings(string teamID, string leagueID, int point, int win, int draw, int lose, int goalsFor, int goalAgainst)
        {
            return StandingsDAO.Instance.UpdateStandings(teamID, leagueID, point, win, draw, lose, goalsFor, goalAgainst);
        }
        #endregion

        private void btnSearchMatch_Click_1(object sender, EventArgs e)
        {
            if (txtSearchMatch.Text != "")
            {
                grdMatches.DataSource = MatchesDAO.Instance.SearchMatch(txtSearchMatch.Text, cbbLeaguesNameMatch.SelectedValue.ToString());
            }
        }

        private void btnUpdateMatchDate_Click_1(object sender, EventArgs e)
        {
            if (!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameMatch.SelectedValue.ToString()))
            {
                return;
            }
            string leagueID = cbbLeaguesNameMatch.SelectedValue.ToString();
            List<(DateTime, DateTime)> date = LeaguesDAO.Instance.GetDateLeague(leagueID);
            if (dtpkMatchDate.Value.Date < date[0].Item1.Date || dtpkMatchDate.Value.Date > date[0].Item2.Date)
            {
                MessageBox.Show("Thời gian trận đấu đã vượt ra ngoài giải đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (grdMatches.CurrentRow.Cells["Turn"].Value.ToString() == "Away" && MatchesDAO.Instance.CheckDateHomeTurn(leagueID))
            {
                MessageBox.Show("Hãy cập nhất thời gian của tất cả trận lượt đi !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((grdMatches.CurrentRow.Cells["Turn"].Value.ToString() == "Away" && dtpkMatchDate.Value < MatchesDAO.Instance.GetMaxDateHomeTurn(leagueID)) || (grdMatches.CurrentRow.Cells["Turn"].Value.ToString() == "Home" && dtpkMatchDate.Value > MatchesDAO.Instance.GetMinDateAwayTurn(leagueID)))
            {
                MessageBox.Show("Lượt đi phải sau lượt về !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int selectedRowIndex = grdMatches.CurrentRow.Index;
            MatchesDAO.Instance.UpdateMatchDate(dtpkMatchDate.Value, grdMatches.CurrentRow.Cells["MatchID"].Value.ToString());
            LoadMatchesByNumPage((int)nmPage.Value, rowPerPages, leagueID);
            grdMatches.CurrentCell = grdMatches.Rows[selectedRowIndex].Cells[0];
            grdMatches_CellClick(grdMatches, new DataGridViewCellEventArgs(0, selectedRowIndex));
        }

        private void btnResult_Click_1(object sender, EventArgs e)
        {
            if (!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameTeams.SelectedValue.ToString()))
            {
                return;
            }
            if (grdMatches.CurrentRow.Cells["MatchDate"].Value.ToString() == "")
            {
                MessageBox.Show("Chưa cập nhật thời gian trận đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Convert.ToDateTime(grdMatches.CurrentRow.Cells["MatchDate"].Value) > DateTime.Now)
            {
                MessageBox.Show("Chưa tới ngày thi đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Sau khi tính điểm sẽ không thể sửa được nữa. Bạn có chắc không ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                int selectedRowIndex = grdMatches.CurrentRow.Index;
                UpdateMatch((int)nmPointHome.Value, (int)nmPointAway.Value, grdMatches.CurrentRow.Cells["MatchID"].Value.ToString());
                string homeTeamID = grdMatches.CurrentRow.Cells["HomeTeamID"].Value.ToString();
                string awayTeamID = grdMatches.CurrentRow.Cells["AwayTeamID"].Value.ToString();
                string leagueID = cbbLeaguesNameMatch.SelectedValue.ToString();
                int goalHome = (int)nmPointHome.Value;
                int goalAway = (int)nmPointAway.Value;
                List<(int, int, int, int, int, int)> pointHomeTeam = StandingsDAO.Instance.GetPointStandingsByTeamIDAndLeagueID(homeTeamID, leagueID);
                List<(int, int, int, int, int, int)> pointAwayTeam = StandingsDAO.Instance.GetPointStandingsByTeamIDAndLeagueID(awayTeamID, leagueID);
                if (goalHome > goalAway)
                {
                    UpdateStandings(homeTeamID, leagueID, pointHomeTeam[0].Item1 + 3, pointHomeTeam[0].Item2 + 1, pointHomeTeam[0].Item3, pointHomeTeam[0].Item4, pointHomeTeam[0].Item5 + goalHome, pointHomeTeam[0].Item6 + goalAway);
                    UpdateStandings(awayTeamID, leagueID, pointAwayTeam[0].Item1, pointAwayTeam[0].Item2, pointAwayTeam[0].Item3, pointAwayTeam[0].Item4 + 1, pointAwayTeam[0].Item5 + goalAway, pointAwayTeam[0].Item6 + goalHome);
                }
                else if (goalHome < goalAway)
                {
                    UpdateStandings(homeTeamID, leagueID, pointHomeTeam[0].Item1, pointHomeTeam[0].Item2, pointHomeTeam[0].Item3, pointHomeTeam[0].Item4 + 1, pointHomeTeam[0].Item5 + goalHome, pointHomeTeam[0].Item6 + goalAway);
                    UpdateStandings(awayTeamID, leagueID, pointAwayTeam[0].Item1 + 3, pointAwayTeam[0].Item2 + 1, pointAwayTeam[0].Item3, pointAwayTeam[0].Item4, pointAwayTeam[0].Item5 + goalAway, pointAwayTeam[0].Item6 + goalHome);
                }
                else
                {
                    if (goalHome == 0 && goalAway == 0)
                    {
                        UpdateStandings(homeTeamID, leagueID, pointHomeTeam[0].Item1, pointHomeTeam[0].Item2, pointHomeTeam[0].Item3 + 1, pointHomeTeam[0].Item4, pointHomeTeam[0].Item5, pointHomeTeam[0].Item6);
                        UpdateStandings(awayTeamID, leagueID, pointAwayTeam[0].Item1, pointAwayTeam[0].Item2, pointAwayTeam[0].Item3 + 1, pointAwayTeam[0].Item4, pointAwayTeam[0].Item5, pointAwayTeam[0].Item6);
                    }
                    else
                    {
                        UpdateStandings(homeTeamID, leagueID, pointHomeTeam[0].Item1 + 1, pointHomeTeam[0].Item2, pointHomeTeam[0].Item3 + 1, pointHomeTeam[0].Item4, pointHomeTeam[0].Item5 + goalHome, pointHomeTeam[0].Item6 + goalAway);
                        UpdateStandings(awayTeamID, leagueID, pointAwayTeam[0].Item1 + 1, pointAwayTeam[0].Item2, pointAwayTeam[0].Item3 + 1, pointAwayTeam[0].Item4, pointAwayTeam[0].Item5 + goalAway, pointAwayTeam[0].Item6 + goalHome);
                    }
                }
                LoadMatchesByNumPage((int)nmPage.Value, rowPerPages, leagueID);
                LoadStandingsByLeagueID(leagueID);
                grdMatches.CurrentCell = grdMatches.Rows[selectedRowIndex].Cells[0];
                grdMatches_CellClick(grdMatches, new DataGridViewCellEventArgs(0, selectedRowIndex));
                this.ActiveControl = null;
            }
        }

        private void btnDeleteMatches_Click_1(object sender, EventArgs e)
        {
            if (!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameMatch.SelectedValue.ToString()))
            {
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xoá tất cả lịch thi đấu ? Bảng xếp hạng cũng sẽ bị xoá.", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                MatchesDAO.Instance.DeleteMatchesByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
                grdMatches.DataSource = null;
                pnInfoMatch.Visible = false;
                pnPage.Visible = false;
                LoadStandingsByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
            }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            if (grdMatches.Rows.Count > 0)
            {
                btnShowAllMatches.PerformClick();
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = cbbLeaguesNameMatch.GetItemText(cbbLeaguesNameMatch.SelectedItem).Trim() + ".pdf";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("Không thể ghi dữ liệu vào đĩa." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            PdfPTable pdfTable = new PdfPTable(grdMatches.Columns.Count - 2);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            foreach (DataGridViewColumn column in grdMatches.Columns)
                            {
                                if (column.Name != "TeamID" &&
                                    column.Name != "TeamID1")
                                {
                                    PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                    pdfTable.AddCell(cell);
                                }
                            }

                            foreach (DataGridViewRow row in grdMatches.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    if (cell.OwningColumn.Name == "HomeTeamLogo" ||
                                        cell.OwningColumn.Name == "AwayTeamLogo")
                                    {
                                        byte[] imgBytes = (byte[])cell.Value;
                                        ImageConverter imgConverter = new ImageConverter();
                                        Image img = (Image)imgConverter.ConvertFrom(imgBytes);
                                        iTextSharp.text.Image pdfImg = iTextSharp.text.Image.GetInstance(img,               System.Drawing.Imaging.ImageFormat.Png);
                                        PdfPCell imgCell = new PdfPCell(pdfImg, true);
                                        pdfTable.AddCell(imgCell);
                                    }
                                    else if (cell.OwningColumn.Name != "TeamID" &&
                                        cell.OwningColumn.Name != "TeamID1")
                                    {
                                        pdfTable.AddCell(cell.Value.ToString());
                                    }
                                }
                            }

                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();

                            }

                            MessageBox.Show("Xuất file thành công !", "Info");
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Không có dòng nào để xuất file !", "Info");
            }
        }

        private void btnShowAllMatches_Click_1(object sender, EventArgs e)
        {
            grdMatches.DataSource = MatchesDAO.Instance.LoadAllMatchesByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
            if (grdMatches.Rows.Count > 0)
            {
                grdMatches.Columns[1].Visible = false;
                grdMatches.Columns[2].Visible = false;
            }
        }

        private void btnPrintStandings_Click(object sender, EventArgs e)
        {
            if (grdStandings.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Standing " + cbbLeagueStandings.GetItemText(cbbLeagueStandings.SelectedItem).Trim() + ".pdf";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("Không thể ghi dữ liệu vào đĩa." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            PdfPTable pdfTable = new PdfPTable(grdStandings.Columns.Count);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            foreach (DataGridViewColumn column in grdStandings.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                pdfTable.AddCell(cell);
                            }

                            foreach (DataGridViewRow row in grdStandings.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    if (cell.OwningColumn.Name == "Logo")
                                    {
                                        byte[] imgBytes = (byte[])cell.Value;
                                        ImageConverter imgConverter = new ImageConverter();
                                        Image img = (Image)imgConverter.ConvertFrom(imgBytes);
                                        iTextSharp.text.Image pdfImg = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Png);
                                        PdfPCell imgCell = new PdfPCell(pdfImg, true);
                                        pdfTable.AddCell(imgCell);
                                    }
                                    else
                                    {
                                        pdfTable.AddCell(cell.Value.ToString());
                                    }
                                }
                            }

                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("Xuất file thành công !", "Info");
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Không có dòng nào để xuất file !", "Info");
            }
        }

        private void btnSearchStandings_Click(object sender, EventArgs e)
        {
            if (txtSearchStandings.Text == "")
            {
                LoadStandingsByLeagueID(cbbLeagueStandings.SelectedValue.ToString());
            }
            else
            {
                grdStandings.DataSource = StandingsDAO.Instance.SearchStandings(txtSearchStandings.Text, cbbLeagueStandings.SelectedValue.ToString());
            }
        }

        private void txtGenerateMatch_Click_1(object sender, EventArgs e)
        {
            if (!LeaguesDAO.Instance.CheckLeagueAdmin(acc.Username, cbbLeaguesNameMatch.SelectedValue.ToString()))
            {
                return;
            }
            string leagueID = cbbLeaguesNameMatch.SelectedValue.ToString();
            int count = TeamsDAO.Instance.CountTeamsByLeagueID(leagueID);
            if (grdMatches.Rows.Count > 1)
            {
                MessageBox.Show("Đã có lịch thi đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (count % 2 != 0 || count < 1)
            {
                MessageBox.Show("Số lượng bóng phải là số chẵn !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<Teams> teams = GetAllTeamsByLeagueID(leagueID);
            List<(string, string)> allFirstLegMatches = new List<(string, string)>();
            int countTeams = teams.Count;
            int numberOfRounds = countTeams - 1;
            int matchesPerRound = countTeams / 2;
            totalPages = numberOfRounds * 2;
            rowPerPages = matchesPerRound;

            for (int i = 0; i < numberOfRounds; i++)
            {
                for (int j = 0; j < matchesPerRound; j++)
                {
                    int homeIndex = (i + j) % (teams.Count - 1);
                    int awayIndex = (teams.Count - 1 - j + i) % (teams.Count - 1);

                    if (j == 0)
                    {
                        awayIndex = teams.Count - 1;
                    }

                    string homeTeamID = teams[homeIndex].Id;
                    string awayTeamID = teams[awayIndex].Id;
                    allFirstLegMatches.Add((awayTeamID, homeTeamID));
                    string MatchesId = GenerateMatchesId();
                    try
                    {
                        InsertMatches(MatchesId, homeTeamID, awayTeamID, leagueID, "Home");
                    }
                    catch (Exception)
                    {
                        MatchesDAO.Instance.DeleteMatchesByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
                        MessageBox.Show("Có lỗi khi tạo giải đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            allFirstLegMatches.Reverse();
            foreach (var match in allFirstLegMatches)
            {
                string MatchesId = GenerateMatchesId();
                try
                {
                    InsertMatches(MatchesId, match.Item1, match.Item2, leagueID, "Away");
                }
                catch (Exception)
                {
                    MatchesDAO.Instance.DeleteMatchesByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
                    MessageBox.Show("Có lỗi khi tạo giải đấu !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            pnInfoMatch.Visible = true;
            pnPage.Visible = true;
            nmPage.Maximum = totalPages;
            txtNumPage.Text = "Vòng 1 / " + totalPages;
            LoadMatchesByNumPage(1, rowPerPages, leagueID);
            foreach (var team in teams)
            {
                var standingsID = StandingsDAO.Instance.GenerateID();
                StandingsDAO.Instance.InsertStandings(standingsID, leagueID, team.Id, 0, 0, 0, 0);
            }
            LoadStandingsByLeagueID(leagueID);
            DataGridViewImageColumn Logo = new DataGridViewImageColumn();
            Logo = (DataGridViewImageColumn)grdStandings.Columns["Logo"];
            Logo.ImageLayout = DataGridViewImageCellLayout.Zoom;
            grdMatches_CellClick(grdMatches, new DataGridViewCellEventArgs(0, 0));
        }

        private void cbbLeaguesNameMatch_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            List<Teams> teams = GetAllTeamsByLeagueID(cbbLeaguesNameMatch.SelectedValue.ToString());
            int count = teams.Count;
            totalPages = (count - 1) * 2;
            rowPerPages = count / 2;
            txtNumPage.Text = "Vòng 1 / " + totalPages;
            nmPage.Maximum = totalPages;
            LoadMatchesByNumPage(1, rowPerPages, cbbLeaguesNameMatch.SelectedValue.ToString());
            if (grdMatches.Rows.Count < 1)
            {
                ptbAway.Image = null;
                ptbHome.Image = null;
                pnInfoMatch.Visible = false;
                pnPage.Visible = false;
            }
            else if (grdMatches.Rows.Count > 1)
            {
                grdMatches_CellClick(grdMatches, new DataGridViewCellEventArgs(0, 0));
                pnInfoMatch.Visible = true;
                pnPage.Visible = true;
            }
        }

        private void cbbLeagueStandings_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStandingsByLeagueID(cbbLeagueStandings.SelectedValue.ToString());
        }

        private void btnSearchTeam_Click(object sender, EventArgs e)
        {
            if (txtSearchTeam.Text == "")
            {
                LoadTeamsByLeagueID(cbbLeaguesNameTeams.SelectedValue.ToString());
            }
            else
            {
                grdTeams.DataSource = TeamsDAO.Instance.SearchTeam(txtSearchTeam.Text, cbbLeaguesNameTeams.SelectedValue.ToString());
            }
        }

        private void grdMatches_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            btnDeleteTeam.Enabled = true;
            btnEditTeam.Enabled = true;
            try
            {
                if (grdMatches.CurrentRow != null)
                {
                    txtNameHomeTeam.Text = grdMatches.CurrentRow.Cells["HomeTeam"].Value.ToString();
                    txtNameAwayTeam.Text = grdMatches.CurrentRow.Cells["AwayTeam"].Value.ToString();
                    if (grdMatches.CurrentRow.Cells["MatchDate"].Value.ToString() != "")
                    {
                        dtpkMatchDate.CustomFormat = "dd/MM/yyyy HH:mm";
                        dtpkMatchDate.Value = Convert.ToDateTime(grdMatches.CurrentRow.Cells["MatchDate"].Value);
                    }
                    else
                    {
                        dtpkMatchDate.CustomFormat = " ";
                    }
                    MemoryStream ms1 = new MemoryStream((byte[])grdMatches.CurrentRow.Cells["HomeTeamLogo"].Value);
                    ptbHome.Image = Image.FromStream(ms1);
                    MemoryStream ms2 = new MemoryStream((byte[])grdMatches.CurrentRow.Cells["AwayTeamLogo"].Value);
                    ptbAway.Image = Image.FromStream(ms2);
                    if (grdMatches.CurrentRow.Cells["Status"].Value.ToString() == "Finished")
                    {
                        nmPointHome.Enabled = false;
                        nmPointAway.Enabled = false;
                        btnUpdateMatchDate.Enabled = false;
                        btnResult.Enabled = false;
                    }
                    else
                    {
                        nmPointHome.Enabled = true;
                        nmPointAway.Enabled = true;
                        btnUpdateMatchDate.Enabled = true;
                        btnResult.Enabled = true;
                    }
                }
            }
            catch (Exception)
            {
                if (Convert.IsDBNull(grdMatches.CurrentRow.Cells["AwayTeamLogo"].Value))
                {
                    ptbAway.Image = null;
                }
                if (Convert.IsDBNull(grdMatches.CurrentRow.Cells["HomeTeamLogo"].Value))
                {
                    ptbHome.Image = null;
                }
            }
        }

        private void nmPage_ValueChanged_1(object sender, EventArgs e)
        {
            if (grdMatches.Rows.Count > 0)
            {
                grdMatches.Columns[1].Visible = false;
                grdMatches.Columns[2].Visible = false;
            }
            if (nmPage.Value > totalPages / 2)
            {
                cbbTurn.SelectedIndex = 1;
            }
            else
            {
                cbbTurn.SelectedIndex = 0;
            }
            txtNumPage.Text = "Vòng " + nmPage.Value.ToString() + " / " + totalPages;
            LoadMatchesByNumPage((int)nmPage.Value, rowPerPages, cbbLeaguesNameMatch.SelectedValue.ToString());
            grdMatches_CellClick(grdMatches, new DataGridViewCellEventArgs(0, 0));

        }

        private void cbbTurn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbTurn.SelectedIndex == 1 && nmPage.Value < totalPages / 2)
            {
                nmPage.Value = totalPages / 2 + 1;
            }
            else if (cbbTurn.SelectedIndex == 0 && nmPage.Value > totalPages / 2)
            {
                nmPage.Value = 1;
            }
        }

        private void grdAcc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (grdAcc.CurrentRow != null)
            {
                txtUsername.Text = grdAcc.CurrentRow.Cells["Username"].Value.ToString();
                nmRoles.Value = (bool)grdAcc.CurrentRow.Cells["Roles"].Value == true ? 1 : 0;
            }
        }

        private void nmRoles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void nmPointHome_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void nmPointAway_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnDeleteAcc_Click(object sender, EventArgs e)
        {
            if (grdAcc.CurrentRow != null)
            {
                string username = grdAcc.CurrentRow.Cells["Username"].Value.ToString();
                DialogResult dialogResult = MessageBox.Show($"Bạn có chắc muốn xoá tài khoản {username} ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if(dialogResult == DialogResult.OK) 
                {
                    LeaguesDAO.Instance.DeleteLeagueByUsername(username);
                    LoadAcc();
                }
            }
        }

        private void btnEditAcc_Click(object sender, EventArgs e)
        {
            if(grdAcc.CurrentRow != null)
            {
                AccountDAO.Instance.UpdateRoleAccount((int)nmRoles.Value, grdAcc.CurrentRow.Cells["Username"].Value.ToString());
                LoadAcc();
            }
        }

        private void grdMatches_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDeleteTeam.Enabled = true;
            btnEditTeam.Enabled = true;
            try
            {
                if (grdMatches.CurrentRow != null)
                {
                    txtNameHomeTeam.Text = grdMatches.CurrentRow.Cells["HomeTeam"].Value.ToString();
                    txtNameAwayTeam.Text = grdMatches.CurrentRow.Cells["AwayTeam"].Value.ToString();
                    if (grdMatches.CurrentRow.Cells["MatchDate"].Value.ToString() != "")
                    {
                        dtpkMatchDate.CustomFormat = "dd/MM/yyyy HH:mm";
                        dtpkMatchDate.Value = Convert.ToDateTime(grdMatches.CurrentRow.Cells["MatchDate"].Value);
                    }
                    else
                    {
                        dtpkMatchDate.CustomFormat = " ";
                    }
                    MemoryStream ms1 = new MemoryStream((byte[])grdMatches.CurrentRow.Cells["HomeTeamLogo"].Value);
                    ptbHome.Image = Image.FromStream(ms1);
                    MemoryStream ms2 = new MemoryStream((byte[])grdMatches.CurrentRow.Cells["AwayTeamLogo"].Value);
                    ptbAway.Image = Image.FromStream(ms2);
                    if (grdMatches.CurrentRow.Cells["Status"].Value.ToString() == "Finished")
                    {
                        nmPointHome.Enabled = false;
                        nmPointAway.Enabled = false;
                        btnUpdateMatchDate.Enabled = false;
                        btnResult.Enabled = false;
                    }
                    else
                    {
                        nmPointHome.Enabled = true;
                        nmPointAway.Enabled = true;
                        btnUpdateMatchDate.Enabled = true;
                        btnResult.Enabled = true;
                    }
                }
            }
            catch (Exception)
            {
                if (Convert.IsDBNull(grdMatches.CurrentRow.Cells["AwayTeamLogo"].Value))
                {
                    ptbAway.Image = null;
                }
                if (Convert.IsDBNull(grdMatches.CurrentRow.Cells["HomeTeamLogo"].Value))
                {
                    ptbHome.Image = null;
                }
            }
        }
    }
}
