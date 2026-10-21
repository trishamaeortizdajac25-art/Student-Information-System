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

            SelectedRole = "Teacher";
            SIS.Instance.OpenChildForm(new tcrSignIn());
        }

        private void pick_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(btnStudent, 70);
            ControlRounder.MakeRounded(btnTeacher, 70);
            ControlRounder.MakeRounded(panel2, 75);
            ControlRounder.MakeRounded(panel3, 75);
            ControlRounder.MakeRounded(panel1, 16);


        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);

        }
    }
}
