// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 私聊消息转发模块
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
    internal class ForwardMessage : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            FriendMessageReceiver receiver = @base.Concretize<FriendMessageReceiver>();
            if (receiver.FriendId != BotMain.OwnerQQ)
            {
                Logger.Info($"接收到私聊消息！\n消息来自：{receiver.FriendName} ({receiver.FriendId})\n消息内容：\n{receiver.MessageChain.MiraiCode}");
                // 记录上一条消息的来源
                SendMessage.LastMessageFrom = receiver.FriendId;
                // 转发给机器人主人
                try
                {
                    MessageBase test = new MiraiCodeMessage($"消息来自：{receiver.FriendName} ({receiver.FriendId})\n消息内容：\n{receiver.MessageChain.MiraiCode}\n（你可以使用!send <目标QQ>来发送私聊消息，或者使用!reply回复这条消息）");
                    await MessageManager.SendFriendMessageAsync(BotMain.OwnerQQ, test);
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
