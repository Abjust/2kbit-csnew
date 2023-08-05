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
        public static bool ObjectExistsInArray(string filename, string location, string index, string? objectindex = null)
        {
            JObject o = ReadFile(filename);
            JObject o1 = new();
            JArray ar;
            string[] indexsplit = index.Split("_");
            if (location.Contains("."))
            {
                foreach (string part in location.Split("."))
                {
                    if (location.Split(".").Last() != part)
                    {
                        o1 = (JObject)o[part]!;
                    }
                    else
                    {
                        ar = (JArray)o1[part]!;
                        foreach (JObject obj in ar.Cast<JObject>())
                        {
                            if ((string)obj[indexsplit[0]]! == indexsplit[1])
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else if (location.Contains(">"))
            {
                string[] split = location.Split(">");
                string[] objectsplit = objectindex!.Split("_");
                for (int i = 0; i < o[split[0]]!.Count(); i++)
                {
                    if ((string)o[split[0]]![i]![objectsplit[0]]! == objectsplit[1])
                    {
                        return true;
                    }
                }
                ar = (JArray)o1[split[1]]!;
                foreach (JObject obj in ar.Cast<JObject>())
                {
                    if ((string)obj[indexsplit[0]]! == indexsplit[1])
                    {
                        return true;
                    }
                }
            }
            else
            {
                ar = (JArray)o[location]!;
                foreach (JObject obj in ar.Cast<JObject>())
                {
                    if ((string)obj[indexsplit[0]]! == indexsplit[1])
                    {
                        return true;
                    }
                }
            }
            return false;
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
            using FileStream fs = File.OpenRead($"{path}\\data\\{filename}.json");
            using StreamReader file = new(fs);
            using JsonTextReader reader = new(file);
            JObject o = (JObject)JToken.ReadFrom(reader);
            reader.Close();
            file.Close();
            return o;
        }
        public static void AddObjectToArray(string filename, string location, JObject objects, string? property = null, object? value = null)
        {
            JObject o = ReadFile(filename);
            JObject o1 = new();
            JArray ar;
            if (location.Contains("."))
            {
                foreach (string part in location.Split("."))
                {
                    if (location.Split(".").Last() != part)
                    {
                        o1 = (JObject)o[part]!;
                    }
                    else
                    {
                        ar = (JArray)o1[part]!;
                        ar.Add(objects);
                        File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
                    }
                }
            }
            else if (location.Contains(">"))
            {
                string[] split = location.Split(">");
                for (int i = 0; i < o[split[0]]!.Count(); i++)
                {
                    if ((string)o[split[0]]![i]![property!]! == (string)value!)
                    {
                        o1 = (JObject)o[split[0]]![i]!;
                        break;
                    }
                }
                ar = (JArray)o1[split[1]]!;
                ar.Add(objects);
                File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
            }
            else
            {
                ar = (JArray)o[location]!;
                ar.Add(objects);
                File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
            }
        }
        public static void DeleteObjectFromArray(string filename, string location, string index, string? objectindex = null)
        {
            JObject o = ReadFile(filename);
            JObject o1 = new();
            JArray ar;
            string[] indexsplit = index.Split("_");
            if (location.Contains("."))
            {
                foreach (string part in location.Split("."))
                {
                    if (location.Split(".").Last() != part)
                    {
                        o1 = (JObject)o[part]!;
                    }
                    else
                    {
                        ar = (JArray)o1[part]!;
                        foreach (JObject obj in ar.Cast<JObject>())
                        {
                            if ((string)obj[indexsplit[0]]! == indexsplit[1])
                            {
                                ar.Remove(obj);
                                break;
                            }
                        }
                        File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
                    }
                }
            }
            else if (location.Contains(">"))
            {
                string[] split = location.Split(">");
                string[] objectsplit = objectindex!.Split("_");
                for (int i = 0; i < o[split[0]]!.Count(); i++)
                {
                    if ((string)o[split[0]]![i]![objectsplit[0]]! == objectsplit[1])
                    {
                        o1 = (JObject)o[split[0]]![i]!;
                        break;
                    }
                }
                ar = (JArray)o1[split[1]]!;
                foreach (JObject obj in ar.Cast<JObject>())
                {
                    if ((string)obj[indexsplit[0]]! == indexsplit[1])
                    {
                        ar.Remove(obj);
                        break;
                    }
                }
                File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
            }
            else
            {
                ar = (JArray)o[location]!;
                foreach (JObject obj in ar.Cast<JObject>())
                {
                    if ((string)obj[indexsplit[0]]! == indexsplit[1])
                    {
                        ar.Remove(obj);
                        break;
                    }
                }
                File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
            }
        }
        public static void ModifyObjectFromArray(string filename, string location, string index, string property, Type type, object value, string? objectindex = null)
        {
            JObject o = ReadFile(filename);
            JObject o1 = new();
            JArray ar;
            string[] indexsplit = index.Split("_");
            if (location.Contains("."))
            {
                foreach (string part in location.Split("."))
                {
                    if (location.Split(".").Last() != part)
                    {
                        o1 = (JObject)o[part]!;
                    }
                    else
                    {
                        ar = (JArray)o1[part]!;
                        foreach (JObject obj in ar.Cast<JObject>())
                        {
                            if ((string)obj[indexsplit[0]]! == indexsplit[1])
                            {
                                switch (type)
                                {
                                    case Type intType when intType == typeof(int):
                                        obj[property] = int.Parse((string)value);
                                        break;
                                    case Type longType when longType == typeof(long):
                                        obj[property] = long.Parse((string)value);
                                        break;
                                    case Type stringType when stringType == typeof(string):
                                        obj[property] = (string)value;
                                        break;
                                }
                                break;
                            }
                        }
                        File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
                    }
                }
            }
            else if (location.Contains(">"))
            {
                string[] split = location.Split(">");
                string[] objectsplit = objectindex!.Split("_");
                for (int i = 0; i < o[split[0]]!.Count(); i++)
                {
                    if ((string)o[split[0]]![i]![objectsplit[0]]! == objectsplit[1])
                    {
                        o1 = (JObject)o[split[0]]![i]!;
                        break;
                    }
                }
                ar = (JArray)o1[split[1]]!;
                Logger.Debug("Array:");
                Logger.Debug(ar);
                foreach (JObject obj in ar.Cast<JObject>())
                {
                    if ((string)obj[indexsplit[0]]! == indexsplit[1])
                    {
                        switch (type)
                        {
                            case Type intType when intType == typeof(int):
                                obj[property] = int.Parse((string)value);
                                break;
                            case Type longType when longType == typeof(long):
                                obj[property] = long.Parse((string)value);
                                break;
                            case Type stringType when stringType == typeof(string):
                                obj[property] = (string)value;
                                break;
                        }
                        break;
                    }
                }
                Logger.Debug("Modified Array:");
                Logger.Debug(ar);
                File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
            }
            else
            {
                ar = (JArray)o[location]!;
                foreach (JObject obj in ar.Cast<JObject>())
                {
                    if ((string)obj[indexsplit[0]]! == indexsplit[1])
                    {
                        switch (type)
                        {
                            case Type intType when intType == typeof(int):
                                obj[property] = (int)value;
                                break;
                            case Type longType when longType == typeof(long):
                                obj[property] = (long)value;
                                break;
                            case Type doubleType when doubleType == typeof(double):
                                obj[property] = (double)value;
                                break;
                            case Type stringType when stringType == typeof(string):
                                obj[property] = (string)value;
                                break;
                        }
                        break;
                    }
                }
                File.WriteAllText($"{path}\\data\\{filename}.json", o.ToString());
            }
        }
        public static void ReplaceObjectFromArray(string filename, string location, string index, JObject objects, string? objectindex = null)
        {

        }
    }
}