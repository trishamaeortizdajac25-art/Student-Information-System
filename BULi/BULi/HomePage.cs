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
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";

        private DataTable studentDataTable = new DataTable();


        private readonly string[] programKeywords = new string[]
        {
            "Biology",
            "Chemistry",
            "Computer Science",
            "Information Technology",
            "Meteorology"
        };

        
        private ListBox lstSuggestions;
        private bool suppressSearchEvent = false;

        private class Suggestion
        {
            public string FullName { get; set; }
            public string Program { get; set; }   
            public override string ToString()
            {
                return FullName + "  (" + Program + ")";
            }
        }

        public HomePage()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            
            ThemeState.ThemeChanged += OnThemeChanged;
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
           
            ApplyHomePageTheme(ThemeState.IsDarkMode);

            
            ControlRounder.MakeRounded(pnlBio, 20);
            ControlRounder.MakeRounded(pnlChem, 20);
            ControlRounder.MakeRounded(pnlAyti, 20);
            ControlRounder.MakeRounded(pnlComSci, 20);
            ControlRounder.MakeRounded(pnlMeteo, 20);      

            ControlRounder.MakeRounded(lblView1, 5);
            ControlRounder.MakeRounded(lblView2, 5);
            ControlRounder.MakeRounded(lblView3, 5);
            ControlRounder.MakeRounded(lblView4, 5);
            ControlRounder.MakeRounded(lblView5, 5);       

            ControlRounder.MakeRounded(btnColorScheme, 10);

            ControlRounder.MakeRounded(txtSearch, 10);
            ControlRounder.MakeRounded(btnSearch, 10);

          
            WireCardClicks(pnlBio, pnlBio_Click);
            WireCardClicks(pnlChem, pnlChem_Click);
            WireCardClicks(pnlAyti, pnlAyti_Click);
            WireCardClicks(pnlComSci, pnlComSci_Click);
            WireCardClicks(pnlMeteo, pnlMeteo_Click);      

           
            btnProfile.Cursor = Cursors.Hand;
            btnProfile.Click += btnProfile_Click;

           
            SetupSuggestionList();
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.KeyDown += txtSearch_KeyDown;
            txtSearch.Leave += txtSearch_Leave;
            btnSearch.Click += btnSearch_Click;

          
            LoadProgramCounts();                           
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
           
            ThemeState.ThemeChanged -= OnThemeChanged;
            base.OnFormClosed(e);
        }


        private string GetProgramKey(string program)
        {
            if (string.IsNullOrWhiteSpace(program)) return null;

           
            if (program.IndexOf("Metreology", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Meteorology";

            foreach (string key in programKeywords)
            {
                if (program.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return key;
            }
            return null;
        }
        // ============================================================
        // DATABASE: total students per program  
        // ============================================================
        private void LoadProgramCounts()
        {
            // Default to 0 so the cards never show stale designer text
            SetCount(lblBio, 0);
            SetCount(lblChem, 0);
            SetCount(lblComSci, 0);
            SetCount(lblAyti, 0);
            SetCount(lblMeteo, 0);

            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (string key in programKeywords) totals[key] = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        @"SELECT program, COUNT(*) AS total
                          FROM students
                          WHERE is_deleted = 0 AND program IS NOT NULL
                          GROUP BY program", conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = GetProgramKey(reader["program"].ToString());
                            if (key != null)
                                totals[key] += Convert.ToInt32(reader["total"]);
                        }
                    }
                }

                SetCount(lblBio, totals["Biology"]);
                SetCount(lblChem, totals["Chemistry"]);
                SetCount(lblComSci, totals["Computer Science"]);
                SetCount(lblAyti, totals["Information Technology"]);
                SetCount(lblMeteo, totals["Meteorology"]);
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

        
        private void pnlBio_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Biology"));
        }

        private void pnlChem_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Chemistry"));
        }

        private void pnlAyti_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Information Technology"));
        }

        private void pnlComSci_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Computer Science"));
        }

        // NEW
        private void pnlMeteo_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Meteorology"));
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
                        @"SELECT full_name, program
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
                                    Program = reader["program"] == DBNull.Value ? "" : reader["program"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
            
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
            OpenProgramForStudent(s.Program, s.FullName);   
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
                        @"SELECT full_name, program
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
                                string program = reader["program"] == DBNull.Value ? "" : reader["program"].ToString();
                                lstSuggestions.Visible = false;
                                OpenProgramForStudent(program, reader["full_name"].ToString());   // CHANGED
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

       
        private void OpenProgramForStudent(string program, string fullName)
        {
            string key = GetProgramKey(program);
            if (key == null)
            {
                MessageBox.Show(fullName + " does not belong to a listed program.", "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SIS.Instance.OpenChildForm(new display(key));
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
            pnlBio.Invalidate();
            pnlChem.Invalidate();
            pnlAyti.Invalidate();
            pnlComSci.Invalidate();
            pnlMeteo.Invalidate();   
            panel1.Invalidate(); 

            
            if (label1 != null)
            {
                label1.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
            }

            
            if (txtSearch != null)
            {
                txtSearch.BackColor = isDark ? Color.FromArgb(20, 25, 45) : Color.White;
                txtSearch.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
            }

          
            Color navyDarkMode = Color.FromArgb(15, 23, 42);
            Color dayOrangeMode = Color.FromArgb(254, 180, 123);

            if (btnColorScheme != null)
            {
                if (isDark)
                {
                    btnColorScheme.BackColor = navyDarkMode;
                    btnColorScheme.ForeColor = Color.White;
                }
                else
                {
                    btnColorScheme.BackColor = dayOrangeMode;
                    btnColorScheme.ForeColor = Color.Black;
                }
            }

            StyleSuggestionList(isDark);
        }

        // Theme Toggle Button
        private void btnColorScheme_Click(object sender, EventArgs e)
        {
            ThemeState.ToggleTheme();
        }

        private void pnlBio_MouseHover(object sender, EventArgs e)
        {
            pnlBio.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlChem_MouseHover(object sender, EventArgs e)
        {
            pnlChem.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlAyti_MouseHover(object sender, EventArgs e)
        {
            pnlAyti.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlComSci_MouseHover(object sender, EventArgs e)
        {
            pnlComSci.BackColor = Color.FromArgb(238, 240, 243);
        }

        // --- Smart Panel Paint Events (Automatically switches between Sunset and Navy) 
        private void pnlBio_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlChem_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlAyti_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlComSci_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyHeaderGradient((Control)sender, e);
        }

        private void pnlMeteo_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }
    }
}