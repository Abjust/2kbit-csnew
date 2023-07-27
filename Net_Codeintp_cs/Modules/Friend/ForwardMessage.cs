using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Friend
{
    internal class ForwardMessage : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<FriendMessageReceiver>();
            if (receiver.FriendId != BotMain.OwnerQQ)
            {
                MessageBase test = new MiraiCodeMessage($"消息来自：{receiver.FriendName} ({receiver.FriendId})\n消息内容：\n{receiver.MessageChain.MiraiCode}\n（你可以使用/send <目标QQ> <消息>来发送私聊消息）");
                await MessageManager.SendFriendMessageAsync(BotMain.OwnerQQ, test);
            }
        }
    }
}
