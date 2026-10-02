using System.Drawing.Drawing2D;

namespace ArtistMirror;

internal sealed class MagicCanvas : Panel
{
    public MagicCanvas()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        Dock = DockStyle.Fill;
        BackColor = Color.Transparent;
        Paint += OnPaintCanvas;
    }

    private void OnPaintCanvas(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = ClientRectangle;

        using (var brush = new LinearGradientBrush(bounds, UiTheme.BgTop, UiTheme.BgBottom, LinearGradientMode.Vertical))
        {
            brush.InterpolationColors = new ColorBlend
            {
                Colors = new[] { UiTheme.BgTop, UiTheme.BgMid, UiTheme.BgBottom },
                Positions = new[] { 0f, 0.45f, 1f }
            };
            g.FillRectangle(brush, bounds);
        }

        using (var soft = new SolidBrush(Color.FromArgb(90, UiTheme.Blob)))
        {
            g.FillEllipse(soft, bounds.Width - 160, -40, 200, 200);
            g.FillEllipse(soft, -80, bounds.Height - 140, 220, 180);
        }

        using (var pink = new SolidBrush(Color.FromArgb(70, UiTheme.Star)))
        {
            g.FillEllipse(pink, 300, 70, 70, 70);
            g.FillEllipse(pink, 60, 300, 50, 50);
        }

        UiShapes.FillStar(g, 400, 50, 9, Color.FromArgb(160, UiTheme.Accent));
        UiShapes.FillStar(g, 430, 110, 6, Color.FromArgb(140, UiTheme.Ok));
        UiShapes.FillStar(g, 360, 130, 5, Color.FromArgb(120, UiTheme.Accent));
        UiShapes.FillStar(g, 40, 250, 7, Color.FromArgb(100, UiTheme.Blob));
    }
}
