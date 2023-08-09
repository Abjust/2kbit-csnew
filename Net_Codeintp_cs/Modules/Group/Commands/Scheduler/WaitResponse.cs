// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 定时任务计划模块：等待回应
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Group.Tasks;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Scheduler
{
    internal class WaitResponse : IModule
    {
        public static bool WaitingResponse = false;
        public static bool Modifying = false;
        public static long? WaitUntil { get; set; }
        public static string? Executor { get; set; }
        public static int? TaskId { get; set; }
        public static string? Time { get; set; }
        public static string? Scope { get; set; }
        public bool? IsEnable { get; set; }
        public async void Execute(MessageReceiverBase @base)
        {
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            if (WaitingResponse == true && receiver.Sender.Id == Executor)
            {
                if (receiver.MessageChain.GetPlainMessage() == "!cancel" || TimeNow >= WaitUntil)
                {
                    Logger.Info($"已取消新建或者修改定时任务！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    if (receiver.MessageChain.GetPlainMessage() == "!cancel")
                    {
                        try
                        {
                            await receiver.SendMessageAsync("已取消新建或者修改定时任务！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                }
                else if (Modifying)
                {
                    Logger.Info($"已修改定时任务！\n任务ID：{TaskId!}\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    Json.ModifyObjectFromArray("schedules", "schedules", "taskid", TaskId!, "message", receiver.MessageChain.MiraiCode);
                    try
                    {
                        await receiver.SendMessageAsync($"已修改ID为 {TaskId!} 的定时任务！");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
                else
                {
                    Random random = new();
                    int taskid = random.Next(100000, 1000000);
                    while (Json.ObjectExistsInArray("schedules", "schedules", "taskid", taskid.ToString()))
                    {
                        taskid = random.Next(100000, 1000000);
                        if (!Json.ObjectExistsInArray("schedules", "schedules", "taskid", taskid.ToString()))
                        {
                            break;
                        }
                    }
                    JObject obj = new(
                        new JProperty("taskid", taskid),
                        new JProperty("scope", Scope),
                        new JProperty("time", Time),
                        new JProperty("message", receiver.MessageChain.MiraiCode),
                        new JProperty("enabled", true));
                    Json.AddObjectToArray("schedules", "schedules", obj);
                    Logger.Info($"已新建定时任务！\n任务ID：{taskid}\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    await Schedules.Initialize();
                    try
                    {
                        await receiver.SendMessageAsync($"已新建定时任务！其任务ID为：{taskid}");
                    }
                    catch (Exception e)
                    {
                        Logger.Error("群消息发送失败！");
                        Logger.Debug($"错误信息：\n{e.Message}");
                    }
                }
                WaitingResponse = false;
                Modifying = false;
            }
        }
    }
}
