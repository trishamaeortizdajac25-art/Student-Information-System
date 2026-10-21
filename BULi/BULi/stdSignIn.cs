using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BULi
{
    public partial class stdSignIn : Form
    {
<<<<<<< HEAD
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L*07_26;";
=======
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";
>>>>>>> 0f7bc033b31dab9c0e9360c7270eb7e67c284ab4

        public stdSignIn()
        {
            InitializeComponent();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string hashedPassword = ComputeSha256Hash(password);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT username 
                        FROM students 
                        WHERE username = @Username 
                          AND password_hash = @PasswordHash 
                          AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string activeUsername = reader["username"].ToString();
                                MessageBox.Show("Sign in successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                SIS.Instance.OpenChildForm(new setUp(activeUsername, false));
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSignUpRDT_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new SignUp());
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

        private void txtUsername_TextChanged(object sender, EventArgs e) { }
        private void txtPassword_TextChanged(object sender, EventArgs e) { }
        private void stdSignIn_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(pnlPlaceholder, 20);
            ControlRounder.MakeRounded(txtUsername, 6);
            ControlRounder.MakeRounded(txtPassword, 6);
            ControlRounder.MakeRounded(btnSignUp, 8);
            ControlRounder.MakeRounded(btnReturn, 8);


        }

        private void lnkSignUpRDT_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SIS.Instance.OpenChildForm(new SignUp());
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new pick());

        }

        private void pnlPlaceholder_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }



    }
}