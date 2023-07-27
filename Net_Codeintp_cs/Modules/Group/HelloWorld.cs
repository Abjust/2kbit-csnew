using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    public class HelloWorld : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            switch (receiver.MessageChain.GetPlainMessage())
            {
                case "!test":
                    try
                    {
                        await receiver.SendMessageAsync("Hello World!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出现错误！错误信息：{ex}");
                    }
                    break;
            }
        }
    }
}
