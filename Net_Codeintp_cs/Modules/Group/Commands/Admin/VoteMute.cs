// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块：表决禁言
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;

namespace Net_Codeintp_cs.Modules.Group.Commands.Admin
{
    internal class VoteMute : IModule
    {
        public bool? IsEnable { get; set; }
        List<Vote>? Votes { get; set; }
        internal class Voter
        {
            public string? Id { get; set; }
            public bool Yes { get; set; }
        }
        internal class Vote
        {
            public int VoteId { get; set; }
            public long StartedAt { get; set; }
            public string? GroupId { get; set; }
            public string? WhoStarted { get; set; }
            public string? MuteWho { get; set; }
            public int VoteDuration { get; set; }
            public int Duration { get; set; }
            public List<Voter>? Voters { get; set; }
        }
        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.MiraiCode.Trim().Split(" ");
            if (s[0] == "!votemute" && s.Length >= 2)
            {
                Votes ??= new();
                switch (s[1])
                {
                    case "list":
                        if (Votes is not null && Votes.Count != 0)
                        {
                            IEnumerable<Vote> votelist = Votes.Where(x => x.GroupId == receiver.GroupId);
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(receiver.Sender.Id)
                                .Plain("\n投票列表\n")
                                .Build();
                            if (votelist.Any())
                            {
                                foreach (Vote vote in votelist)
                                {
                                    int yes = 0;
                                    int no = 0;
                                    if (vote.Voters is not null && vote.Voters.Count > 0)
                                    {
                                        yes = vote.Voters!.Where(x => x.Yes == true).Count();
                                        no = vote.Voters!.Where(x => x.Yes == false).Count();
                                    }
                                    DateTime time = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                                    time = time.AddSeconds(vote.StartedAt + vote.VoteDuration);
                                    MessageChain messageChain1 = new MessageChainBuilder()
                                        .Plain($@"
-----
投票ID：{vote.VoteId}
发起人：{vote.WhoStarted}
被禁言者：{vote.MuteWho}
禁言时长：{vote.Duration} 分钟
目前票数：{yes} : {no}
结束时间：{TimeZoneInfo.ConvertTime(time, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"))}
-----\n
".Trim())
                                        .Build();
                                    foreach (MessageBase message in messageChain1)
                                    {
                                        messageChain.Add(message);
                                    }
                                }
                                await receiver.QuoteMessageAsync(messageChain);
                            }
                            else
                            {
                                await TrySend.Quote(receiver, "本群目前没有投票正在进行！");
                            }
                        }
                        else
                        {
                            await TrySend.Quote(receiver, "目前没有任何投票正在进行！");
                        }
                        break;
                    case "start":
                        int minutes = 0;
                        int duration = 0;
                        int vote_duration = 0;
                        if (s.Length >= 3 && long.TryParse(Identify.Do(s[2]), out _) && 7 <= Identify.Do(s[2]).Length && Identify.Do(s[2]).Length <= 10)
                        {
                            switch (s.Length)
                            {
                                case 5:
                                    if (int.TryParse(s[4], out int vote_minutes) && vote_minutes >= 3 && vote_minutes <= 30)
                                    {
                                        vote_duration = vote_minutes * 60;
                                    }
                                    if (int.TryParse(s[3], out minutes) && minutes >= 30 && minutes <= 43199)
                                    {
                                        duration = minutes;
                                    }
                                    break;
                                case 4:
                                    vote_duration = 180;
                                    if (int.TryParse(s[3], out minutes) && minutes >= 30 && minutes <= 43199)
                                    {
                                        duration = minutes;
                                    }
                                    break;
                                case 3:
                                    vote_duration = 180;
                                    duration = 30;
                                    break;
                            }
                            if (duration > 0 && vote_duration > 0)
                            {
                                if (!Permission.IsGroupAdmin(receiver.GroupId, Identify.Do(s[2])))
                                {
                                    bool hasvote = false;
                                    if (Votes is not null && Votes.Count > 0)
                                    {
                                        foreach (Vote vote in Votes)
                                        {
                                            if (vote.GroupId == receiver.GroupId && vote.MuteWho == Identify.Do(s[2]))
                                            {
                                                hasvote = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!hasvote)
                                    {
                                        long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                        Random r = new();
                                        int voteid = r.Next(100000, 1000000);
                                        while (Votes!.Where(x => x.VoteId == voteid).Any())
                                        {
                                            if (!Votes!.Where(x => x.VoteId == voteid).Any())
                                            {
                                                break;
                                            }
                                            voteid = r.Next(100000, 1000000);
                                        }
                                        Votes!.Add(new Vote()
                                        {
                                            VoteId = voteid,
                                            StartedAt = TimeNow,
                                            GroupId = receiver.GroupId,
                                            WhoStarted = receiver.Sender.Id,
                                            MuteWho = Identify.Do(s[2]),
                                            Duration = duration,
                                            VoteDuration = vote_duration,
                                            Voters = new()
                                        });
                                        Thread thread = new(async () =>
                                        {
                                            await TrySend.Quote(receiver, $"投票表决已开始，持续 {vote_duration} 秒！其投票ID为：{voteid}");
                                            Thread.Sleep(vote_duration * 1000);
                                            await TrySend.Quote(receiver, $"投票ID为 {voteid} 的投票已经结束！开始宣读结果。。。");
                                            Vote vote = Votes!.Find(x => x.VoteId == voteid)!;
                                            int yes = 0;
                                            int no = 0;
                                            if (vote.Voters is not null && vote.Voters.Count > 0)
                                            {
                                                yes = vote.Voters!.Where(x => x.Yes == true).Count();
                                                no = vote.Voters!.Where(x => x.Yes == false).Count();
                                            }
                                            Thread.Sleep(1500);
                                            if (yes <= no || vote.Voters!.Count < 3)
                                            {
                                                await TrySend.Quote(receiver, $"投票ID为 {voteid} 的投票，以 {yes} : {no} 的结果，表决不通过！");
                                            }
                                            else
                                            {
                                                await TrySend.Quote(receiver, $"投票ID为 {voteid} 的投票，以 {yes} : {no} 的结果，表决通过！");
                                                try
                                                {
                                                    await GroupManager.MuteAsync(Identify.Do(s[2]), receiver.GroupId, duration * 60);
                                                    Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}\n时长：{duration} 分钟");
                                                    await TrySend.Quote(receiver, $"已禁言 {Identify.Do(s[2])}：{duration} 分钟");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify.Do(s[1])}\n时长：{duration} 分钟");
                                                    Logger.Debug($"错误信息：\n{ex.Message}");
                                                    await TrySend.Quote(receiver, $"无法禁言 {Identify.Do(s[2])}：机器人踏马的有权限？人家踏马的事群管？这让我怎么搞？（恼）");
                                                }
                                            }
                                            Votes.Remove(Votes!.Find(x => x.VoteId == voteid)!);
                                        });
                                        thread.Start();
                                    }
                                    else
                                    {
                                        await TrySend.Quote(receiver, $"无法表决禁言 {Identify.Do(s[2])}：上个表决尚未结束");
                                    }
                                }
                                else
                                {
                                    await TrySend.Quote(receiver, $"无法表决禁言 {Identify.Do(s[2])}：人家踏马事机器人管理员（恼）");
                                }
                            }
                            else
                            {
                                await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）\n（禁言时长范围是30~43199分钟，表决时长范围是3~30分钟）");
                            }
                        }
                        else
                        {
                            await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）（只允许输入7~10位的QQ号）");
                        }
                        break;
                    case "vote":
                        if (s.Length == 4)
                        {
                            if (int.TryParse(s[2], out int voteid))
                            {
                                if (Votes!.Where(x => x.VoteId == voteid).Any())
                                {
                                    Vote vote = Votes!.Find(x => x.VoteId == voteid)!;
                                    if (vote.GroupId == receiver.GroupId)
                                    {
                                        if (!vote.Voters!.Where(x => x.Id == receiver.Sender.Id).Any())
                                        {
                                            if (s[3].ToLower() == "yes" || s[3].ToLower() == "y")
                                            {
                                                Votes!.Find(x => x.VoteId == voteid)!.Voters!.Add(new Voter()
                                                {
                                                    Id = receiver.Sender.Id,
                                                    Yes = true
                                                });
                                                await TrySend.Quote(receiver, "成功投出同意票！");
                                            }
                                            else if (s[3].ToLower() == "no" || s[3].ToLower() == "n")
                                            {
                                                Votes!.Find(x => x.VoteId == voteid)!.Voters!.Add(new Voter()
                                                {
                                                    Id = receiver.Sender.Id,
                                                    Yes = false
                                                });
                                                await TrySend.Quote(receiver, "成功投出否决票！");
                                            }
                                            else
                                            {
                                                await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                                            }
                                        }
                                        else
                                        {
                                            await TrySend.Quote(receiver, "宁踏马不是投过票了吗？（恼）");
                                        }
                                    }
                                    else
                                    {
                                        await TrySend.Quote(receiver, "此ID的投票不是在此群发起的！（禁止跨群投票）");
                                    }
                                }
                                else
                                {
                                    await TrySend.Quote(receiver, "此ID的投票不存在！");
                                }
                            }
                            else
                            {
                                await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                            }
                        }
                        else
                        {
                            await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                        }
                        break;
                }
            }
        }
    }
}
