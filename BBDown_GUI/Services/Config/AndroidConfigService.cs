using System;
using System.IO;
using System.Text.Json;
using BBDown_GUI.Models;

namespace BBDown_GUI.Services.Config;

public class AndroidConfigService : BaseConfigService
{
    public override ConfigModel LoadConfig()
    {
        var filePath = GetConfigFilePath();

        if (!File.Exists(filePath)) return new ConfigModel();
        
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<ConfigModel>(json, JsonOptions) ?? new ConfigModel();
    }
    
    public override void SaveConfig(ConfigModel config)
    {
        var filePath = GetConfigFilePath();
        var directory = Path.GetDirectoryName(filePath);
        
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        var json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(filePath, json);
    }
    
    private string GetConfigFilePath()
    {
        // Android 内部存储，应用私有目录
        var personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return Path.Combine(personal, ConfigFileName);
    }
}