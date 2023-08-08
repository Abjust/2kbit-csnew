// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：列举全局机器人管理员
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class ListOpGlobal : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!listopg")
            {
                if (Permission.OpsGlobal != null)
                {
                    string ids = "";
                    foreach (string qq in Permission.OpsGlobal)
                    {
                        if (Permission.OpsGlobal.IndexOf(qq) == 0)
                        {
                            ids += qq;
                        }
                        else
                        {
                            ids += $"、{qq}";
                        }
                    }
                    Logger.Info($"已尝试列举全局机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    try
                    {
                        await receiver.SendMessageAsync($"当前全局机器人管理员：{ids}");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
                else
                {
                    Logger.Warning($"未尝试列举全局机器人管理员，因为没有设置全局机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    try
                    {
                        await receiver.SendMessageAsync("当前没有设置全局机器人管理员！");
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
}
