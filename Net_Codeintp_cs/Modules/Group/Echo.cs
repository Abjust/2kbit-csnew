using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class Echo : IModule
    {
        bool? IModule.IsEnable { get; set; }

        async void IModule.Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            if (receiver.MessageChain.GetPlainMessage().StartsWith("!echo") && receiver.MessageChain.Count >= 2)
            {
                try
                {
                    if (!receiver.MessageChain.MiraiCode.Replace("!echo ", "").Split(" ")[0].Contains("echo"))
                    {
                        await receiver.SendMessageAsync(new MiraiCodeMessage(receiver.MessageChain.MiraiCode.Replace("!echo ", "")));
                    }
                    else
                    {
                        await receiver.SendMessageAsync("请不要妄图递归执行echo指令！");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误！错误信息：{ex}");
                }
            }
            else if (receiver.MessageChain.GetPlainMessage().StartsWith("!echo"))
            {
                await receiver.SendMessageAsync("参数错误");
            }
        }
    }
}
