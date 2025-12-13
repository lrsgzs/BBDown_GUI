using System.Threading.Tasks;
using BBDown_GUI.Models;

namespace BBDown_GUI.Abstraction;

public interface IConfigService
{
    ConfigModel LoadConfig();
    void SaveConfig(ConfigModel config);
}