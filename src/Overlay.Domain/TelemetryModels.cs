namespace Overlay.Domain;

public enum FlagState
{
    Green,
    Yellow,
    Blue,
    White,
    Black,
    Checkered,
    Unknown
}

public sealed record NearbyCar(string DriverName, int CarNumber, int ClassPosition, double GapSeconds);

public sealed record TelemetryFrame(
    DateTimeOffset Timestamp,
    double FuelLiters,
    double FuelPerLapLiters,
    bool PitLimiterOn,
    FlagState FlagState,
    double SpeedKph,
    int Gear,
    double Throttle,
    double Brake,
    NearbyCar? CarAhead,
    NearbyCar? CarBehind
);

public interface ITelemetrySource
{
    bool IsConnected { get; }
    ValueTask<bool> TryConnectAsync(CancellationToken cancellationToken);
    ValueTask<TelemetryFrame?> ReadFrameAsync(CancellationToken cancellationToken);
}

public interface IWidget<out TState>
{
    string Id { get; }
    TState BuildState(TelemetryFrame frame);
}
