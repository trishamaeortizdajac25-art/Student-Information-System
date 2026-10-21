using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace BULi
{
    public partial class infoCard : Form
    {

        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nunez26*7;";

        private string currentUsername;

        public infoCard()
        {
            InitializeComponent();
        }

        public infoCard(string username) : this()
        {
            this.currentUsername = username;
        }

        private void infoCard_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);

            // Display only: hide the + button
            btnDrop.Visible = false;

            lnkReturn.LinkClicked -= lnkReturn_LinkClicked;
            lnkReturn.LinkClicked += lnkReturn_LinkClicked;

            btnExit.Click -= btnExit_Click;
            btnExit.Click += btnExit_Click;

            if (!string.IsNullOrEmpty(currentUsername))
            {
                LoadStudentCardData(currentUsername);
            }

            lnkReturn.Text = string.Equals(pick.SelectedRole, "Teacher", StringComparison.OrdinalIgnoreCase)
                ? "Return to List"
                : "Return";
        }

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

        private void LoadStudentCardData(string username)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT student_number, first_name, middle_name, last_name,
                               year_level, program, block, contact_number, profile_picture
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

                                string fullName = $"{firstName} {middleName} {lastName}"
                                    .Replace("  ", " ").Trim();

                                lblName.Text = string.IsNullOrEmpty(fullName) ? "N/A" : fullName;
                                lblID.Text = reader["student_number"].ToString();
                                lblProgram.Text = reader["program"].ToString();
                                lblYearlvl.Text = reader["year_level"].ToString();
                                lblBloc.Text = reader["block"].ToString();
                                lblContact.Text = reader["contact_number"].ToString();

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
                            else
                            {
                                MessageBox.Show("Student details could not be found.", "Record Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Student Card: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.Equals(pick.SelectedRole, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                SIS.Instance.OpenChildForm(new display());
            }
            else
            {
                SIS.Instance.OpenChildForm(new setUp(currentUsername, false));
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Confirm Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Application.Exit();
        }
    }
}