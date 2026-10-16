using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BULi
{
    public partial class setUp : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=your_mysql_password;";
        private string currentUsername;

        public setUp()
        {
            InitializeComponent();
        }

        public setUp(string username) : this()
        {
            this.currentUsername = username;
        }

        private void setUp_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentUsername))
            {
                LoadStudentData(currentUsername);
            }
        }

        private void LoadStudentData(string username)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT student_number, first_name, middle_name, last_name, gender, 
                               birthdate, year_level, program, block, contact_number, email,
                               TIMESTAMPDIFF(YEAR, birthdate, CURDATE()) AS calculated_age
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
                                string lastName = reader["last_name"].ToString();

                                // Check if account has initial placeholder data from SignUp.cs
                                bool isNewAccount = (firstName == "New" && lastName == "Student");

                                if (isNewAccount)
                                {
                                    // Leave all fields empty for fresh sign-up completion
                                    txtName.Text = "";
                                    txtID.Text = "";
                                    txtProgram.Text = "";
                                    txtYearlvl.Text = "";
                                    txtBloc.Text = "";
                                    txtDate.Text = "";
                                    txtAge.Text = "";
                                    txtGender.Text = "";
                                    txtContact.Text = "";
                                    txtEmailAdd.Text = "";
                                }
                                else
                                {
                                    // Load stored student details
                                    string middleName = reader["middle_name"].ToString();
                                    txtName.Text = $"{firstName} {middleName} {lastName}".Replace("  ", " ").Trim();
                                    txtID.Text = reader["student_number"].ToString();
                                    txtProgram.Text = reader["program"].ToString();
                                    txtYearlvl.Text = reader["year_level"].ToString();
                                    txtBloc.Text = reader["block"].ToString();

                                    if (reader["birthdate"] != DBNull.Value)
                                    {
                                        txtDate.Text = Convert.ToDateTime(reader["birthdate"]).ToString("yyyy-MM-dd");
                                    }

                                    txtAge.Text = reader["calculated_age"].ToString();
                                    txtGender.Text = reader["gender"].ToString();
                                    txtContact.Text = reader["contact_number"].ToString();
                                    txtEmailAdd.Text = reader["email"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 1. UPDATE / SAVE BUTTON
        // ==========================================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse full name entered in txtName
                string rawName = txtName.Text.Trim();
                string[] nameParts = rawName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                DateTime birthdate;
                bool validDate = DateTime.TryParse(txtDate.Text.Trim(), out birthdate);
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"
                UPDATE students 
                SET student_number = @StudentID,
                    first_name = @FirstName, 
                    middle_name = @MiddleName, 
                    last_name = @LastName, 
                    gender = @Gender, 
                    birthdate = @Birthdate, 
                    year_level = @YearLevel, 
                    program = @Program, 
                    block = @Block, 
                    contact_number = @Contact, 
                    email = @Email 
                WHERE username = @Username AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", currentUsername ?? "");
                        cmd.Parameters.AddWithValue("@StudentID", txtID.Text.Trim());
                        cmd.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(firstName) ? "N/A" : firstName);
                        cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName);
                        cmd.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(lastName) ? "N/A" : lastName);
                        cmd.Parameters.AddWithValue("@Gender", string.IsNullOrEmpty(txtGender.Text.Trim()) ? "Prefer not to say" : txtGender.Text.Trim());
                        cmd.Parameters.AddWithValue("@Birthdate", validDate ? birthdate.ToString("yyyy-MM-dd") : "2000-01-01");
                        cmd.Parameters.AddWithValue("@YearLevel", string.IsNullOrEmpty(txtYearlvl.Text.Trim()) ? "Freshman" : txtYearlvl.Text.Trim());
                        cmd.Parameters.AddWithValue("@Program", string.IsNullOrEmpty(txtProgram.Text.Trim()) ? "BS Information Technology" : txtProgram.Text.Trim());
                        cmd.Parameters.AddWithValue("@Block", string.IsNullOrEmpty(txtBloc.Text.Trim()) ? "Block A" : txtBloc.Text.Trim());
                        cmd.Parameters.AddWithValue("@Contact", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmailAdd.Text.Trim()) ? (object)DBNull.Value : txtEmailAdd.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            SIS.Instance.OpenChildForm(new HomePage());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 2. DELETE ACCOUNT BUTTON
        // ==========================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete your account? This action cannot be undone.",
                "Confirm Account Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        // Soft delete execution matching database schema
                        string deleteQuery = "UPDATE students SET is_deleted = 1 WHERE username = @Username";

                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", currentUsername ?? "");
                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                MessageBox.Show("Your account has been deleted.", "Account Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Log out and redirect back to Sign In
                                SIS.Instance.OpenChildForm(new stdSignIn());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // 3. SHOW STUDENT CARD BUTTON
        // ==========================================
        private void btnShowCard_Click(object sender, EventArgs e)
        {
            // Opens the Student Card form passing the current logged-in username
            SIS.Instance.OpenChildForm(new infoCard(currentUsername));
        }
    }
}
