using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BULi
{
    public partial class SignUp : Form
    {



        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L07_26;";


  


        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new setUp());
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);
            ControlRounder.MakeRounded(txtUsername,6);
            ControlRounder.MakeRounded(txtPassword, 6);
            ControlRounder.MakeRounded(btnSignIn, 6);

        }
    }
}

