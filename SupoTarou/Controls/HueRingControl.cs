using System.Windows;
using System.Windows.Media;
using ServicesColorConverter = SupoTarou.Services.ColorConverter;

namespace SupoTarou.Controls;

public sealed class HueRingControl : FrameworkElement
{
    private static readonly SolidColorBrush[] HueBrushes = CreateHueBrushes();

    /// <summary>
    /// Hue プロパティ
    /// </summary>
    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue),
        typeof(double),
        typeof(HueRingControl),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// RingThickness プロパティ
    /// </summary>
    public static readonly DependencyProperty RingThicknessProperty = DependencyProperty.Register(
        nameof(RingThickness),
        typeof(double),
        typeof(HueRingControl),
        new FrameworkPropertyMetadata(40d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// MarkerRadius プロパティ
    /// </summary>
    public static readonly DependencyProperty MarkerRadiusProperty = DependencyProperty.Register(
        nameof(MarkerRadius),
        typeof(double),
        typeof(HueRingControl),
        new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Hue 値 (0-360)
    /// </summary>
    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    /// <summary>
    /// リングの厚さ
    /// </summary>
    public double RingThickness
    {
        get => (double)GetValue(RingThicknessProperty);
        set => SetValue(RingThicknessProperty, value);
    }

    /// <summary>
    /// マーカーの半径
    /// </summary>
    public double MarkerRadius
    {
        get => (double)GetValue(MarkerRadiusProperty);
        set => SetValue(MarkerRadiusProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (RenderSize.Width <= 0d || RenderSize.Height <= 0d)
        {
            return;
        }

        var center = new Point(RenderSize.Width / 2d, RenderSize.Height / 2d);
        var markerRadius = Clamp(MarkerRadius, 1d, 24d);
        var maximumRadius = Math.Max(0d, (Math.Min(RenderSize.Width, RenderSize.Height) / 2d) - markerRadius - 1d);
        var ringThickness = Clamp(RingThickness, 1d, Math.Max(1d, maximumRadius));
        var ringRadius = Math.Max(0d, maximumRadius - ringThickness / 2d);

        var ringPen = new Pen
        {
            Thickness = ringThickness,
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round
        };

        for (var hue = 0; hue < 360; hue++)
        {
            var startPoint = ToPoint(center, ringRadius, hue);
            var endPoint = ToPoint(center, ringRadius, hue + 1d);
            ringPen.Brush = HueBrushes[hue];
            drawingContext.DrawLine(ringPen, startPoint, endPoint);
        }

        var markerPoint = ToPoint(center, ringRadius, Hue);
        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Black, 2d), markerPoint, markerRadius, markerRadius);
        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.White, 1d), markerPoint, markerRadius - 1d, markerRadius - 1d);
    }

    private static Point ToPoint(Point center, double radius, double hue)
    {
        var angleInRadians = (NormalizeHue(hue) - 150d) * Math.PI / 180d;
        return new Point(
            center.X + Math.Cos(angleInRadians) * radius,
            center.Y + Math.Sin(angleInRadians) * radius);
    }

    private static double NormalizeHue(double hue)
    {
        var normalizedHue = hue % 360d;
        return normalizedHue < 0d ? normalizedHue + 360d : normalizedHue;
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    private static SolidColorBrush[] CreateHueBrushes()
    {
        var brushes = new SolidColorBrush[360];
        for (var hue = 0; hue < 360; hue++)
        {
            var brush = new SolidColorBrush(ServicesColorConverter.HsvToMediaColor(hue, 100d, 100d));
            brush.Freeze();
            brushes[hue] = brush;
        }

        return brushes;
    }
}
