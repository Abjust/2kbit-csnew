// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 定时任务计划模块：修改任务
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Net_Codeintp_cs.Modules.Group.Commands.Scheduler
{
    internal class ModifySchedule : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!modify_schedule")
            {
                if (Json.FileExists("schedules"))
                {
                    switch (s.Length)
                    {
                        case 3:
                            switch (s[1])
                            {
                                case "byid":
                                    if (int.TryParse(s[2], out int taskid))
                                    {
                                        JObject obj = Json.ReadFile("schedules");
                                        JObject item = (JObject)obj["schedules"]!.Where(x => x.SelectToken("taskid")!.ToString() == taskid.ToString()).FirstOrDefault()!;
                                        if (item != null)
                                        {
                                            switch ((string)item["scope"]!)
                                            {
                                                case "all":
                                                    switch (Permission.IsGlobalAdmin(receiver.Sender.Id))
                                                    {
                                                        case true:
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("请输入新的要发送的内容！（如果想取消，请输入!cancel）");
                                                                WaitResponse.WaitingResponse = true;
                                                                WaitResponse.Executor = receiver.Sender.Id;
                                                                WaitResponse.TaskId = (int)obj["taskid"]!;
                                                                WaitResponse.WaitUntil = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 45;
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                        default:
                                                            Logger.Warning($"未尝试修改定时任务，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("无法修改定时任务：权限不足");
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
                                                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                                                    {
                                                        case true:
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("请输入新的要发送的内容！（如果想取消，请输入!cancel）");
                                                                WaitResponse.WaitingResponse = true;
                                                                WaitResponse.Executor = receiver.Sender.Id;
                                                                WaitResponse.TaskId = (int)obj["taskid"]!;
                                                                WaitResponse.WaitUntil = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 45;
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                        default:
                                                            Logger.Warning($"未尝试修改定时任务，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("无法修改定时任务：权限不足");
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Logger.Warning($"未尝试修改定时任务，因为找不到该任务！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                            try
                                            {
                                                await receiver.SendMessageAsync("无法修改定时任务：找不到");
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.Error("群消息发送失败！");
                                                Logger.Debug($"错误信息：\n{e.Message}");
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            await receiver.SendMessageAsync("任务ID格式有误！");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                    }
                                    break;
                                case "bytime":
                                    if (TimeSpan.TryParseExact($"{s[2]}", "h\\:mm", CultureInfo.CurrentCulture, out TimeSpan timespan))
                                    {
                                        JObject obj = Json.ReadFile("schedules");
                                        IEnumerable<JToken> items = obj["schedules"]!;
                                        JObject selected = new();
                                        bool isall = false;
                                        foreach (JToken item in items)
                                        {
                                            if ((string)item["time"]! == $"{timespan.Hours}:{timespan.Minutes:00}")
                                            {
                                                if ((string)item["scope"]! == receiver.GroupId)
                                                {
                                                    selected = (JObject)item;
                                                    break;
                                                }
                                                else if ((string)item["scope"]! == "all")
                                                {
                                                    selected = (JObject)item;
                                                    isall = true;
                                                }
                                            }
                                        }
                                        if (selected.Count > 0)
                                        {
                                            switch (isall)
                                            {
                                                case true:
                                                    switch (Permission.IsGlobalAdmin(receiver.Sender.Id))
                                                    {
                                                        case true:
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("请输入新的要发送的内容！（如果想取消，请输入!cancel）");
                                                                WaitResponse.WaitingResponse = true;
                                                                WaitResponse.Executor = receiver.Sender.Id;
                                                                WaitResponse.TaskId = (int)selected["taskid"]!;
                                                                WaitResponse.WaitUntil = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 45;
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                        case false:
                                                            Logger.Warning($"未尝试修改定时任务，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("无法修改定时任务：权限不足");
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                case false:
                                                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                                                    {
                                                        case true:
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("请输入新的要发送的内容！（如果想取消，请输入!cancel）");
                                                                WaitResponse.WaitingResponse = true;
                                                                WaitResponse.Executor = receiver.Sender.Id;
                                                                WaitResponse.TaskId = (int)selected["taskid"]!;
                                                                WaitResponse.WaitUntil = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 45;
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                        case false:
                                                            Logger.Warning($"未尝试修改定时任务，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("无法修改定时任务：权限不足");
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error("群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Logger.Warning($"未尝试修改定时任务，因为找不到该任务！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                            try
                                            {
                                                await receiver.SendMessageAsync("无法修改定时任务：找不到");
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
                                            await receiver.SendMessageAsync("时间格式有误！");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                    }
                                    break;
                                default:
                                    try
                                    {
                                        await receiver.SendMessageAsync("索引类型有误");
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
}
