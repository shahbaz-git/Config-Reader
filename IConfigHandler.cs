using System.Collections.Generic;

namespace TgwAssignment
{
    public interface IConfigHandler
    {
        IList<ConfigFile> GetALlConfigFiles();
        string GetValueFromConfigs(string configId, string dataType);
        void LoadConfigFiles();
        string SetConfigValue(string key, string value);
    }
}