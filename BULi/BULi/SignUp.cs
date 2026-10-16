
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BULi
{
    public partial class SignUp : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L07_26;";

        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, EventArgs e) // Sign Up Button
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string hashedPassword = ComputeSha256Hash(password);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Check if username exists
                    string checkQuery = "SELECT COUNT(1) FROM students WHERE username = @Username";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            MessageBox.Show("Username already taken. Please choose another.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // 2. Generate Next Student Number (e.g. 2026-0001)
                    string genNumQuery = "SELECT CONCAT(YEAR(CURDATE()), '-', LPAD(IFNULL(MAX(CAST(SUBSTRING(student_number, 6) AS UNSIGNED)), 0) + 1, 4, '0')) FROM students WHERE student_number LIKE CONCAT(YEAR(CURDATE()), '-%')";
                    string nextStudentNum = "2026-0001";
                    using (MySqlCommand numCmd = new MySqlCommand(genNumQuery, conn))
                    {
                        object result = numCmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            nextStudentNum = result.ToString();
                        }
                    }

                    // 3. Insert initial student record matching strict foreign keys
                    string insertQuery = @"
                        INSERT INTO students 
                        (student_number, first_name, last_name, gender, birthdate, year_level, program, block, contact_number, username, password_hash)
                        VALUES 
                        (@StudentNumber, 'New', 'Student', 'Male', '2000-01-01', 'Freshman', 'BS Information Technology', 'Block A', '00000000000', @Username, @PasswordHash)";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@StudentNumber", nextStudentNum);
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        insertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Account created successfully! Please complete your setup details.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SIS.Instance.OpenChildForm(new setUp(username));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void SignIn_Load(object sender, EventArgs e) { }
    }
}

