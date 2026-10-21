using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BULi
{
    public partial class infoCard : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";
        private string currentUsername;

        public infoCard()
        {
            InitializeComponent();
        }

        public infoCard(string username) : this()
        {
            this.currentUsername = username;
        }

        private void infoCard_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentUsername))
            {
                LoadStudentCardData(currentUsername);
            }

            if (string.Equals(pick.SelectedRole, "Student", StringComparison.OrdinalIgnoreCase))
            {
                lnkReturn.Text = "Return";
            }
            else if (string.Equals(pick.SelectedRole, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                lnkReturn.Text = "Return to List";
            }
            else
            {
                lnkReturn.Text = "Return";
            }
        }

        private void LoadStudentCardData(string username)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT student_number, first_name, middle_name, last_name, 
                               year_level, program, block, contact_number 
                        FROM students 
                        WHERE username = @Username AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["first_name"].ToString();
                                string middleName = reader["middle_name"].ToString();
                                string lastName = reader["last_name"].ToString();

                                string fullName = $"{firstName} {middleName} {lastName}".Replace("  ", " ").Trim();
                                lblName.Text = string.IsNullOrEmpty(fullName) ? "N/A" : fullName;
                                lblID.Text = reader["student_number"].ToString();
                                lblProgram.Text = reader["program"].ToString();
                                lblYearlvl.Text = reader["year_level"].ToString();
                                lblBloc.Text = reader["block"].ToString();
                                lblContact.Text = reader["contact_number"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Student details could not be found.", "Record Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Student Card: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================================
        // RETURN BUTTON -> OPENS SETUP WITH ACTIVE USERNAME PASSED
        // ==========================================================
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.Equals(pick.SelectedRole, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                SIS.Instance.OpenChildForm(new display());
            }
            else
            {
                // Passes currentUsername so setUp loads with all fields populated
                SIS.Instance.OpenChildForm(new setUp(currentUsername, false));
            }
        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnDrop, new Point(0, btnDrop.Height));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }

        private void infoCard_Load_1(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);
        }
    }
}