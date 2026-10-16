
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BULi
{
    public partial class setUp : Form
    {
        // Update connection string details if needed (server, user, password)
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L*07_26;";
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
                                string middleName = reader["middle_name"].ToString();
                                string lastName = reader["last_name"].ToString();

                                // Combine full name for display
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
            catch (Exception ex)
            {
                MessageBox.Show("Error loading details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse full name back into components
                string[] nameParts = txtName.Text.Trim().Split(' ');
                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                DateTime birthdate;
                if (!DateTime.TryParse(txtDate.Text.Trim(), out birthdate))
                {
                    birthdate = DateTime.Now; // Default fallback if parsing fails
                }

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
                        WHERE username = @Username";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", currentUsername ?? "");
                        cmd.Parameters.AddWithValue("@StudentID", txtID.Text.Trim());
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@MiddleName", middleName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Gender", txtGender.Text.Trim());
                        cmd.Parameters.AddWithValue("@Birthdate", birthdate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@YearLevel", txtYearlvl.Text.Trim());
                        cmd.Parameters.AddWithValue("@Program", txtProgram.Text.Trim());
                        cmd.Parameters.AddWithValue("@Block", txtBloc.Text.Trim());
                        cmd.Parameters.AddWithValue("@Contact", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmailAdd.Text.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Information saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            SIS.Instance.OpenChildForm(new HomePage());
                        }
                        else
                        {
                            MessageBox.Show("No matching student account found to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void txtAge_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtBloc_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtYearlvl_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtGender_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
