using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BULi
{
    public partial class tcrSignIn : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L*07_26;";

        public tcrSignIn()
        {
            InitializeComponent();
        }

        // Helper method to hash the password in C# perfectly matching MySQL's SHA2(256)
        private string ComputeSHA256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" makes it lowercase hex, matching MySQL exactly
                }
                return builder.ToString();
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new pick());

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }

        private void tcrSignIn_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);
            ControlRounder.MakeRounded(btnReturn, 8);
            ControlRounder.MakeRounded(btnEnter, 8);
            ControlRounder.MakeRounded(txtName, 8);
            ControlRounder.MakeRounded(txtPassword, 8);
        }


        private void btnEnter_Click(object sender, EventArgs e)
        {
            string username = txtName.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Retrieve the hash and status based ONLY on the username to bypass MySQL driver bugs
                    string query = "SELECT password_hash, is_active FROM teachers WHERE username = @Username";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // If the username exists in the database
                            if (reader.Read())
                            {
                                string dbHash = reader["password_hash"].ToString();
                                bool isActive = Convert.ToBoolean(reader["is_active"]);

                                string inputHash = ComputeSHA256Hash(password);

                                // Check if the entered password matches the database hash
                                if (dbHash.Equals(inputHash, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Check if the account is active
                                    if (isActive)
                                    {
                                        pick.SelectedRole = "Teacher";
                                        MessageBox.Show("Sign in successful! Welcome Teacher.", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        SIS.Instance.OpenChildForm(new HomePage());
                                    }
                                    else
                                    {
                                        MessageBox.Show("Your account is inactive.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Invalid password.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username not found.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connection Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       

        private void txtName_TextChanged(object sender, EventArgs e) { }

  

        private void txtPassword_TextChanged(object sender, EventArgs e) { }
    }
}