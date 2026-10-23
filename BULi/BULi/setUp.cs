using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Windows.Forms;
using static BULi.SIS;

namespace BULi
{
    public partial class setUp : Form
    {
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nunez26*7;";

        private string currentUsername;
        private bool isSignUpMode;

        private const int ContactLength = 11;
        private const int AgeMaxLength = 2;

        private bool isExpanded = false;

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
            TextBoxHelper.AddPlaceholder(txtProgram, "e.g. BS Biology");
            TextBoxHelper.AddPlaceholder(txtID, "Enter Student ID");
            TextBoxHelper.AddPlaceholder(txtDate, "YYYY-MM-DD");
            TextBoxHelper.AddPlaceholder(txtYearlvl, "e.g. 1st Year");
            TextBoxHelper.AddPlaceholder(txtContact, "e.g. 09123456789");

            pnlSidebar.Width = 0;

            WireControls();
            txtNewUsername.Text = currentUsername;

            btnSaveAccount.Click -= btnSaveAccount_Click;
            btnSaveAccount.Click += btnSaveAccount_Click;

            btnCancelAccount.Click -= btnCancelAccount_Click;
            btnCancelAccount.Click += btnCancelAccount_Click;

            chkShow.CheckedChanged -= chkShow_CheckedChanged;
            chkShow.CheckedChanged += chkShow_CheckedChanged;
            chkShow_CheckedChanged(null, EventArgs.Empty);

            btnLogout.Click -= btnLogout_Click;
            btnLogout.Click += btnLogout_Click;
            ControlRounder.MakeRounded(btnLogout, 8);

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

            // Contact number: digits only, never more than 13
            txtContact.MaxLength = ContactLength;
            txtContact.KeyPress -= txtContact_KeyPress;
            txtContact.KeyPress += txtContact_KeyPress;
            txtContact.TextChanged -= txtContact_Sanitize;
            txtContact.TextChanged += txtContact_Sanitize;

            // Student ID: no letters (digits and symbols allowed)
            txtID.KeyPress -= txtID_KeyPress;
            txtID.KeyPress += txtID_KeyPress;

            // Age: digits only, max 2
            txtAge.MaxLength = AgeMaxLength;
            txtAge.KeyPress -= txtAge_KeyPress;
            txtAge.KeyPress += txtAge_KeyPress;
            txtAge.TextChanged -= txtAge_Sanitize;
            txtAge.TextChanged += txtAge_Sanitize;

            // Birthdate: digits and '-' only, max 10 (YYYY-MM-DD)
            txtDate.MaxLength = 10;
            txtDate.KeyPress -= txtDate_KeyPress;
            txtDate.KeyPress += txtDate_KeyPress;

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

