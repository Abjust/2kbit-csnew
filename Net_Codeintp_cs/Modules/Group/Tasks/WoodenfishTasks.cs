// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 木鱼伪自动任务
**/

using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Tasks
{
    internal class WoodenfishTasks
    {
        public static void GetExp(string playerid)
        {
            // 获取当前时间戳
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            // 获取玩家数据
            JObject obj = Json.ReadFile("woodenfish");
            JObject p = (JObject)obj["players"]!.Where(x => x.SelectToken("playerid")!.Value<string>()! == playerid).FirstOrDefault()!;
            double cyclespeed = (int)Math.Ceiling(60 * Math.Pow(0.978, (int)p["level"]! - 1));
            // 如果玩家没有被封禁且时间超过了一个周期，那么就给玩家增加功德
            if ((int)p["ban"]! == 0 && TimeNow - (long)p["time"]! >= (int)Math.Ceiling((double)cyclespeed))
            {
                double e = (double)p["e"]!;
                int gongde = 0;
                int cycles = Math.Min(12 + (int)p["level"]! - 1, (int)Math.Floor((TimeNow - (long)p["time"]!) / Math.Ceiling(cyclespeed)));
                // 限制最大增长轮数为120轮
                int actual_cycles = Math.Min(cycles, 120);
                for (int i = 0; i < cycles; i++)
                {
                    if (i >= actual_cycles || e >= 200)
                    {
                        continue;
                    }
                    // 木鱼的功德增长公式
                    e = Math.Log10((Math.Pow(10, e) + (int)p["gongde"]!) * Math.Pow(Math.E, (double)p["nirvana"]!) + (int)p["level"]!);
                    // 使用e值的零头来增长原始功德
                    gongde += (int)Math.Round(Math.Pow(10, e - Math.Floor(e)));
                }
                // 更新玩家数据
                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", playerid, "e", e >= 6 ? e : 0);
                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", playerid, "gongde", e >= 6 ? gongde : (int)Math.Round(Math.Pow(10, e)) + gongde);
                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", playerid, "time", TimeNow - (long)((TimeNow - (long)p["time"]!) % Math.Ceiling(cyclespeed)));
            }
        }
    }
}
