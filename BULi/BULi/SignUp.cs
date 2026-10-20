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


        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122";


=======
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";
>>>>>>> 675f73809431e97523c0e17e81706cc609515221

        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new setUp());
        }
    }
}

