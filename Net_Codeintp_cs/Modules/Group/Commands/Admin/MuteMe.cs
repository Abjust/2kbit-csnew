// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：禁言自己
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class MuteMe : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!muteme")
            {
                switch (s.Length)
                {
                    case 2:
                        if (int.TryParse(s[1], out int minutes) && minutes >= 1 && minutes <= 43199)
                        {
                            try
                            {
                                await GroupManager.MuteAsync(receiver.Sender.Id, receiver.GroupId, minutes * 60);
                                Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：{minutes} 分钟");
                                try
                                {
                                    await receiver.SendMessageAsync($"已禁言 {receiver.Sender.Id}：{minutes} 分钟");
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
                                Logger.Info($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：{minutes} 分钟");
                                Logger.Debug($"错误详细：\n{ex.Message}");
                                try
                                {
                                    await receiver.SendMessageAsync($"无法禁言 {receiver.Sender.Id}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                        }
                        else
                        {
                            Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                        break;
                    case 1:
                        try
                        {
                            await GroupManager.MuteAsync(receiver.Sender.Id, receiver.GroupId, 600);
                            Console.WriteLine($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：10 分钟");
                            try
                            {
                                await receiver.SendMessageAsync($"已禁言 {receiver.Sender.Id}：10 分钟");
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
                            Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：10 分钟");
                            Logger.Debug($"错误详细：\n{ex.Message}");
                            try
                            {
                                await receiver.SendMessageAsync($"无法禁言 {receiver.Sender.Id}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                        }
                        break;
                    default:
                        Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
