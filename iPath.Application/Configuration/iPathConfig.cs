namespace iPath.Application.Configuration;

public class iPathConfig
{
    public string BaseUrl { get; set; }
    public string DataPath { get; set; }
    public string DataPathReadonly { get; set; }

    public int ThumbSize { get; set; } = 120;

    public bool AutoMigrateDatabase { get; set; }
}
