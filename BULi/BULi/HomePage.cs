using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BULi
{
    public partial class HomePage : Form
    {

        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nunez26*7;";


        private DataTable studentDataTable = new DataTable();

        // DB year_level value -> name expected by display.cs
        private readonly Dictionary<string, string> yearToDisplayName = new Dictionary<string, string>
        {
            { "1st Year", "Freshmen"   },
            { "2nd Year", "Sophomore"  },
            { "3rd Year", "Junior"     },
            { "4th Year", "Senior"     }
        };

        // Search suggestions (created in code, so no Designer changes needed)
        private ListBox lstSuggestions;
        private bool suppressSearchEvent = false;

        private class Suggestion
        {
            public string FullName { get; set; }
            public string YearLevel { get; set; }
            public override string ToString()
            {
                return FullName + "  (" + YearLevel + ")";
            }
        }

        public HomePage()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Subscribe to global live theme toggles
            ThemeState.ThemeChanged += OnThemeChanged;
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            // Apply initial theme styles for header label and search box on load
            ApplyHomePageTheme(ThemeState.IsDarkMode);

            // UI Rounding setup
            ControlRounder.MakeRounded(pnlFreshie, 20);
            ControlRounder.MakeRounded(pnlSophomore, 20);
            ControlRounder.MakeRounded(pnlJunior, 20);
            ControlRounder.MakeRounded(pnlSenior, 20);

            ControlRounder.MakeRounded(lblView1, 5);
            ControlRounder.MakeRounded(lblView2, 5);
            ControlRounder.MakeRounded(lblView3, 5);
            ControlRounder.MakeRounded(lblView4, 5);

            ControlRounder.MakeRounded(btnColorScheme, 10);

            ControlRounder.MakeRounded(txtSearch, 10);
            ControlRounder.MakeRounded(btnSearch, 10);

            // Make the whole card clickable (labels / icons inside the panel too)
            WireCardClicks(pnlFreshie, pnlFreshie_Click);
            WireCardClicks(pnlSophomore, pnlSophomore_Click);
            WireCardClicks(pnlJunior, pnlJunior_Click);
            WireCardClicks(pnlSenior, pnlSenior_Click);

            // Logout icon (upper right)
            btnProfile.Cursor = Cursors.Hand;
            btnProfile.Click += btnProfile_Click;

            // Search wiring
            SetupSuggestionList();
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.KeyDown += txtSearch_KeyDown;
            txtSearch.Leave += txtSearch_Leave;
            btnSearch.Click += btnSearch_Click;

            // Load the real numbers from the database
            LoadYearLevelCounts();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Unsubscribe from global theme event to prevent memory leaks
            ThemeState.ThemeChanged -= OnThemeChanged;
            base.OnFormClosed(e);
        }

        // ============================================================
        // DATABASE: total students per year level
        // ============================================================
        private void LoadYearLevelCounts()
        {
            // Default to 0 so the cards never show stale designer text
            SetCount(label14, 0);
            SetCount(label3, 0);
            SetCount(label11, 0);
            SetCount(label16, 0);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT year_level, total_students FROM vw_year_level_counts", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string year = reader.GetString("year_level");
                            int total = Convert.ToInt32(reader["total_students"]);

                            switch (year)
                            {
                                case "1st Year": SetCount(label14, total); break;
                                case "2nd Year": SetCount(label3, total); break;
                                case "3rd Year": SetCount(label11, total); break;
                                case "4th Year": SetCount(label16, total); break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load student counts: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetCount(Label lbl, int total)
        {
            lbl.Text = total.ToString();
        }

        // ============================================================
        // CARD CLICKS
        // ============================================================
        private void WireCardClicks(Panel card, EventHandler handler)
        {
            card.Cursor = Cursors.Hand;
            WireChildren(card, handler);
        }

        private void WireChildren(Control parent, EventHandler handler)
        {
            foreach (Control child in parent.Controls)
            {
                child.Cursor = Cursors.Hand;
                child.Click += handler;
                WireChildren(child, handler);
            }
        }

        private void pnlFreshie_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Freshmen"));
        }

        private void pnlSophomore_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Sophomore"));
        }

        private void pnlJunior_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Junior"));
        }

        private void pnlSenior_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Senior"));
        }

        // ============================================================
        // SEARCH: suggestions from the database while typing
        // ============================================================
        private void SetupSuggestionList()
        {
            lstSuggestions = new ListBox();
            lstSuggestions.Visible = false;
            lstSuggestions.BorderStyle = BorderStyle.FixedSingle;
            lstSuggestions.Font = txtSearch.Font;
            lstSuggestions.IntegralHeight = false;
            lstSuggestions.MouseDown += lstSuggestions_MouseDown;
            lstSuggestions.KeyDown += lstSuggestions_KeyDown;
            lstSuggestions.Leave += (s, ev) => HideSuggestionsIfUnfocused();

            // Added to the form (not the header panel) so it can float over the cards
            this.Controls.Add(lstSuggestions);
            lstSuggestions.BringToFront();

            StyleSuggestionList(ThemeState.IsDarkMode);
        }

        private void StyleSuggestionList(bool isDark)
        {
            if (lstSuggestions == null) return;
            lstSuggestions.BackColor = isDark ? Color.FromArgb(20, 25, 45) : Color.White;
            lstSuggestions.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (suppressSearchEvent) return;

            string text = txtSearch.Text.Trim();
            if (text.Length == 0)
            {
                lstSuggestions.Visible = false;
                return;
            }

            List<Suggestion> matches = new List<Suggestion>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        @"SELECT full_name, year_level
                          FROM vw_student_information
                          WHERE full_name LIKE @q
                             OR CONCAT(last_name, ' ', first_name) LIKE @q
                          ORDER BY last_name, first_name
                          LIMIT 8", conn))
                    {
                        cmd.Parameters.AddWithValue("@q", "%" + text + "%");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                matches.Add(new Suggestion
                                {
                                    FullName = reader["full_name"].ToString(),
                                    YearLevel = reader["year_level"] == DBNull.Value ? "" : reader["year_level"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                // A search hiccup should never crash the home page
                lstSuggestions.Visible = false;
                return;
            }

            if (matches.Count == 0)
            {
                lstSuggestions.Visible = false;
                return;
            }

            lstSuggestions.BeginUpdate();
            lstSuggestions.Items.Clear();
            foreach (Suggestion s in matches) lstSuggestions.Items.Add(s);
            lstSuggestions.EndUpdate();

            // Position directly under the search box
            Point p = this.PointToClient(txtSearch.Parent.PointToScreen(new Point(txtSearch.Left, txtSearch.Bottom)));
            lstSuggestions.Location = new Point(p.X, p.Y + 2);
            lstSuggestions.Width = txtSearch.Width;
            lstSuggestions.Height = Math.Min(matches.Count, 8) * lstSuggestions.ItemHeight + 4;
            lstSuggestions.Visible = true;
            lstSuggestions.BringToFront();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (lstSuggestions.Visible && lstSuggestions.SelectedItem != null)
                    ChooseSuggestion((Suggestion)lstSuggestions.SelectedItem);
                else
                    RunSearch();
            }
            else if (e.KeyCode == Keys.Down && lstSuggestions.Visible)
            {
                e.SuppressKeyPress = true;
                lstSuggestions.Focus();
                if (lstSuggestions.Items.Count > 0) lstSuggestions.SelectedIndex = 0;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                lstSuggestions.Visible = false;
            }
        }

        private void lstSuggestions_MouseDown(object sender, MouseEventArgs e)
        {
            int index = lstSuggestions.IndexFromPoint(e.Location);
            if (index >= 0)
                ChooseSuggestion((Suggestion)lstSuggestions.Items[index]);
        }

        private void lstSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && lstSuggestions.SelectedItem != null)
            {
                e.SuppressKeyPress = true;
                ChooseSuggestion((Suggestion)lstSuggestions.SelectedItem);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                lstSuggestions.Visible = false;
                txtSearch.Focus();
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            HideSuggestionsIfUnfocused();
        }

        private void HideSuggestionsIfUnfocused()
        {
            // Wait until focus has actually moved, so clicking a suggestion still works
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (!txtSearch.Focused && !lstSuggestions.Focused)
                    lstSuggestions.Visible = false;
            });
        }

        private void ChooseSuggestion(Suggestion s)
        {
            suppressSearchEvent = true;
            txtSearch.Text = s.FullName;
            suppressSearchEvent = false;

            lstSuggestions.Visible = false;
            OpenYearForStudent(s.YearLevel, s.FullName);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RunSearch();
        }

        private void RunSearch()
        {
            string text = txtSearch.Text.Trim();
            if (text.Length == 0) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        @"SELECT full_name, year_level
                          FROM vw_student_information
                          WHERE full_name LIKE @q
                             OR CONCAT(last_name, ' ', first_name) LIKE @q
                          ORDER BY (full_name = @exact) DESC, last_name, first_name
                          LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@q", "%" + text + "%");
                        cmd.Parameters.AddWithValue("@exact", text);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string year = reader["year_level"] == DBNull.Value ? "" : reader["year_level"].ToString();
                                lstSuggestions.Visible = false;
                                OpenYearForStudent(year, reader["full_name"].ToString());
                            }
                            else
                            {
                                MessageBox.Show("No student found matching \"" + text + "\".", "Search",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenYearForStudent(string yearLevel, string fullName)
        {
            string displayName;
            if (!yearToDisplayName.TryGetValue(yearLevel ?? "", out displayName))
            {
                MessageBox.Show(fullName + " has not set a year level yet.", "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SIS.Instance.OpenChildForm(new display(displayName));
        }

        // ============================================================
        // LOG OUT (icon in the upper right)
        // ============================================================
        private void btnProfile_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Log Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Log out: navigate directly to the teacher sign-in form
                SIS.Instance.OpenChildForm(new tcrSignIn());
            }
            // No / Close: do nothing and stay signed in
        }

        // ============================================================
        // THEME
        // ============================================================
        private void OnThemeChanged(object sender, bool isDark)
        {
            ApplyHomePageTheme(isDark);
        }

        private void ApplyHomePageTheme(bool isDark)
        {
            // Force all custom painted panels to redraw with the new theme gradients live
            pnlFreshie.Invalidate();
            pnlSophomore.Invalidate();
            pnlJunior.Invalidate();
            pnlSenior.Invalidate();
            panel1.Invalidate(); // Header panel

            // ONLY change the header title text color (label1).
            if (label1 != null)
            {
                label1.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
            }

            // Update search box colors for dark/light mode
            if (txtSearch != null)
            {
                txtSearch.BackColor = isDark ? Color.FromArgb(20, 25, 45) : Color.White;
                txtSearch.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
            }

            StyleSuggestionList(isDark);
        }

        // --- Theme Toggle Button ---
        private void btnColorScheme_Click(object sender, EventArgs e)
        {
            ThemeState.ToggleTheme();
        }

        private void pnlFreshie_MouseHover(object sender, EventArgs e)
        {
            pnlFreshie.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlSophomore_MouseHover(object sender, EventArgs e)
        {
            pnlSophomore.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlJunior_MouseHover(object sender, EventArgs e)
        {
            pnlJunior.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlSenior_MouseHover(object sender, EventArgs e)
        {
            pnlSenior.BackColor = Color.FromArgb(238, 240, 243);
        }

        // --- Smart Panel Paint Events (Automatically switches between Sunset and Navy) ---
        private void pnlFreshie_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlSophomore_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlJunior_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlSenior_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyHeaderGradient((Control)sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


    }
}