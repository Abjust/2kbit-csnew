// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 版本显示模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    public class DisplayVersion : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            if (receiver.MessageChain.GetPlainMessage() == "版本")
            {
                List<string> splashes = new()
                        {
                            "也试试KuoHuBit罢！Also try KuoHuBit!",
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
                    await receiver.SendMessageAsync($"机器人版本：2kbit C# Edition: New b1.2.2\n上次更新日期：2023/8/10\n更新内容：修复了一部分有关木鱼的算法的问题\n---------\n{splashes[random]}");
                }
                catch (Exception e)
                {
                    Logger.Error("群消息发送失败！");
                    Logger.Debug($"错误信息：\n{e.Message}");
                }
            }
        }
    }
}
