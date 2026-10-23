using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static BULi.SIS;

namespace BULi
{
    public partial class setUp : Form
    {


        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nunez26*7;";




        private string currentUsername;
        private bool isSignUpMode;

        public setUp()
        {
            InitializeComponent();

            // Subscribe to global theme changes and apply initial theme
            ThemeState.ThemeChanged += OnThemeChanged;
            ApplyTheme(ThemeState.IsDarkMode);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ThemeState.ThemeChanged -= OnThemeChanged;
            base.OnFormClosed(e);
        }

        private void OnThemeChanged(object sender, bool isDark)
        {
            ApplyTheme(isDark);
        }

        private void ApplyTheme(bool isDark)
        {
            Color navyDarkMode = Color.FromArgb(15, 23, 42);

            if (isDark)
            {
                this.BackgroundImage = Properties.Resources.darknull;
                this.BackgroundImageLayout = ImageLayout.Stretch;

                // Style pnlGlassCard to navy dark mode
                pnlGlassCard.BackColor = navyDarkMode;
            }
            else
            {
                this.BackgroundImage = Properties.Resources._default;
                this.BackgroundImageLayout = ImageLayout.Stretch;

                // Revert pnlGlassCard back to transparent so its Paint event handles the sunset gradient normally
                pnlGlassCard.BackColor = Color.Transparent;
            }

            this.Invalidate(true);
        }

        public setUp(string username, bool isSignUp = false) : this()
        {
            this.currentUsername = username;
            this.isSignUpMode = isSignUp;
        }

        private void setUp_Load(object sender, EventArgs e)
        {
            this.ActiveControl = null;

            TextBoxHelper.AddPlaceholder(txtName, "e.g. Juan Santos");
            TextBoxHelper.AddPlaceholder(txtDate, "YYYY-MM-DD");
            TextBoxHelper.AddPlaceholder(txtYearlvl, "e.g. 1st Year");
            TextBoxHelper.AddPlaceholder(txtProgram, "e.g. BS Biology");
            TextBoxHelper.AddPlaceholder(txtID, "Enter Student ID");


            WireControls();

            if (isSignUpMode)
            {
                ClearAllFields();
            }
            else if (!string.IsNullOrEmpty(currentUsername))
            {
                LoadStudentData(currentUsername);
            }

            ControlRounder.MakeRounded(btnUpdate, 8);
            ControlRounder.MakeRounded(btnShowCard, 8);
            ControlRounder.MakeRounded(pnlGlassCard, 16);
        }

        // Connect events in code (unsubscribe first to avoid double firing)
        private void WireControls()
        {
            btnPhoto.Click -= btnPhoto_Click;
            btnPhoto.Click += btnPhoto_Click;

            btnGender.Click -= btnGender_Click;
            btnGender.Click += btnGender_Click;

            btnUpdate.Click -= btnSave_Click;
            btnUpdate.Click += btnSave_Click;

            btnShowCard.Click -= btnShowCard_Click;
            btnShowCard.Click += btnShowCard_Click;

            bool uploadWired = false, deleteWired = false;

            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                if (item is ToolStripSeparator) continue;

                string t = (item.Text ?? "").ToLower();

                if (t.Contains("delete") || t.Contains("remove") || t.Contains("clear"))
                {
                    item.Click -= deletePhotoOption_Click;
                    item.Click += deletePhotoOption_Click;
                    deleteWired = true;
                }
                else if (t.Contains("upload") || t.Contains("add") || t.Contains("change") ||
                         t.Contains("choose") || t.Contains("update") || t.Contains("select"))
                {
                    item.Click -= uploadPhotoOption_Click;
                    item.Click += uploadPhotoOption_Click;
                    uploadWired = true;
                }
            }

