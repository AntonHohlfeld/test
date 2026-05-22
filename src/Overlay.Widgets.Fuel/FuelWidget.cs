using Overlay.Domain;
using Overlay.Processing;

namespace Overlay.Widgets.Fuel;

public sealed class FuelWidget : IWidget<FuelWidgetState>
{
    private readonly TelemetryProcessor _processor = new();
    public string Id => "fuel";
    public FuelWidgetState BuildState(TelemetryFrame frame) => _processor.BuildFuel(frame);
}
