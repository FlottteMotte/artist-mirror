using System.Drawing.Drawing2D;

namespace ArtistMirror;

internal static class UiShapes
{
    public static Region RoundedRegion(int width, int height, int radius)
    {
        using var path = new GraphicsPath();
        var d = radius * 2;
        path.AddArc(0, 0, d, d, 180, 90);
        path.AddArc(width - d, 0, d, d, 270, 90);
        path.AddArc(width - d, height - d, d, d, 0, 90);
        path.AddArc(0, height - d, d, d, 90, 90);
        path.CloseFigure();
        return new Region(path);
    }

    public static void FillStar(Graphics graphics, float cx, float cy, float radius, Color color)
    {
        using var path = new GraphicsPath();
        var points = new PointF[10];
        for (var i = 0; i < 10; i++)
        {
            var angle = -Math.PI / 2 + i * Math.PI / 5;
            var r = i % 2 == 0 ? radius : radius * 0.45f;
            points[i] = new PointF(
                cx + (float)(Math.Cos(angle) * r),
                cy + (float)(Math.Sin(angle) * r));
        }

        path.AddPolygon(points);
        using var brush = new SolidBrush(color);
        graphics.FillPath(brush, path);
    }
}
