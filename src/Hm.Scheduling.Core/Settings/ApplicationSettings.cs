namespace Hm.Scheduling.Core.Settings;

public class ApplicationSettings
{
    public string ConnectionString { get; set; } = default!;

    public bool FirstTimeDbSetup { get; set; }
}
