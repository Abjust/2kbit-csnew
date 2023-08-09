using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Net_Codeintp_cs.Modules.Group.Commands.Scheduler
{
    internal class AddSchedule : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!add_schedule")
            {
                if (!Json.FileExists("schedules"))
                {
                    JObject obj = new(
                        new JProperty("schedules",
                        new JArray()));
                    Json.CreateFile("schedules", obj);
                }
                switch (s.Length)
                {
                    case 3:
                        string[] time = s[2].Split(":");
                        if (TimeSpan.TryParseExact($"{s[2]}", "h\\:mm", CultureInfo.CurrentCulture, out TimeSpan timespan))
                        {
                            JObject obj = Json.ReadFile("schedules");
                            JObject item = (JObject)obj["schedules"]!.Where(x => x.SelectToken("time")!.Value<string>()! == $"{timespan.Hours}:{timespan.Minutes:00}").FirstOrDefault()!;
                            if (item == null || ((string)item["scope"]! != "all" && (string)item["scope"]! != receiver.GroupId))
                            {
                                switch (s[1])
                                {
                                    case "all":
                                        switch (Permission.IsGlobalAdmin(receiver.Sender.Id))
                                        {
                                            case true:
                                                try
                                                {
                                                    await receiver.SendMessageAsync("请输入定时任务要发送的内容！（如果想取消，请输入!cancel）");
                                                    WaitResponse.WaitingResponse = true;
                                                    WaitResponse.Time = $"{timespan.Hours}:{timespan.Minutes:00}";
                                                    WaitResponse.Scope = "all";
                                                    WaitResponse.Executor = receiver.Sender.Id;
                                                    WaitResponse.WaitUntil = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 45;
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                                break;
                                            default:
                                                Logger.Warning($"未尝试新建定时任务，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                                try
                                                {
                                                    await receiver.SendMessageAsync("无法新建定时任务：权限不足");
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                                break;
                                        }
                                        break;
                                    case "this":
                                        switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                                        {
                                            case true:
                                                try
                                                {
                                                    await receiver.SendMessageAsync("请输入定时任务要发送的内容！（如果想取消，请输入!cancel）");
                                                    WaitResponse.WaitingResponse = true;
                                                    WaitResponse.Time = $"{timespan.Hours}:{timespan.Minutes:00}";
                                                    WaitResponse.Scope = receiver.GroupId;
                                                    WaitResponse.Executor = receiver.Sender.Id;
                                                    WaitResponse.WaitUntil = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 45;
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                                break;
                                            default:
                                                Logger.Warning($"未尝试新建定时任务，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                                try
                                                {
                                                    await receiver.SendMessageAsync("无法新建全局定时任务：权限不足");
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
                                            await receiver.SendMessageAsync("定时任务范围格式有误！（只能是this或者all）");
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
                                Logger.Warning($"未尝试新建定时任务，因为相同时间执行的定时任务已经定义！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                                try
                                {
                                    await receiver.SendMessageAsync("相同时间执行的定时任务已存在！");
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
                }
            }
        }
    }
}
