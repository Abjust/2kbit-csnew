// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 选择权模块：重新加入项目
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Choice
{
    internal class OptIn : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage();
            if (s == "!optin")
            {
                if (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                {
                    if (Json.ObjectExistsInArray("optedout", "groups", "groupid", receiver.GroupId))
                    {
                        Logger.Info($"已令“{receiver.GroupName} ({receiver.GroupId})”重新加入 2kbit Beta 项目！");
                        Json.DeleteObjectFromArray("optedout", "groups", "groupid", receiver.GroupId);
                        Update.Do();
                        try
                        {
                            await receiver.SendMessageAsync("已重新加入 2kbit Beta 项目（此群将开始收到此机器人的消息）");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    else
                    {
                        Logger.Warning($"未尝试令“{receiver.GroupName} ({receiver.GroupId})”重新加入 2kbit Beta 项目，因为此群已经加入了！");
                        try
                        {
                            await receiver.SendMessageAsync("无法重新加入 2kbit Beta 项目：此群已经加入");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                }
                else
                {
                    Logger.Warning($"未尝试令“{receiver.GroupName} ({receiver.GroupId})”重新加入 2kbit Beta 项目，因为执行者权限不足！");
                }
            }
        }
    }
}
