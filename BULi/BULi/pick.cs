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
        public static string SelectedRole { get; set; }
        public pick()
        {
            InitializeComponent();
        }

        private void btnStudent_Click(object sender, EventArgs e)
        {

            SelectedRole = "Student";
            SIS.Instance.OpenChildForm(new stdSignIn());
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {

            SelectedRole = "Student";
            SIS.Instance.OpenChildForm(new tcrSignIn());
        }

    }
}
