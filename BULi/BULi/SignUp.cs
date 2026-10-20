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
<<<<<<< HEAD
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";
=======
     HEAD
        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=remia2006;";

        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=Trishamae1122;";
     d2ec6d6 (updated code)
>>>>>>> 6c0d2c86e67e5c833cd34ddbee3b8729b0d96498
=======
        
>>>>>>> 063135f (buttons)

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

