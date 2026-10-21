using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace BULi
{
    public partial class HomePage : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122;";
        private DataTable studentDataTable = new DataTable();

        public HomePage()
        {
            InitializeComponent();
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            LoadYearLevelCounts();
            LoadAllStudents();
        }

        // 1. Fetch counts from vw_year_level_counts and update panel labels
        private void LoadYearLevelCounts()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT year_level, total_students FROM vw_year_level_counts";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int freshie = 0, sophomore = 0, junior = 0, senior = 0;

                        while (reader.Read())
                        {
                            string level = reader["year_level"].ToString();
                            int count = Convert.ToInt32(reader["total_students"]);

                            switch (level)
                            {
                                case "1st Year": freshie = count; break;
                                case "2nd Year": sophomore = count; break;
                                case "3rd Year": junior = count; break;
                                case "4th Year": senior = count; break;
                            }
                        }

                        UpdatePanelCount(pnlFreshie, freshie);
                        UpdatePanelCount(pnlSophomore, sophomore);
                        UpdatePanelCount(pnlJunior, junior);
                        UpdatePanelCount(pnlSenior, senior);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student counts: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePanelCount(Panel panel, int count)
        {
            if (panel == null) return;
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is Label)
                {
                    ctrl.Text = count.ToString();
                    break;
                }
            }
        }

        // Helper to locate any DataGridView control on the form dynamically
        private DataGridView GetStudentGrid()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is DataGridView dgv) return dgv;
                foreach (Control subCtrl in ctrl.Controls)
                {
                    if (subCtrl is DataGridView subDgv) return subDgv;
                }
            }
            return null;
        }

        // 2. Load all active students
        private void LoadAllStudents()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM vw_student_information";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        studentDataTable.Clear();
                        adapter.Fill(studentDataTable);

                        DataGridView grid = GetStudentGrid();
                        if (grid != null)
                        {
                            grid.DataSource = studentDataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students list: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. Search filter
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (studentDataTable == null) return;

            string filterText = txtSearch.Text.Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(filterText))
            {
                studentDataTable.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                studentDataTable.DefaultView.RowFilter = string.Format(
                    "student_number LIKE '%{0}%' OR full_name LIKE '%{0}%' OR program LIKE '%{0}%' OR year_level LIKE '%{0}%'",
                    filterText
                );
            }
        }

        // =========================================================================
        // REQUIRED UI EVENT HANDLERS (Resolves CS1061 designer compilation errors)
        // =========================================================================
        private void pnlFreshie_Click(object sender, EventArgs e) { }
        private void pnlFreshie_MouseHover(object sender, EventArgs e) { }

        private void pnlSophomore_Click(object sender, EventArgs e) { }
        private void pnlSophomore_MouseHover(object sender, EventArgs e) { }

        private void pnlJunior_Click(object sender, EventArgs e) { }
        private void pnlJunior_MouseHover(object sender, EventArgs e) { }

        private void pnlSenior_Click(object sender, EventArgs e) { }
        private void pnlSenior_MouseHover(object sender, EventArgs e) { }

        private void btnColorScheme_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }

        private void pnlFreshie_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}