            // Fallback: first item = upload, second = delete
            if (!uploadWired && !deleteWired && contextMenuStrip1.Items.Count >= 2)
            {
                contextMenuStrip1.Items[0].Click += uploadPhotoOption_Click;
                contextMenuStrip1.Items[1].Click += deletePhotoOption_Click;
            }

            foreach (ToolStripItem item in contextMenuStrip2.Items)
            {
                item.Click -= genderOption_Click;
                item.Click += genderOption_Click;
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
            SetPicture(null);
        }

        // Keeps the largest centered square and resizes it to size x size
        private Bitmap CropToSquare(Image src, int size = 600)
        {
            int side = Math.Min(src.Width, src.Height);
            int x = (src.Width - side) / 2;
            int y = (src.Height - side) / 2;

            Bitmap result = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(src,
                    new Rectangle(0, 0, size, size),
                    new Rectangle(x, y, side, side),
                    GraphicsUnit.Pixel);
            }
            return result;
        }

        private void SetPicture(Image img)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // ==========================================================
        // LOAD (returning users / editing)
        // ==========================================================
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

                                txtName.Text = string.Join(" ",
                                    new[] { firstName, middleName, lastName }
                                        .Where(p => !string.IsNullOrWhiteSpace(p))).Trim();
                                txtID.Text = reader["student_number"].ToString();
                                txtProgram.Text = reader["program"].ToString();
                                txtYearlvl.Text = reader["year_level"].ToString();
                                txtBloc.Text = reader["block"].ToString();

                                if (reader["birthdate"] != DBNull.Value)
                                {
                                    txtDate.Text = Convert.ToDateTime(reader["birthdate"]).ToString("yyyy-MM-dd");
                                }

                                // birthdate is NULL until the student completes this form for the first time.
                                // Accounts created by the teacher start with a placeholder gender,
                                // so leave gender/age blank until then and let the student choose.
                                bool profileDone = reader["birthdate"] != DBNull.Value;

                                txtAge.Text = profileDone ? reader["calculated_age"].ToString() : "";
                                btnGender.Text = profileDone ? reader["gender"].ToString() : "";
                                txtContact.Text = reader["contact_number"].ToString();
                                txtEmailAdd.Text = reader["email"].ToString();

