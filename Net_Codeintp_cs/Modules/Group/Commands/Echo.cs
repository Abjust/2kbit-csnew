// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 复述文本模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    internal class Echo : IModule
    {
        bool? IModule.IsEnable { get; set; }

        async void IModule.Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            // 如果消息以 !echo 开头且消息链长度大于等于 2
            if (receiver.MessageChain.GetPlainMessage().StartsWith("!echo") && receiver.MessageChain.Count >= 2)
            {
                // 如果要复述的消息的第一部分不包含“echo”
                try
                {
                    if (!receiver.MessageChain.MiraiCode.Replace("!echo ", "").Split(" ")[0].Contains("echo"))
                    {
                        try
                        {
                            await receiver.SendMessageAsync(new MiraiCodeMessage(receiver.MessageChain.MiraiCode.Replace("!echo ", "")));
                            Logger.Info($"已复述文本！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n内容：{receiver.MessageChain.MiraiCode.Replace("!echo ", "")}");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"\n错误信息：\n{e.Message}");
                        }
                    }
                    // 如果要复述的消息的第一部分包含“echo”
                    else
                    {
                        Logger.Warning($"未尝试复述文本，因为这条请求有递归执行的嫌疑！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("群消息发送失败！");
                    Logger.Debug($"\n错误信息：\n{e.Message}");
                }
            }
            // 如果消息以 !echo 开头但消息链长度小于 2
            else if (receiver.MessageChain.GetPlainMessage().StartsWith("!echo"))
            {
                Logger.Warning($"未尝试复述文本，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                await TrySend.Quote(receiver, "参数错误");
            }
        }
    }
}