            if (!IsValidStudentId(studentId))
            {
                MessageBox.Show("Student ID must not contain letters or spaces (numbers and symbols only).",
                    "Invalid Student ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtID.Focus();
                return;
            }

            // contact: exactly 13 digits (0-9 only), and not all zeros
            if (!IsValidContact(contact))
            {
                MessageBox.Show("Contact number must be exactly " + ContactLength + " digits.",
                    "Invalid Contact Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return;
            }

            DateTime birthdate;
            if (!DateTime.TryParseExact(txtDate.Text.Trim(), "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out birthdate)
                || birthdate > DateTime.Today)
            {
                MessageBox.Show("Please enter a valid birthdate in YYYY-MM-DD format (e.g. 2004-05-21).", "Validation Error",
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
        // CONTACT NUMBER: 13 digits, positive whole numbers only
        // ==========================================================
        private static bool IsValidContact(string value)
        {
            if (value == null || value.Length != ContactLength) return false;

            bool allZero = true;
            foreach (char c in value)
            {
                if (c < '0' || c > '9') return false;   // no letters, '-', '+', '.', spaces
                if (c != '0') allZero = false;
            }
            return !allZero;
        }

        // blocks anything that is not 0-9 as it is typed (Backspace etc. still work)
        private void txtContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        // cleans pasted / programmatically set text: keep digits only, cut to 13
        private bool _sanitizingContact = false;
        private void txtContact_Sanitize(object sender, EventArgs e)
        {
            if (_sanitizingContact) return;

            string digits = new string(txtContact.Text.Where(c => c >= '0' && c <= '9').ToArray());
            if (digits.Length > ContactLength) digits = digits.Substring(0, ContactLength);

            if (digits != txtContact.Text)
            {
                _sanitizingContact = true;
                txtContact.Text = digits;
                txtContact.SelectionStart = txtContact.Text.Length;
                _sanitizingContact = false;
            }
        }

        // Student ID: no letters and no spaces; digits and symbols (e.g. -, /) are allowed
        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)))
                e.Handled = true;
        }

        private static bool IsValidStudentId(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            if (id.Any(c => char.IsLetter(c) || char.IsWhiteSpace(c))) return false;
            return id.Any(char.IsDigit);   // symbols alone (e.g. "---") are not an ID
        }

        // ==========================================================
        // AGE: digits only, max 2
        // ==========================================================
        private void txtAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private bool _sanitizingAge = false;
        private void txtAge_Sanitize(object sender, EventArgs e)
        {
            if (_sanitizingAge) return;

            string digits = new string(txtAge.Text.Where(c => c >= '0' && c <= '9').ToArray());
            if (digits.Length > AgeMaxLength) digits = digits.Substring(0, AgeMaxLength);

            if (digits != txtAge.Text)
            {
                _sanitizingAge = true;
                txtAge.Text = digits;
                txtAge.SelectionStart = txtAge.Text.Length;
                _sanitizingAge = false;
            }
        }

        // ==========================================================
        // BIRTHDATE: only digits and '-' while typing (format YYYY-MM-DD, checked on save)
        // ==========================================================
        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && e.KeyChar != '-' && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
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
        private void label9_Click(object sender, EventArgs e) { }

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

        private void pnlSidebar_Paint(object sender, PaintEventArgs e)
        {
            if (!ThemeState.IsDarkMode)
                UIcolor.ApplySunsetGradient((Control)sender, e);
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            isExpanded = !isExpanded;

            if (isExpanded)
            {
                pnlSidebar.Width = 230;
                pnlSidebar.Visible = true;
            }
            else
            {
                pnlSidebar.Width = 0;
                pnlSidebar.Visible = false;
            }
        }

        // ==========================================================
        // ACCOUNT: change username / password (sidebar)
        // ==========================================================
        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {
            bool hide = !chkShow.Checked;
            foreach (TextBox tb in new[] { txtCurrentPassword, txtNewPassword, txtConfirmPassword })
            {
                tb.UseSystemPasswordChar = false;
                tb.PasswordChar = hide ? '•' : '\0';
                tb.Refresh();
            }
        }

        private void btnCancelAccount_Click(object sender, EventArgs e)
        {
            ResetAccountFields();
            CloseSidebar();
        }

        private void ResetAccountFields()
        {
            txtNewUsername.Text = currentUsername;
            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();      // FIXED: was clearing txtCurrentPassword twice
            chkShow.Checked = false;
        }

        private void CloseSidebar()
        {
            isExpanded = false;
            pnlSidebar.Width = 0;
            pnlSidebar.Visible = false;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // FIXED: rewritten as one clean method with unique command names
        private void btnSaveAccount_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentUsername))
            {
                MessageBox.Show("No active account. Please sign in again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // same trimming as stdSignIn / SignUp
            string newUser = txtNewUsername.Text.Trim();
            string currentPass = txtCurrentPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirm = txtConfirmPassword.Text.Trim();   // FIXED: was reading txtCurrentPassword

            if (newUser.Length == 0)
            {
                MessageBox.Show("Username cannot be empty.", "Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newUser.Contains(" "))
            {
                MessageBox.Show("Username cannot contain spaces.", "Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (currentPass.Length == 0)
            {
                MessageBox.Show("Please enter your current password to confirm the change.", "Account",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool changeUser = !string.Equals(newUser, currentUsername, StringComparison.Ordinal);
            bool changePass = newPass.Length > 0;

            if (changePass)
            {
                if (newPass.Length < 6)
                {
                    MessageBox.Show("New password must be at least 6 characters.", "Account",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newPass != confirm)
                {
                    MessageBox.Show("New password and confirmation do not match.", "Account",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!changeUser && !changePass)
            {
                MessageBox.Show("Nothing to change.", "Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 1) verify the current password (same hashing as stdSignIn)
                    long studentId;
                    string storedHash;
                    using (MySqlCommand verifyCmd = new MySqlCommand(
                        "SELECT student_id, password_hash FROM students WHERE username = @u AND is_deleted = 0", conn))
                    {
                        verifyCmd.Parameters.AddWithValue("@u", currentUsername);
                        using (MySqlDataReader rd = verifyCmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("Account not found for username \"" + currentUsername + "\". Please sign in again.",
                                    "Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            studentId = Convert.ToInt64(rd["student_id"]);
                            storedHash = rd["password_hash"].ToString();
                        }
                    }

                    if (!string.Equals(storedHash, ComputeSha256Hash(currentPass), StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Current password is incorrect.", "Account",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 2) new username must be free
                    if (changeUser)
                    {
                        using (MySqlCommand checkCmd = new MySqlCommand(
                            "SELECT COUNT(*) FROM students WHERE username = @n AND student_id <> @id", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@n", newUser);
                            checkCmd.Parameters.AddWithValue("@id", studentId);
                            if (Convert.ToInt64(checkCmd.ExecuteScalar()) > 0)
                            {
                                MessageBox.Show("That username is already taken. Please choose another one.", "Account",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // 3) save
                    string sql = "UPDATE students SET username = @n";
                    if (changePass) sql += ", password_hash = @nh";
                    sql += " WHERE student_id = @id AND is_deleted = 0";

                    using (MySqlCommand saveCmd = new MySqlCommand(sql, conn))
                    {
                        saveCmd.Parameters.AddWithValue("@n", newUser);
                        if (changePass) saveCmd.Parameters.AddWithValue("@nh", ComputeSha256Hash(newPass));
                        saveCmd.Parameters.AddWithValue("@id", studentId);
                        if (saveCmd.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("Nothing was updated. Please sign in again and retry.", "Account",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // 4) log it (never the password itself)
                    string what = changeUser && changePass ? "username and password changed"
                                : changeUser ? "username changed"
                                : "password changed";
                    try
                    {
                        using (MySqlCommand logCmd = new MySqlCommand(
                            @"INSERT INTO activity_logs (actor_type, actor_id, action, target_student_id, details)
                              VALUES ('Student', @id, 'EDIT', @id, @d)", conn))
                        {
                            logCmd.Parameters.AddWithValue("@id", studentId);
                            logCmd.Parameters.AddWithValue("@d", "Student " + what + " (" + currentUsername + " -> " + newUser + ")");
                            logCmd.ExecuteNonQuery();
                        }
                    }
                    catch { /* logging must never break the main action */ }
                }

                // keep the form in sync, otherwise the profile Save would look for the old username
                currentUsername = newUser;
                ResetAccountFields();
                CloseSidebar();

                MessageBox.Show("Account updated successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("That username is already taken. Please choose another one.", "Account",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating account: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Log Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            // clear the session on this form, then go back to the student sign-in
            currentUsername = null;
            ResetAccountFields();
            CloseSidebar();

            SIS.Instance.OpenChildForm(new stdSignIn());
        }

        private void txtCurrentPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}