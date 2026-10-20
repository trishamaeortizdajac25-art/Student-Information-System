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
    public partial class HomePage : Form
    {
        private Color targetColor;
        private int currentStep = 0;
        private const int TotalSteps = 2;
        public HomePage()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void pnlFreshie_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display());
            SIS.Instance.OpenChildForm(new display("Freshmen"));
        }

        private void pnlSophomore_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display());
            SIS.Instance.OpenChildForm(new display("Sophomore"));
        }

        private void pnlJunior_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display());
            SIS.Instance.OpenChildForm(new display("Junior"));
        }

        private void pnlSenior_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display());
            SIS.Instance.OpenChildForm(new display("Senior"));
        }

        private void btnColorScheme_Click(object sender, EventArgs e)
        {
            if (this.BackColor == SystemColors.Control)
            {
                targetColor = Color.FromArgb(64, 64, 64);
            }
            else
            {
                targetColor = SystemColors.Control;
            }

            currentStep = 0;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            currentStep++;
            int r = this.BackColor.R + (targetColor.R - this.BackColor.R) * currentStep / TotalSteps;
            int g = this.BackColor.G + (targetColor.G - this.BackColor.G) * currentStep / TotalSteps;
            int b = this.BackColor.B + (targetColor.B - this.BackColor.B) * currentStep / TotalSteps;

            this.BackColor = Color.FromArgb(r, g, b);
            if (currentStep >= TotalSteps)
            {
                timer1.Stop();
                this.BackColor = targetColor;
            }
        }

        private void pnlFreshie_MouseHover(object sender, EventArgs e)
        {
            pnlFreshie.BackColor=Color.FromArgb(238, 240, 243);
        }

        private void pnlSophomore_MouseHover(object sender, EventArgs e)
        {
            pnlSophomore.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlJunior_MouseHover(object sender, EventArgs e)
        {
            pnlJunior.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void pnlSenior_MouseHover(object sender, EventArgs e)
        {
            pnlSenior.BackColor = Color.FromArgb(238, 240, 243);
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            ControlRounder.MakeRounded(pnlFreshie, 20);
        }

    }
}
