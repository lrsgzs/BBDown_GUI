using System.ComponentModel;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Models;

namespace BBDown_GUI.Services.Config;

public class ConfigHandler
{
    public ConfigModel Data { get; private set; }
    private ConfigService ConfigService { get; }
    
    public ConfigHandler(ConfigService configService)
    {
        ConfigService = configService;
        Data = new ConfigModel();
        Data.PropertyChanged += OnPropertyChanged;
        InitializeConfig();
    }

    /// <summary>
    /// 初始化配置文件，检查路径是否存在并加载或创建配置文件。
    /// </summary>
    public void InitializeConfig()
    {
        Data = ConfigService.LoadConfig();
    }
    
    /// <summary>
    /// 当数据属性更改时触发，调用 Save 方法保存配置。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">属性更改事件参数</param>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }

    /// <summary>
    /// 保存数据到配置文件，记录日志并在发生异常时进行处理。
    /// </summary>
    public void Save()
    {
        ConfigService.SaveConfig(Data);
    }
}