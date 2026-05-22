using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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
    private readonly string _settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private OverlaySettings _settings = new();
    private long _frames;

    private const int GwlExstyle = -20;
    private const int WsExTransparent = 0x20;

    public MainWindow()
    {
        InitializeComponent();

        _settings = OverlaySettings.Load(_settingsPath);
        Left = _settings.WindowLeft;
        Top = _settings.WindowTop;
        Width = _settings.WindowWidth;
        Height = _settings.WindowHeight;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += OnTick;

        Loaded += async (_, _) =>
        {
            await _telemetry.TryConnectAsync(CancellationToken.None);
            SetClickThrough(_settings.ClickThroughEnabled);
            _timer.Start();
        };

        Closing += (_, _) => SaveSettings();
        KeyDown += OnKeyDown;
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
        PerfText.Text = $"Update FPS: {fps:0.0} | Click-through: {(_settings.ClickThroughEnabled ? "ON" : "OFF")}";
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift)
            && e.Key == Key.O)
        {
            _settings.ClickThroughEnabled = !_settings.ClickThroughEnabled;
            SetClickThrough(_settings.ClickThroughEnabled);
            SaveSettings();
            e.Handled = true;
        }
    }

    private void SaveSettings()
    {
        _settings.WindowLeft = Left;
        _settings.WindowTop = Top;
        _settings.WindowWidth = Width;
        _settings.WindowHeight = Height;
        _settings.Save(_settingsPath);
    }

    private void SetClickThrough(bool enabled)
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd == IntPtr.Zero)
        {
            return;
        }

        var style = GetWindowLong(hwnd, GwlExstyle);
        style = enabled ? style | WsExTransparent : style & ~WsExTransparent;
        SetWindowLong(hwnd, GwlExstyle, style);
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
