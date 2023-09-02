// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 定时任务执行
**/

using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions.Http.Managers;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;


namespace Net_Codeintp_cs.Modules.Group.Tasks
{
    // 单群定时任务
    internal class Schedule : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string groupid = context.MergedJobDataMap.GetString("groupid")!;
            string message = context.MergedJobDataMap.GetString("message")!;
            Logger.Info($"已执行单群定时任务！\n群：{groupid}");
            try
            {
                await MessageManager.SendGroupMessageAsync(groupid, new MiraiCodeMessage(message));
            }
            catch (Exception e)
            {
                Logger.Error("群消息发送失败！");
                Logger.Debug($"错误信息：\n{e.Message}");
            }
        }
    }
    // 全局定时任务
    internal class ScheduleAll : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string message = context.MergedJobDataMap.GetString("message")!;
            Logger.Info("已开始执行全局定时任务！");
            // 获取所有群
            IEnumerable<Mirai.Net.Data.Shared.Group> groups = AccountManager.GetGroupsAsync().GetAwaiter().GetResult();
            foreach (Mirai.Net.Data.Shared.Group group in groups)
            {
                // 判断是否为免打扰群
                if (!Permission.IsOptedOut(group.Id))
                {
                    // 发送消息
                    try
                    {
                        await group.SendGroupMessageAsync(new MiraiCodeMessage(message));
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
    internal class Schedules
    {
        static bool Initialized = false;
        public static async Task DoSomething(IScheduler scheduler, CancellationToken ct)
        {
            if (Initialized)
            {
                await scheduler.Clear(ct);
            }
            await scheduler.Start(ct);
            JObject obj = Json.ReadFile("schedules");
            foreach (JObject item in obj["schedules"]!.Cast<JObject>())
            {
                if ((string)item["scope"]! == "all" && (bool)item["enabled"]! == true)
                {
                    JobDataMap data = new()
                    {
                        {
                            "message", (string)item["message"]!
                        }
                    };
                    string[] time = ((string)item["time"]!).Split(":");
                    IJobDetail job = JobBuilder.Create<ScheduleAll>()
                        .WithIdentity($"all-job-{(int)item["taskid"]!}", "schedules")
                        .UsingJobData(data)
                        .Build();
                    ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"all-trigger-{(int)item["taskid"]!}", "schedules")
                    .ForJob(job)
                    .WithSchedule(CronScheduleBuilder
                        .CronSchedule($"0 {time[1]} {time[0]} ? * *")
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")))
                    .Build();
                    await scheduler.ScheduleJob(job, trigger, ct);
                }
                else if ((string)item["scope"]! != "all" && (bool)item["enabled"]! == true)
                {
                    JobDataMap data = new()
                    {
                        {
                            "groupid", (string)item["scope"]!
                        },
                        {
                            "message", (string)item["message"]!
                        }
                    };
                    string[] time = ((string)item["time"]!).Split(":");
                    IJobDetail job = JobBuilder.Create<Schedule>()
                        .WithIdentity($"group-job-{(int)item["taskid"]!}", "schedules")
                        .UsingJobData(data)
                        .Build();
                    ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"group-trigger-{(int)item["taskid"]!}", "schedules")
                    .ForJob(job)
                    .WithSchedule(CronScheduleBuilder
                        .CronSchedule($"0 {time[1]} {time[0]} ? * *")
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")))
                    .Build();
                    await scheduler.ScheduleJob(job, trigger, ct);
                }
            }
        }
        public static async Task Initialize()
        {
            StdSchedulerFactory factory = new();
            IScheduler scheduler = await factory.GetScheduler();
            CancellationToken ct = new();
            await DoSomething(scheduler, ct);
            Initialized = true;
        }
    }
}
