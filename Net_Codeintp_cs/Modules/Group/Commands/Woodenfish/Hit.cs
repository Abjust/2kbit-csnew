// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 木鱼模块：敲木鱼
**/

using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Woodenfish
{
    internal class Hit : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage();
            if (ChineseConverter.Convert(s, ChineseConversionDirection.TraditionalToSimplified) == "敲木鱼")
            {
                long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (Json.FileExists("woodenfish") && Json.ObjectExistsInArray("woodenfish", "players", "playerid", receiver.Sender.Id))
                {
                    JObject obj = Json.ReadFile("woodenfish");
                    JObject item = (JObject)obj["players"]!.Where(x => x.SelectToken("playerid")!.Value<string>()! == receiver.Sender.Id).FirstOrDefault()!;
                    switch ((int)item["ban"]!)
                    {
                        case 0:
                            int[] add = new int[] { 1, 4, 5 };
                            Random r = new();
                            int random = r.Next(add.Length);
                            int hit_count;
                            long endtime;
                            if (TimeNow - (long)item["end_time"]! <= 3)
                            {
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "hit_count", (int)item["hit_count"]! + 1);
                                hit_count = (int)item["hit_count"]! + 1;
                                endtime = (long)item["end_time"]!;
                            }
                            else
                            {
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "end_time", TimeNow);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "hit_count", 1);
                                hit_count = 1;
                                endtime = TimeNow;
                            }
                            if (TimeNow - endtime <= 3 && hit_count > 5 && (int)item["total_ban"]! < 4)
                            {
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ban", 2);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "hit_count", 0);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "total_ban", (int)item["total_ban"]! + 1);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "gongde", (int)Math.Floor((int)item["gongde"]! * 0.5));
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ee", (double)item["ee"]! * 0.5);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", (double)item["e"]! * 0.5);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "dt", TimeNow + 5400);
                                await TrySend.Quote(receiver, "DoS佛祖是吧？这就给你封了（恼）（你被封禁 90 分钟，功德扣掉 50%）");
                            }
                            else if (TimeNow - (long)item["end_time"]! <= 3 && (int)item["hit_count"]! > 5 && (int)item["total_ban"]! >= 4)
                            {
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ban", 1);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "hit_count", 0);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "total_ban", 5);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "gongde", 0);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ee", 0);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", 0);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "level", 1);
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "nirvana", 1);
                                await TrySend.Quote(receiver, "多次DoS佛祖，死不悔改，罪加一等（恼）（你被永久封禁，等级、涅槃值重置，功德清零）");
                            }
                            else
                            {
                                Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "gongde", (int)item["gongde"]! + add[random]);
                                await TrySend.Quote(receiver, $" 功德 +{add[random]}");
                            }
                            break;
                        default:
                            await TrySend.Quote(receiver, "敲拟吗呢？宁踏马被佛祖封号辣（恼）");
                            break;
                    }
                }
                else
                {
                    await TrySend.Quote(receiver, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                }
            }
        }
    }
}
