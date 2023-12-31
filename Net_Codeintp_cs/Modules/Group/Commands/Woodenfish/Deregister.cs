﻿// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 木鱼模块：还你木鱼
**/

using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Woodenfish
{
    internal class Deregister : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage();
            if (ChineseConverter.Convert(s, ChineseConversionDirection.TraditionalToSimplified) == "还你木鱼")
            {
                if (Json.FileExists("woodenfish") && Json.ObjectExistsInArray("woodenfish", "players", "playerid", receiver.Sender.Id))
                {
                    Json.DeleteObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id);
                    Logger.Info($"已注销“{receiver.Sender.Name} ({receiver.Sender.Id})”的木鱼账号！");
                    await TrySend.Quote(receiver, "不知道写啥，反正，账号注销辣！");
                }
                else
                {
                    Logger.Warning($"未尝试注销“{receiver.Sender.Name} ({receiver.Sender.Id})”的木鱼账号，因为此人还没有注册过！");
                    await TrySend.Quote(receiver, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                }
            }
        }
    }
}
