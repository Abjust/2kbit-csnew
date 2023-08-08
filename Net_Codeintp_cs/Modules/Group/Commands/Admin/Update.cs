// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：更新列表缓存
**/

using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class Update
    {
        public static void Do()
        {
            List<string> JsonList = new()
            {
                "ops",
                "blocklist",
                "ignores"
            };
            foreach (string item in JsonList)
            {
                if (!Json.FileExists(item))
                {
                    JObject objects = new(
                        new JProperty("global",
                            new JObject(
                                new JProperty("list",
                                    new JArray()
                                    )
                                )
                            ),
                        new JProperty("groups",
                            new JArray())
                        );
                    Json.CreateFile(item, objects);
                }
                JObject obj = Json.ReadFile(item);
                switch (item)
                {
                    case "ops":
                        Permission.OpsGlobal = new();
                        Permission.Ops = new();
                        if ((JArray)obj["global"]!["list"]! != null)
                        {
                            foreach (JObject item2 in ((JArray)obj["global"]!["list"]!).Cast<JObject>())
                            {
                                Permission.OpsGlobal!.Add((string)item2["qq"]!);
                            }
                        }
                        if (((JArray)obj["groups"]!).Any())
                        {
                            foreach (JObject o in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if (o["list"]!.Any())
                                {
                                    foreach (JObject item3 in o["list"]!.Cast<JObject>())
                                    {
                                        Permission.Ops!.Add($"{o["groupid"]}_{item3["qq"]}");
                                    }
                                }
                            }
                        }
                        break;
                    case "blocklist":
                        Permission.BlocklistGlobal = new();
                        Permission.Blocklist = new();
                        if ((JArray)obj["global"]!["list"]! != null)
                        {
                            foreach (JObject item2 in ((JArray)obj["global"]!["list"]!).Cast<JObject>())
                            {
                                Permission.BlocklistGlobal!.Add((string)item2["qq"]!);
                            }
                        }
                        if (((JArray)obj["groups"]!).Any())
                        {
                            foreach (JObject o in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if (o["list"]!.Any())
                                {
                                    foreach (JObject item3 in o["list"]!.Cast<JObject>())
                                    {
                                        Permission.Blocklist!.Add($"{o["groupid"]}_{item3["qq"]}");
                                    }
                                }
                            }
                        }
                        break;
                    case "ignores":
                        Permission.IgnoresGlobal = new();
                        Permission.Ignores = new();
                        if ((JArray)obj["global"]!["list"]! != null)
                        {
                            foreach (JObject item2 in ((JArray)obj["global"]!["list"]!).Cast<JObject>())
                            {
                                Permission.IgnoresGlobal!.Add((string)item2["qq"]!);
                            }
                        }
                        if (((JArray)obj["groups"]!).Any())
                        {
                            foreach (JObject o in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if (o["list"]!.Any())
                                {
                                    foreach (JObject item3 in o["list"]!.Cast<JObject>())
                                    {
                                        Permission.Ignores!.Add($"{o["groupid"]}_{item3["qq"]}");
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
