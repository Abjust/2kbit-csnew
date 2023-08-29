// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：下管
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class Deop : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.MiraiCode.Trim().Split(" ");
            if (s[0] == "!deop")
            {
                if (s.Length == 2 && long.TryParse(Identify.Do(s[1]), out _) && 7 <= Identify.Do(s[1]).Length && Identify.Do(s[1]).Length <= 10)
                {
                    if (receiver.Sender.Permission.ToString() == "Owner" || Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        switch (Permission.IsGroupAdmin(receiver.GroupId, Identify.Do(s[1])))
                        {
                            case true:
                                Json.DeleteObjectFromArray("ops", "groups.list", "qq", Identify.Do(s[1]), "groupid", receiver.GroupId);
                                Logger.Info($"已撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}");
                                await TrySend.Quote(receiver, $"已撤销 {Identify.Do(s[1])} 的本群机器人管理员权限");
                                Update.Do();
                                break;
                            default:
                                Logger.Info($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为被执行者不具备此权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}");
                                await TrySend.Quote(receiver, $"无法撤销 {Identify.Do(s[1])} 的本群机器人管理员权限：宁踏马搁这虚空打靶呢？（恼）");
                                break;
                        }
                    }
                    else
                    {
                        Logger.Warning($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1]) ?? null}");
                        await TrySend.Quote(receiver, $"无法撤销 {Identify.Do(s[1])} 的本群机器人管理员权限：宁踏马有权限吗？（恼）");
                    }
                }
                else
                {
                    Logger.Warning($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                }
            }
        }
    }
}
