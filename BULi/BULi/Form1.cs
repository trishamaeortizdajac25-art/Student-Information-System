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

            // 2. Apply initial theme settings on startup
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
        private void btnColorScheme_Click(object sender, EventArgs e)
        {
            // Toggle your global theme state (dark <-> light)
            ThemeState.ToggleTheme();
        }

        // --- Global Theme Event Listener ---
        private void OnGlobalThemeChanged(object sender, bool isDark)
        {
            ApplyThemeToSIS(isDark);

            // Refresh current child form's background and panel colors live
            if (currentChildForm != null)
            {
                ApplyFormTheme(currentChildForm, isDark);
                currentChildForm.Invalidate(true);
            }
        }

        // --- Apply Theme to Main Container (SIS) ---
        private void ApplyThemeToSIS(bool isDark)
        {
            if (isDark)
            {
                this.BackColor = Color.FromArgb(10, 15, 35);
                pnlMain.BackColor = Color.Transparent;
                pnlMain.BackgroundImage = null;
            }
            else
            {
                this.BackColor = Color.FromArgb(254, 250, 236);
                pnlMain.BackColor = Color.FromArgb(254, 250, 236);
                pnlMain.BackgroundImage = null;
            }

            pnlMain.Invalidate();
            this.Invalidate(true);
        }

        // --- Centralized Background & Panel Theme Handler ---
        private void ApplyFormTheme(Form form, bool isDark)
        {
            if (form == null) return;

            string formName = form.GetType().Name;
            form.BackgroundImageLayout = ImageLayout.Stretch;

            // 1. EXCLUDED FORMS (NO CHANGE - Keep their designated background images and default styles)
            if (formName == "Starter")
            {
                form.BackgroundImage = Properties.Resources.starter;
                return;
            }
            else if (formName == "pick")
            {
                form.BackgroundImage = Properties.Resources.gradientnull;
                return;
            }
            else if (formName == "SignUp")
            {
                form.BackgroundImage = Properties.Resources._default;
                return;
            }
            else if (formName == "stdSignIn")
            {
                form.BackgroundImage = Properties.Resources.gradient;
                return;
            }
            else if (formName == "tcrSignIn")
            {
                form.BackgroundImage = Properties.Resources.tcrnull;
                return;
            }

            // 2. AFFECTED FORMS (Switch background image + apply navy dark mode to panels)
            Color navyDarkMode = Color.FromArgb(15, 23, 42);

            if (isDark)
            {
                form.BackgroundImage = Properties.Resources.darknull;

                // Recursively style all panels and text on this form to match dark navy mode
                StyleControlsForDarkTheme(form, navyDarkMode, true);
            }
            else
            {
                // Revert to respective light mode backgrounds
                if (formName == "infoCard" || formName == "display")
                {
                    form.BackgroundImage = Properties.Resources.gradient;
                }
                else if (formName == "HomePage")
                {
                    form.BackgroundImage = Properties.Resources.homepage;
                }
                else if (formName == "setUp")
                {
                    form.BackgroundImage = Properties.Resources._default;
                }
                else
                {
                    form.BackgroundImage = null;
                }

                // Revert panel/text styles for light mode
                StyleControlsForDarkTheme(form, Color.White, false);
            }
        }

        // --- Helper to style panels and labels recursively across any form ---
        private void StyleControlsForDarkTheme(Control parentControl, Color panelColor, bool isDark)
        {
            foreach (Control ctrl in parentControl.Controls)
            {
                // If it's a panel, update its background
                if (ctrl is Panel)
                {
                    ctrl.BackColor = isDark ? panelColor : Color.Transparent; // Adjust if light mode needs specific panel colors
                }
                // If it's a label or title, adjust text color for visibility
                else if (ctrl is Label)
                {
                    ctrl.ForeColor = isDark ? Color.White : Color.Black;
                }

                // If the control contains child controls (like panels inside panels), recurse deeper
                if (ctrl.HasChildren)
                {
                    StyleControlsForDarkTheme(ctrl, panelColor, isDark);
                }
            }
        }

        // --- Display child forms in the main panel (pnlMain) ---
        public void OpenChildForm(Form childForm)
        {
            pnlMain.SuspendLayout();

            try
            {
                if (currentChildForm != null)
                {
                    currentChildForm.Close();
                    currentChildForm.Dispose();
                }

                currentChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                // Apply correct background image and panel styling based on current theme
                ApplyFormTheme(childForm, ThemeState.IsDarkMode);

                pnlMain.Controls.Clear();
                pnlMain.Controls.Add(childForm);
                pnlMain.Tag = childForm;

                childForm.BringToFront();
                childForm.Show();
            }
            finally
            {
                pnlMain.ResumeLayout(true);
            }
        }
    }
}