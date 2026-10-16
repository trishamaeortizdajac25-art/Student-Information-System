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
    public partial class Starter : Form
    {
        private int startStep = 0;
        private const int totalStep = 35;
        public Starter()
        {
            InitializeComponent();
        }

        private void tmrOpen_Tick(object sender, EventArgs e)
        {

            startStep++;
            if (startStep >= totalStep)
            {
                tmrOpen.Stop();
                SIS.Instance.OpenChildForm(new pick());
            }
        }

        private void Starter_Load(object sender, EventArgs e)
        {
            tmrOpen.Start();
        }
    }
}
