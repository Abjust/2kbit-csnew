// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。



using Net_Codeintp_cs.Modules.Utils;


using Newtonsoft.Json.Linq;
/**
 * 2kbit C# Edition: New
 * 面包厂面包生产伪自动任务
**/

namespace Net_Codeintp_cs.Modules.Group.Tasks
{
    internal class BreadFactory
    {
        public static void Produce(string group)
        {
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            JObject obj = Json.ReadFile("materials");
            JObject g = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == group).FirstOrDefault()!;
            List<int> compare = new()
            {
                (int)g["flour"]!,
                (int)g["egg"]!,
                (int)g["yeast"]!
            };
            obj = Json.ReadFile("breadfactory");
            g = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == group).FirstOrDefault()!;
            if (!((string)g["factory_mode"]!).Contains("infinite"))
            {
                int cycle = 300 - 20 * ((int)g["factory_level"]! - 1) - 10 * (int)g["speed_level"]!;
                int maxstorage = 64 * (int)(Math.Pow(4, (int)g["factory_level"]! - 1) * Math.Pow(2, (int)g["storage_level"]!));
                bool isfull = maxstorage <= (int)g["breads"]!;
                if ((int)Math.Floor((double)(TimeNow - (long)g["last_produce"]!) / cycle) >= 1 && !isfull)
                {
                    int breads = (int)g["breads"]!;
                    int output = (int)Math.Pow(4, (int)g["factory_level"]!) * (int)Math.Pow(2, (int)g["output_level"]!);
                    compare.Add(output);
                    int maxoutput = Math.Min((int)Math.Floor((double)compare[0] / 5), (int)Math.Floor((double)compare[1] / 2));
                    maxoutput = Math.Min((int)Math.Floor((double)maxoutput), (int)Math.Floor((double)compare[2]));
                    maxoutput = Math.Min((int)Math.Floor((double)maxoutput), (int)Math.Floor((double)compare[3]));
                    for (int i = 0; i < (int)Math.Floor((double)(TimeNow - (long)g["last_produce"]!) / cycle); i++)
                    {
                        Random random = new();
                        int randint = random.Next(maxoutput);
                        if (breads + randint >= maxstorage)
                        {
                            breads += maxstorage - breads;
                            compare[0] -= (maxstorage - breads) * 5;
                            compare[1] -= (maxstorage - breads) * 2;
                            compare[2] -= (maxstorage - breads);
                            break;
                        }
                        else
                        {
                            breads += randint;
                            compare[0] -= randint * 5;
                            compare[1] -= randint * 2;
                            compare[2] -= randint;
                            maxoutput = Math.Min((int)Math.Floor((double)compare[0] / 5), (int)Math.Floor((double)compare[1] / 2));
                            maxoutput = Math.Min((int)Math.Floor((double)maxoutput), (int)Math.Floor((double)compare[2]));
                            maxoutput = Math.Min((int)Math.Floor((double)maxoutput), (int)Math.Floor((double)compare[3]));
                        }

                    }
                    long last_produce = (long)g["last_produce"]! + ((int)Math.Floor((double)(TimeNow - (long)g["last_produce"]!) / cycle) * cycle);
                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", group, "last_produce", TimeNow - (TimeNow % last_produce));
                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", group, "breads", breads);
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "flour", compare[0]);
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "egg", compare[1]);
                    Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "yeast", compare[2]);
                }
            }
        }
    }
}
