// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 面包厂模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands
{
    internal class Bread : IModule
    {
        const int breadfactory_maxlevel = 5;
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
            {
                if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                {
                    JObject obj = Json.ReadFile("breadfactory");
                    foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                    {
                        if ((string)item["groupid"]! == receiver.GroupId)
                        {
                            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)item["last_expfull"]! >= 86400)
                            {
                                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)item["last_expgain"]! >= 86400)
                                {
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "last_expgain", typeof(long), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_exp", typeof(int), (int)item["factory_exp"]! + 1);
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "exp_gained_today", typeof(int), 1);
                                }
                                else if ((int)item["exp_gained_today"]! < (300 * Math.Pow(2, (int)item["factory_level"]! - 1)))
                                {
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "last_expgain", typeof(long), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_exp", typeof(int), (int)item["factory_exp"]! + 1);
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "exp_gained_today", typeof(int), (int)item["exp_gained_today"]! + 1);
                                }
                                else
                                {
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "last_expfull", typeof(long), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "exp_gained_today", typeof(int), 0);
                                    try
                                    {
                                        await receiver.SendMessageAsync("本群已达到今日获取经验上限！");
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
            }
            switch (s[0])
            {
                case "!givebread":
                    switch (s.Length)
                    {
                        case 2:
                            if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                            {
                                if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}") && Json.FileExists("breadfactory"))
                                {
                                    JObject obj = Json.ReadFile("breadfactory");
                                    foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                                    {
                                        if ((string)item["groupid"]! == receiver.GroupId)
                                        {
                                            if ((string)item["factory_mode"]! == "normal")
                                            {
                                                if (int.TryParse(s[1], out int number) && number >= 1)
                                                {
                                                    if ((int)item["breads"]! + number <= (int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1) * Math.Pow(2, (int)item["storage_level"]!)))
                                                    {
                                                        Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "breads", typeof(int), (int)item["breads"]! + number);
                                                        Logger.Info($"有面包厂接收了 {number} 块面包，现在该分厂仓库有 {(int)item["breads"]! + number} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            MessageChain messagechain = new MessageChainBuilder()
                                                                .At(receiver.Sender.Id)
                                                                .Plain($" 现在库存有 {(int)item["breads"]! + number} 块面包辣！")
                                                                .Build();
                                                            await receiver.SendMessageAsync(messagechain);
                                                            break;
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error("群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Logger.Warning($"有面包厂未能接收 {number} 块面包，因为该分厂仓库已满！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync("抱歉，库存已经满了。。。");
                                                            break;
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
                                                    Logger.Warning($"有面包未能送入该分厂，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync("请输入正确的数字！");
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
                                                Logger.Warning($"有面包未能送入该分厂，因为生产模式不允许！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                try
                                                {
                                                    await receiver.SendMessageAsync("请将生产模式设置成“单一化供应”，否则你不能给面包厂面包！");
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
                                else
                                {
                                    Logger.Warning($"有面包未能送入该地区，因为该地区尚未兴建面包厂！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("本群还没有面包厂！");
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
                                Logger.Warning($"有地区未能接收面包，因为没有任何地区兴建了面包厂！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync("面包厂数据文件尚未初始化！（这意味着目前尚没有群建造面包厂）");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                            break;
                        default:
                            Logger.Warning($"有进货订单未被回应，因为提供的参数有误！\n地区：{receiver.GroupName} ({receiver.GroupId})");
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
                case "!getbread":
                    switch (s.Length)
                    {
                        case 2:
                            if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                            {
                                if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                                {
                                    JObject obj = Json.ReadFile("breadfactory");
                                    foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                                    {
                                        if ((string)item["groupid"]! == receiver.GroupId)
                                        {
                                            List<string> bread_types = new()
                                                        {
                                                            char.ConvertFromUtf32(0x1F35E),
                                                            char.ConvertFromUtf32(0x1F956),
                                                            char.ConvertFromUtf32(0x1F950),
                                                            char.ConvertFromUtf32(0x1F96F),
                                                            char.ConvertFromUtf32(0x1F369)
                                                        };
                                            switch ((string)item["factory_mode"]!)
                                            {
                                                case "infinite_diverse":
                                                    if (int.TryParse(s[1], out int number1) && number1 >= bread_types.Count)
                                                    {
                                                        Random rnd = new();
                                                        int[] fields = new int[bread_types.Count];
                                                        int sum = 0;
                                                        for (int i = 0; i < fields.Length - 1; i++)
                                                        {
                                                            double exponent = 0.5 - (i * (0.5 / bread_types.Count));
                                                            fields[i] = rnd.Next(1, (int)Math.Floor((number1 - sum) * (1 - exponent)));
                                                            sum += fields[i];
                                                        }
                                                        fields[fields.Length - 1] = number1 - sum;
                                                        string text = " ";
                                                        switch (number1)
                                                        {
                                                            case <= 20:
                                                                for (int i = 0; i < bread_types.Count; i++)
                                                                {
                                                                    text += string.Join("", Enumerable.Repeat(bread_types[i], fields[i]));
                                                                }
                                                                break;
                                                            default:
                                                                for (int i = 0; i < bread_types.Count; i++)
                                                                {
                                                                    text += $"\n{bread_types[i]}*{fields[i]}";
                                                                }
                                                                break;
                                                        }
                                                        MessageChain? messageChain = new MessageChainBuilder()
                                                           .At(receiver.Sender.Id)
                                                           .Plain(text)
                                                           .Build();
                                                        Logger.Info($"有面包厂向该地区的一名客户送出了 {number1} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync(messageChain);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error($"从“{receiver.GroupName} ({receiver.GroupId})”运出的 {number1} 块面包在运输途中丢了，因为群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Logger.Warning($"有面包厂未能供应 {number1} 块面包，因为请求进货的面包数太少！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync($"你请求进货的面包数太少了！（至少要有 {bread_types.Count} 块）");
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error("群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                    }
                                                    break;
                                                case "infinite":
                                                    if (int.TryParse(s[1], out int number2) && number2 >= 1)
                                                    {
                                                        Logger.Info($"有面包厂向该地区的一名客户送出了 {number2} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            string message = " ";
                                                            message = number2 switch
                                                            {
                                                                <= 20 => string.Join("", Enumerable.Repeat(char.ConvertFromUtf32(0x1F35E), number2)),
                                                                _ => $"{char.ConvertFromUtf32(0x1F35E)} * {number2}",
                                                            };
                                                            MessageChain messageChain = new MessageChainBuilder()
                                                                .At(receiver.Sender.Id)
                                                                .Plain(message)
                                                                .Build();
                                                            await receiver.SendMessageAsync(messageChain);
                                                            break;
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error($"从“{receiver.GroupName} ({receiver.GroupId})”运出的 {number2} 块面包在运输途中丢了，因为群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Logger.Warning($"有面包厂未能回应供货订单，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync("请输入正确的数字！");
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error("群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                    }
                                                    break;
                                                case "diverse":
                                                    if (int.TryParse(s[1], out int number3) && number3 >= bread_types.Count)
                                                    {
                                                        if ((int)item["breads"]! >= number3)
                                                        {
                                                            Random rnd = new();
                                                            int[] fields = new int[bread_types.Count];
                                                            int sum = 0;
                                                            for (int i = 0; i < fields.Length - 1; i++)
                                                            {
                                                                double exponent = 0.5 - (i * (0.5 / bread_types.Count));
                                                                fields[i] = rnd.Next(1, (int)Math.Floor((number3 - sum) * (1 - exponent)));
                                                                sum += fields[i];
                                                            }
                                                            fields[fields.Length - 1] = number3 - sum;
                                                            Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "breads", typeof(int), (int)item["breads"]! - number3);
                                                            Logger.Info($"有面包厂向该地区的一名客户送出了 {number3} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                            string text = " ";
                                                            switch (number3)
                                                            {
                                                                case <= 20:
                                                                    for (int i = 0; i < bread_types.Count; i++)
                                                                    {
                                                                        text += string.Join("", Enumerable.Repeat(bread_types[i], fields[i]));
                                                                    }
                                                                    break;
                                                                default:
                                                                    for (int i = 0; i < bread_types.Count; i++)
                                                                    {
                                                                        text += $"\n{bread_types[i]}*{fields[i]}";
                                                                    }
                                                                    break;
                                                            }
                                                            MessageChain? messageChain = new MessageChainBuilder()
                                                           .At(receiver.Sender.Id)
                                                           .Plain(text)
                                                           .Build();
                                                            Logger.Info($"有面包厂向该地区的一名客户送出了 {number3} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync(messageChain);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error($"从“{receiver.GroupName} ({receiver.GroupId})”运出的 {number3} 块面包在运输途中丢了，因为群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Logger.Warning($"有面包厂未能供应 {number3} 块面包，因为该分厂库存内的面包不够！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("抱歉，面包不够了。。。");
                                                                break;
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
                                                        Logger.Warning($"有面包厂未能供应 {number3} 块面包，因为请求进货的面包数太少！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync($"你请求进货的面包数太少了！（至少要有 {bread_types.Count} 块）");
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Logger.Error("群消息发送失败！");
                                                            Logger.Debug($"错误信息：\n{e.Message}");
                                                        }
                                                    }
                                                    break;
                                                case "normal":
                                                    if (int.TryParse(s[1], out int number4) && number4 >= 1)
                                                    {
                                                        if ((int)item["breads"]! >= number4)
                                                        {
                                                            Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "breads", typeof(int), (int)item["breads"]! - number4);
                                                            Logger.Info($"有面包厂向该地区的一名客户送出了 {number4} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                            try
                                                            {
                                                                string message = " ";
                                                                message = number4 switch
                                                                {
                                                                    <= 20 => string.Join("", Enumerable.Repeat(char.ConvertFromUtf32(0x1F35E), number4)),
                                                                    _ => $"{char.ConvertFromUtf32(0x1F35E)} * {number4}",
                                                                };
                                                                MessageChain messageChain = new MessageChainBuilder()
                                                                    .At(receiver.Sender.Id)
                                                                    .Plain(message)
                                                                    .Build();
                                                                await receiver.SendMessageAsync(messageChain);
                                                                break;
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Logger.Error($"从“{receiver.GroupName} ({receiver.GroupId})”运出的 {number4} 块面包在运输途中丢了，因为群消息发送失败！");
                                                                Logger.Debug($"错误信息：\n{e.Message}");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Logger.Warning($"有面包厂未能供应 {number4} 块面包，因为该分厂库存内的面包不够！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                            try
                                                            {
                                                                await receiver.SendMessageAsync("抱歉，面包不够了。。。");
                                                                break;
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
                                                        Logger.Warning($"有面包厂未能回应供货订单，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                        try
                                                        {
                                                            await receiver.SendMessageAsync("请输入正确的数字！");
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
                                else
                                {
                                    Logger.Warning($"有地区未能响应该地区客户的供货订单，因为该地区尚未兴建面包厂！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("本群还没有面包厂！");
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
                                Logger.Warning($"有地区未能响应该地区客户的供货订单，因为没有任何地区兴建了面包厂！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync("面包厂数据文件尚未初始化！（这意味着目前尚没有群建造面包厂）");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                            break;
                        default:
                            Logger.Warning($"有供货订单未被回应，因为提供的参数有误！\n地区：{receiver.GroupName} ({receiver.GroupId})");
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
                case "!change_mode":
                    if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                    {
                        if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    if (((string)item["factory_mode"]!).Contains("infinite"))
                                    {
                                        switch (s[1])
                                        {
                                            case "normal":
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_mode", typeof(string), "normal");
                                                Logger.Info($"有面包厂已将生产模式切换为：“单一化供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                try
                                                {
                                                    await receiver.SendMessageAsync("已将本群生产模式修改为：单一化供应");
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                                break;
                                            default:
                                                Logger.Warning($"有分厂未能修改生产模式，因为当前生产模式是“无限供应”或其变体！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                try
                                                {
                                                    await receiver.SendMessageAsync("由于当前面包厂生产模式是“无限供应”或其变体，因此仅允许切换到“单一化供应”！");
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
                                        if ((int)item["breads"]! == 0 || ((string)item["factory_mode"]!).Contains("infinite"))
                                        {
                                            switch (s[1])
                                            {
                                                case "infinite_diverse":
                                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_mode", typeof(string), "infinite_diverse");
                                                    Logger.Info($"有面包厂已将生产模式切换为：“多样化无限供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync("已将本群生产模式修改为：多样化无限供应");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                    break;
                                                case "infinite":
                                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_mode", typeof(string), "infinite");
                                                    Logger.Info($"有面包厂已将生产模式切换为：“无限供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync("已将本群生产模式修改为：无限供应");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                    break;
                                                case "diverse":
                                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_mode", typeof(string), "diverse");
                                                    Logger.Info($"有面包厂已将生产模式切换为：“多样化供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync("已将本群生产模式修改为：多样化供应");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                    break;
                                                case "normal":
                                                    Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_mode", typeof(string), "normal");
                                                    Logger.Info($"有面包厂已将生产模式切换为：“单一化供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                                    try
                                                    {
                                                        await receiver.SendMessageAsync("已将本群生产模式修改为：单一化供应");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Logger.Error("群消息发送失败！");
                                                        Logger.Debug($"错误信息：\n{e.Message}");
                                                    }
                                                    break;
                                                default:
                                                    Logger.Warning($"有分厂未能修改生产模式，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
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
                                            Logger.Warning($"有分厂未能修改生产模式，因为面包厂库存不是空的！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                            try
                                            {
                                                await receiver.SendMessageAsync("切换生产模式之前，必须先清空面包厂库存！");
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
                            Logger.Warning($"生产模式切换请求未能执行，因为该地区尚未兴建面包厂！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                            try
                            {
                                await receiver.SendMessageAsync("本群还没有面包厂！");
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
                        Logger.Warning($"生产模式切换请求未能执行，因为没有任何地区兴建了面包厂！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                        try
                        {
                            await receiver.SendMessageAsync("面包厂数据文件尚未初始化！（这意味着目前尚没有群建造面包厂）");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!query":
                    if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                    {
                        if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                        {
                            int flour = 0;
                            int egg = 0;
                            int yeast = 0;
                            JObject obj = Json.ReadFile("materials");
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    flour = (int)item["flour"]!;
                                    egg = (int)item["egg"]!;
                                    yeast = (int)item["yeast"]!;
                                    break;
                                }
                            }
                            obj = Json.ReadFile("breadfactory");
                            bool is_maxlevel = false;
                            string mode = "";
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    if ((int)item["factory_level"]! == breadfactory_maxlevel)
                                    {
                                        is_maxlevel = true;
                                    }
                                    switch ((string)item["factory_mode"]!)
                                    {
                                        case "infinite_diverse":
                                            mode = "多样化无限供应";
                                            break;
                                        case "infinite":
                                            mode = "无限供应";
                                            break;
                                        case "diverse":
                                            mode = "多样化供应";
                                            break;
                                        case "normal":
                                            mode = "单一化供应";
                                            break;
                                    }
                                    MessageChain messageChain;
                                    if (is_maxlevel)
                                    {
                                        messageChain = new MessageChainBuilder()
                                            .At(receiver.Sender.Id)
                                            .Plain($@"
本群 ({receiver.GroupId}) 面包厂信息如下：
-----面包厂属性-----
面包厂等级： {breadfactory_maxlevel} 级（满级）
库存升级次数：{(int)item["storage_level"]!} 次
生产速度升级次数：{(int)item["speed_level"]!} 次
产量升级次数：{(int)item["output_level"]!} 次
面包厂经验：{(int)item["factory_exp"]!} XP
今日已获得经验：{(int)item["exp_gained_today"]!} / {(int)(300 * Math.Pow(2, (int)item["factory_level"]! - 1))} XP
生产（供应）模式：{mode}
-----面包厂配置-----
面包库存上限：{(int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1)) * Math.Pow(2, (int)item["storage_level"]!)} 块
生产周期：{300 - 20 * ((int)item["factory_level"]! - 1) - 10 * (int)item["speed_level"]!} 秒
每周期最大产量：{(int)Math.Pow(4, (int)item["factory_level"]!) * (int)Math.Pow(2, (int)item["output_level"]!)} 块
-----物品库存-----
现有原材料：{flour} 份面粉、{egg} 份鸡蛋、{yeast} 份酵母
现有面包：{(int)item["breads"]!} / {(int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1)) * Math.Pow(2, (int)item["storage_level"]!)} 块
")
                                            .Build();
                                    }
                                    else
                                    {
                                        messageChain = new MessageChainBuilder()
                                            .At(receiver.Sender.Id)
                                            .Plain($@"
本群 ({receiver.GroupId}) 面包厂信息如下：
-----面包厂属性-----
面包厂等级：{(int)item["factory_level"]!} / {breadfactory_maxlevel} 级
库存升级次数：{(int)item["storage_level"]!} 次
生产速度升级次数：{(int)item["speed_level"]!} 次
产量升级次数：{(int)item["output_level"]!} 次
面包厂经验：{(int)item["factory_exp"]!} XP
今日已获得经验：{(int)item["exp_gained_today"]!} / {(int)(300 * Math.Pow(2, (int)item["factory_level"]! - 1))} XP
生产（供应）模式：{mode}
-----面包厂配置-----
面包库存上限：{(int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1)) * Math.Pow(2, (int)item["storage_level"]!)} 块
生产周期：{300 - 20 * ((int)item["factory_level"]! - 1) - 10 * (int)item["speed_level"]!} 秒
每周期最大产量：{(int)Math.Pow(4, (int)item["factory_level"]!) * (int)Math.Pow(2, (int)item["output_level"]!)} 块
-----物品库存-----
现有原材料：{flour} 份面粉、{egg} 份鸡蛋、{yeast} 份酵母
现有面包：{(int)item["breads"]!} / {(int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1)) * Math.Pow(2, (int)item["storage_level"]!)} 块
")
                                            .Build();
                                    }
                                    try
                                    {
                                        await receiver.SendMessageAsync(messageChain);
                                        Logger.Info($"已提供分厂“{receiver.GroupName} ({receiver.GroupId})”的完整信息！");
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
                        else
                        {
                            Logger.Warning($"未能提供地区“{receiver.GroupName} ({receiver.GroupId})”的分厂信息，因为该地区尚未兴建面包厂！");
                            try
                            {
                                await receiver.SendMessageAsync("本群还没有面包厂！");
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
                        Logger.Warning($"未能提供地区“{receiver.GroupName} ({receiver.GroupId})”的分厂信息，因为没有任何地区兴建了面包厂！");
                        try
                        {
                            await receiver.SendMessageAsync("面包厂数据文件尚未初始化！（这意味着目前尚没有群建造面包厂）");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!upgrade_factory":
                    if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                    {
                        if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    if ((int)item["factory_level"]! != breadfactory_maxlevel)
                                    {
                                        int formula = 900 * (int)Math.Pow(2, (int)item["factory_level"]! - 1);
                                        if ((int)item["factory_exp"]! >= formula)
                                        {
                                            Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_level", typeof(int), (int)item["factory_level"]! + 1);
                                            Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_exp", typeof(int), (int)item["factory_exp"]! - formula);
                                            Logger.Warning($"已升级“{receiver.GroupName} ({receiver.GroupId})”的面包厂等级！");
                                            try
                                            {
                                                await receiver.SendMessageAsync($"已将面包厂等级升级到 {(int)item["factory_level"]! + 1} 级");
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.Error("群消息发送失败！");
                                                Logger.Debug($"错误信息：\n{e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的面包厂等级，因为该分厂积累的经验值不够！");
                                            try
                                            {
                                                await receiver.SendMessageAsync($"升级面包厂等级需要 {formula} 点经验，但是本群面包厂还差 {formula - (int)item["factory_exp"]!} 点经验");
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
                                        Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的面包厂等级，因为该分厂已经满级了！");
                                        try
                                        {
                                            await receiver.SendMessageAsync("本群面包厂已经满级了！");
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
                    break;
                case "!build_factory":
                    if (!Json.FileExists("breadfactory"))
                    {
                        JObject obj = new(
                            new JProperty("groups",
                            new JArray()));
                        Json.CreateFile("breadfactory", obj);
                    }
                    if (!Json.FileExists("materials"))
                    {
                        JObject obj = new(
                            new JProperty("groups",
                            new JArray()));
                        Json.CreateFile("materials", obj);
                    }
                    if (!Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                    {
                        JObject obj = new(
                            new JProperty("groupid", receiver.GroupId),
                            new JProperty("factory_level", 1),
                            new JProperty("storage_level", 0),
                            new JProperty("speed_level", 0),
                            new JProperty("output_level", 0),
                            new JProperty("factory_mode", "normal"),
                            new JProperty("factory_exp", 0),
                            new JProperty("breads", 0),
                            new JProperty("exp_gained_today", 0),
                            new JProperty("last_expfull", 946656000),
                            new JProperty("last_expgain", 946656000),
                            new JProperty("last_produce", 946656000)
                            );
                        Json.AddObjectToArray("breadfactory", "groups", obj);
                        JObject obj1 = new(
                            new JProperty("groupid", receiver.GroupId),
                            new JProperty("flour", 0),
                            new JProperty("egg", 0),
                            new JProperty("yeast", 0),
                            new JProperty("last_produce", 946656000)
                            );
                        Json.AddObjectToArray("materials", "groups", obj1);
                        Logger.Info($"已为地区“{receiver.GroupName} ({receiver.GroupId})”兴建面包厂！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                        try
                        {
                            await receiver.SendMessageAsync("成功为本群建造面包厂！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    else
                    {
                        Logger.Warning($"未尝试为地区“{receiver.GroupName} ({receiver.GroupId})”兴建面包厂，因为该地区已有面包厂！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                        try
                        {
                            await receiver.SendMessageAsync("本群已经有面包厂了！");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                    }
                    break;
                case "!upgrade_storage":
                    if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                    {
                        if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    if ((int)item["factory_level"]! == breadfactory_maxlevel)
                                    {
                                        if ((int)item["storage_level"]! != 16)
                                        {
                                            int formula = (int)Math.Ceiling(2000 * Math.Pow(1.28, (int)item["storage_level"]!));
                                            if ((int)item["factory_exp"]! >= formula)
                                            {
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "storage_level", typeof(int), (int)item["storage_level"]! + 1);
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_exp", typeof(int), (int)item["factory_exp"]! - formula);
                                                Logger.Warning($"已升级“{receiver.GroupName} ({receiver.GroupId})”的库存等级！");
                                                try
                                                {
                                                    await receiver.SendMessageAsync($"已将面包厂库存等级升级到 {(int)item["storage_level"]! + 1} 级");
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                            }
                                            else
                                            {
                                                Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的库存等级，因为该分厂积累的经验值不够！");
                                                try
                                                {
                                                    await receiver.SendMessageAsync($"升级库存等级需要 {formula} 点经验，但是本群面包厂还差 {formula - (int)item["factory_exp"]!} 点经验");
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
                                            Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的库存等级，因为该分厂的库存等级已经满级！");
                                            try
                                            {
                                                await receiver.SendMessageAsync("本群面包厂库存等级已经满级了！");
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
                                        Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的库存等级，因为该分厂尚未满级！");
                                        try
                                        {
                                            await receiver.SendMessageAsync("本群面包厂还没有满级！");
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
                    break;
                case "!upgrade_speed":
                    if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                    {
                        if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    if ((int)item["factory_level"]! == breadfactory_maxlevel)
                                    {
                                        if ((int)item["speed_level"]! != 16)
                                        {
                                            int formula = (int)Math.Ceiling(9600 * Math.Pow(1.14, (int)item["speed_level"]!));
                                            if ((int)item["factory_exp"]! >= formula)
                                            {
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "speed_level", typeof(int), (int)item["speed_level"]! + 1);
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_exp", typeof(int), (int)item["factory_exp"]! - formula);
                                                Logger.Warning($"已升级“{receiver.GroupName} ({receiver.GroupId})”的速度等级！");
                                                try
                                                {
                                                    await receiver.SendMessageAsync($"已将面包厂速度等级升级到 {(int)item["speed_level"]! + 1} 级");
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                            }
                                            else
                                            {
                                                Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的速度等级，因为该分厂积累的经验值不够！");
                                                try
                                                {
                                                    await receiver.SendMessageAsync($"升级速度等级需要 {formula} 点经验，但是本群面包厂还差 {formula - (int)item["factory_exp"]!} 点经验");
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
                                            Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的速度等级，因为该分厂的速度等级已经满级！");
                                            try
                                            {
                                                await receiver.SendMessageAsync("本群面包厂速度等级已经满级了！");
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
                                        Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的速度等级，因为该分厂尚未满级！");
                                        try
                                        {
                                            await receiver.SendMessageAsync("本群面包厂还没有满级！");
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
                    break;
                case "!upgrade_output":
                    if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
                    {
                        if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid_{receiver.GroupId}"))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                            {
                                if ((string)item["groupid"]! == receiver.GroupId)
                                {
                                    if ((int)item["factory_level"]! == breadfactory_maxlevel)
                                    {
                                        if ((int)item["output_level"]! != 16)
                                        {
                                            int formula = (int)Math.Ceiling(4800 * Math.Pow(1.21, (int)item["output_level"]!));
                                            if ((int)item["factory_exp"]! >= formula)
                                            {
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "output_level", typeof(int), (int)item["output_level"]! + 1);
                                                Json.ModifyObjectFromArray("breadfactory", "groups", $"groupid_{receiver.GroupId}", "factory_exp", typeof(int), (int)item["factory_exp"]! - formula);
                                                Logger.Warning($"已升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级！");
                                                try
                                                {
                                                    await receiver.SendMessageAsync($"已将面包厂产量等级升级到 {(int)item["output_level"]! + 1} 级");
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Error("群消息发送失败！");
                                                    Logger.Debug($"错误信息：\n{e.Message}");
                                                }
                                            }
                                            else
                                            {
                                                Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该分厂积累的经验值不够！");
                                                try
                                                {
                                                    await receiver.SendMessageAsync($"升级产量等级需要 {formula} 点经验，但是本群面包厂还差 {formula - (int)item["factory_exp"]!} 点经验");
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
                                            Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该分厂的产量等级已经满级！");
                                            try
                                            {
                                                await receiver.SendMessageAsync("本群面包厂产量等级已经满级了！");
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
                                        Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该分厂尚未满级！");
                                        try
                                        {
                                            await receiver.SendMessageAsync("本群面包厂还没有满级！");
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
                    break;
            }
        }
    }
}