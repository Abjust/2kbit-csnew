// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * JSON操作工具模块
**/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Net_Codeintp_cs.Modules.Utils
{
    internal class Json
    {
        static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        public static bool FileExists(string filename)
        {
            if (File.Exists($"{path}\\data\\{filename}.json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CreateFile(string filename, JObject objects)
        {
            using StreamWriter file = File.CreateText($"{path}\\data\\{filename}.json");
            using JsonTextWriter writer = new(file);
            writer.Formatting = Formatting.Indented;
            objects.WriteTo(writer);
            writer.Close();
            file.Close();
        }
        public static JObject ReadFile(string filename)
        {
            using StreamReader file = new($"{path}\\data\\{filename}.json");
            using JsonTextReader reader = new(file);
            JObject o = (JObject)JToken.ReadFrom(reader);
            reader.Close();
            file.Close();
            return o;
        }
        public static bool ObjectExistsInArray(string filename, string location, string property, object value)
        {
            JObject o = ReadFile(filename);
            JArray ar = (JArray)o!.SelectToken(location)!;
            if (ar.Children().Where(x => x.SelectToken(property)!.Value<string>()! == (string)value).FirstOrDefault()! != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void AddObjectToArray(string filename, string location, JObject objects, string? index = null, object? indexvalue = null)
        {
            JObject o = ReadFile(filename);
            string[] s = location.Split(".");
            JArray ar;
            switch (index)
            {
                case null:
                    switch (location.Contains("."))
                    {
                        case true:
                            ar = (JArray)o.SelectToken(s[0])!.SelectToken(s[1])!;
                            break;
                        case false:
                            ar = (JArray)o!.SelectToken(s[0])!;
                            break;
                    }
                    break;
                default:
                    switch (location.Contains("."))
                    {
                        case true:
                            ar = (JArray)o!.SelectToken(s[0])!.Children().Where(x => x.SelectToken(index)!.ToString() == indexvalue!.ToString()).FirstOrDefault()!.SelectToken(s[1])!;
                            break;
                        case false:
                            ar = (JArray)o!.SelectToken(s[0])!;
                            break;
                    }
                    break;
            }
            ar.Add(objects);
            File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
        }
        public static void DeleteObjectFromArray(string filename, string location, string index, object indexvalue, string? objectindex = null, object? objectindexvalue = null)
        {
            JObject o = ReadFile(filename);
            string[] s = location.Split(".");
            JArray ar;
            switch (objectindex)
            {
                case null:
                    switch (location.Contains("."))
                    {
                        case true:
                            ar = (JArray)o.SelectToken(s[0])!.SelectToken(s[1])!;
                            break;
                        case false:
                            ar = (JArray)o!.SelectToken(s[0])!;
                            break;
                    }
                    break;
                default:
                    switch (location.Contains("."))
                    {
                        case true:;
                            ar = (JArray)o!.SelectToken(s[0])!.Children().Where(x => x.SelectToken(objectindex)!.ToString() == objectindexvalue!.ToString()).FirstOrDefault()!.SelectToken(s[1])!;
                            break;
                        case false:
                            ar = (JArray)o!.SelectToken(s[0])!;
                            break;
                    }
                    break;
            }
            ar.Remove(ar.Children().Where(x => x.SelectToken(index)!.ToString() == indexvalue.ToString()).FirstOrDefault()!);
            File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
        }
        public static void ModifyObjectFromArray(string filename, string location, string index, object indexvalue, string property, object value, string? objectindex = null, object? objectindexvalue = null)
        {
            JObject o = ReadFile(filename);
            string[] s = location.Split(".");
            JArray ar;
            switch (objectindex)
            {
                case null:
                    switch (location.Contains("."))
                    {
                        case true:
                            ar = (JArray)o.SelectToken(s[0])!.SelectToken(s[1])!;
                            break;
                        case false:
                            ar = (JArray)o!.SelectToken(s[0])!;
                            break;
                    }
                    break;
                default:
                    switch (location.Contains("."))
                    {
                        case true:
                            ar = (JArray)o!.SelectToken(s[0])!.Children().Where(x => x.SelectToken(objectindex)!.ToString() == objectindexvalue!.ToString()).FirstOrDefault()!.SelectToken(s[1])!;
                            break;
                        case false:
                            ar = (JArray)o!.SelectToken(s[0])!;
                            break;
                    }
                    break;
            }
            JObject obj = (JObject)ar.Children().Where(x => x.SelectToken(index)!.ToString() == indexvalue!.ToString()).FirstOrDefault()!;
            obj[property] = JToken.FromObject(value);
            File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
        }
        public static JArray Sort(JArray array, string index, bool desc)
        {
            return desc switch
            {
                true => new JArray(array.OrderByDescending(obj => (string)obj[index]!)),
                _ => new JArray(array.OrderBy(obj => (string)obj[index]!)),
            };
        }
    }
}