// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 叫人模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    internal class Call : IModule
    {
        const int call_cd = 40;
        static long? last_call;
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.MiraiCode.TrimEnd().Split(" ");
            string person;
            if (s[0] == "!call" && (last_call == null || DateTimeOffset.UtcNow.ToUnixTimeSeconds() - last_call >= call_cd))
            {
                if (s.Length >= 2)
                {
                    if (s[1].Contains("[mirai:at:"))
                    {
                        person = s[1].Replace("[mirai:at:", "").Replace("]", "");
                    }
                    else
                    {
                        person = s[1];
                    }
                    MessageChain messages = new MessageChainBuilder()
                        .At(person)
                        .Plain(" 2kbit正在呼叫你")
                        .Build();
                    switch (s.Length)
                    {
                        case 3:
                            if (int.TryParse(s[2], out int number))
                            {
                                if (number >= 10)
                                {
                                    number = 10;
                                }
                                else if (number < 1)
                                {
                                    Logger.Warning($"未尝试执行叫人请求，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                                Logger.Info($"已尝试执行叫人请求！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                for (int i = 0; i < number; i++)
                                {
                                    try
                                    {
                                        await receiver.SendMessageAsync(messages);
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                    Thread.Sleep(333);
                                }
                                last_call = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            }
                            else
                            {
                                Logger.Warning($"未尝试执行叫人请求，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                try
                                {
                                    await receiver.SendMessageAsync("参数错误");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                            break;
                        case 2:
                            Logger.Info($"已尝试执行叫人请求！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                            for (int i = 0; i < 5; i++)
                            {
                                try
                                {
                                    await receiver.SendMessageAsync(messages);
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                                Thread.Sleep(333);
                            }
                            last_call = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            break;
                        default:
                            Logger.Warning($"未尝试执行叫人请求，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                else if (s[0] == "!call")
                {
                    Logger.Warning($"未尝试执行叫人请求，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    try
                    {
                        await receiver.SendMessageAsync("参数错误");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
            }
            else if (s[0] == "!call")
            {
                Logger.Warning($"未尝试执行叫人请求，因为CD时间未到！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                try
                {
                    await receiver.SendMessageAsync($"CD未到，请别急！CD还剩： {call_cd - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - last_call)} 秒");
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
