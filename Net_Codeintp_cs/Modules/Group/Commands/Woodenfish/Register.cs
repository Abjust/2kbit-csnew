// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 木鱼模块：给我木鱼
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
    internal class Register : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage();
            if (ChineseConverter.Convert(s, ChineseConversionDirection.TraditionalToSimplified) == "给我木鱼")
            {
                if (!Json.FileExists("woodenfish"))
                {
                    JObject obj = new(
                        new JProperty("players",
                        new JArray()));
                    Json.CreateFile("woodenfish", obj);
                }
                if (!Json.ObjectExistsInArray("woodenfish", "players", "playerid", receiver.Sender.Id))
                {
                    long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    JObject obj = new(
                        new JProperty("playerid", receiver.Sender.Id),
                        new JProperty("time", TimeNow),
                        new JProperty("level", 1),
                        new JProperty("gongde", 0),
                        new JProperty("e", 0),
                        new JProperty("ee", 0),
                        new JProperty("nirvana", 1),
                        new JProperty("ban", 0),
                        new JProperty("dt", 946656000),
                        new JProperty("end_time", 946656000),
                        new JProperty("hit_count", 0),
                        new JProperty("info_time", 946656000),
                        new JProperty("info_count", 0),
                        new JProperty("info_ctrl", 946656000),
                        new JProperty("total_ban", 0)
                        );
                    Json.AddObjectToArray("woodenfish", "players", obj);
                    Logger.Info($"已为“{receiver.Sender.Name} ({receiver.Sender.Id})”注册木鱼账号！");
                    await TrySend.Quote(receiver, "注册成功辣！");
                }
                else
                {
                    Logger.Warning($"未尝试为“{receiver.Sender.Name} ({receiver.Sender.Id})”注册木鱼账号，因为此人已经注册过了！");
                    await TrySend.Quote(receiver, "宁踏马不是注册过了吗？");
                }
            }
        }
    }
}
