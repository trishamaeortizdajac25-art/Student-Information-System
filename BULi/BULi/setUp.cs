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
    public partial class setUp : Form
    {
        public setUp()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new infoCard());
        }

<<<<<<< HEAD
        private void setUp_Load(object sender, EventArgs e)
=======
        private void textBox1_TextChanged(object sender, EventArgs e)
>>>>>>> main
        {

        }
    }
}
