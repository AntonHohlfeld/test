namespace Overlay.Processing;

public sealed record RelativeWidgetState(string Ahead, string Behind);
public sealed record FuelWidgetState(double FuelLiters, double FuelPerLap, double EstimatedLapsRemaining);
public sealed record FlagsWidgetState(string FlagText, bool PitLimiterOn);
