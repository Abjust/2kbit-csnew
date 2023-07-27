using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    public class DisplayVersion : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            if (receiver.MessageChain.GetPlainMessage() == "版本")
            {
                List<string> splashes = new()
                        {
                            "也试试HanBot罢！Also try HanBot!",
                            "誓死捍卫微软苏维埃！",
                            "打倒MF独裁分子！",
                            "要把反革命分子的恶臭思想，扫进历史的垃圾堆！",
                            "PHP是世界上最好的编程语言（雾）",
                            "社会主义好，社会主义好~",
                            "Minecraft很好玩，但也可以试试Terraria！",
                            "So Nvidia, f**k you!",
                            "战无不胜的马克思列宁主义万岁！",
                            "Bug是杀不完的，你杀死了一个Bug，就会有千千万万个Bug站起来！",
                            "跟张浩扬博士一起来学Jvav罢！",
                            "哼哼哼，啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊",
                            "你知道吗？其实你什么都不知道！",
                            "Tips:这是一条烫...烫..烫知识（）",
                            "你知道成功的秘诀吗？我告诉你成功的秘诀就是：我操你妈的大臭逼",
                            "有时候ctmd不一定是骂人 可能是传统美德",
                            "python不一定是编程语言 也可能是屁眼通红",
                            "这条标语虽然没有用，但是是有用的，因为他被加上了标语",
                            "使用C#编写！"
                        };
                Random r = new();
                int random = r.Next(splashes.Count);
                try
                {
                    await receiver.SendMessageAsync($"机器人版本：2kbit C# Edition: New 1.0.0\r\n上次更新日期：2023/7/27\r\n更新内容：2kbit C# Edition: New 初始版本\r\n---------\r\n{splashes[random]}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误！错误信息：{ex}");
                }
            }
        }
    }
}
