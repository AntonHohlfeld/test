using Overlay.Domain;
using Overlay.Processing;

namespace Overlay.Widgets.Relative;

public sealed class RelativeWidget : IWidget<RelativeWidgetState>
{
    private readonly TelemetryProcessor _processor = new();
    public string Id => "relative";
    public RelativeWidgetState BuildState(TelemetryFrame frame) => _processor.BuildRelative(frame);
}
