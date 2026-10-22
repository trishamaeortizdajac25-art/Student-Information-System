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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(panel1, 16);

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient(panel1,e,45f);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            label3.Text = "BULi(BiyU Learner's Information) is a specialized Student Information System \n" +
                          " designed exclusively for the Bicol University College of Science. It provides\n " +
                          "a secure, centralized platform for managing academic records and institutional\n " +
                          "user roles. Developed using C# Windows Forms and MySQL, the system ensures efficient\n," +
                          " real-time data handling for students and faculty.";
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    } 
}
