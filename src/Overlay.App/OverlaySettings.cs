using System.Text.Json;

namespace Overlay.App;

public sealed class OverlaySettings
{
    public bool ClickThroughEnabled { get; set; }
    public double WindowLeft { get; set; } = 50;
    public double WindowTop { get; set; } = 50;
    public double WindowWidth { get; set; } = 420;
    public double WindowHeight { get; set; } = 220;

    public static OverlaySettings Load(string path)
    {
        if (!File.Exists(path))
        {
            return new OverlaySettings();
        }

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<OverlaySettings>(json) ?? new OverlaySettings();
        }
        catch
        {
            return new OverlaySettings();
        }
    }

    public void Save(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
