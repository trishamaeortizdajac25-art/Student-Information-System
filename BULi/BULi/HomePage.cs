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

        private string connectionString = "Server=localhost;Database=sis_db;Uid=root;Pwd=kleenpIn*1;";

        private DataTable studentDataTable = new DataTable();

        public HomePage()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // 1. Subscribe to global live theme toggles
            ThemeState.ThemeChanged += OnThemeChanged;
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            // Apply initial theme styles for header label and search box on load
            ApplyHomePageTheme(ThemeState.IsDarkMode);

            // UI Rounding setup
            ControlRounder.MakeRounded(pnlFreshie, 20);
            ControlRounder.MakeRounded(pnlSophomore, 20);
            ControlRounder.MakeRounded(pnlJunior, 20);
            ControlRounder.MakeRounded(pnlSenior, 20);

            ControlRounder.MakeRounded(lblView1, 5);
            ControlRounder.MakeRounded(lblView2, 5);
            ControlRounder.MakeRounded(lblView3, 5);
            ControlRounder.MakeRounded(lblView4, 5);

            ControlRounder.MakeRounded(btnColorScheme, 10);

            ControlRounder.MakeRounded(txtSearch, 10);
            ControlRounder.MakeRounded(btnSearch, 10);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Unsubscribe from global theme event to prevent memory leaks
            ThemeState.ThemeChanged -= OnThemeChanged;
            base.OnFormClosed(e);
        }

        private void OnThemeChanged(object sender, bool isDark)
        {
            ApplyHomePageTheme(isDark);
        }

        private void ApplyHomePageTheme(bool isDark)
        {
            // 1. Force all custom painted panels to redraw with the new theme gradients live
            pnlFreshie.Invalidate();
            pnlSophomore.Invalidate();
            pnlJunior.Invalidate();
            pnlSenior.Invalidate();
            panel1.Invalidate(); // Header panel

            // 2. ONLY change the header title text color (label1). 
            // We do NOT touch any other controls, labels, or panels!
            if (label1 != null)
            {
                label1.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
            }

            // 3. Update search box colors for dark/light mode
            if (txtSearch != null)
            {
                txtSearch.BackColor = isDark ? Color.FromArgb(20, 25, 45) : Color.White;
                txtSearch.ForeColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);
            }
        }

        private void pnlFreshie_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Freshmen"));
        }

        private void pnlSophomore_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Sophomore"));
        }

        private void pnlJunior_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Junior"));
        }

        private void pnlSenior_Click(object sender, EventArgs e)
        {
            SIS.Instance.OpenChildForm(new display("Senior"));
        }

        // --- Theme Toggle Button ---
        private void btnColorScheme_Click(object sender, EventArgs e)
        {
            ThemeState.ToggleTheme();
        }

        private void pnlFreshie_MouseHover(object sender, EventArgs e)
        {
            pnlFreshie.BackColor = Color.FromArgb(238, 240, 243);
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

        // --- Smart Panel Paint Events (Automatically switches between Sunset and Navy) ---
        private void pnlFreshie_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlSophomore_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlJunior_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void pnlSenior_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyPanelGradient((Control)sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            UIcolor.ApplyHeaderGradient((Control)sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}