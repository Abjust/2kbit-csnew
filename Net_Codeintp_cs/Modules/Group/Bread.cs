using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class Bread : IModule
    {
        public bool? IsEnable { get; set; }

        public void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage().Split(" ")[0];
            switch (s)
            {
                case "!givebread":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!getbread":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!change_mode":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!query":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!upgrade_factory":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!build_factory":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!upgrade_storage":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!upgrade_speed":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!upgrade_output":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
            }
        }
    }
}
