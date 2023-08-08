// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：禁言
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class Mute : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.MiraiCode.Trim().Split(" ");
            if (s[0] == "!mute")
            {
                switch (s.Length)
                {
                    case >= 2:
                        switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                        {
                            case true:
                                switch (Permission.IsGroupAdmin(receiver.GroupId, Identify.Do(s[1])))
                                {
                                    case false:
                                        string minutes = s[2] ?? "10";
                                        try
                                        {
                                            await GroupManager.MuteAsync(Identify.Do(s[1]), receiver.GroupId, int.Parse(minutes) * 60);
                                            Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}\n时长：{minutes} 分钟");
                                            try
                                            {
                                                await receiver.SendMessageAsync($"已禁言 {Identify.Do(s[1])}：{minutes} 分钟");
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.Error("群消息发送失败！");
                                                Logger.Debug($"错误信息：\n{e.Message}");
                                            }
                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}\n时长：{minutes} 分钟");
                                            Logger.Debug($"错误信息：\n{ex.Message}");
                                            try
                                            {
                                                await receiver.SendMessageAsync($"无法禁言 {Identify.Do(s[1])}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.Error("群消息发送失败！");
                                                Logger.Debug($"错误信息：\n{e.Message}");
                                            }
                                        }
                                        break;
                                    default:
                                        Logger.Warning($"未尝试执行禁言操作，因为被执行者是机器人管理员！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法禁言 {Identify.Do(s[1])}：被执行者是机器人管理员");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                        break;
                                }
                                break;
                            default:
                                Logger.Warning($"未尝试执行禁言操作，因为执行者权限不足！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}");
                                try
                                {
                                    await receiver.SendMessageAsync($"无法禁言 {Identify.Do(s[1])}：权限不足（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                                break;
                        }
                        break;
                    default:
                        Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                        try
                        {
                            await receiver.SendMessageAsync("参数错误");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                        break;
                }
            }
        }
    }
}
