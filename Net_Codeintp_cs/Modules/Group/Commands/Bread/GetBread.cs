// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：拿面包
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class GetBread : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!getbread")
            {
                switch (s.Length)
                {
                    case 2:
                        if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                            List<string> bread_types = new()
                                                        {
                                                            char.ConvertFromUtf32(0x1F35E),
                                                            char.ConvertFromUtf32(0x1F956),
                                                            char.ConvertFromUtf32(0x1F950),
                                                            char.ConvertFromUtf32(0x1F96F),
                                                            char.ConvertFromUtf32(0x1F369)
                                                        };
                            string text = " ";
                            if (int.TryParse(s[1], out int number) && number >= 1)
                            {
                                if (((string)item["factory_mode"]!).Contains("infinite") || (int)item["breads"]! >= number)
                                {
                                    switch (((string)item["factory_mode"]!).Contains("diverse"))
                                    {
                                        case true:
                                            Random rnd = new();
                                            int[] fields = new int[bread_types.Count];
                                            int sum = 0;
                                            for (int i = 0; i < fields.Length; i++)
                                            {
                                                double exponent = 0.5 - (i * (0.5 / bread_types.Count));
                                                fields[i] = rnd.Next((int)Math.Floor((number - sum) * (1 - exponent)) + 1);
                                                sum += fields[i];
                                            }
                                            fields[^1] = number - sum;
                                            switch (number)
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
                                            break;
                                        case false:
                                            text = number switch
                                            {
                                                <= 20 => string.Join("", Enumerable.Repeat(char.ConvertFromUtf32(0x1F35E), number)),
                                                _ => $"{char.ConvertFromUtf32(0x1F35E)} * {number}",
                                            };
                                            break;
                                    }
                                    if (!((string)item["factory_mode"]!).Contains("infinite"))
                                    {
                                        Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", (int)item["breads"]! - number);
                                    }
                                    MessageChain? messageChain = new MessageChainBuilder()
                                               .At(receiver.Sender.Id)
                                               .Plain(text)
                                               .Build();
                                    Logger.Info($"有面包厂向该地区的一名客户送出了 {number} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync(messageChain);
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error($"从“{receiver.GroupName} ({receiver.GroupId})”运出的 {number} 块面包在运输途中丢了，因为群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                }
                                else
                                {
                                    Logger.Warning($"有面包厂未能供应 {number} 块面包，因为该分厂库存内的面包不够！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
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
                        break;
                    case 1:
                        if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                            List<string> bread_types = new()
                                                        {
                                                            char.ConvertFromUtf32(0x1F35E),
                                                            char.ConvertFromUtf32(0x1F956),
                                                            char.ConvertFromUtf32(0x1F950),
                                                            char.ConvertFromUtf32(0x1F96F),
                                                            char.ConvertFromUtf32(0x1F369)
                                                        };
                            string text = " ";
                            if (((string)item["factory_mode"]!).Contains("infinite") || (int)item["breads"]! >= 1)
                            {
                                switch (((string)item["factory_mode"]!).Contains("diverse"))
                                {
                                    case true:
                                        Random rnd = new();
                                        int[] fields = new int[bread_types.Count];
                                        int sum = 0;
                                        for (int i = 0; i < fields.Length; i++)
                                        {
                                            double exponent = 0.5 - (i * (0.5 / bread_types.Count));
                                            fields[i] = rnd.Next((int)Math.Floor((1 - sum) * (1 - exponent)));
                                            sum += fields[i];
                                        }
                                        fields[^1] = 1 - sum;
                                        for (int i = 0; i < bread_types.Count; i++)
                                        {
                                            text += string.Join("", Enumerable.Repeat(bread_types[i], fields[i]));
                                        }
                                        break;
                                    case false:
                                        text = string.Join("", Enumerable.Repeat(char.ConvertFromUtf32(0x1F35E), 1));
                                        break;
                                }
                                if (!((string)item["factory_mode"]!).Contains("infinite"))
                                {
                                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", (int)item["breads"]! - 1);
                                }
                                MessageChain? messageChain = new MessageChainBuilder()
                                           .At(receiver.Sender.Id)
                                           .Plain(text)
                                           .Build();
                                Logger.Info($"有面包厂向该地区的一名客户送出了 1 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync(messageChain);
                                }
                                catch (Exception e)
                                {
                                    Logger.Error($"从“{receiver.GroupName} ({receiver.GroupId})”运出的 1 块面包在运输途中丢了，因为群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                            else
                            {
                                Logger.Warning($"有面包厂未能供应 1 块面包，因为该分厂库存内的面包不够！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
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
            }
        }
    }
}