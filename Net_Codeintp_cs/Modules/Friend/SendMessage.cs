// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 发送私聊消息模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Friend
{
    internal class SendMessage : IModule
    {
        public bool? IsEnable { get; set; }

        public static string? LastMessageFrom { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<FriendMessageReceiver>();
            if (receiver.FriendId == BotMain.OwnerQQ && receiver.MessageChain.GetPlainMessage().StartsWith("!send"))
            {
                string code = receiver.MessageChain.MiraiCode;
                string[] s = code.Split(" ");
                if (s.Length >= 3)
                {
                    try
                    {
                        await MessageManager.SendFriendMessageAsync(s[1], new MiraiCodeMessage(code.Replace($"!send {s[1]} ", "")));
                    }
                    catch (Exception e)
                    {
                        Logger.Error("私聊消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                        await receiver.SendMessageAsync("私聊消息发送失败！");
                    }
                }
                else
                {
                    await receiver.SendMessageAsync("缺少参数");
                }
            }
            else if (receiver.FriendId == BotMain.OwnerQQ && receiver.MessageChain.GetPlainMessage().StartsWith("!reply"))
            {
                if (LastMessageFrom != null)
                {
                    string code = receiver.MessageChain.MiraiCode;
                    string[] s = code.Split(" ");
                    if (s.Length >= 2)
                    {
                        try
                        {
                            await MessageManager.SendFriendMessageAsync(LastMessageFrom, new MiraiCodeMessage(code.Replace($"!send {s[1]} ", "")));
                        }
                        catch (Exception e)
                        {
                            Logger.Error("私聊消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                            await receiver.SendMessageAsync("私聊消息发送失败！");
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendFriendMessageAsync(BotMain.OwnerQQ, "没有找到上一条消息的来源");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("私聊消息未能转发给机器人主人！（可能机器人主人不是机器人账号的好友）");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
            }
        }
    }
}
