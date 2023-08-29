// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 木鱼模块：涅槃重生
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Woodenfish
{
    internal class Nirvana : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage();
            if (s == "涅槃重生")
            {
                if (Json.FileExists("woodenfish") && Json.ObjectExistsInArray("woodenfish", "players", "playerid", receiver.Sender.Id))
                {
                    JObject obj = Json.ReadFile("woodenfish");
                    JObject item = (JObject)obj["players"]!.Where(x => x.SelectToken("playerid")!.Value<string>()! == receiver.Sender.Id).FirstOrDefault()!;
                    if ((double)item["nirvana"]! < 5)
                    {
                        if ((int)item["ban"]! == 0 && (double)item["ee"]! >= 10 + (Math.Truncate(((double)item["nirvana"]! - 1) / 0.05) * 1.5))
                        {
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "nirvana", Math.Truncate(((double)item["nirvana"]! + 0.05) * 100) / 100);
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "level", 1);
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ee", 0);
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", 0);
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "gongde", 0);
                            await TrySend.Quote(receiver, "涅槃重生，功德圆满（喜）");
                        }
                        else if ((int)item["ban"]! == 0 && (double)item["ee"]! <= 10 + (Math.Ceiling(((double)item["nirvana"]! - 1) / 0.05) * 2))
                        {
                            await TrySend.Quote(receiver, "宁踏马功德不够，涅槃重生个毛啊（恼）");
                        }
                        else if ((int)item["ban"]! != 0)
                        {
                            await TrySend.Quote(receiver, "宁踏马被佛祖封号辣（恼）");
                        }
                    }
                    else
                    {
                        await TrySend.Quote(receiver, "宁踏马已经不能涅槃辣（恼）");
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
