using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BULi
{
    public static class ControlRounder
    {

        /// <param name="control">The control to round.</param>
        /// <param name="cornerRadius">Radius of the corners in pixels.</param>
        public static void MakeRounded(Control control, int cornerRadius = 15)
        {
            if (control == null) return;

            void ApplyRegion()
            {
                if (control.Width <= 0 || control.Height <= 0) return;

                int diameter = cornerRadius * 2;
                Rectangle rect = control.ClientRectangle;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.StartFigure();
                    path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                    path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                    path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                    path.CloseFigure();

                    control.Region?.Dispose();
                    control.Region = new Region(path);
                }
            }

            if (control is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }

            if (control is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.None;
            }

            ApplyRegion();

            control.SizeChanged += (s, e) => ApplyRegion();
        }
    }
}