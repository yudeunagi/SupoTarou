using System.Windows.Media;
using SupoTarou.Models;

namespace SupoTarou.Services;

public static class ColorConverter
{
    public static ColorData FromRgb(byte red, byte green, byte blue)
    {
        var redNormalized = red / 255d;
        var greenNormalized = green / 255d;
        var blueNormalized = blue / 255d;

        var max = Math.Max(redNormalized, Math.Max(greenNormalized, blueNormalized));
        var min = Math.Min(redNormalized, Math.Min(greenNormalized, blueNormalized));
        var delta = max - min;

        var hue = CalculateHue(redNormalized, greenNormalized, blueNormalized, max, delta);
        var saturation = max <= 0d ? 0d : (delta / max) * 100d;
        var value = max * 100d;

        return new ColorData(red, green, blue, hue, saturation, value);
    }

    public static Color HsvToMediaColor(double hue, double saturation, double value)
    {
        var normalizedHue = NormalizeHue(hue);
        var normalizedSaturation = Clamp(saturation, 0d, 100d) / 100d;
        var normalizedValue = Clamp(value, 0d, 100d) / 100d;

        if (normalizedSaturation <= 0d)
        {
            var gray = (byte)Math.Round(normalizedValue * 255d);
            return Color.FromRgb(gray, gray, gray);
        }

        var chroma = normalizedValue * normalizedSaturation;
        var huePrime = normalizedHue / 60d;
        var secondLargest = chroma * (1d - Math.Abs((huePrime % 2d) - 1d));
        var match = normalizedValue - chroma;

        var (red, green, blue) = huePrime switch
        {
            >= 0d and < 1d => (chroma, secondLargest, 0d),
            >= 1d and < 2d => (secondLargest, chroma, 0d),
            >= 2d and < 3d => (0d, chroma, secondLargest),
            >= 3d and < 4d => (0d, secondLargest, chroma),
            >= 4d and < 5d => (secondLargest, 0d, chroma),
            _ => (chroma, 0d, secondLargest)
        };

        return Color.FromRgb(
            (byte)Math.Round((red + match) * 255d),
            (byte)Math.Round((green + match) * 255d),
            (byte)Math.Round((blue + match) * 255d));
    }

    private static double CalculateHue(
        double red,
        double green,
        double blue,
        double max,
        double delta)
    {
        if (delta <= 0d)
        {
            return 0d;
        }

        double hue = max switch
        {
            _ when max.Equals(red) => 60d * (((green - blue) / delta) % 6d),
            _ when max.Equals(green) => 60d * (((blue - red) / delta) + 2d),
            _ => 60d * (((red - green) / delta) + 4d)
        };

        if (hue < 0d)
        {
            hue += 360d;
        }

        return hue;
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
}
