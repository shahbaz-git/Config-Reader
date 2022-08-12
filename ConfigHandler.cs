using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TgwAssignment
{
    public class ConfigHandler : IConfigHandler
    {
        private readonly IList<ConfigFile> _configFiles;

        public ConfigHandler()
        {
            _configFiles = new List<ConfigFile>();
            LoadConfigFiles();
        }

        public string GetValueFromConfigs(string configId, string dataType)
        {
            string value = _configFiles.Select(x =>
            {
                return x.ConfigValues.FirstOrDefault(t => t.Key.Equals(configId.ToLower())
                                                          && t.DataType.Contains(dataType.ToLower()
                                                          ))?.Value;

            }).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = $"The Config-Id '{configId}' having datatype '{dataType}' could not be found in any layers. => No value available to be set. => Error";
            }
            return value;
        }

        public string SetConfigValue(string key, string value)
        {
            var result = string.Empty;

            var dataType = GetDataType(value);

            _configFiles.ToList().ForEach(x => x.ConfigValues.Where(
                x =>
                {
                    if (x.Key.Equals(key.ToLower()) && !x.DataType.Equals(dataType))
                    {
                        result = "Invalid Datatype Passed";
                        return false;
                    }
                    else
                    {
                        return x.Key.Equals(key.ToLower()) && x.DataType.Equals(dataType);
                    }
                }
                )
            .ToList().ForEach(t => t.Value = value));
            return result;
        }

        public IList<ConfigFile> GetALlConfigFiles()
        {
            _configFiles.ToList().ForEach(x => x.ConfigValues.ToList().ForEach(t =>
            Console.WriteLine($"Key : {t.Key}, Value : {t.Value}, DataType : {t.DataType}")
            ));

            return new List<ConfigFile>();
        }

        public void LoadConfigFiles()
        {
            var folderPath = "../../../Config_Files/";

            foreach (string file in Directory.EnumerateFiles(folderPath, "*_config.txt"))
            {
                string[] lines = File.ReadAllLines(file);

                ConfigFile configFile = new ConfigFile();
                configFile.ConfigValues = new List<ConfigData>();

                foreach (var line in lines)
                {
                    if (line.Contains("\t"))
                    {
                        var cleanline = line.Replace(":\t", "~").Replace("\t", string.Empty);

                        var array = cleanline.Split("~");

                        ConfigData configdata = new ConfigData();
                        configdata.Key = array[0].ToLower();

                        cleanline = cleanline.Replace("~", "//");
                        array = cleanline.Split("//");

                        configdata.Value = array[1];
                        configdata.DataType = GetDataType(array[1]);
                        configFile.ConfigValues.Add(configdata);

                    }
                }

                _configFiles.Add(configFile);

            }
            Console.WriteLine("-Config Files Loaded-\n");

            ConsoleMessage();
            Console.ReadLine();
        }

        private int ConsoleMessage()
        {

            Console.WriteLine("Type 1 to load all values from all config files");
            Console.WriteLine("Type 2 to fetch a particular value");
            Console.WriteLine("Type 3 to set a config value");

            int.TryParse(Console.ReadLine(), out int intValue);

            switch (intValue)
            {

                case 1:
                    GetALlConfigFiles();
                    Console.WriteLine("-------------\n");
                    ConsoleMessage();
                    break;
                case 2:
                    Console.WriteLine("Enter Key to get value");
                    var key = Console.ReadLine();
                    Console.WriteLine("Enter data type to get value");
                    var dataType = Console.ReadLine();
                    var result = GetValueFromConfigs(key, dataType);
                    Console.WriteLine($"The Value for Key '{key}' is '{result}' ");
                    Console.WriteLine("-------------\n");
                    ConsoleMessage();
                    break;
                case 3:
                    Console.WriteLine("Enter Key");
                    key = Console.ReadLine();
                    Console.WriteLine("Enter Value");
                    var value = Console.ReadLine();
                    result = SetConfigValue(key, value);
                    Console.WriteLine(result);
                    Console.WriteLine("-------------\n");
                    ConsoleMessage();
                    break;
                default:
                    break;
            }

            return intValue;
        }

        private static string GetDataType(string value)
        {
            StringConverter.Default.Culture = CultureInfo.InvariantCulture;

            Type type = value.GetType();

            if (StringConverter.Default.TryConvert(value, out object result))
            {
                type = result.GetType();
            }

            return type.ToString().ToLower();
        }
    }
}
