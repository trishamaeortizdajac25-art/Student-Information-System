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

        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122";



        public tcrSignIn()

        {

            InitializeComponent();

        }



        private void btnEnter_Click(object sender, EventArgs e)

        {

            // Using your exact control names: txtName and txtPassword

            string username = txtName.Text.Trim();

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



                    // Query the 'teachers' table for an active user matching username and password hash

                    string query = @"

SELECT username

FROM teachers

WHERE username = @Username

AND password_hash = @PasswordHash

AND is_active = 1";



                    using (MySqlCommand cmd = new MySqlCommand(query, conn))

                    {

                        cmd.Parameters.AddWithValue("@Username", username);

                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);



                        using (MySqlDataReader reader = cmd.ExecuteReader())

                        {

                            if (reader.Read())

                            {

                                // Set global role state

                                pick.SelectedRole = "Teacher";



                                MessageBox.Show("Sign in successful! Welcome Teacher.", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);



                                // Navigate directly to HomePage

                                SIS.Instance.OpenChildForm(new HomePage());

                            }

                            else

                            {

                                MessageBox.Show("Invalid teacher credentials or account is inactive.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);

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



        private void tcrSignIn_Load(object sender, EventArgs e) { }

    }

}