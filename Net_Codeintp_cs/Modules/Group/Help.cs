using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class Help : IModule
    {
        public bool? IsEnable { get; set; }

        public void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            if (receiver.MessageChain.GetPlainMessage() == "菜单" || receiver.MessageChain.GetPlainMessage() == "!menu")
            {
                NotImplemented.Do(receiver.GroupId, "!menu");
            }
            else if (receiver.MessageChain.GetPlainMessage().StartsWith("!help"))
            {
                NotImplemented.Do(receiver.GroupId, "!help");
            }
        }
    }
}
