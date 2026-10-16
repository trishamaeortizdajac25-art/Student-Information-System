using System;
using System.Data;
using System.Data.SqlClient; // Adjust namespace if using MySql, OleDb, etc.
using System.Windows.Forms;

namespace BULi
{
    public enum FormMode
    {
        SignIn,
        CreateAccount
    }

    public partial class setUp : Form
    {
        private FormMode currentMode;
        private string studentId;

        // Constructor for Sign In (View/Display mode)
        public setUp(string studentId)
        {
            InitializeComponent();
            this.currentMode = FormMode.SignIn;
            this.studentId = studentId;
        }

        // Constructor for Create Account (New Input mode)
        public setUp()
        {
            InitializeComponent();
            this.currentMode = FormMode.CreateAccount;
        }

        private void setUp_Load(object sender, EventArgs e)
        {
            if (currentMode == FormMode.SignIn)
            {
                // Display existing data from database
                SetFieldsReadOnly(true);
                btnCreate.Text = "Proceed"; // Or "Continue"
                LoadStudentData(studentId);
            }
            else if (currentMode == FormMode.CreateAccount)
            {
                // Prepare form for fresh inputs
                SetFieldsReadOnly(false);
                btnCreate.Text = "Save & Create";
                ClearFields();
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (currentMode == FormMode.CreateAccount)
            {
                // Save new record to the database
                bool isSaved = SaveNewStudent();
                if (!isSaved) return;
            }

            // Navigate to the next form
            SIS.Instance.OpenChildForm(new infoCard());
        }

        private void SetFieldsReadOnly(bool isReadOnly)
        {
            txtName.ReadOnly = isReadOnly;
            txtID.ReadOnly = isReadOnly;
            txtProgram.ReadOnly = isReadOnly;
            txtYearlvl.ReadOnly = isReadOnly;
            txtBloc.ReadOnly = isReadOnly;
            txtDate.ReadOnly = isReadOnly;
            txtAge.ReadOnly = isReadOnly;
            txtGender.ReadOnly = isReadOnly;
            txtContact.ReadOnly = isReadOnly;
            txtEmailAdd.ReadOnly = isReadOnly;
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtID.Clear();
            txtProgram.Clear();
            txtYearlvl.Clear();
            txtBloc.Clear();
            txtDate.Clear();
            txtAge.Clear();
            txtGender.Clear();
            txtContact.Clear();
            txtEmailAdd.Clear();
        }

        private void LoadStudentData(string id)
        {
            // Database query logic goes here
            // Example:
            // string query = "SELECT * FROM Students WHERE StudentID = @id";
            // Read values and populate textboxes:
            // txtName.Text = reader["Name"].ToString();
            // txtID.Text = reader["StudentID"].ToString();
            // ...
        }

        private bool SaveNewStudent()
        {
            // Database insert logic goes here
            // Example:
            // string query = "INSERT INTO Students (...) VALUES (...)";
            return true;
        }

        // Event handlers left clean
        private void txtName_TextChanged(object sender, EventArgs e) { }
        private void txtID_TextChanged(object sender, EventArgs e) { }
        private void txtProgram_TextChanged(object sender, EventArgs e) { }
        private void txtYearlvl_TextChanged(object sender, EventArgs e) { }
        private void txtBloc_TextChanged(object sender, EventArgs e) { }
        private void txtDate_TextChanged(object sender, EventArgs e) { }
        private void txtAge_TextChanged(object sender, EventArgs e) { }
        private void txtGender_TextChanged(object sender, EventArgs e) { }
        private void txtContact_TextChanged(object sender, EventArgs e) { }
        private void txtEmailAdd_TextChanged(object sender, EventArgs e) { }
    }
}
