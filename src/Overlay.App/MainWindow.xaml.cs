using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using Overlay.Processing;
using Overlay.Telemetry;

namespace Overlay.App;

public partial class MainWindow : Window
{
    private readonly ReplayTelemetrySource _telemetry = new();
    private readonly TelemetryProcessor _processor = new();
    private readonly DispatcherTimer _timer;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private long _frames;

    public MainWindow()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += OnTick;

        Loaded += async (_, _) =>
        {
            await _telemetry.TryConnectAsync(CancellationToken.None);
            _timer.Start();
        };
    }

    private async void OnTick(object? sender, EventArgs e)
    {
        var frame = await _telemetry.ReadFrameAsync(CancellationToken.None);
        if (frame is null)
        {
            FlagsText.Text = "Telemetry: disconnected";
            return;
        }

        var relative = _processor.BuildRelative(frame);
        var fuel = _processor.BuildFuel(frame);
        var flags = _processor.BuildFlags(frame);

        RelativeAheadText.Text = relative.Ahead;
        RelativeBehindText.Text = relative.Behind;
        FuelText.Text = $"Fuel: {fuel.FuelLiters:0.0}L | Burn: {fuel.FuelPerLap:0.00}L/lap | Est: {fuel.EstimatedLapsRemaining:0.0} laps";
        FlagsText.Text = $"Flag: {flags.FlagText} | Pit limiter: {(flags.PitLimiterOn ? "ON" : "OFF")}";

        _frames++;
        var fps = _frames / Math.Max(0.001, _stopwatch.Elapsed.TotalSeconds);
        PerfText.Text = $"Update FPS: {fps:0.0}";
    }
}
