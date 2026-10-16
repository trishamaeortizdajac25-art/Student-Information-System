using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BULi
{
    public partial class stdSignIn : Form
    {
        public stdSignIn()
        {
            InitializeComponent();
        }

        private void btnSignUpRDT_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new SignUp());
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }

<<<<<<< HEAD
        private void stdSignIn_Load(object sender, EventArgs e)
=======
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
>>>>>>> main
        {

        }
    }
}
