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
    public partial class infoCard : Form
    {
        private bool isExpanded = false;
        private int menuMaxHeight = 120; 
        private int menuMinHeight = 0; 
        public infoCard()
        { 
            InitializeComponent();
            pnlMenu.Height = menuMinHeight;
            pnlMenu.Visible = false;
        }

        private void btnDropDownHeader_Click(object sender, EventArgs e)
        {
            pnlMenu.BringToFront();
            tmrDropDown.Start();
        }

        private void tmrDropDown_Tick(object sender, EventArgs e)
        {
            if (!isExpanded)
            {
                pnlMenu.Visible = true;
                pnlMenu.Height += 15; // Unroll speed

                if (pnlMenu.Height >= menuMaxHeight)
                {
                    pnlMenu.Height = menuMaxHeight;
                    tmrDropDown.Stop();
                    isExpanded = true;
                }
            }
            else
            {
                pnlMenu.Height -= 15; // Roll up speed

                if (pnlMenu.Height <= menuMinHeight)
                {
                    pnlMenu.Height = menuMinHeight;
                    pnlMenu.Visible = false;
                    tmrDropDown.Stop();
                    isExpanded = false;
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void lnbReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SIS.Instance.OpenChildForm(new HomePage());
        }
    }
}
