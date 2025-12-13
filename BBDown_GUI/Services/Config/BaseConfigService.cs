using System.Text.Json;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Models;

namespace BBDown_GUI.Services.Config;

public abstract class BaseConfigService : IConfigService
{
    protected readonly string ConfigFileName = "config.json";
    protected readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
    
    public abstract ConfigModel LoadConfig();
    public abstract void SaveConfig(ConfigModel config);
}