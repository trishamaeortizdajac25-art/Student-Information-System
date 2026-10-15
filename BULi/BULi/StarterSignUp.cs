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
    public partial class StarterSignUp : Form
    {
        public StarterSignUp()
        {
            InitializeComponent();
        }

        private void btnSignUpRDT_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new SignIn());
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }
    }
}
