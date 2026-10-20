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
    public partial class pick : Form
    {
        public pick()
        {
            InitializeComponent();
        }

        private void btnStudent_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new stdSignIn());
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new tcrSignIn());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
