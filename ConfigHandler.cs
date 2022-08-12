using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TgwAssignment
{
    public class ConfigHandler
    {
        private readonly IList<ConfigFile> _configFiles;
        public ConfigHandler()
        {
            _configFiles = new List<ConfigFile>();
            LoadConfigFiles();
            GetALlConfigFiles();
        }

        public void LoadConfigFiles()
        {
            var folderPath = "../../../Config_Files/";

            foreach (string file in Directory.EnumerateFiles(folderPath, "*_config.txt"))
            {
                string[] lines = File.ReadAllLines(file);


                foreach (var line in lines)
                {
                    if (line.Contains("\t"))
                    {
                        var cleanline = line.Replace("\t", string.Empty);
                        var match = Regex.Match(cleanline, @"\:(.*?)\/", RegexOptions.IgnoreCase);
                        Console.WriteLine(match.Groups[1].Value);
                     // var a =   line.Split("\t");
                    }
                }

                //Logic to extract the key and values
                ConfigFile configFile = new ConfigFile();
                configFile.ConfigValues = new List<ConfigData>();

                ConfigData configdata = new ConfigData();
                configdata.Key = "ordersPerHour".ToLower();//tolowercase
                configdata.Value = "6000";
                configdata.DataType = GetDataType("6000");
                configFile.ConfigValues.Add(configdata);

                _configFiles.Add(configFile);

            }

            var result = GetValueFromConfigs("ordersPerHour", "system.int32");

            Console.WriteLine(result);

            SetConfigValue("ordersPerHous", "200");

            result = GetValueFromConfigs("ordersPerHour", "system.int32");

            Console.WriteLine(result);
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

        public List<ConfigFile> GetALlConfigFiles()
        {
            _configFiles.ToList().ForEach(x => x.ConfigValues.ToList().ForEach(t =>
            Console.WriteLine(t.Key)
            ));
            return new List<ConfigFile>();
        }
    }
}
