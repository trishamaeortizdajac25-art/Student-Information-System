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
    public partial class display : Form
    {
        private String yearLvl;
        public display()
        {
            InitializeComponent();
        }

        public display(string yearLevel) : this()
        {
            yearLvl = yearLevel;
            lblYearTitle.Text = $"{yearLvl} Student Register";
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lnbReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }

        private void lblYearTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
