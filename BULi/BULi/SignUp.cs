using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BULi
{
    public partial class SignUp : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=your_mysql_password;";

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

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string checkUserQuery = "SELECT COUNT(*) FROM students WHERE username = @Username";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkUserQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        long userCount = Convert.ToInt64(checkCmd.ExecuteScalar());

                        if (userCount > 0)
                        {
                            MessageBox.Show("Username is already taken. Please choose another one.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string hashedPassword = ComputeSha256Hash(password);

                    string insertQuery = @"
                        INSERT INTO students 
                        (username, password_hash, first_name, last_name, is_deleted, created_at) 
                        VALUES 
                        (@Username, @PasswordHash, 'New', 'Student', 0, NOW())";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        int rowsAffected = insertCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Account registered successfully! Please complete your profile details.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Opens setUp form in Sign-Up mode (isSignUp = true) to keep textboxes blank
                            SIS.Instance.OpenChildForm(new setUp(username, true));
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
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void btnSignInRedirect_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new stdSignIn());
        }

        // Handles designer event bindings
        private void SignUp_Load(object sender, EventArgs e) { }
        private void SignIn_Load(object sender, EventArgs e) { }
    }
}

