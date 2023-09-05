using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace Wpf_IDI
{
    public static class ParametersManager
    {
        static ParametersManager()
        {
            if (!Directory.Exists(parametersFolderPath))
            {
                Directory.CreateDirectory(parametersFolderPath);
            }
        }

        static string parametersFolderPath = "D:/Parameters/"; // 文件路径

        /// <summary>
        /// 如果文件不存在则创建
        /// </summary>
        /// <param name="filename"></param>
        static void CreateFile(ref string filename)
        {
            string file = parametersFolderPath + filename + ".json";
            if (!File.Exists(file))
            {
                File.Create(file).Close();
            }
            filename = file;
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="fileName"></param>
        public static void SaveParameters(List<TextBox> keys, string fileName)
        {
            Dictionary<string, Dictionary<string, object>> parameters = LoadParameters(fileName);

            if (!parameters.ContainsKey(fileName))
            {
                parameters.Add(fileName, new Dictionary<string, object>());
            }

            foreach (var key in keys)
            {
                if (!parameters[fileName].ContainsKey(key.Name))
                {
                    parameters[fileName].Add(key.Name, key.Text);
                }
                else
                {
                    parameters[fileName][key.Name] = key.Text;
                }
            }

            string jsonData = JsonConvert.SerializeObject(parameters, Formatting.Indented);
            fileName = parametersFolderPath + fileName + ".json";
            File.WriteAllText(fileName, jsonData);
        }

        /// <summary>
        /// 读取参数
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, object>> LoadParameters(string fileName)
        {
            CreateFile(ref fileName);

            string data = File.ReadAllText(fileName);
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(data);
            return jsonData ?? new Dictionary<string, Dictionary<string, object>>();
        }

        public enum PName
        {
            Calibration,
            SpiiTcp,
            SpiiMotor,
            LaserTcp
        }
        /// <summary>
        /// 获取参数文件名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPatameteName(PName name)
        {
            switch (name)
            {
                case PName.Calibration:
                    {
                        return "Calibration";
                    }
                case PName.SpiiTcp:
                    {
                        return "SpiiTcp";
                    }
                case PName.SpiiMotor:
                    {
                        return "SpiiMotor";
                    }
                case PName.LaserTcp:
                    {
                        return "LaserTcp";
                    }
                default:
                    {
                        return "";
                    }
            }
        }
    }
}
