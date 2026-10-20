<<<<<<< HEAD
﻿using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
=======
﻿using System;
>>>>>>> 21d713051af4c74e0a1bfd08895d59824b09b2ff
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BULi
{
    public partial class infoCard : Form
    {
<<<<<<< HEAD
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";
=======
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L07_26;";
>>>>>>> 21d713051af4c74e0a1bfd08895d59824b09b2ff
        private string currentUsername;

        // Default constructor
        public infoCard()
        {
            InitializeComponent();
        }
                
        // Parameterized constructor accepting active username (fixes CS1729)
        public infoCard(string username) : this()
        {
            this.currentUsername = username;
        }

        private void infoCard_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentUsername))
            {
                LoadStudentCardData(currentUsername);
            }
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
                               year_level, program, block, contact_number 
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

                                string fullName = $"{firstName} {middleName} {lastName}".Replace("  ", " ").Trim();
                                lblName.Text = string.IsNullOrEmpty(fullName) ? "N/A" : fullName;
                                lblID.Text = reader["student_number"].ToString();
                                lblProgram.Text = reader["program"].ToString();
                                lblYearlvl.Text = reader["year_level"].ToString();
                                lblBloc.Text = reader["block"].ToString();
                                lblContact.Text = reader["contact_number"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Student details could not be found.", "Record Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Student Card: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}