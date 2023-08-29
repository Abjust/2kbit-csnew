// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：禁言自己
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class MuteMe : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!muteme")
            {
                int duration = 0;
                switch (s.Length)
                {
                    case 2:
                        if (int.TryParse(s[1], out int minutes) && minutes >= 1 && minutes <= 43199)
                        {
                            duration = minutes;
                        }
                        break;
                    case 1:
                        duration = 10;
                        break;
                }
                if (duration != 0)
                {
                    try
                    {
                        await GroupManager.MuteAsync(receiver.Sender.Id, receiver.GroupId, duration * 60);
                        Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：{duration} 分钟");
                        await TrySend.Quote(receiver, $"已禁言 {receiver.Sender.Id}：{duration} 分钟");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：{duration} 分钟");
                        Logger.Debug($"错误详细：\n{ex.Message}");
                        await TrySend.Quote(receiver, $"无法禁言 {receiver.Sender.Id}：机器人踏马的有权限？宁自己踏马的事群管？这让我怎么搞？（恼）");
                    }
                }
                else
                {
                    Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                }
            }
        }
    }
}
