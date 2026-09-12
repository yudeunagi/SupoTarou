using System.Windows;
using System.Windows.Media;
using ServicesColorConverter = SupoTarou.Services.ColorConverter;

namespace SupoTarou.Controls;

public sealed class SVBoxControl : FrameworkElement
{
    private static readonly LinearGradientBrush WhiteOverlayBrush = CreateWhiteOverlayBrush();
    private static readonly LinearGradientBrush BlackOverlayBrush = CreateBlackOverlayBrush();

    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue),
        typeof(double),
        typeof(SVBoxControl),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
        nameof(Saturation),
        typeof(double),
        typeof(SVBoxControl),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(SVBoxControl),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MarkerRadiusProperty = DependencyProperty.Register(
        nameof(MarkerRadius),
        typeof(double),
        typeof(SVBoxControl),
        new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    public double Saturation
    {
        get => (double)GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double MarkerRadius
    {
        get => (double)GetValue(MarkerRadiusProperty);
        set => SetValue(MarkerRadiusProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var rect = new Rect(0.5d, 0.5d, Math.Max(0d, RenderSize.Width - 1d), Math.Max(0d, RenderSize.Height - 1d));
        if (rect.Width <= 0d || rect.Height <= 0d)
        {
            return;
        }

        var baseColorBrush = new SolidColorBrush(ServicesColorConverter.HsvToMediaColor(Hue, 100d, 100d));

        drawingContext.DrawRectangle(baseColorBrush, null, rect);
        drawingContext.DrawRectangle(WhiteOverlayBrush, null, rect);
        drawingContext.DrawRectangle(BlackOverlayBrush, null, rect);
        drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1d), rect);

        var markerRadius = Clamp(MarkerRadius, 1d, 20d);
        var x = rect.Left + rect.Width * (Clamp(Saturation, 0d, 100d) / 100d);
        var y = rect.Top + rect.Height * (1d - Clamp(Value, 0d, 100d) / 100d);
        var markerPoint = new Point(x, y);

        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Black, 2d), markerPoint, markerRadius, markerRadius);
        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.White, 1d), markerPoint, markerRadius - 1d, markerRadius - 1d);
    }

    private static LinearGradientBrush CreateWhiteOverlayBrush()
    {
        var brush = new LinearGradientBrush(
            Colors.White,
            Color.FromArgb(0, 255, 255, 255),
            new Point(0d, 0.5d),
            new Point(1d, 0.5d));
        brush.Freeze();
        return brush;
    }

    private static LinearGradientBrush CreateBlackOverlayBrush()
    {
        var brush = new LinearGradientBrush(
            Color.FromArgb(0, 0, 0, 0),
            Colors.Black,
            new Point(0.5d, 0d),
            new Point(0.5d, 1d));
        brush.Freeze();
        return brush;
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Min(Math.Max(value, min), max);
    }
}
