// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 面包厂原材料生产伪自动任务
**/

using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Tasks
{
    internal class MaterialsFactory
    {
        public static void Produce(string group)
        {
            // 获取当前时间戳
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            // 读取面包厂数据
            JObject obj = Json.ReadFile("breadfactory");
            JObject g = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == group).FirstOrDefault()!;
            // 计算面包厂生产周期
            int cycle = 300 - 20 * ((int)g["factory_level"]! - 1) - 10 * (int)g["speed_level"]!;
            int output = (int)Math.Ceiling(Math.Pow(4, (int)g["factory_level"]!) * (int)Math.Pow(2, (int)g["output_level"]!));
            int maxstorage = 64 * (int)(Math.Pow(4, (int)g["factory_level"]! - 1) * Math.Pow(2, (int)g["storage_level"]!));
            int difference = maxstorage - (int)g["breads"]!;
            bool isfull = maxstorage <= (int)g["breads"]!;
            int isdiverse = (string)g["factory_mode"]! == "diverse" ? 1 : 0;
            string factory_mode = (string)g["factory_mode"]!;
            // 读取面包厂原材料数据
            obj = Json.ReadFile("materials");
            g = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == group).FirstOrDefault()!;
            // 判断面包厂是否为无限供应模式
            if (!factory_mode.Contains("infinite"))
            {
                if ((int)Math.Floor((double)(TimeNow - (long)g["last_produce"]!) / cycle) >= 1)
                {
                    int flour = (int)g["flour"]!;
                    int egg = (int)g["egg"]!;
                    int yeast = (int)g["yeast"]!;
                    for (int i = 0; i < (int)Math.Floor((double)(TimeNow - (long)g["last_produce"]!) / cycle); i++)
                    {
                        // 判断面包厂是否库存已满
                        isfull = isfull || ((flour >= difference * 5 * Math.Pow(4, isdiverse)) && (egg >= difference * 2 * Math.Pow(4, isdiverse)) && (yeast >= difference * Math.Pow(4, isdiverse)));
                        if (isfull)
                        {
                            continue;
                        }
                        Random random = new();
                        flour += random.Next(1, output * 5 + 1);
                        egg += random.Next(1, output * 2 + 1);
                        yeast += random.Next(1, output + 1);
                    }
                    // 更新面包厂数据
                    long last_produce = (long)g["last_produce"]! + ((int)Math.Floor((double)(TimeNow - (long)g["last_produce"]!) / cycle) * cycle);
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "last_produce", TimeNow - (TimeNow % last_produce));
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "flour", flour);
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "egg", egg);
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "yeast", yeast);
                }
            }
        }
    }
}
