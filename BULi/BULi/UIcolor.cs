using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// Global state manager to track whether Dark Mode is active
public static class ThemeState
{
    public static bool IsDarkMode { get; private set; } = false;

    // Event that notifies all open forms when the theme changes
    public static event EventHandler<bool> ThemeChanged;

    public static void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        ThemeChanged?.Invoke(null, IsDarkMode);
    }
}

public static class UIcolor
{
    // --- SMART SWITCHERS (Automatically picks Light or Navy based on ThemeState) ---
    public static void ApplyPanelGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        if (ThemeState.IsDarkMode)
            ApplyNavyPanelGradient(control, e, angle);
        else
            ApplySunsetGradient(control, e, angle);
    }

    public static void ApplyHeaderGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        if (ThemeState.IsDarkMode)
            ApplyNavyHeaderGradient(control, e, angle);
        else
            ApplyDeepHeaderGradient(control, e, angle);
    }

    public static void ApplyInnerPanelGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        if (ThemeState.IsDarkMode)
            ApplyNavyInnerPanelGradient(control, e, angle);
        else
            ApplyLightInnerPanelGradient(control, e, angle);
    }

    // --- INDIVIDUAL GRADIENT DEFINITIONS ---
    public static void ApplySunsetGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        DrawCustomGradient(control, e, angle,
            Color.FromArgb(210, 253, 216, 93), Color.FromArgb(215, 246, 160, 64), Color.FromArgb(220, 235, 120, 38),
            Color.FromArgb(180, 255, 230, 150), 2f);
    }

    public static void ApplyDeepHeaderGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        DrawCustomGradient(control, e, angle,
            Color.FromArgb(225, 245, 188, 98), Color.FromArgb(230, 227, 137, 45), Color.FromArgb(235, 199, 81, 29),
            Color.FromArgb(190, 255, 210, 130), 2f);
    }

    public static void ApplyLightInnerPanelGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        DrawCustomGradient(control, e, angle,
            Color.FromArgb(180, 255, 252, 235), Color.FromArgb(185, 254, 235, 160), Color.FromArgb(190, 246, 195, 110),
            Color.FromArgb(160, 255, 240, 180), 1.5f);
    }

    public static void ApplyNavyPanelGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        DrawCustomGradient(control, e, angle,
            Color.FromArgb(200, 30, 40, 70), Color.FromArgb(205, 20, 25, 50), Color.FromArgb(210, 15, 18, 40),
            Color.FromArgb(60, 255, 255, 255), 1f);
    }

    public static void ApplyNavyHeaderGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        DrawCustomGradient(control, e, angle,
            Color.FromArgb(220, 50, 60, 100), Color.FromArgb(225, 35, 45, 80), Color.FromArgb(230, 25, 35, 60),
            Color.FromArgb(80, 255, 255, 255), 1.5f);
    }

    public static void ApplyNavyInnerPanelGradient(Control control, PaintEventArgs e, float angle = 45f)
    {
        DrawCustomGradient(control, e, angle,
            Color.FromArgb(150, 50, 55, 80), Color.FromArgb(155, 40, 45, 65), Color.FromArgb(160, 30, 35, 55),
            Color.FromArgb(50, 255, 255, 255), 1f);
    }

    private static void DrawCustomGradient(Control control, PaintEventArgs e, float angle, Color c1, Color c2, Color c3, Color borderColor, float borderWidth)
    {
        if (control.Width <= 0 || control.Height <= 0) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using (var brush = new LinearGradientBrush(control.ClientRectangle, Color.Empty, Color.Empty, angle))
        {
            var blend = new ColorBlend(3)
            {
                Colors = new Color[] { c1, c2, c3 },
                Positions = new float[] { 0.0f, 0.5f, 1.0f }
            };
            brush.InterpolationColors = blend;
            e.Graphics.FillRectangle(brush, control.ClientRectangle);
        }

        using (var borderPen = new Pen(borderColor, borderWidth))
        {
            e.Graphics.DrawRectangle(borderPen, 0, 0, control.Width - 1, control.Height - 1);
        }
    }
}