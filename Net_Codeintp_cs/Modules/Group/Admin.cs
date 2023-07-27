using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class Admin : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage().Split(" ")[0];
            switch (s)
            {
                case "!mute":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!unmute":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!kick":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!block":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!unblock":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!blockg":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!unblockg":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!op":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!deop":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!opg":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!deopg":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!ignore":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!ignoreg":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "!purge":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
            }
        }
    }
}
