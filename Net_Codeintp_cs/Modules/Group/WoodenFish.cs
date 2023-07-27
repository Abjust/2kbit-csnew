using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class WoodenFish : IModule
    {
        public bool? IsEnable { get; set; }

        public void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage().Split(" ")[0];
            switch (s)
            {
                case "我的木鱼":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "给我木鱼":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "敲木鱼":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "升级木鱼":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "涅槃重生":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "功德榜":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "封禁榜":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "1":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
            }
        }
    }
}
