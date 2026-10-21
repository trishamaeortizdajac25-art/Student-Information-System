using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BULi
{
    public partial class setUp : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122;";

        private string currentUsername;
        private bool isSignUpMode;

        public setUp()
        {
            InitializeComponent();
        }

        public setUp(string username, bool isSignUp = false) : this()
        {
            this.currentUsername = username;
            this.isSignUpMode = isSignUp;
        }

        private void setUp_Load(object sender, EventArgs e)
        {
            if (isSignUpMode)
            {
                ClearAllFields();
            }
            else if (!string.IsNullOrEmpty(currentUsername))
            {
                LoadStudentData(currentUsername);
            }
        }

        private void ClearAllFields()
        {
            txtName.Text = "";
            txtID.Text = "";
            txtProgram.Text = "";
            txtYearlvl.Text = "";
            txtBloc.Text = "";
            txtDate.Text = "";
            txtAge.Text = "";
            btnGender.Text = "";
            txtContact.Text = "";
            txtEmailAdd.Text = "";
            pictureBox1.Image = null;
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
                               profile_picture,
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
                                btnGender.Text = reader["gender"].ToString();
                                txtContact.Text = reader["contact_number"].ToString();
                                txtEmailAdd.Text = reader["email"].ToString();

                                // Load Profile Picture
                                if (reader["profile_picture"] != DBNull.Value)
                                {
                                    byte[] imgBytes = (byte[])reader["profile_picture"];
                                    using (MemoryStream ms = new MemoryStream(imgBytes))
                                    {
                                        pictureBox1.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    pictureBox1.Image = null;
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

        // ==========================================================
        // UPDATE BUTTON -> SAVES CHANGES & DIRECTLY OPENS INFOCARD
        // ==========================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string rawName = txtName.Text.Trim();
                string[] nameParts = rawName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                DateTime birthdate;
                bool validDate = DateTime.TryParse(txtDate.Text.Trim(), out birthdate);

                // Convert picturebox image to byte array for MySQL BLOB storage
                byte[] imageBytes = null;
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Clone image to avoid GDI+ lock during save
                        using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        imageBytes = ms.ToArray();
                    }
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
                            email = @Email,
                            profile_picture = @ProfilePic
                        WHERE username = @Username AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", currentUsername ?? "");
                        cmd.Parameters.AddWithValue("@StudentID", txtID.Text.Trim());
                        cmd.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(firstName) ? "N/A" : firstName);
                        cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName);
                        cmd.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(lastName) ? "N/A" : lastName);
                        cmd.Parameters.AddWithValue("@Gender", string.IsNullOrEmpty(btnGender.Text.Trim()) ? "Prefer not to say" : btnGender.Text.Trim());
                        cmd.Parameters.AddWithValue("@Birthdate", validDate ? birthdate.ToString("yyyy-MM-dd") : "2000-01-01");
                        cmd.Parameters.AddWithValue("@YearLevel", string.IsNullOrEmpty(txtYearlvl.Text.Trim()) ? "1st Year" : txtYearlvl.Text.Trim());
                        cmd.Parameters.AddWithValue("@Program", string.IsNullOrEmpty(txtProgram.Text.Trim()) ? "BS Information Technology" : txtProgram.Text.Trim());
                        cmd.Parameters.AddWithValue("@Block", string.IsNullOrEmpty(txtBloc.Text.Trim()) ? "Block A" : txtBloc.Text.Trim());
                        cmd.Parameters.AddWithValue("@Contact", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmailAdd.Text.Trim()) ? (object)DBNull.Value : txtEmailAdd.Text.Trim());
                        cmd.Parameters.AddWithValue("@ProfilePic", imageBytes != null ? (object)imageBytes : DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Directly navigates to infoCard passing active username
                            SIS.Instance.OpenChildForm(new infoCard(currentUsername));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShowCard_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new infoCard(currentUsername));
        }

        // ==========================================================
        // PHOTO DROPDOWN & MANAGEMENT (contextMenuStrip1)
        // ==========================================================
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnPhoto, new Point(0, btnPhoto.Height));
        }

        // Bind this method to the "Upload Photo" item in contextMenuStrip1
        private void uploadPhotoOption_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Select Profile Picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        // Bind this method to the "Delete Photo" / "Remove Photo" item in contextMenuStrip1
        private void deletePhotoOption_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void btnGender_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show(btnGender, new Point(0, btnGender.Height));
        }

        private void genderOption_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;

            if (clickedItem != null)
            {
                btnGender.Text = clickedItem.Text;
            }
        }

        // Dummy handlers for designer bindings
        private void txtBloc_TextChanged(object sender, EventArgs e) { }
        private void txtID_TextChanged(object sender, EventArgs e) { }
        private void txtGender_TextChanged(object sender, EventArgs e) { }
        private void txtYearlvl_TextChanged(object sender, EventArgs e) { }
        private void lbl_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}