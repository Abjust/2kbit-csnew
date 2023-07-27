using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Request;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Shared;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using System.Reactive.Linq;
using System.Reflection;

namespace Net_Codeintp_cs
{
    public class BotMain
    {
        public static string OwnerQQ = "";
        public static string BotQQ = "";
        public static string VerifyKey = "";
        static async Task Main()
        {
            // 初始化配置文件
            if (!System.IO.File.Exists("config.txt"))
            {
                string[] lines =
                {
                    "owner_qq=", "bot_qq=","verify_key="
                };
                System.IO.File.Create("config.txt").Close();
                await System.IO.File.WriteAllLinesAsync("config.txt", lines);
                Console.WriteLine("配置文件已创建！现在，你需要前往项目文件夹或者程序文件夹找到config.txt并按照要求编辑");
                Environment.Exit(0);
            }
            else
            {
                foreach (string line in System.IO.File.ReadLines("config.txt"))
                {
                    string[] split = line.Split("=");
                    if (split.Length == 2)
                    {
                        switch (split[0])
                        {
                            case "owner_qq":
                                OwnerQQ = split[1];
                                break;
                            case "bot_qq":
                                BotQQ = split[1];
                                break;
                            case "verify_key":
                                VerifyKey = split[1];
                                break;
                        }
                    }
                }
            }
            // 初始化bot
            var bot = new MiraiBot
            {
                Address = "localhost:8080",
                QQ = BotQQ,
                VerifyKey = VerifyKey
            };
            await bot.LaunchAsync();
            Console.WriteLine("2kbit-cs已启动！");
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
                if (e.FromId == OwnerQQ)
                {
                    // 同意邀请
                    await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Approve, "");
                    Console.WriteLine("机器人已同意加入 " + e.GroupId);
                }
                else
                {
                    // 拒绝邀请
                    await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Reject, "");
                    Console.WriteLine("机器人已拒绝加入 " + e.GroupId);
                }
            });
            // 侦测改名
            bot.EventReceived
            .OfType<MemberCardChangedEvent>()
            .Subscribe(async receiver =>
            {
                if (receiver.Current != "")
                {
                    Console.WriteLine($"侦测到改名！\r\n群：{receiver.Group.Name} ({receiver.Group.Id})\r\nQQ号：{receiver.Member.Id}\r\n原昵称：{receiver.Origin}\r\n新昵称：{receiver.Current}");
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"QQ号：{receiver.Member.Id}\r\n原昵称：{receiver.Origin}\r\n新昵称：{receiver.Current}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出现错误！错误信息：{ex}");
                    }
                }

            });
            // 侦测撤回
            bot.EventReceived
           .OfType<GroupMessageRecalledEvent>()
           .Subscribe(async receiver =>
           {
               var messageChain = new MessageChainBuilder()
                .At(receiver.Operator.Id)
                .Plain(" 你又撤回了什么见不得人的东西？")
                .Build();
               if (receiver.AuthorId != receiver.Operator.Id)
               {
                   if (receiver.Operator.Permission.ToString() != "Administrator" && receiver.Operator.Permission.ToString() != "Owner")
                   {
                       Console.WriteLine($"侦测到撤回！\r\n群：{receiver.Group.Name} ({receiver.Group.Id})\r\n执行者：{receiver.Operator.Name} ({receiver.Operator.Id})\r\n被执行者：{receiver.AuthorId}");
                       try
                       {
                           await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine($"出现错误！错误信息：{ex}");
                       }
                   }
               }
               else
               {
                   Console.WriteLine($"侦测到撤回！\r\n群：{receiver.Group.Name} ({receiver.Group.Id})\r\n执行者：{receiver.Operator.Name} ({receiver.Operator.Id})\r\n被执行者：{receiver.AuthorId}");
                   try
                   {
                       await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine($"出现错误！错误信息：{ex}");
                   }
               }
           });
            // 侦测踢人
            bot.EventReceived
            .OfType<MemberKickedEvent>()
            .Subscribe(async receiver =>
            {
                Console.WriteLine($"侦测到踢人！\r\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\r\n执行者：{receiver.Operator.Name} ({receiver.Operator.Id})\r\n被执行者：{receiver.Member.Name} ({receiver.Member.Id})");
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"{receiver.Member.Name} ({receiver.Member.Id}) 被踢出去辣，好似，开香槟咯！");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误！错误信息：{ex}");
                }
            });
            // 侦测退群
            bot.EventReceived
            .OfType<MemberLeftEvent>()
            .Subscribe(async receiver =>
            {
                Console.WriteLine($"侦测到退群！\r\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\r\n当事者：{receiver.Member.Name} ({receiver.Member.Id})");
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"{receiver.Member.Name} ({receiver.Member.Id}) 退群力（悲）");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误！错误信息：{ex}");
                }
            });
            // 侦测入群
            bot.EventReceived
            .OfType<MemberJoinedEvent>()
            .Subscribe(async receiver =>
            {
                MessageChain? messageChain = new MessageChainBuilder()
               .At(receiver.Member.Id)
               .Plain(" 来辣，让我们一起撅新人！（bushi")
               .Build();
                Console.WriteLine($"侦测到入群！\r\n群：{receiver.Member.Group.Name} ({receiver.Member.Group.Id})\r\n当事者：{receiver.Member.Name} ({receiver.Member.Id})");
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, messageChain);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误！错误信息：{ex}");
                }
            });
            // 加载私聊消息模块
            var friend_modules = new Modules.Friend.LoadModules().GetModules();
            foreach (var module in friend_modules)
            {
                module.IsEnable = true;
            }
            bot.MessageReceived.SubscribeFriendMessage(friend =>
            {
                friend_modules.Raise(friend);
            });
            // 加载群消息模块
            var group_modules = new Modules.Group.LoadModules().GetModules();
            foreach (var module in group_modules)
            {
                module.IsEnable = true;
            }
            bot.MessageReceived.SubscribeGroupMessage(group =>
            {
                Random random1 = new();
                Thread.Sleep(random1.Next(1, 1330));
                group_modules.Raise(group);
            });
            // 阻塞主线程
            var signal = new ManualResetEvent(false);
            signal.WaitOne();
        }
    }
}