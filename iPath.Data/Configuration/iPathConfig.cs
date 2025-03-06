namespace iPath.Data.Configuration;

public class iPathConfig
{
    public string DbProvider { get; set; }
    public bool DbSeedingAvtice { get; set; }
    public bool DbAutoMigrate { get; set; }

    public string TempDataPath { get; set; }
    public string LocalDataPath { get; set; }

    public bool ExportNodeJson { get; set; }

    public int ThumbSize { get; set; } = 100;

    public string InstallationName { get; set; } = "dev";
}
