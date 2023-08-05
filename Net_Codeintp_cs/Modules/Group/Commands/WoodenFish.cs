// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 木鱼模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    internal class WoodenFish : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage().Split(" ")[0];
            switch (s)
            {
                case "我的木鱼":
                    string status = "";
                    string word = "";
                    if (Json.FileExists("woodenfish"))
                    {
                        if (Json.ObjectExistsInArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}"))
                        {
                            JObject obj = Json.ReadFile("woodenfish");
                            foreach (JObject item in ((JArray)obj["players"]!).Cast<JObject>())
                            {
                                if ((string)item["playerid"]! == receiver.Sender.Id)
                                {
                                    if ((long)item["info_ctrl"]! < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
                                    {
                                        switch ((int)item["ban"]!)
                                        {
                                            case 0:
                                                status = "正常";
                                                word = "【敲电子木鱼，见机甲佛祖，取赛博真经】";
                                                if (Math.Log10((int)item["gongde"]!) >= 1 && (double)item["e"]! <= 200)
                                                {
                                                    Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "e", typeof(double), Math.Log10(Math.Pow(10, (double)item["e"]!) + (long)item["gongde"]!));
                                                    Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "gongde", typeof(int), 0);
                                                }
                                                if (Math.Log10((double)item["e"]!) >= 1)
                                                {
                                                    Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "ee", typeof(double), Math.Log10(Math.Pow(10, (double)item["ee"]!) + (double)item["e"]!));
                                                    Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "e", typeof(double), 0);
                                                }
                                                break;
                                            case 1:
                                                status = "永久封禁中";
                                                word = "【我说那个佛祖啊，我刚刚在刷功德的时候，你有在偷看罢？】";
                                                break;
                                            case 2:
                                                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() < (long)item["dt"]!)
                                                {
                                                    DateTime time = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                                                    time = time.AddSeconds((long)item["dt"]!);
                                                    status = $"暂时封禁中（直至：{TimeZoneInfo.ConvertTime(time, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"))}）";
                                                    word = "【待封禁结束后，可发送“我的木鱼”解封】";
                                                }
                                                else
                                                {
                                                    status = "正常";
                                                    word = "【敲电子木鱼，见机甲佛祖，取赛博真经】";
                                                    Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "ban", typeof(int), 0);
                                                    Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "time", typeof(long), new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
                                                }
                                                break;
                                        }
                                        if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - (long)item["info_time"]! <= 10)
                                        {
                                            Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "info_count", typeof(int), (int)item["info_count"]! + 1);
                                        }
                                        else
                                        {
                                            Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "info_count", typeof(int), 1);
                                        }
                                        string gongde;
                                        string expression = "";
                                        if ((double)item["ee"]! >= 1)
                                        {
                                            expression = $"10^10^{Math.Truncate(10000 * (double)item["ee"]!) / 10000}";
                                            gongde = $"ee{Math.Truncate(10000 * (double)item["ee"]!) / 10000}（{expression}）";
                                        }
                                        else if ((double)item["e"]! >= 1)
                                        {
                                            expression = $"10^{Math.Truncate(10000 * (double)item["e"]!) / 10000}";
                                            gongde = $"e{Math.Truncate(10000 * (double)item["e"]!) / 10000}（{expression}）";
                                        }
                                        else
                                        {
                                            gongde = ((int)item["gongde"]!).ToString();
                                        }
                                        if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - (long)item["info_time"]! <= 10 && (int)item["info_count"]! > 5)
                                        {
                                            Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "info_ctrl", typeof(int), new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 180);
                                            Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "info_count", typeof(int), 0);
                                            Logger.Warning($"因疑似刷屏，有人被暂时禁止使用我的木鱼功能！\n被执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                            MessageChain messageChain = new MessageChainBuilder()
                                                    .At(receiver.Sender.Id)
                                                    .Plain(" 宁踏马3分钟之内别想用我的木鱼辣（恼）")
                                                    .Build();
                                            try
                                            {
                                                await receiver.SendMessageAsync(messageChain);
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.Error("群消息发送失败！");
                                                Logger.Debug($"错误信息：\n{e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Logger.Info($"已显示“{receiver.Sender.Name} ({receiver.Sender.Id})”的木鱼账号的详细信息！");
                                            MessageChain messageChain = new MessageChainBuilder()
                                                .At(receiver.Sender.Id)
                                                .Plain($@"
赛博账号：{receiver.Sender.Id}
账号状态：{status}
木鱼等级：{(int)item["level"]!}
涅槃值：{(double)item["nirvana"]!}
当前速度：{(int)Math.Round(60 * Math.Pow(0.95, (int)item["level"]!))} 秒/周期
当前功德：{gongde}
{word}")
                                                .Build();
                                            try
                                            {
                                                await receiver.SendMessageAsync(messageChain);
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
                        else
                        {
                            Logger.Warning($"未尝试显示“{receiver.Sender.Name} ({receiver.Sender.Id})”的木鱼账号，因为此人没有注册木鱼账号！");
                            try
                            {
                                await receiver.SendMessageAsync("宁踏马害没注册？快发送“给我木鱼”注册罢！");
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
                        Logger.Warning($"未尝试显示“{receiver.Sender.Name} ({receiver.Sender.Id})”的木鱼账号，因为没有人注册了木鱼账号！");
                        try
                        {
                            await receiver.SendMessageAsync("还没有人注册木鱼！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "给我木鱼":
                    if (!Json.FileExists("woodenfish"))
                    {
                        JObject obj = new(
                            new JProperty("players",
                            new JArray()));
                        Json.CreateFile("woodenfish", obj);
                    }
                    if (!Json.ObjectExistsInArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}"))
                    {
                        JObject obj = new(
                            new JProperty("playerid", receiver.Sender.Id),
                            new JProperty("time", 946656000),
                            new JProperty("level", 0),
                            new JProperty("gongde", 0),
                            new JProperty("e", 0),
                            new JProperty("ee", 0),
                            new JProperty("nirvana", 0),
                            new JProperty("ban", 0),
                            new JProperty("dt", 946656000),
                            new JProperty("end_time", 946656000),
                            new JProperty("hit_count", 0),
                            new JProperty("info_time", 946656000),
                            new JProperty("info_count", 0),
                            new JProperty("info_ctrl", 946656000),
                            new JProperty("total_ban", 0)
                            );
                        Json.AddObjectToArray("woodenfish", "players", obj);
                        Logger.Info($"已为“{receiver.Sender.Name} ({receiver.Sender.Id})”注册木鱼账号！");
                        try
                        {
                            await receiver.SendMessageAsync("注册成功辣！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    else
                    {
                        Logger.Warning($"未尝试为“{receiver.Sender.Name} ({receiver.Sender.Id})”注册木鱼账号，因为此人已经注册过了！");
                        try
                        {
                            await receiver.SendMessageAsync("宁踏马不是注册过了吗？");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "敲木鱼":
                    long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (Json.FileExists("woodenfish"))
                    {
                        if (Json.ObjectExistsInArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}"))
                        {
                            JObject obj = Json.ReadFile("woodenfish");
                            foreach (JObject item in ((JArray)obj["players"]!).Cast<JObject>())
                            {
                                if ((string)item["playerid"]! == receiver.Sender.Id)
                                {
                                    switch ((int)item["ban"]!)
                                    {
                                        case 0:
                                            Random r = new();
                                            int random = r.Next(1, 6);
                                            int hit_count;
                                            long endtime;
                                            if (TimeNow - (long)item["end_time"]! <= 3)
                                            {
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "hit_count", typeof(int), (int)item["hit_count"]! + 1);
                                                hit_count = (int)item["hit_count"]! + 1;
                                                endtime = (long)item["end_time"]!;
                                            }
                                            else
                                            {
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "end_time", typeof(long), TimeNow);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "hit_count", typeof(int), 1);
                                                hit_count = 1;
                                                endtime = TimeNow;
                                            }
                                            if (TimeNow - endtime <= 3 && hit_count > 5 && (int)item["total_ban"]! < 4)
                                            {
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "ban", typeof(int), 2);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "hit_count", typeof(int), 0);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "total_ban", typeof(int), (int)item["total_ban"]! + 1);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "gongde", typeof(int), (int)Math.Floor((int)item["gongde"]! * 0.5));
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "ee", typeof(double), (double)item["ee"]! * 0.5);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "e", typeof(double), (double)item["e"]! * 0.5);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "dt", typeof(long), TimeNow + 5400);
                                                MessageChain messageChain1 = new MessageChainBuilder()
                                                    .At(receiver.Sender.Id)
                                                    .Plain($" DoS佛祖是吧？这就给你封了（恼）（你被封禁 90 分钟，功德扣掉 50%）")
                                                    .Build();
                                                try
                                                {
                                                    await receiver.SendMessageAsync(messageChain1);
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                            }
                                            else if (TimeNow - (long)item["end_time"]! <= 3 && (int)item["hit_count"]! > 5 && (int)item["total_ban"]! >= 4)
                                            {
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "ban", typeof(int), 1);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "hit_count", typeof(int), 0);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "total_ban", typeof(int), 5);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "gongde", typeof(int), 0);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "ee", typeof(double), 0);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "e", typeof(double), 0);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "level", typeof(int), 0);
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "nirvana", typeof(double), 1);
                                                MessageChain messageChain1 = new MessageChainBuilder()
                                                    .At(receiver.Sender.Id)
                                                    .Plain($" 多次DoS佛祖，死不悔改，罪加一等（恼）（你被永久封禁，等级、涅槃值重置，功德清零）")
                                                    .Build();
                                                try
                                                {
                                                    await receiver.SendMessageAsync(messageChain1);
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                            }
                                            else
                                            {
                                                Json.ModifyObjectFromArray("woodenfish", "players", $"playerid_{receiver.Sender.Id}", "gongde", typeof(int), (int)item["gongde"]! + random);
                                                MessageChain messageChain1 = new MessageChainBuilder()
                                                    .At(receiver.Sender.Id)
                                                    .Plain($" 功德 +{random}")
                                                    .Build();
                                                try
                                                {
                                                    await receiver.SendMessageAsync(messageChain1);
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                            }
                                            break;
                                        default:
                                            MessageChain messageChain = new MessageChainBuilder()
                                                .At(receiver.Sender.Id)
                                                .Plain(" 宁踏马被佛祖封号辣（恼）")
                                                .Build();
                                            try
                                            {
                                                await receiver.SendMessageAsync(messageChain);
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
                        else
                        {
                            try
                            {
                                await receiver.SendMessageAsync("宁踏马害没注册？快发送“给我木鱼”注册罢！");
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
                            await receiver.SendMessageAsync("还没有人注册木鱼！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "升级木鱼":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "涅槃重生":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "功德榜":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "封禁榜":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
                case "1":
                    NotImplemented.Do(receiver.GroupId, s);
                    break;
            }
        }
    }
}
