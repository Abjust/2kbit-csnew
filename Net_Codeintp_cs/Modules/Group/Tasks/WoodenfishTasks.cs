// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。



using Net_Codeintp_cs.Modules.Utils;


using Newtonsoft.Json.Linq;
/**
 * 2kbit C# Edition: New
 * 木鱼伪自动任务
**/

namespace Net_Codeintp_cs.Modules.Group.Tasks
{
    internal class WoodenfishTasks
    {
        public static void GetExp(string playerid)
        {
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            JObject obj = Json.ReadFile("woodenfish");
            JObject p = (JObject)obj["players"]!.Where(x => x.SelectToken("playerid")!.Value<string>()! == playerid).FirstOrDefault()!;
            double cyclespeed = (int)Math.Ceiling(60 * Math.Pow(0.98, (int)p["level"]! - 1));
            Logger.Debug(cyclespeed);
            if ((int)p["ban"]! == 0 && TimeNow - (long)p["time"]! >= (int)Math.Ceiling((double)cyclespeed))
            {
                double e = (double)p["e"]!;
                int cycles = Math.Min(12 + (int)Math.Floor(((double)p["nirvana"]! - 1) / 0.05), (int)Math.Floor((TimeNow - (long)p["time"]!) / Math.Ceiling(cyclespeed)));
                for (int i = 0; i < cycles; i++)
                {
                    e = (e * Math.Pow(Math.Sqrt(Math.E), (double)p["nirvana"]!)) + Math.Log10((int)p["level"]!);
                }
                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", playerid, "e", e);
                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", playerid, "time", TimeNow - (long)((TimeNow - (long)p["time"]!) % Math.Ceiling(cyclespeed)));
            }
        }
    }
}
