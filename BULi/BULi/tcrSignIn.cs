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
    public partial class tcrSignIn : Form
    {
        public tcrSignIn()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new pick());

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }

        private void tcrSignIn_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);
            ControlRounder.MakeRounded(btnReturn, 8);
            ControlRounder.MakeRounded(btnEnter, 8);
            ControlRounder.MakeRounded(txtName, 8);
            ControlRounder.MakeRounded(txtPassword, 8);
        }


    }
}
