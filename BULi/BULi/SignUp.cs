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
<<<<<<< HEAD


        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=nU^Ns3L07_26;";


=======
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122";
>>>>>>> f9799a7003e45930f01a19f439106ed5436f39bb

        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new setUp());
        }
    }
}

