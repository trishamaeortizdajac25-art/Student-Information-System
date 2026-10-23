using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BULi
{
   
    public partial class tcrInfoCard : Form
    {
        private const string ConnStr = "Server=localhost;Database=sis_db;Uid=root;Pwd=nunez26*7;";
        private const string NotYet = "Not yet updated";

        private readonly string _username;
        private readonly string _yearLevel;   // title name sent by display, e.g. "Senior", so Return goes back to the same list

        public tcrInfoCard()
        {
            InitializeComponent();
        }

        public tcrInfoCard(string username, string yearLevel = null) : this()
        {
            _username = username;
            _yearLevel = yearLevel;
        }

        private void tcrInfoCard_Load(object sender, EventArgs e)  
        {
            ControlRounder.MakeRounded(panel1, 16);
            LoadCard();
        }

        // =====================================================
        //  LOAD STUDENT DATA
        // =====================================================
        private void LoadCard()
        {
            if (string.IsNullOrEmpty(_username)) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    string sql = @"
                        SELECT student_number, first_name, middle_name, last_name, gender,
                               birthdate, year_level, program, block, contact_number, email,
                               profile_picture,
                               TIMESTAMPDIFF(YEAR, birthdate, CURDATE()) AS calculated_age
                        FROM students
                        WHERE username = @u AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", _username);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                        {
                            if (!r.Read())
                            {
                                MessageBox.Show("Student record not found.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // ---- filled in by the teacher ----
                            string first = r["first_name"].ToString();
                            string middle = r["middle_name"].ToString();
                            string last = r["last_name"].ToString();
                            lblName.Text = (first + " " + middle + " " + last)
                                .Replace("  ", " ").Trim();
                            lblID.Text = Or(r["student_number"]);
                            lblYearlvl.Text = Or(r["year_level"]);
                            lblBloc.Text = Or(r["block"]);
                            lblProgram.Text = Or(r["program"]);

                            // ---- filled in by the student (setUp) ----
                            // birthdate is required when the student saves their profile,
                            // so a NULL birthdate means the student has not completed it yet.
                            bool profileDone = r["birthdate"] != DBNull.Value;

                            lblAge.Text = profileDone ? Or(r["calculated_age"]) : NotYet;
                            lblGender.Text = profileDone ? Or(r["gender"]) : NotYet;
                            lblContact.Text = Or(r["contact_number"]);
                            lblEmailAdd.Text = Or(r["email"]);

                            ShowPhoto(r["profile_picture"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student information: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // empty / null -> "Not yet updated"
        private static string Or(object value)
        {
            if (value == null || value == DBNull.Value) return NotYet;
            string s = value.ToString().Trim();
            return s.Length == 0 ? NotYet : s;
        }

        private void ShowPhoto(object dbValue)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            if (dbValue != DBNull.Value)
            {
                try
                {
                    byte[] bytes = (byte[])dbValue;
                    using (MemoryStream ms = new MemoryStream(bytes))
                    using (Image temp = Image.FromStream(ms))
                    {
                        pictureBox1.Image = new Bitmap(temp);
                    }
                    return;
                }
                catch
                {
                    // fall through to the placeholder
                }
            }

            // no photo yet: draw a small "Not yet updated" placeholder
            int w = Math.Max(pictureBox1.Width, 10);
            int h = Math.Max(pictureBox1.Height, 10);
            Bitmap placeholder = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(placeholder))
            using (Font f = new Font("Segoe UI", 9f, FontStyle.Italic))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.Clear(Color.FromArgb(176, 176, 176));
                g.DrawString("Photo\nnot yet updated", f, Brushes.White, new RectangleF(0, 0, w, h), sf);
            }
            pictureBox1.Image = placeholder;
        }

        // =====================================================
        //  BUTTONS / PAINT
        // =====================================================
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient(panel1, e, 45f);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }

        // "Return to ..." -> back to the register list of the same year level
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_yearLevel))
                SIS.Instance.OpenChildForm(new display());
            else
                SIS.Instance.OpenChildForm(new display(_yearLevel));
        }

        // =====================================================
        //  DESIGNER-WIRED HANDLERS (kept so the designer still compiles)
        // =====================================================
        private void label5_Click(object sender, EventArgs e) { }
        private void lblContact_Click(object sender, EventArgs e) { }
        private void lblGender_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void lblAge_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void lblYearlvl_Click(object sender, EventArgs e) { }
        private void lblID_Click(object sender, EventArgs e) { }
        private void lblBloc_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void lblName_Click(object sender, EventArgs e) { }
        private void lblProgram_Click(object sender, EventArgs e) { }
        private void lblEmailAdd_Click(object sender, EventArgs e) { }
    }
}