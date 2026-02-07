using System.IO;
using System.Text.Json;
using BBDown_GUI.Models;

namespace BBDown_GUI.Services.Config;

public class ConfigService
{
    public readonly string ConfigFileName = "config.json";
    public readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
    
    public ConfigModel LoadConfig()
    {
        var filePath = GetConfigFilePath();

        if (!File.Exists(filePath)) return new ConfigModel();
        
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<ConfigModel>(json, JsonOptions) ?? new ConfigModel();
    }
    
    public void SaveConfig(ConfigModel config)
    {
        var filePath = GetConfigFilePath();
        var json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(filePath, json);
    }
    
    private string GetConfigFilePath()
    {
        return Utils.GetFilePath("Config", ConfigFileName);
    }
}