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



        private void lblYearTitle_Click(object sender, EventArgs e)
        {

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());

        }

        private void display_Load(object sender, EventArgs e)
        {
            pnlScroll.MouseEnter += (s, ev) => pnlScroll.Focus();
            ControlRounder.MakeRounded(btnReturn, 10);
            ControlRounder.MakeRounded(pnlhawakupdate, 16);
            ControlRounder.MakeRounded(pnlID, 6);
            ControlRounder.MakeRounded(pnlName, 6);
            ControlRounder.MakeRounded(pnlBloc, 6);
            ControlRounder.MakeRounded(pnlYear, 6);
            ControlRounder.MakeRounded(pnlProgram, 6);
            ControlRounder.MakeRounded(btnAddSI, 8);
            ControlRounder.MakeRounded(btnUpdateSI, 8);
            ControlRounder.MakeRounded(btnDeleteSI, 8);
            ControlRounder.MakeRounded(btnView, 8);
            ControlRounder.MakeRounded(txtSearch, 6);




        }

        private void btnView_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new infoCard());
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnSort, new Point(0, btnSort.Height));
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyDeepHeaderGradient((Control)sender, e);

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplySunsetGradient((Control)sender, e);

        }



        private void pnlhawakupdate_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlID_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);
        }

        private void pnlName_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);

        }

        private void pnlBloc_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);

        }

        private void pnlYear_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);

        }

        private void pnlProgram_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyLightInnerPanelGradient((Control)sender, e);

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
