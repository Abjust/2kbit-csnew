// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 跨群公告播报模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    internal class Announce : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            if (receiver.MessageChain.GetPlainMessage().StartsWith("!announce") && receiver.MessageChain.GetPlainMessage().Split(" ").Length >= 2)
            {
                if (receiver.Sender.Id == BotMain.OwnerQQ)
                {
                    Logger.Info($"已跨群播报公告！\n执行者：{receiver.Sender.Id}");
                    IEnumerable<Mirai.Net.Data.Shared.Group> groups = AccountManager.GetGroupsAsync().GetAwaiter().GetResult();
                    foreach (Mirai.Net.Data.Shared.Group group in groups)
                    {
                        if (!Permission.IsOptedOut(group.Id))
                        {
                            try
                            {
                                await group.SendGroupMessageAsync(new MiraiCodeMessage(receiver.MessageChain.MiraiCode.Replace("!announce ", "")));
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                        }
                    }
                }
                else
                {
                    Logger.Warning($"未尝试播报跨群公告，因为执行者不是机器人主人！\n执行者：{receiver.Sender.Id}");
                    await TrySend.Quote(receiver, "你不是机器人主人");
                }
            }
        }
    }
}
