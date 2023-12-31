﻿// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 主程序
**/

using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Events.Concretes.Request;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Shared;
using Mirai.Net.Modules;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Group;
using Net_Codeintp_cs.Modules.Group.Tasks;
using Net_Codeintp_cs.Modules.Utils;
using System.Reactive.Linq;
using System.Reflection;

namespace Net_Codeintp_cs
{
    public class BotMain
    {
        public static string? OwnerQQ { get; set; }
        public static string? BotQQ { get; set; }
        public static string? VerifyKey { get; set; }
        // 是否启用调试模式
        public static bool DebugEnabled { get; set; }
        static async Task Main()
        {
            // 初始化配置文件
            if (!System.IO.File.Exists("config.txt"))
            {
                string[] lines =
                {
                    "owner_qq=", "bot_qq=","verify_key=","debug=false"
                };
                System.IO.File.Create("config.txt").Close();
                await System.IO.File.WriteAllLinesAsync("config.txt", lines);
                Logger.Info($"配置文件已创建！现在，你需要前往项目文件夹或者程序文件夹（{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!}）找到config.txt并按照要求编辑");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                // 读取配置文件
                try
                {
                    foreach (string line in System.IO.File.ReadLines("config.txt"))
                    {
                        string[] split = line.Split("=");
                        if (split.Length == 2)
                        {
                            switch (split[0])
                            {
                                case "owner_qq":
                                    OwnerQQ = split[1].Trim();
                                    break;
                                case "bot_qq":
                                    BotQQ = split[1].Trim();
                                    break;
                                case "verify_key":
                                    VerifyKey = split[1].Trim();
                                    break;
                                case "debug":
                                    switch (split[1].Trim())
                                    {
                                        case "true":
                                            DebugEnabled = true;
                                            break;
                                        case "false":
                                            DebugEnabled = false;
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("配置文件出错！");
                    Logger.Error(ex.Message);
                    Console.ReadLine();
                    Environment.Exit(1);
                }
            }
            // 初始化数据文件夹
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            // 初始化权限
            Modules.Group.Commands.Admin.Update.Do();
            // 初始化免打扰群列表
            Modules.Group.Commands.Choice.Update.Do();
            // 初始化bot
            MiraiBot bot = new();
            try
            {
                bot = new()
                {
                    Address = "localhost:8080",
                    QQ = BotQQ,
                    VerifyKey = VerifyKey
                };
                await bot.LaunchAsync();
                Logger.Info("2kbit C# Edition: New已启动！");
                Logger.Debug("请注意：调试模式已启用！（这意味着调试日志将在控制台显示，并写入到日志文件）");
            }
            catch (Exception ex)
            {
                Logger.Error("2kbit C# Edition: New启动失败！");
                Logger.Error($"错误信息：\n{ex.Message}");
                Console.ReadLine();
                Environment.Exit(1);
            }
            // 戳一戳效果
            bot.EventReceived
            .OfType<NudgeEvent>()
            .Subscribe(receiver =>
            {

                if (receiver.Target == BotQQ && receiver.Subject.Kind == "Group" && !Permission.IsOptedOut(receiver.Subject.Id))
                {
                    Zuan.Execute(receiver.Subject.Id, receiver.FromId);
                }
            });
            // bot被加好友
            bot.EventReceived
            .OfType<NewFriendRequestedEvent>()
            .Subscribe(async e =>
            {
                await e.ApproveAsync();
            });
            // bot加群
            bot.EventReceived
            .OfType<NewInvitationRequestedEvent>()
            .Subscribe(async e =>
            {
                await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Approve, "");
                Logger.Info("机器人已同意加入 " + e.GroupId);
            });
            // 侦测加群请求
            bot.EventReceived
            .OfType<NewMemberRequestedEvent>()
            .Subscribe(async e =>
            {
                if (Permission.IsBlocked(e.GroupId, e.FromId) && !Permission.IsOptedOut(e.GroupId))
                {
                    await e.RejectAsync("2kbit 已将此人识别为黑名单成员，禁止进入");
                }
            });
            // 侦测改名
            bot.EventReceived
            .OfType<MemberCardChangedEvent>()
            .Subscribe(async receiver =>
            {
                if (receiver.Current != "")
                {
                    Logger.Info($"侦测到改名！\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\r\nQQ号：{receiver.Member.Id}\n原昵称：{receiver.Origin}\n新昵称：{receiver.Current}");
                    if (!Permission.IsOptedOut(receiver.Member.Group.Id))
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"QQ号：{receiver.Member.Id}\n原昵称：{receiver.Origin}\n新昵称：{receiver.Current}");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                }
            });
            // 侦测撤回
            bot.EventReceived
           .OfType<GroupMessageRecalledEvent>()
           .Subscribe(async receiver =>
           {
               Logger.Info($"侦测到撤回！\n群：{receiver.Group.Name} ({receiver.Group.Id})\n执行者：{receiver.Operator.Name} ({receiver.Operator.Id})\n被执行者：{receiver.AuthorId}");
               MessageChain messageChain = new MessageChainBuilder()
                .At(receiver.Operator.Id)
                .Plain(" 宁踏马又撤回了什么见不得人的东西？（恼）")
                .Build();
               if (!Permission.IsOptedOut(receiver.Group.Id))
               {
                   if (receiver.AuthorId != receiver.Operator.Id)
                   {
                       if (receiver.Operator.Permission.ToString() != "Administrator" && receiver.Operator.Permission.ToString() != "Owner")
                       {
                           try
                           {
                               await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
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
                       try
                       {
                           await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
                       }
                       catch (Exception e)
                       {
                           Logger.Error("群消息发送失败！");
                           Logger.Debug($"错误信息：\n{e.Message}");
                       }
                   }
               }
           });
            // 侦测踢人
            bot.EventReceived
            .OfType<MemberKickedEvent>()
            .Subscribe(async receiver =>
            {
                Logger.Info($"侦测到踢人！\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\n执行者：{receiver.Operator.Name} ({receiver.Operator.Id})\n被执行者：{receiver.Member.Name} ({receiver.Member.Id})");
                if (!Permission.IsOptedOut(receiver.Member.Group.Id))
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"{receiver.Member.Name} ({receiver.Member.Id}) 被踢出去辣，好似，开香槟咯！");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
            });
            // 侦测退群
            bot.EventReceived
            .OfType<MemberLeftEvent>()
            .Subscribe(async receiver =>
            {
                Logger.Info($"侦测到退群！\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\n当事者：{receiver.Member.Name} ({receiver.Member.Id})");
                if (!Permission.IsOptedOut(receiver.Member.Group.Id))
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"{receiver.Member.Name} ({receiver.Member.Id}) 退群力（悲）");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
            });
            // 侦测入群
            bot.EventReceived
            .OfType<MemberJoinedEvent>()
            .Subscribe(async receiver =>
            {
                Logger.Info($"侦测到入群！\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\n当事者：{receiver.Member.Name} ({receiver.Member.Id})");
                MessageChain? messageChain = new MessageChainBuilder()
               .At(receiver.Member.Id)
               .Plain(" 来辣，让我们一起撅新人！（bushi")
               .Build();
                if (!Permission.IsOptedOut(receiver.Member.Group.Id))
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, messageChain);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
            });
            // 加载私聊消息模块
            List<IModule> friend_modules = new Modules.Friend.LoadModules().GetModules();
            foreach (IModule module in friend_modules)
            {
                module.IsEnable = true;
                Logger.Debug($"私聊消息模块 {module} 已加载！");
            }
            bot.MessageReceived.SubscribeFriendMessage(friend =>
            {
                friend_modules.Raise(friend);
            });
            // 加载群消息模块
            List<IModule> group_modules = new LoadModules().GetModules();
            foreach (IModule module in group_modules)
            {
                module.IsEnable = true;
                Logger.Debug($"群消息模块 {module} 已加载！");
            }
            bot.MessageReceived.SubscribeGroupMessage(group =>
            {
                if (!Permission.IsOptedOut(group.GroupId) || group.MessageChain.GetPlainMessage() == "!optin")
                {
                    if (!Permission.IsIgnored(group.GroupId, group.Sender.Id))
                    {
                        group_modules.Raise(group);
                    }
                }
            });
            await Schedules.Initialize();
            // 阻塞主线程
            ManualResetEvent signal = new(false);
            signal.WaitOne();
        }
    }
}