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
    public partial class display : Form
    {
        private const string ConnStr = "Server=localhost;Database=sis_db;Uid=root;Pwd=nunez26*7;";

        private String yearLvl;          // what the homepage sent, e.g. "Senior"
        private string dbYearLevel;      // what the database stores, e.g. "4th Year"

        private int _selectedId = -1;    // student_id of the selected student (-1 = none)
        private string _selectedUsername = null;
        private bool _loading = false;   // blocks selection events while the grid is rebinding
        private string _sortMode = "Alphabetical";

        public display()
        {
            InitializeComponent();
        }

        public display(string yearLevel) : this()
        {
            yearLvl = yearLevel;
            dbYearLevel = ToDbYearLevel(yearLevel);
            yearLvl = ToTitleName(dbYearLevel);
            lblYearTitle.Text = $"{yearLvl} Student Register";
        }

        // =====================================================
        //  LOAD / SETUP
        // =====================================================
        private void display_Load(object sender, EventArgs e)
        {
            pnlScroll.MouseEnter += (s, ev) => pnlScroll.Focus();
            ControlRounder.MakeRounded(btnReturn, 10);
            ControlRounder.MakeRounded(pnlhawakupdate, 16);
            ControlRounder.MakeRounded(pnlID, 6);
            ControlRounder.MakeRounded(pnlName, 6);
            ControlRounder.MakeRounded(pnlBloc, 6);
            ControlRounder.MakeRounded(pnlProgram, 6);
            ControlRounder.MakeRounded(btnAddSI, 8);
            ControlRounder.MakeRounded(btnUpdateSI, 8);
            ControlRounder.MakeRounded(btnDeleteSI, 8);
            ControlRounder.MakeRounded(btnView, 8);
            ControlRounder.MakeRounded(btnClear, 6);
            ControlRounder.MakeRounded(txtSearch, 6);

            // opened without a year (e.g. returning from the info card with no argument)
            if (string.IsNullOrEmpty(dbYearLevel))
            {
                yearLvl = "Freshmen";
                dbYearLevel = ToDbYearLevel(yearLvl);
                lblYearTitle.Text = $"{yearLvl} Student Register";
            }

          
            SetupGrid();
            SetupSortMenu();

            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            LoadStudents();
        }

        private void SetupGrid()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }

        private void SetupSortMenu()
        {
            contextMenuStrip1.Items.Clear();
            string[] modes = { "Alphabetical", "Bloc", "Program", "Student ID" };
            foreach (string m in modes)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(m);
                item.Checked = (m == _sortMode);
                contextMenuStrip1.Items.Add(item);
            }
            contextMenuStrip1.ItemClicked -= contextMenuStrip1_ItemClicked;
            contextMenuStrip1.ItemClicked += contextMenuStrip1_ItemClicked;
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _sortMode = e.ClickedItem.Text;
            foreach (ToolStripItem it in contextMenuStrip1.Items)
            {
                ToolStripMenuItem mi = it as ToolStripMenuItem;
                if (mi != null) mi.Checked = (mi.Text == _sortMode);
            }
            LoadStudents();
        }

        // =====================================================
        //  YEAR LEVEL MAPPING
        // =====================================================
        private static string ToDbYearLevel(string y)
        {
            if (string.IsNullOrWhiteSpace(y)) return null;
            string s = y.Trim().ToLowerInvariant();

            if (s.StartsWith("fresh") || s.StartsWith("1")) return "1st Year";
            if (s.StartsWith("soph") || s.StartsWith("2")) return "2nd Year";
            if (s.StartsWith("jun") || s.StartsWith("3")) return "3rd Year";
            if (s.StartsWith("sen") || s.StartsWith("4")) return "4th Year";
            return y.Trim();
        }

        // "4th Year" -> "Senior", so the title stays the same however the form was opened
        private static string ToTitleName(string dbYear)
        {
            switch (dbYear)
            {
                case "1st Year": return "Freshmen";
                case "2nd Year": return "Sophomore";
                case "3rd Year": return "Junior";
                case "4th Year": return "Senior";
                default: return dbYear;
            }
        }

        // =====================================================
        //  LOADING THE LIST (search + sort happen in SQL)
        // =====================================================
        private void LoadStudents()
        {
            int keepId = _selectedId;

            try
            {
                string orderBy;
                switch (_sortMode)
                {
                    case "Bloc": orderBy = "block, last_name, first_name"; break;
                    case "Program": orderBy = "program, last_name, first_name"; break;
                    case "Student ID": orderBy = "student_number"; break;
                    default: orderBy = "last_name, first_name"; break;
                }

                string search = (txtSearch.Text ?? "").Trim();

                string sql = @"
                    SELECT student_id, username, first_name, middle_name, last_name,
                           student_number AS `Student ID`,
                           CONCAT(last_name, ', ', first_name,
                                  COALESCE(CONCAT(' ', NULLIF(middle_name, '')), '')) AS `Name`,
                           block AS `Bloc`,
                           program AS `Program`
                    FROM students
                    WHERE is_deleted = 0 AND year_level = @yl";

                if (search.Length > 0)
                {
                    sql += @"
                      AND (student_number LIKE @s
                        OR first_name LIKE @s
                        OR last_name LIKE @s
                        OR CONCAT(first_name, ' ', last_name) LIKE @s
                        OR CONCAT(last_name, ', ', first_name) LIKE @s
                        OR block LIKE @s
                        OR program LIKE @s)";
                }
                sql += " ORDER BY " + orderBy;

                DataTable dt = new DataTable();
                using (MySqlConnection conn = new MySqlConnection(ConnStr))
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@yl", dbYearLevel);
                    if (search.Length > 0)
                    {
                        string escaped = search.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
                        cmd.Parameters.AddWithValue("@s", "%" + escaped + "%");
                    }
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                _loading = true;
                dataGridView1.DataSource = dt;

                // internal columns stay hidden
                foreach (string hidden in new[] { "student_id", "username", "first_name", "middle_name", "last_name" })
                {
                    if (dataGridView1.Columns.Contains(hidden))
                        dataGridView1.Columns[hidden].Visible = false;
                }

                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;

                // keep the previous selection if that student is still in the list
                DataGridViewRow match = null;
                if (keepId != -1)
                {
                    foreach (DataGridViewRow r in dataGridView1.Rows)
                    {
                        if (Convert.ToInt32(r.Cells["student_id"].Value) == keepId)
                        {
                            match = r;
                            break;
                        }
                    }
                }
                _loading = false;

                if (match != null)
                {
                    match.Selected = true;
                    PopulateFromRow(match);
                }
                else
                {
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                _loading = false;
                MessageBox.Show("Error loading students: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        //  SELECTION <-> MODIFICATION BARS
        // =====================================================
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            if (dataGridView1.SelectedRows.Count == 0)
            {
                ClearFields();
                return;
            }
            PopulateFromRow(dataGridView1.SelectedRows[0]);
        }

        private void PopulateFromRow(DataGridViewRow row)
        {
            _selectedId = Convert.ToInt32(row.Cells["student_id"].Value);
            _selectedUsername = row.Cells["username"].Value.ToString();

            txtID.Text = row.Cells["Student ID"].Value.ToString();
            txtName.Text = row.Cells["Name"].Value.ToString();
            txtBloc.Text = row.Cells["Bloc"].Value.ToString();
            txtProgram.Text = row.Cells["Program"].Value.ToString();
        }

        private void ClearFields()
        {
            _selectedId = -1;
            _selectedUsername = null;
            txtID.Clear();
            txtName.Clear();
            txtBloc.Clear();
            txtProgram.Clear();
        }

        /// <summary>
        /// Deselects the student and empties the modification bars so a new one can be added.
        /// Not wired to any button yet: later, just add a button and set its Click to btnClear_Click.
        /// </summary>
        private void ClearForm()
        {
            bool wasLoading = _loading;
            _loading = true;
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            _loading = wasLoading;
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool HasSelection()
        {
            return _selectedId != -1;
        }

        // =====================================================
        //  INPUT HELPERS
        // =====================================================
        // Accepts "Last, First Middle" (what the list shows) or "First [Middle] Last".
        private static bool ParseName(string input, out string first, out string middle, out string last)
        {
            first = middle = last = null;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string clean = string.Join(" ", input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            if (clean.Contains(","))
            {
                string[] parts = clean.Split(new[] { ',' }, 2);
                last = parts[0].Trim();
                string[] rest = parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (last.Length == 0 || rest.Length == 0) return false;

                first = rest[0];
                if (rest.Length > 1) middle = string.Join(" ", rest.Skip(1));
            }
            else
            {
                string[] t = clean.Split(' ');
                if (t.Length < 2) return false;

                first = t[0];
                last = t[t.Length - 1];
                if (t.Length > 2) middle = string.Join(" ", t.Skip(1).Take(t.Length - 2));
            }
            return true;
        }

        // Matches the typed program to the programs table (full name or short code).
        private string ResolveProgram(MySqlConnection conn, string typed)
        {
            if (string.IsNullOrWhiteSpace(typed)) return null;
            string t = typed.Trim();

            Dictionary<string, string> alias = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "BSIT",   "BS Information Technology" },
                { "BSCS",   "BS Computer Science" },
                { "BSMET",  "BS Metreology" },
                { "BSBIO",  "BS Biology" },
                { "BSCHEM", "BS Chemistry" }
            };
            if (alias.ContainsKey(t)) t = alias[t];

            using (MySqlCommand cmd = new MySqlCommand("SELECT program_name FROM programs WHERE program_name = @p", conn))
            {
                cmd.Parameters.AddWithValue("@p", t);
                object result = cmd.ExecuteScalar();
                return result == null ? null : result.ToString();
            }
        }

        private bool ValidateInputs(MySqlConnection conn, out string studentNo, out string first,
                                    out string middle, out string last, out string block, out string program)
        {
            studentNo = txtID.Text.Trim();
            block = txtBloc.Text.Trim();
            first = middle = last = null;
            program = null;

            if (studentNo.Length == 0)
            {
                MessageBox.Show("Please enter the Student ID.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!ParseName(txtName.Text, out first, out middle, out last))
            {
                MessageBox.Show("Please enter the full name as \"Last, First Middle\" (e.g. Dela Cruz, Juan Santos).",
                    "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (block.Length == 0)
            {
                MessageBox.Show("Please enter the Bloc.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            program = ResolveProgram(conn, txtProgram.Text);
            if (program == null)
            {
                MessageBox.Show("Program not recognized. Use one of:\nBS Information Technology (BSIT)\nBS Computer Science (BSCS)\nBS Metreology (BSMet)\nBS Biology (BSBio)\nBS Chemistry (BSChem)",
                    "Invalid Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private static void LogActivity(MySqlConnection conn, string action, int? targetId, string details)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO activity_logs (actor_type, actor_id, action, target_student_id, details)
                      VALUES ('Teacher', NULL, @a, @t, @d)", conn))
                {
                    cmd.Parameters.AddWithValue("@a", action);
                    cmd.Parameters.AddWithValue("@t", targetId.HasValue ? (object)targetId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@d", details);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // logging must never break the main action
            }
        }

        // =====================================================
        //  BUTTONS
        // =====================================================
        private void btnAddSI_Click(object sender, EventArgs e)
        {
            if (HasSelection())
            {
                MessageBox.Show("A student is currently selected. Please clear the selection first before adding a new student.",
                    "Clear First", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    string studentNo, first, middle, last, block, program;
                    if (!ValidateInputs(conn, out studentNo, out first, out middle, out last, out block, out program))
                        return;

                    // Login defaults: username = student ID, password = student ID (student can change it later)
                    string sql = @"
                        INSERT INTO students
                            (student_number, first_name, middle_name, last_name, gender, birthdate,
                             year_level, program, block, contact_number, status, username, password_hash)
                        VALUES
                            (@no, @fn, @mn, @ln, 'Prefer not to say', NULL,
                             @yl, @prog, @blk, '', 'Active', @user, SHA2(@pass, 256))";

                    long newId;
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@no", studentNo);
                        cmd.Parameters.AddWithValue("@fn", first);
                        cmd.Parameters.AddWithValue("@mn", string.IsNullOrEmpty(middle) ? (object)DBNull.Value : middle);
                        cmd.Parameters.AddWithValue("@ln", last);
                        cmd.Parameters.AddWithValue("@yl", dbYearLevel);
                        cmd.Parameters.AddWithValue("@prog", program);
                        cmd.Parameters.AddWithValue("@blk", block);
                        cmd.Parameters.AddWithValue("@user", studentNo);
                        cmd.Parameters.AddWithValue("@pass", studentNo);
                        cmd.ExecuteNonQuery();
                        newId = cmd.LastInsertedId;
                    }

                    LogActivity(conn, "ADD", (int)newId, "Student " + studentNo + " added");
                }

                MessageBox.Show("Student added successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                LoadStudents();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("That Student ID already exists.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding student: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateSI_Click(object sender, EventArgs e)
        {
            if (!HasSelection())
            {
                MessageBox.Show("Please select a student from the list first.", "No Student Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    string studentNo, first, middle, last, block, program;
                    if (!ValidateInputs(conn, out studentNo, out first, out middle, out last, out block, out program))
                        return;

                    string sql = @"
                        UPDATE students
                        SET student_number = @no, first_name = @fn, middle_name = @mn,
                            last_name = @ln, program = @prog, block = @blk
                        WHERE student_id = @id AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@no", studentNo);
                        cmd.Parameters.AddWithValue("@fn", first);
                        cmd.Parameters.AddWithValue("@mn", string.IsNullOrEmpty(middle) ? (object)DBNull.Value : middle);
                        cmd.Parameters.AddWithValue("@ln", last);
                        cmd.Parameters.AddWithValue("@prog", program);
                        cmd.Parameters.AddWithValue("@blk", block);
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.ExecuteNonQuery();
                    }

                    LogActivity(conn, "EDIT", _selectedId, "Student " + studentNo + " updated");
                }

                MessageBox.Show("Student information updated.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadStudents();   // keeps the student selected
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("That Student ID is already used by another student.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating student: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSI_Click(object sender, EventArgs e)
        {
            if (!HasSelection())
            {
                MessageBox.Show("Please select a student from the list first.", "No Student Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete " + txtName.Text + "?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string studentNo = txtID.Text.Trim();
                using (MySqlConnection conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    // soft delete (recycle bin), matches the is_deleted design in the database
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE students SET is_deleted = 1 WHERE student_id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedId);
                        cmd.ExecuteNonQuery();
                    }
                    LogActivity(conn, "DELETE", _selectedId, "Student " + studentNo + " deleted");
                }

                ClearForm();
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting student: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (!HasSelection())
            {
                MessageBox.Show("Please select a student from the list first.", "No Student Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SIS.Instance.OpenChildForm(new infoCard(_selectedUsername));
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnSort, new Point(0, btnSort.Height));
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }

        // Top-right icon = log out
        private void btnProfile_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Log Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SIS.Instance.OpenChildForm(new tcrSignIn());
            }
        }

        // Live search: shows matching students of this year level as the teacher types
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dbYearLevel == null) return;   // fires before Load finishes
            LoadStudents();
        }

        // =====================================================
        //  DESIGNER-WIRED HANDLERS (kept so the designer still compiles)
        // =====================================================
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void lblYearTitle_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void txtID_TextChanged(object sender, EventArgs e) { }
        private void txtName_TextChanged(object sender, EventArgs e) { }
        private void txtBloc_TextChanged(object sender, EventArgs e) { }
        private void txtYearlvl_TextChanged(object sender, EventArgs e) { }
        private void txtProgram_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyDeepHeaderGradient((Control)sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);
        }

        private void pnlhawakupdate_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlID_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlName_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlBloc_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlYear_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlProgram_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }


    }
}