                                if (reader["profile_picture"] != DBNull.Value)
                                {
                                    byte[] imgBytes = (byte[])reader["profile_picture"];
                                    using (MemoryStream ms = new MemoryStream(imgBytes))
                                    using (Image temp = Image.FromStream(ms))
                                    {
                                        SetPicture(CropToSquare(temp));
                                    }
                                }
                                else
                                {
                                    SetPicture(null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading details: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================================
        // SAVE (Update button) -> saves everything, then opens infoCard
        // ==========================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentUsername))
            {
                MessageBox.Show("No active account. Please sign up or sign in again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string rawName = txtName.Text.Trim();
            string[] nameParts = rawName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length < 2)
            {
                MessageBox.Show("Please enter your full name (first and last name).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string studentId = txtID.Text.Trim();
            string program = txtProgram.Text.Trim();
            string yearLevel = txtYearlvl.Text.Trim();
            string block = txtBloc.Text.Trim();
            string gender = btnGender.Text.Trim();
            string contact = txtContact.Text.Trim();
            string email = txtEmailAdd.Text.Trim();

            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(program) ||
                string.IsNullOrEmpty(yearLevel) || string.IsNullOrEmpty(block) ||
                string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(contact))
            {
                MessageBox.Show("Please fill out all required fields (email is optional).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime birthdate;
            if (!DateTime.TryParse(txtDate.Text.Trim(), out birthdate) || birthdate > DateTime.Today)
            {
                MessageBox.Show("Please enter a valid birthdate (e.g. 2004-05-21).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string firstName = nameParts[0];
            string lastName = nameParts[nameParts.Length - 1];
            string middleName = nameParts.Length > 2
                ? string.Join(" ", nameParts, 1, nameParts.Length - 2)
                : "";

            // Picture -> byte[] (already 1x1 from CropToSquare)
            byte[] imageBytes = null;
            if (pictureBox1.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    using (MySqlCommand dupCmd = new MySqlCommand(
                        @"SELECT COUNT(*) FROM students
                          WHERE student_number = @StudentID
                            AND username <> @Username
                            AND is_deleted = 0", conn))
                    {
                        dupCmd.Parameters.AddWithValue("@StudentID", studentId);
                        dupCmd.Parameters.AddWithValue("@Username", currentUsername);

                        if (Convert.ToInt64(dupCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("This student number is already registered to another account.",
                                "Duplicate Student Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string updateQuery = @"
                        UPDATE students
                        SET student_number  = @StudentID,
                            first_name      = @FirstName,
                            middle_name     = @MiddleName,
                            last_name       = @LastName,
                            gender          = @Gender,
                            birthdate       = @Birthdate,
                            year_level      = @YearLevel,
                            program         = @Program,
                            block           = @Block,
                            contact_number  = @Contact,
                            email           = @Email,
                            profile_picture = @ProfilePic
                        WHERE username = @Username AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", currentUsername);
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@MiddleName",
                            string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Birthdate", birthdate.Date);
                        cmd.Parameters.AddWithValue("@YearLevel", yearLevel);
                        cmd.Parameters.AddWithValue("@Program", program);
                        cmd.Parameters.AddWithValue("@Block", block);
                        cmd.Parameters.AddWithValue("@Contact", contact);
                        cmd.Parameters.AddWithValue("@Email",
                            string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                        cmd.Parameters.Add("@ProfilePic", MySqlDbType.LongBlob).Value =
                            imageBytes != null ? (object)imageBytes : DBNull.Value;

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            int age = DateTime.Today.Year - birthdate.Year;
                            if (birthdate.Date > DateTime.Today.AddYears(-age)) age--;
                            txtAge.Text = age.ToString();

                            MessageBox.Show("Profile saved successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            SIS.Instance.OpenChildForm(new infoCard(currentUsername));
                        }
                        else
                        {
                            MessageBox.Show("No account was updated. Please sign in again and retry.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving details: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShowCard_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new infoCard(currentUsername));
        }

        // ==========================================================
        // PHOTO: upload / change / delete (saved when Update is clicked)
        // ==========================================================
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnPhoto, new Point(0, btnPhoto.Height));
        }

        private void uploadPhotoOption_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                dlg.Title = "Select Profile Picture";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    // Works whether or not a picture already exists; replaces it
                    using (Image temp = Image.FromFile(dlg.FileName))
                    {
                        SetPicture(CropToSquare(temp));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load image: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deletePhotoOption_Click(object sender, EventArgs e)
        {
            SetPicture(null);
        }

        // ==========================================================
        // GENDER DROPDOWN
        // ==========================================================
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

        private void pnlGlassCard_Paint(object sender, PaintEventArgs e)
        {
            if (!ThemeState.IsDarkMode)
                UIcolor.ApplySunsetGradient((Control)sender, e);
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

        private void btnTheme_Click(object sender, EventArgs e)
        {
            ThemeState.ToggleTheme();

            if (ThemeState.IsDarkMode)
            {
                this.BackgroundImage = Properties.Resources.darknull;
                this.BackgroundImageLayout = ImageLayout.Stretch;

                // Dark Mode: Change button to Navy Blue
                btnTheme.BackColor = Color.FromArgb(15, 23, 42);
                btnTheme.ForeColor = Color.White;
            }
            else
            {
                this.BackgroundImage = Properties.Resources._default; // Or your light background
                this.BackgroundImageLayout = ImageLayout.Stretch;

                // Day/Light Mode: Change button to Orange (or whatever your light theme color is)
                btnTheme.BackColor = Color.FromArgb(254, 180, 123);
                btnTheme.ForeColor = Color.Black;
            }
        }


    }
}