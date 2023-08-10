// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 木鱼模块：我的木鱼
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Group.Tasks;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Woodenfish
{
    internal class Info : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string s = receiver.MessageChain.GetPlainMessage();
            if (s == "我的木鱼")
            {
                string status = "";
                string word = "";
                if (Json.FileExists("woodenfish") && Json.ObjectExistsInArray("woodenfish", "players", "playerid", receiver.Sender.Id))
                {
                    JObject obj = Json.ReadFile("woodenfish");
                    JObject item = (JObject)obj["players"]!.Where(x => x.SelectToken("playerid")!.Value<string>()! == receiver.Sender.Id).FirstOrDefault()!;
                    if ((long)item["info_ctrl"]! < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
                    {
                        switch ((int)item["ban"]!)
                        {
                            case 0:
                                status = "正常";
                                word = "【敲电子木鱼，见机甲佛祖，取赛博真经】";
                                WoodenfishTasks.GetExp(receiver.Sender.Id);
                                if (Math.Log10((int)item["gongde"]!) >= 1 && (double)item["e"]! <= 200)
                                {
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", Math.Log10(Math.Pow(10, (double)item["e"]!) + (long)item["gongde"]!));
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "gongde", 0);
                                }
                                if (Math.Log10((double)item["e"]!) >= 1)
                                {
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ee", Math.Log10(Math.Pow(10, (double)item["ee"]!) + (double)item["e"]!));
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", 0);
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
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ban", 0);
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "time", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
                                }
                                break;
                        }
                        if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - (long)item["info_time"]! <= 10)
                        {
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "info_count", (int)item["info_count"]! + 1);
                        }
                        else
                        {
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "info_count", 1);
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "info_time", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
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
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "info_ctrl", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 180);
                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "info_count", 0);
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
当前速度：{(int)Math.Ceiling(60 * Math.Pow(0.98, (int)item["level"]!))} 秒/周期
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
        }
    }
}
