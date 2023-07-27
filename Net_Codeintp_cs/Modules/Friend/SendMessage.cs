using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Friend
{
    internal class SendMessage : IModule
    {
        public bool? IsEnable { get; set; }

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
                    catch (Exception ex)
                    {
                        await receiver.SendMessageAsync($"出现错误！错误信息：{ex}");
                        Console.WriteLine($"出现错误！错误信息：{ex}");
                    }
                }
                else
                {
                    await receiver.SendMessageAsync("缺少参数");
                }
            }
        }
    }
}
