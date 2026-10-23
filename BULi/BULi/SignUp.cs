using BULi;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BULi
{
    public partial class SignUp : Form
    {

        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122;";


        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in both the username and password fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (username.Contains(" "))
            {
                MessageBox.Show("Username cannot contain spaces.", "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool registered = false;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if username already exists
                    using (MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM students WHERE username = @Username", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        if (Convert.ToInt64(checkCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Username is already taken. Please choose another one.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insert ONLY username + password; profile fields stay empty (NULL)
                    string insertQuery = @"
                        INSERT INTO students
                        (username, password_hash, first_name, last_name, is_deleted, created_at)
                        VALUES
                        (@Username, @PasswordHash, '', '', 0, NOW())";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@PasswordHash", ComputeSha256Hash(password));

                        if (insertCmd.ExecuteNonQuery() > 0)
                        {
                            long newId = insertCmd.LastInsertedId;

                            // Record the registration in the activity log
                            using (MySqlCommand logCmd = new MySqlCommand(
                                @"INSERT INTO activity_logs (actor_type, actor_id, action, target_student_id, details)
      VALUES ('Student', @Id, 'REGISTER', @Id, @Details)", conn))
                            {
                                logCmd.Parameters.AddWithValue("@Id", newId);
                                logCmd.Parameters.AddWithValue("@Details", "Student account '" + username + "' registered");
                                logCmd.ExecuteNonQuery();
                            }

                            registered = true;
                        }
                        else
                        {
                            MessageBox.Show("Failed to register account. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error during registration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open setUp only after the account is saved and the connection is closed
            if (registered)
            {
                MessageBox.Show("Account registered! Please complete your profile details.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SIS.Instance.OpenChildForm(new setUp(username, true));
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);
            ControlRounder.MakeRounded(txtUsername, 6);
            ControlRounder.MakeRounded(txtPassword, 6);
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}