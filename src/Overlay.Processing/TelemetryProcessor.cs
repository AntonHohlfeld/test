using Overlay.Domain;

namespace Overlay.Processing;

public sealed class TelemetryProcessor
{
    public RelativeWidgetState BuildRelative(TelemetryFrame frame)
    {
        var ahead = frame.CarAhead is null
            ? "Ahead: --"
            : $"Ahead: #{frame.CarAhead.CarNumber} {frame.CarAhead.DriverName} (+{frame.CarAhead.GapSeconds:0.00}s)";

        var behind = frame.CarBehind is null
            ? "Behind: --"
            : $"Behind: #{frame.CarBehind.CarNumber} {frame.CarBehind.DriverName} (+{frame.CarBehind.GapSeconds:0.00}s)";

        return new RelativeWidgetState(ahead, behind);
    }

    public FuelWidgetState BuildFuel(TelemetryFrame frame)
    {
        var estLaps = frame.FuelPerLapLiters <= 0.001 ? 0 : frame.FuelLiters / frame.FuelPerLapLiters;
        return new FuelWidgetState(frame.FuelLiters, frame.FuelPerLapLiters, estLaps);
    }

    public FlagsWidgetState BuildFlags(TelemetryFrame frame)
        => new(frame.FlagState.ToString().ToUpperInvariant(), frame.PitLimiterOn);
}
