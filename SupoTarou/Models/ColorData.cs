namespace SupoTarou.Models;

public readonly record struct ColorData(
    byte R,
    byte G,
    byte B,
    double H,
    double S,
    double V);
