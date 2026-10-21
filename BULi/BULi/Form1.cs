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
    public partial class SIS : Form
    {
        public static SIS Instance { get; private set; }
        private Form currentChildForm = null;

        public SIS()
        {
            InitializeComponent();
            Instance = this;

            // 1. Subscribe to global theme changes
            ThemeState.ThemeChanged += OnGlobalThemeChanged;

            // 2. Apply initial theme settings on startup (Checks if dark mode is active)
            ApplyThemeToSIS(ThemeState.IsDarkMode);

            // 3. Load initial starter form
            OpenChildForm(new Starter());
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Unsubscribe from event to prevent memory leaks
            ThemeState.ThemeChanged -= OnGlobalThemeChanged;
            base.OnFormClosed(e);
        }

        // --- Theme Toggle Button Click Handler ---
        // Bind this to your btnColorScheme click event in the Designer
        private void btnColorScheme_Click(object sender, EventArgs e)
        {
            ThemeState.ToggleTheme();
        }

        // --- Global Theme Event Listener ---
        private void OnGlobalThemeChanged(object sender, bool isDark)
        {
            ApplyThemeToSIS(isDark);

            // If a child form is currently open, refresh its paint and text colors live
            if (currentChildForm != null)
            {
                currentChildForm.Invalidate(true);
                UpdateControlTextColors(currentChildForm, isDark);
            }
        }

        // --- Apply Theme to Main Container (SIS) ---
        private void ApplyThemeToSIS(bool isDark)
        {
            if (isDark)
            {
                this.BackColor = Color.FromArgb(10, 15, 35);
                this.BackgroundImage = null; // Clear form bg to let panel handle it

                // Force pnlMain to paint the darknull background image across the workspace
                pnlMain.Paint -= PnlMain_Paint; // Prevent duplicate event binding
                pnlMain.Paint += PnlMain_Paint;
            }
            else
            {
                this.BackColor = Color.FromArgb(254, 250, 236);
                this.BackgroundImage = null;

                // Remove the custom dark background paint handler
                pnlMain.Paint -= PnlMain_Paint;
                pnlMain.BackColor = Color.FromArgb(254, 250, 236);
            }

            pnlMain.Invalidate();
            this.Invalidate(true);
        }

        // --- Custom Paint Handler for Dark Background Image ---
        private void PnlMain_Paint(object sender, PaintEventArgs e)
        {
            if (ThemeState.IsDarkMode)
            {
                object darkResource = Properties.Resources.ResourceManager.GetObject("darknull");
                Image darkImage = (darkResource as Image) ?? (darkResource as Bitmap);

                if (darkImage != null)
                {
                    // Stretch the darknull image to fill the entire pnlMain canvas perfectly
                    e.Graphics.DrawImage(darkImage, pnlMain.ClientRectangle);
                }
            }
        }
        // --- Recursive Font & Input Color Switcher ---
        public static void UpdateControlTextColors(Control parent, bool isDark)
        {
            Color targetColor = isDark ? Color.White : Color.FromArgb(51, 41, 36);

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label || ctrl is CheckBox || ctrl is RadioButton)
                {
                    ctrl.ForeColor = targetColor;
                }
                else if (ctrl is TextBox || ctrl is ComboBox)
                {
                    ctrl.ForeColor = targetColor;
                    ctrl.BackColor = isDark ? Color.FromArgb(20, 25, 45) : Color.White;
                }

                if (ctrl.HasChildren)
                {
                    UpdateControlTextColors(ctrl, isDark);
                }
            }
        }

        // --- display ng forms sa main panel (pnlMain) ---
        public void OpenChildForm(Form childForm)
        {
            // 1. Freeze the panel layout to prevent mid-air flashing
            pnlMain.SuspendLayout();

            try
            {
                if (currentChildForm != null)
                {
                    currentChildForm.Close();
                    currentChildForm.Dispose(); // Ensure old form resources are freed
                }

                currentChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                // 2. Clear previous controls from the host panel safely
                pnlMain.Controls.Clear();

                // 3. Add and display the new child form
                pnlMain.Controls.Add(childForm);
                pnlMain.Tag = childForm;

                // 4. Enforce active theme text colors on the newly injected child form instantly
                UpdateControlTextColors(childForm, ThemeState.IsDarkMode);

                childForm.BringToFront();
                childForm.Show();
            }
            finally
            {
                // 5. Resume layout and force a single clean draw pass
                pnlMain.ResumeLayout(true);
                // Inside your SIS() constructor, add this:
pnlMain.BackColor = Color.Transparent;

            }
        }
    }
}