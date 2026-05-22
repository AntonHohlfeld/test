using Overlay.Domain;

namespace Overlay.Telemetry;

public sealed class ReplayTelemetrySource : ITelemetrySource
{
    private readonly Random _random = new();
    private double _fuel = 45;
    private int _tick;

    public bool IsConnected { get; private set; }

    public ValueTask<bool> TryConnectAsync(CancellationToken cancellationToken)
    {
        IsConnected = true;
        return ValueTask.FromResult(true);
    }

    public ValueTask<TelemetryFrame?> ReadFrameAsync(CancellationToken cancellationToken)
    {
        if (!IsConnected)
        {
            return ValueTask.FromResult<TelemetryFrame?>(null);
        }

        _tick++;
        _fuel = Math.Max(0, _fuel - 0.03);

        var frame = new TelemetryFrame(
            Timestamp: DateTimeOffset.UtcNow,
            FuelLiters: _fuel,
            FuelPerLapLiters: 2.3,
            PitLimiterOn: _tick % 20 < 2,
            FlagState: _tick % 200 > 170 ? FlagState.Yellow : FlagState.Green,
            SpeedKph: 120 + _random.NextDouble() * 80,
            Gear: Math.Max(1, _random.Next(1, 7)),
            Throttle: _random.NextDouble(),
            Brake: _random.NextDouble() * 0.8,
            CarAhead: new NearbyCar("Ahead Driver", 12, 3, 0.6 + _random.NextDouble()),
            CarBehind: new NearbyCar("Behind Driver", 22, 4, 0.4 + _random.NextDouble())
        );

        return ValueTask.FromResult<TelemetryFrame?>(frame);
    }
}
