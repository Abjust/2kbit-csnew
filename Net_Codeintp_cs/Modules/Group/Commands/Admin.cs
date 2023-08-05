// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 群管模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    internal class Admin : IModule
    {
        public bool? IsEnable { get; set; }
        static void Update()
        {
            List<string> JsonList = new()
            {
                "ops",
                "blocklist",
                "ignores"
            };
            foreach (string item in JsonList)
            {
                if (!Json.FileExists(item))
                {
                    JObject objects = new(
                        new JProperty("global",
                            new JObject(
                                new JProperty("list",
                                    new JArray()
                                    )
                                )
                            ),
                        new JProperty("groups",
                            new JArray())
                        );
                    Json.CreateFile(item, objects);
                }
                JObject obj = Json.ReadFile(item);
                switch (item)
                {
                    case "ops":
                        Permission.OpsGlobal = new();
                        Permission.Ops = new();
                        if ((JArray)obj["global"]!["list"]! != null)
                        {
                            foreach (JObject item2 in ((JArray)obj["global"]!["list"]!).Cast<JObject>())
                            {
                                Permission.OpsGlobal!.Add((string)item2["qq"]!);
                            }
                        }
                        if (((JArray)obj["groups"]!).Any())
                        {
                            foreach (JObject o in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if (o["list"]!.Any())
                                {
                                    foreach (JObject item3 in o["list"]!.Cast<JObject>())
                                    {
                                        Permission.Ops!.Add($"{o["groupid"]}_{item3["qq"]}");
                                    }
                                }
                            }
                        }
                        break;
                    case "blocklist":
                        Permission.BlocklistGlobal = new();
                        Permission.Blocklist = new();
                        if ((JArray)obj["global"]!["list"]! != null)
                        {
                            foreach (JObject item2 in ((JArray)obj["global"]!["list"]!).Cast<JObject>())
                            {
                                Permission.BlocklistGlobal!.Add((string)item2["qq"]!);
                            }
                        }
                        if (((JArray)obj["groups"]!).Any())
                        {
                            foreach (JObject o in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if (o["list"]!.Any())
                                {
                                    foreach (JObject item3 in o["list"]!.Cast<JObject>())
                                    {
                                        Permission.Blocklist!.Add($"{o["groupid"]}_{item3["qq"]}");
                                    }
                                }
                            }
                        }
                        break;
                    case "ignores":
                        Permission.IgnoresGlobal = new();
                        Permission.Ignores = new();
                        if ((JArray)obj["global"]!["list"]! != null)
                        {
                            foreach (JObject item2 in ((JArray)obj["global"]!["list"]!).Cast<JObject>())
                            {
                                Permission.IgnoresGlobal!.Add((string)item2["qq"]!);
                            }
                        }
                        if (((JArray)obj["groups"]!).Any())
                        {
                            foreach (JObject o in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if (o["list"]!.Any())
                                {
                                    foreach (JObject item3 in o["list"]!.Cast<JObject>())
                                    {
                                        Permission.Ignores!.Add($"{o["groupid"]}_{item3["qq"]}");
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        static string Identify(string rawstring)
        {
            if (rawstring.Contains("[mirai:at:"))
            {
                return rawstring.Replace("[mirai:at:", "").Replace("]", "");
            }
            else
            {
                return rawstring;
            }
        }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.MiraiCode.TrimEnd().Split(" ");
            if (!Permission.Initialized)
            {
                Update();
                Permission.Initialized = true;
            }
            switch (s[0])
            {
                case "!mute":
                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        case true:
                            if (s.Length != 1)
                            {
                                switch (Permission.IsGroupAdmin(receiver.GroupId, Identify(s[1])))
                                {
                                    case false:
                                        switch (s.Length)
                                        {
                                            case 3:
                                                if (int.TryParse(s[2], out int minutes) && minutes >= 1 && minutes <= 43199)
                                                {
                                                    try
                                                    {
                                                        await GroupManager.MuteAsync(Identify(s[1]), receiver.GroupId, minutes * 60);
                                                        Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n时长：{minutes} 分钟");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync($"已禁言 {Identify(s[1])}：{minutes} 分钟");
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error("群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                        break;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n时长：{minutes} 分钟");
                                                        Logger.Debug($"错误信息：\n{ex.Message}");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync($"无法禁言 {Identify(s[1])}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
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
                                                    Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                                                break;
                                            case 2:
                                                try
                                                {
                                                    await GroupManager.MuteAsync(Identify(s[1]), receiver.GroupId, 600);
                                                    Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n时长：10 分钟");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync($"已禁言 {Identify(s[1])}：10 分钟");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n时长：10 分钟");
                                                    Logger.Debug($"错误信息：\n{ex.Message}");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync($"无法禁言 {Identify(s[1])}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                }
                                                break;
                                            default:
                                                Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                                        break;
                                    default:
                                        Logger.Warning($"未尝试执行禁言操作，因为被执行者是机器人管理员！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n时长：10 分钟");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法禁言 {Identify(s[1])}：被执行者是机器人管理员");
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
                            else
                            {
                                Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                        default:
                            Logger.Warning($"未尝试执行禁言操作，因为执行者权限不足！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n时长：10 分钟");
                            try
                            {
                                await receiver.SendMessageAsync($"无法禁言 {Identify(s[1])}：权限不足（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!unmute":
                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        case true:
                            switch (s.Length)
                            {
                                case 2:
                                    try
                                    {
                                        await GroupManager.UnMuteAsync(Identify(s[1]), receiver.GroupId);
                                        Logger.Info($"解禁操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"已解除对 {Identify(s[1])} 的禁言");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error($"已尝试执行解禁操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n错误详细：\n{ex.Message}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法解除对 {Identify(s[1])} 的禁言：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                    }
                                    break;
                                default:
                                    Logger.Warning($"未尝试执行解禁操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                            break;
                        default:
                            Logger.Warning($"未尝试执行解禁操作，因为执行者权限不足！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                            try
                            {
                                await receiver.SendMessageAsync($"无法解除对 {Identify(s[1])} 的禁言：权限不足（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!kick":
                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        case true:
                            if (s.Length != 1)
                            {
                                switch (Permission.IsGroupAdmin(receiver.GroupId, Identify(s[1])))
                                {
                                    case false:
                                        switch (s.Length)
                                        {
                                            case 2:
                                                try
                                                {
                                                    await GroupManager.KickAsync(Identify(s[1]), receiver.GroupId);
                                                    Logger.Info($"踢人操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync($"已从本群踢出 {Identify(s[1])}");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error($"已尝试执行踢人操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}\n错误详细：\n{ex.Message}");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync($"无法踢出 {Identify(s[1])}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                }
                                                break;
                                            default:
                                                Logger.Warning($"未尝试执行踢人操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                                        break;
                                    default:
                                        Logger.Warning($"未尝试执行踢人操作，因为被执行者是机器人管理员！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法踢出 {Identify(s[1])}：被执行者是机器人管理员");
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
                                Logger.Warning($"未尝试执行踢人操作，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                            break;
                        default:
                            Logger.Warning($"未尝试执行踢人操作，因为执行者权限不足！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                            try
                            {
                                await receiver.SendMessageAsync($"无法踢出 {Identify(s[1])}：权限不足（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!block":
                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        case true:
                            try
                            {
                                await receiver.SendMessageAsync("执行成功（迫真）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("权限不足，无法执行操作\n（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!unblock":
                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        case true:
                            try
                            {
                                await receiver.SendMessageAsync("执行成功（迫真）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("权限不足，无法执行操作\n（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!blockg":
                    switch (Permission.IsGlobalAdmin(receiver.Sender.Id))
                    {
                        case true:
                            try
                            {
                                await receiver.SendMessageAsync("执行成功（迫真）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("权限不足，无法执行操作\n（如果是机器人主人，请先尝试使用!opg将自己设置成全局管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!unblockg":
                    switch (Permission.IsGlobalAdmin(receiver.Sender.Id))
                    {
                        case true:
                            try
                            {
                                await receiver.SendMessageAsync("执行成功（迫真）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("权限不足，无法执行操作\n（如果是机器人主人，请先尝试使用!opg将自己设置成全局管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!op":
                    if (receiver.Sender.Permission.ToString() == "Owner" || Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        switch (s.Length)
                        {
                            case 2:
                                switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                                {
                                    case false:
                                        bool isexist = false;
                                        foreach (JObject obj in Json.ReadFile("ops")["groups"]!.Cast<JObject>())
                                        {
                                            if ((string)obj["groupid"]! == receiver.GroupId)
                                            {
                                                isexist = true;
                                            }
                                        }
                                        if (!isexist)
                                        {
                                            JObject obj = new(
                                            new JProperty("groupid", receiver.GroupId),
                                            new JProperty("list",
                                                new JArray()));
                                            Json.AddObjectToArray("ops", "groups", obj);
                                        }
                                        JObject o = new(
                                        new JProperty("qq", Identify(s[1])));
                                        Json.AddObjectToArray("ops", "groups>list", o, "groupid", receiver.GroupId);
                                        Logger.Info($"已将“{receiver.GroupName} ({receiver.GroupId})”的一名成员设置成此群机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"已将 {Identify(s[1])} 设置为本群机器人管理员");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                        Update();
                                        break;
                                    default:
                                        Logger.Info($"未尝试将“{receiver.GroupName} ({receiver.GroupId})”的一名成员设置成此群机器人管理员，因为被执行者已经具备此或更高权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法将 {Identify(s[1])} 设置为本群机器人管理员：已经有此或者更高权限");
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
                                Logger.Warning($"未尝试将“{receiver.GroupName} ({receiver.GroupId})”的一名成员设置成此群机器人管理员，因为提供的参数有误！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                    else
                    {
                        Logger.Warning($"未尝试将“{receiver.GroupName} ({receiver.GroupId})”的一名成员设置成此群机器人管理员，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                        try
                        {
                            await receiver.SendMessageAsync($"无法将 {Identify(s[1])} 设置为本群机器人管理员：权限不足");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!deop":
                    if (receiver.Sender.Permission.ToString() == "Owner" || Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        switch (s.Length)
                        {
                            case 2:
                                switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                                {
                                    case true:
                                        Json.DeleteObjectFromArray("ops", "groups>list", $"qq_{Identify(s[1])}", $"groupid_{receiver.GroupId}");
                                        Logger.Info($"已撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"已撤销 {Identify(s[1])} 的本群机器人管理员权限");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                        Update();
                                        break;
                                    default:
                                        Logger.Info($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为被执行者不具备此权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法撤销 {Identify(s[1])} 的本群机器人管理员权限：被执行者不具备此权限");
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
                                Logger.Warning($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                    else
                    {
                        Logger.Warning($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                        try
                        {
                            await receiver.SendMessageAsync($"无法撤销 {Identify(s[1])} 的本群机器人管理员权限：权限不足");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!opg":
                    if (receiver.Sender.Id == BotMain.OwnerQQ || Permission.IsGlobalAdmin(receiver.Sender.Id))
                    {
                        switch (s.Length)
                        {
                            case 2:
                                switch (Permission.IsGlobalAdmin(Identify(s[1])))
                                {
                                    case false:
                                        JObject o = new(
                                        new JProperty("qq", Identify(s[1])));
                                        Json.AddObjectToArray("ops", "global.list", o);
                                        Logger.Info($"已设置全局机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"已将 {Identify(s[1])} 设置为全局机器人管理员");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                        Update();
                                        break;
                                    default:
                                        Logger.Warning($"未尝试设置全局机器人管理员，因为被执行者已经具备此或更高权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法将 {Identify(s[1])} 设置为全局机器人管理员：已经有此或者更高权限");
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
                                Logger.Warning($"未尝试设置全局机器人管理员，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                    else
                    {
                        Logger.Warning($"未尝试设置全局机器人管理员，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                        try
                        {
                            await receiver.SendMessageAsync($"无法将 {Identify(s[1])} 设置为全局机器人管理员：权限不足");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!deopg":
                    if (receiver.Sender.Id == BotMain.OwnerQQ || Permission.IsGlobalAdmin(receiver.Sender.Id))
                    {
                        switch (s.Length)
                        {
                            case 2:
                                switch (Permission.IsGlobalAdmin(Identify(s[1])))
                                {
                                    case true:
                                        Json.DeleteObjectFromArray("ops", "global.list", $"qq_{Identify(s[1])}");
                                        Logger.Info($"已撤销全局机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"已撤销 {Identify(s[1])} 的全局机器人管理员权限");
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                        Update();
                                        break;
                                    default:
                                        Logger.Warning($"未尝试撤销全局机器人管理员，因为被执行者不具备此权限！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                                        try
                                        {
                                            await receiver.SendMessageAsync($"无法撤销 {Identify(s[1])} 的全局机器人管理员权限：被执行者不具备此权限");
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
                                Logger.Warning($"未尝试撤销“{receiver.GroupName} ({receiver.GroupId})”的一名成员在此群的机器人管理员权限，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                    else
                    {
                        Logger.Warning($"未尝试撤销全局机器人管理员，因为执行者权限不足！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{Identify(s[1])}");
                        try
                        {
                            await receiver.SendMessageAsync($"无法撤销 {Identify(s[1])} 的全局机器人管理员权限：权限不足");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!ignore":
                    switch (Permission.IsGroupAdmin(receiver.GroupId, receiver.Sender.Id))
                    {
                        case true:
                            try
                            {
                                await receiver.SendMessageAsync("执行成功（迫真）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("权限不足，无法执行操作\n（如果是群主，请先尝试使用!op将自己设置成本群管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!ignoreg":
                    switch (Permission.IsGlobalAdmin(receiver.Sender.Id))
                    {
                        case true:
                            try
                            {
                                await receiver.SendMessageAsync("执行成功（迫真）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("权限不足，无法执行操作\n（如果是机器人主人，请先尝试使用!opg将自己设置成全局管理员）");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                            break;
                    }
                    break;
                case "!listop":
                    if (Permission.Ops != null)
                    {
                        string ids = "";
                        foreach (string item in Permission.Ops)
                        {
                            if (item.StartsWith(receiver.GroupId))
                            {
                                if (ids == "")
                                {
                                    ids += item.Replace($"{receiver.GroupId}_", "");
                                }
                                else
                                {
                                    ids += $"、{item.Replace($"{receiver.GroupId}_", "")}";
                                }
                            }
                        }
                        if (ids != "")
                        {
                            Logger.Info($"已尝试列举“{receiver.GroupName} ({receiver.GroupId})”的机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                            try
                            {
                                await receiver.SendMessageAsync($"本群机器人管理员：{ids}");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                        }
                        else
                        {
                            Logger.Warning($"未尝试列举“{receiver.GroupName} ({receiver.GroupId})”的机器人管理员，因为此群没有设置机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                            try
                            {
                                await receiver.SendMessageAsync("本群没有机器人管理员！");
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
                        Logger.Warning($"未尝试列举“{receiver.GroupName} ({receiver.GroupId})”的机器人管理员，因为没有群设置了机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                        try
                        {
                            await receiver.SendMessageAsync("当前没有群设置了机器人管理员！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!listopg":
                    if (Permission.OpsGlobal != null)
                    {
                        string ids = "";
                        foreach (string qq in Permission.OpsGlobal)
                        {
                            if (Permission.OpsGlobal.IndexOf(qq) == 0)
                            {
                                ids += qq;
                            }
                            else
                            {
                                ids += $"、{qq}";
                            }
                        }
                        Logger.Info($"已尝试列举全局机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                        try
                        {
                            await receiver.SendMessageAsync($"当前全局机器人管理员：{ids}");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    else
                    {
                        Logger.Warning($"未尝试列举全局机器人管理员，因为没有设置全局机器人管理员！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                        try
                        {
                            await receiver.SendMessageAsync("当前没有设置全局机器人管理员！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!muteme":
                    switch (s.Length)
                    {
                        case 2:
                            if (int.TryParse(s[1], out int minutes) && minutes >= 1 && minutes <= 43199)
                            {
                                try
                                {
                                    await GroupManager.MuteAsync(receiver.Sender.Id, receiver.GroupId, minutes * 60);
                                    Logger.Info($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：{minutes} 分钟");
                                    try
                                    {
                                        await receiver.SendMessageAsync($"已禁言 {receiver.Sender.Id}：{minutes} 分钟");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Info($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：{minutes} 分钟");
                                    Logger.Debug($"错误详细：\n{ex.Message}");
                                    try
                                    {
                                        await receiver.SendMessageAsync($"无法禁言 {receiver.Sender.Id}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
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
                                Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                            break;
                        case 1:
                            try
                            {
                                await GroupManager.MuteAsync(receiver.Sender.Id, receiver.GroupId, 600);
                                Console.WriteLine($"禁言操作已执行！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：10 分钟");
                                try
                                {
                                    await receiver.SendMessageAsync($"已禁言 {receiver.Sender.Id}：10 分钟");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"已尝试执行禁言操作，但是失败了！\n群：{receiver.GroupName} ({receiver.GroupId})\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})\n被执行者：{receiver.Sender.Id}\n时长：10 分钟");
                                Logger.Debug($"错误详细：\n{ex.Message}");
                                try
                                {
                                    await receiver.SendMessageAsync($"无法禁言 {receiver.Sender.Id}：请检查机器人和被执行者在群内的权限，以及提供的参数是否正确");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                            break;
                        default:
                            Logger.Warning($"未尝试执行禁言操作，因为提供的参数有误！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
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
                    break;
                case "!purge":
                    NotImplemented.Do(receiver.GroupId, s[0]);
                    break;
            }
        }
    }
}