using Overlay.Domain;
using Overlay.Processing;

namespace Overlay.Widgets.Flags;

public sealed class FlagsWidget : IWidget<FlagsWidgetState>
{
    private readonly TelemetryProcessor _processor = new();
    public string Id => "flags";
    public FlagsWidgetState BuildState(TelemetryFrame frame) => _processor.BuildFlags(frame);
}
